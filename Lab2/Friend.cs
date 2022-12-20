namespace Lab2;

public class Friend
{
    private Candidate? _bestCandidate = null;
    private Candidate? _secondCandidate = null;

    public void Compare(ref Candidate candidate)
    {
        if (!_bestCandidate.HasValue) _bestCandidate = candidate;
        else if (IsBetterThenBest(ref candidate))
        {
            _secondCandidate = _bestCandidate;
            _bestCandidate = candidate;
        }
        else if (!_secondCandidate.HasValue) _secondCandidate = candidate;
        else if (IsBetterThenSecond(ref candidate))
        {
            _secondCandidate = candidate;
        }
    }

    public bool IsBetterThenBest(ref Candidate candidate)
    {
        if (_bestCandidate == null) throw new Exception("Invalid best candidate, check algorithm");
        return candidate.Points > _bestCandidate.Value.Points;
    }

    public bool IsBetterThenSecond(ref Candidate candidate)
    {
        if (_secondCandidate == null) throw new Exception("Invalid second candidate, check algorithm");
        return candidate.Points > _secondCandidate.Value.Points;
    }
}