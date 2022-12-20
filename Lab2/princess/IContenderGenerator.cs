namespace Lab2;

public interface IContenderGenerator
{
    Candidate[] Generate();
}

public struct Candidate
{
    public string Name { get; set; }
    public int Points { get; set; }
}