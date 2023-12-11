namespace Aoc2023Cs;

// #define ENABLE_DEBUG

public class Day8
{
    public class Node
    {
        public string name;
        public Node left;
        public Node right;
        
        public static bool IsStartNode(Node node) => (node != null) && node.name.EndsWith("A");
        public static bool IsEndNode(Node node) => (node != null) && node.name.EndsWith("Z");
        public override string ToString() => name;
    }

    public class Nodes : Dictionary<string, Node>
    {
        private void Add(string name, out Node node)
        {
            if (!TryGetValue(name, out node))
            {
                Add(name, node = new() { name = name, left = null, right = null });
            };
        }
        
        public void Add(string name, string left, string right)
        {
            Add(new(name), out Node node);
            Add(left, out node.left);
            Add(right, out node.right);
        }
    }
    
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);

        Nodes nodes = new();

        bool test = false;
        string day = "8" + (partOne ? "" : (test ? "_2" : "")); 
        string[] lines = day.ReadLinesArray(test: test);
        
        Span<char> directions = lines[0].AsSpan();
        Console.WriteLine($"{directions}");
        foreach (string lineStr in lines[2..])
        {
            Span<char> line = lineStr.AsSpan();
            line.ExtractLetters(out var node).ExtractLetters(out var left).ExtractLetters(out var right);
            nodes.Add(new(node), new(left), new(right));
            Console.WriteLine($"{node} = ({left}, {right})");
        }

        if (partOne)
        {
            int steps = 0;
            Node current = nodes["AAA"];
            while (true)
            {
                foreach (char direction in directions)
                {
                    ++steps;
                    if (Node.IsEndNode(current)) break;
                    current = (direction == 'R') ? current.right : current.left;
                }
                if (Node.IsEndNode(current)) break;
            }
            Console.WriteLine($"Part One: {steps}");
        }
        else
        {
            ulong steps = 0;
            HashSet<ulong> primes = new();
            
            Node[] currents = nodes.Values.Where(Node.IsStartNode).ToArray();
            List<ulong> stepses = new();
            foreach (Node start in currents)
            {
                Node current = start;
                steps = 0;
                while (true)
                {
                    foreach (char direction in directions)
                    {
                        ++steps;
                        if (Node.IsEndNode(current)) break;
                        current = (direction == 'R') ? current.right : current.left;
                    }
                    if (Node.IsEndNode(current)) break;
                }
                stepses.Add(steps);
                primes.AddPrimeFactors(steps);
            }
            Console.WriteLine($"-> {stepses.MakeList()}");
            Console.WriteLine($"-> {primes.MakeList()}");

            Console.WriteLine($"Part Two: {primes.Product()}");
        }
    }
}