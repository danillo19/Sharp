using System.Data;
using Lab2.model;
using Lab2.repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lab2;

public class Princess : IHostedService
{

    private Friend _friend;

    private Hall _hall;

    private IServiceScopeFactory _serviceScopeFactory;

    private Simulator? _simulator;
    private TaskCompletionSource<bool> TaskCompletionSource { get; set; } = new TaskCompletionSource<bool>();
    private CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
    private ILogger<Princess>? Logger { get; }

    private IHostApplicationLifetime? ApplicationLifetime { get; }

    public Princess(ILogger<Princess> logger, IHostApplicationLifetime appLifetime, Friend friend, Hall hall,
        IServiceScopeFactory serviceScopeFactory, Simulator simulator)
    {
        Logger = logger;
        ApplicationLifetime = appLifetime;
        _friend = friend;
        _hall = hall;
        _simulator = simulator;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Princess(Friend? friend, Hall? hall)
    {
        _friend = friend;
        _hall = hall;
    }

    public Candidate? MakeChoice()
    {
        for (int i = 0; i < 37; i++)
        {
            Candidate candidate = _hall.GetNextCandidate();
            _friend.Compare(ref candidate);
        }

        for (int i = 37; i < 68; i++)
        {
            Candidate candidate = _hall.GetNextCandidate();
            if (_friend.IsBetterThenBest(ref candidate))
            {
                TaskCompletionSource.SetResult(true);
                return candidate;
            }

            _friend.Compare(ref candidate);
        }

        for (int i = 68; i < 100; i++)
        {
            Candidate candidate = _hall.GetNextCandidate();
            if (_friend.IsBetterThenSecond(ref candidate))
            {
                TaskCompletionSource.SetResult(true);
                return candidate;
            }
        }

        TaskCompletionSource.SetResult(true);
        return null;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_simulator != null)
            _simulator.SimulateNewAttemptsEvent += delegate(object? sender, int i)
            {
                TaskCompletionSource = new TaskCompletionSource<bool>();

                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                _friend = scope.ServiceProvider.GetService<Friend>() ?? new Friend();
                _hall = scope.ServiceProvider.GetService<Hall>() ?? throw new InvalidOperationException();
                Logger?.LogInformation("Attempt " + i + " : ");
                
                SaveAttemptIntoDatabase(GetPrincessChoice(), i);
            };

        _simulator.SimulateExistingAttemptEvent += delegate(object? sender, int attemptId)
        {
            using DatabaseContext db = new DatabaseContext();
            var dbAttempt = db.Attempts.Find(attemptId);
            if (dbAttempt == null)
            {
                throw new ArgumentException("No such attempt in database");
            }

            Candidate[] candidates = new Candidate[100];

            for (int i = 0; i < 100; i++)
            {
                candidates[i] = new Candidate
                    { Name = dbAttempt.CandidatesSequenceNames?[i], Points = dbAttempt.CandidatesSequenceValues[i] };
            }
            
            _hall.SetCandidates(ref candidates);

            var choice = GetPrincessChoice();
            var princessHappiness = choice?.Points ?? 10;

            Logger.LogInformation("Simulated happiness: " + princessHappiness);
            Logger.LogInformation("Happiness from db: " + dbAttempt.PrincessHappiness);
            
            if (princessHappiness != dbAttempt.PrincessHappiness)
            {
                throw new DataException("Wrong simulation result's: happiness doesn't match");
            }
        };
        
        return Task.CompletedTask;
    }

    private void SaveAttemptIntoDatabase(Candidate? choice, int attemptId)
    {
        Candidate[] candidates = _hall.GetCandidates();
        int[] candidatesSequencePoints = candidates.Select(c => c.Points).ToArray();
        String[] candidatesSequenceNames = candidates.Select(c => c.Name).ToArray();

        String? choiceName = choice?.Name;
        int happiness = choice?.Points ?? 10;
        Attempt attempt = new Attempt
        {
            CandidatesSequenceNames = candidatesSequenceNames, CandidatesSequenceValues = candidatesSequencePoints,
            ChoiceName = choiceName, PrincessHappiness = happiness
        };

        using DatabaseContext db = new DatabaseContext();
        Logger.LogInformation("SAVING DB CHANGES...");
        var dbAttempt = db.Attempts.Find(attemptId);
        if (dbAttempt == null)
        {
            db.Attempts.Add(attempt);
        }

        else
        {
            dbAttempt.ChoiceName = attempt.ChoiceName;
            dbAttempt.PrincessHappiness = attempt.PrincessHappiness;
            dbAttempt.CandidatesSequenceNames = attempt.CandidatesSequenceNames;
            dbAttempt.CandidatesSequenceValues = attempt.CandidatesSequenceValues;
            
            db.Entry(dbAttempt).State = EntityState.Modified;
        }
        db.SaveChanges();
    }

    private Candidate? GetPrincessChoice()
    {
        Task<Candidate?> task = Task.Run(MakeChoice, CancellationTokenSource.Token);
        if (task.Result != null)
        {
            Candidate candidate = task.Result.Value;
            Logger?.LogInformation("Name: " + candidate.Name + "; Happiness: " + candidate.Points);
        }
        else
        {
            Logger?.LogInformation("Happiness: 10");
        }

        return task.Result;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}