namespace Aoc2023Cs;

public class Sequence
{
    public List<long> sequence = new();
    public Sequence? derived = null;

    public void Add(long value) => sequence.Add(value);
    public int Length => sequence.Count;

    public void CalcDerivations()
    {
        Sequence derivation = this;
        while (!derivation.IsAllSameValue())
        {
            derivation = derivation.CalcDerivation();
        }
    }

    public Sequence CalcDerivation()
    {
        Sequence newSequence = new();
        derived = newSequence;
        if (Length == 0) return newSequence;
        
        newSequence.sequence.Capacity = Length - 1;
        for (var i = 1; i < Length; i++)
        {
            long diff = sequence[i] - sequence[i - 1];
            newSequence.Add(diff);
        }
        return newSequence;
    }

    public bool IsAllSameValue()
    {
        if (Length == 0) return false;
        return sequence.All(v => v == sequence[0]);
    }

    public long ReconstructBeginning() => (derived != null) ? derived.ReconstructBeginning() + sequence[^1] : sequence[^1];
    public long ReconstructEnd() => (derived != null) ? sequence[0] - derived.ReconstructEnd() : sequence[0];
}

public class Day9
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        Span<string> lines = "9".ReadLinesSpan(test: false);

        long result = 0;
        foreach (string lineStr in lines)
        {
            Sequence sequence = new();
            Span<char> line = lineStr.AsSpan();
            while (line.Length > 0)
            {
                line.ExtractInt(out long value);
                sequence.Add(value);
                line.SkipWhiteRef();
            }

            sequence.CalcDerivations();
            long missing = partOne ? sequence.ReconstructBeginning() : sequence.ReconstructEnd();
            result += missing;
            Console.WriteLine($"-> {missing}");
        }

        if (partOne)
        {
            Console.WriteLine($"Part One: {result}");
        }
        else
        {
            Console.WriteLine($"Part Two: {result}");
        }
    }
}