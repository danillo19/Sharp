
namespace Lab2;

public class Hall
{
    private Candidate[] _candidates;
    
    private int _currentCandidateIndex;
    public Hall(IContenderGenerator contenderGenerator)
    {
        this._candidates = contenderGenerator.Generate();
    }

    public void SetCandidates(ref Candidate[] candidates)
    {
        _candidates = candidates;
        _currentCandidateIndex = 0;
    }
    public Candidate GetNextCandidate()
    {
        _currentCandidateIndex++;
        if (_currentCandidateIndex > _candidates.Length) throw new Exception("Hall is empty");
        return _candidates[_currentCandidateIndex - 1];
    }

    public ref Candidate[] GetCandidates()
    {
        return ref _candidates;
    }
}