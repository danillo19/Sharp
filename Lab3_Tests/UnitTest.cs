using System.Buffers;
using System.Collections;
using System.Security.Cryptography;
using Lab2;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Sdk;

namespace Lab3_Tests;

public class UnitTest
{
    [Fact]
    public void UniqueCandidatesTest()
    {
        ContenderGenerator generator = new ContenderGenerator();
        Candidate[] candidates = generator.Generate();
        String[] names = candidates.Select(c => c.Name).ToArray();
        var namesSet = new HashSet<String>(names);
        var candidatesSet = new HashSet<Candidate>(candidates);
        Assert.Equal(100, namesSet.Count);
        Assert.Equal(100, candidatesSet.Count);
        
    }

    [Fact]
    public void FriendTest()
    {
        Friend friend = new Friend();
        
        for (int i = 0; i < 10; i++)
        {
            var candidate = new Candidate();
            candidate.Name = i.ToString();
            candidate.Points = i;
            friend.Compare(ref candidate);
        }

        var bestCandidate = new Candidate();
        bestCandidate.Name = "best";
        bestCandidate.Points = 11;
        
        var thirdCandidate = new Candidate();
        thirdCandidate.Name = "third place";
        thirdCandidate.Points = 8;

            
        Assert.True(friend.IsBetterThenBest(ref bestCandidate));
        Assert.False(friend.IsBetterThenSecond(ref thirdCandidate));
    }

    [Fact]
    public void HallTest()
    {
        ContenderGenerator generator = new ContenderGenerator();
        Hall testHall = new Hall(generator);

        var candidatesSet = new HashSet<Candidate>();

        for (int i = 0; i < 100; i++)
        {
            candidatesSet.Add(testHall.GetNextCandidate());
        }
        
        Assert.Equal(100, candidatesSet.Count);
        var ex = Assert.Throws<Exception>(() => testHall.GetNextCandidate());
        Assert.Equal("Hall is empty", ex.Message);
    }

    [Fact]
    public void PrincessPickFirstChoiceTest()
    {
        Mock<IContenderGenerator> stub = new Mock<IContenderGenerator>();
        stub.Setup(g => g.Generate()).Returns(GetCandidatesSetup(99, 98));
        Hall? hall = new Hall(stub.Object);
        Friend? friend = new Friend();
        Princess princess = new Princess(friend, hall);
        Assert.Equal(princess.MakeChoice()?.Points, 38);
    }
    
    [Fact]
    public void PrincessPickSecondChoiceTest()
    {
        Mock<IContenderGenerator> stub = new Mock<IContenderGenerator>();
        stub.Setup(g => g.Generate()).Returns(GetCandidatesSetup(0, 68));
        Hall? hall = new Hall(stub.Object);
        Friend? friend = new Friend();
        Princess princess = new Princess(friend, hall);
        Assert.Equal(princess.MakeChoice()?.Points, 99);
    }
    
    [Fact]
    public void PrincessPickNobodyTest()
    {
        Mock<IContenderGenerator> stub = new Mock<IContenderGenerator>();
        stub.Setup(g => g.Generate()).Returns(GetCandidatesSetup(0, 1));
        Hall? hall = new Hall(stub.Object);
        Friend? friend = new Friend();
        Princess princess = new Princess(friend, hall);
        Assert.Null(princess.MakeChoice());
    }

    private Candidate[] GetCandidatesSetup(int indexOfBestCandidate, int indexOfSecondCandidate)
    {
        Candidate[] candidates = new Candidate[100];
        for (int i = 0; i < candidates.Length; i++)
        {
            candidates[i] = new Candidate();
            candidates[i].Name = (i + 1).ToString();
            candidates[i].Points = i + 1;
        }

        (candidates[indexOfBestCandidate], candidates[99]) = (candidates[99], candidates[indexOfBestCandidate]);
        (candidates[indexOfSecondCandidate], candidates[98]) = (candidates[98], candidates[indexOfSecondCandidate]);
        return candidates;
    }
}