namespace Lab2;

public class ContenderGenerator: IContenderGenerator
{
    private readonly String _candidatesNamesListFilePath = "candidates.txt";
    public Candidate[] Generate()
    {
        string[] lines = File.ReadAllLines(_candidatesNamesListFilePath);
        Candidate[] candidates = new Candidate[100];
        for (int i = 0; i < 100; i++)
        {
            candidates[i] = new Candidate();
            candidates[i].Name = lines[i];
            candidates[i].Points = 100 - i;
        }

        Shuffle(ref candidates);
        return candidates;
    }

    private void Shuffle(ref Candidate[] array)
    {
        Random random = new Random();
        for (int i = array.Length - 1; i >= 1; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}