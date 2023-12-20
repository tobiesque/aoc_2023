namespace Aoc2023Cs;

public class Day19
{
    public static bool partOne = true;
    public static bool generate = false;

    public static void Run(int part)
    {
        partOne = (part == 1);
        if (generate)
        {
            Generate();
            return;
        }

        if (partOne)
        {
            foreach (var part_ in Part.parts) part_.in_();
            Console.WriteLine($"Part One: {Part.score}");
            return;
        }
    }

    public static void Generate()
    {
        string[] lines = "19".ReadLinesArray(test: false);

        Console.WriteLine("namespace Aoc2023Cs;\n");
        Console.WriteLine("public partial class Part");
        Console.WriteLine("{");

        // workflows
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith('{')) continue;

            Span<char> lineSp = line.AsSpan();
            lineSp.ExtractRef(out string name, c => c != '{').SkipRef(1);
            name = (name == "in") ? "in_" : name;
            Console.WriteLine($"    public void {name}() {{");

            lineSp.ExtractRef(out string term, c => c != '}');
            var expressions = term.Split(',');
            foreach (var expression in expressions)
            {
                var parts = expression.Split(':');
                if (parts.Length == 1)
                {
                    string funcName = parts[0] == "in" ? "in_" : parts[0];
                    Console.WriteLine($"        {funcName}();");
                }
                else
                {
                    string funcName = parts[1] == "in" ? "in_" : parts[1];
                    Console.WriteLine($"        if ({parts[0]}) {{ {funcName}(); return; }}");
                }
            }
            Console.WriteLine("    }\n");
        }
            
        // parts
        Console.WriteLine("    public static Part[] parts = new [] {");
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (!line.StartsWith('{')) continue;
            Console.WriteLine($"        new Part() {line},");
        }
        
        Console.WriteLine("    };");
        Console.WriteLine("}");
    }
}

public partial class Part
{
    public static long score = 0;
    
    public long x;
    public long m;
    public long a;
    public long s;

    public void A()
    {
        score += x + m + a + s;
    }

    public void R()
    {
    }
}
