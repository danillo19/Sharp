using Lab2.repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lab2;

public class SimulatorService : IHostedService
{
    private readonly Simulator _simulator;
    private IHostApplicationLifetime? ApplicationLifetime { get; }
    
    private ILogger<SimulatorService>? _logger { get; }

    private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

    public SimulatorService(Simulator simulator, ILogger<SimulatorService> logger, IHostApplicationLifetime appLifetime)
    {
        _simulator = simulator;
        _logger = logger;
        ApplicationLifetime = appLifetime;

    }

    public void RunAsync(int? attemptNumber)
    {
        if (attemptNumber == null)
        {
            for (var i = 0; i < 100; i++)
            {
                _simulator.Simulate(i);
            }
            _logger?.LogInformation("Average: " + CalcAverageHappiness());
        }
        else
        {
            _simulator.SimulateSingleEvent(attemptNumber.Value);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        int? attemptNumber = null;
        RunAsync(attemptNumber);
        ApplicationLifetime?.StopApplication();
        return Task.CompletedTask;
    }
    
    private int CalcAverageHappiness()
    {
        using DatabaseContext dbContext = new DatabaseContext();
        int averageHappiness = dbContext.Attempts.Sum(a => a.PrincessHappiness);
        return averageHappiness / 100;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource.Cancel();
        ApplicationLifetime?.StopApplication();
        return Task.CompletedTask;
    }
}