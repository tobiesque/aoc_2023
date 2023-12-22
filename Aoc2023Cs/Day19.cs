using System.Diagnostics;
using Rock.Collections;

namespace Aoc2023Cs;

public class Day19
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);

        if (partOne)
        {
            Day19_PartOne.Run();
            return;
        }

        string[] lines = "19".ReadLinesArray(test: true);
        CreateWorkflows(lines);
        RunWorkflows();

        long result = 0;
        for (var j = 0; j < Workflow.ranges[0].Count; j++)
        {
            long score = 1;
            for (var i = 0; i < 4; i++)
            {
                Workflow.Range singleRange = Workflow.ranges[i][j];
                Console.Write($"{i}:[{singleRange.lower}-{singleRange.upper}] ");
                score *= singleRange.upper - singleRange.lower + 1;
            }
            Console.WriteLine($"= {score}");

            result += score;
        }
        
        Console.WriteLine($"Part Two: {result}");
    }

    public static void RunWorkflows()
    {
        Workflow workflow = Workflow.workflows["in"];
        workflow.Do([new(), new(), new(), new()]);
    }

    public static void CreateWorkflows(string[] lines)
    {
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('{')) continue;

            Span<char> lineSp = line.AsSpan();
            lineSp.ExtractRef(out string workflowName, c => c != '{').SkipRef(1);

            if (!Workflow.workflows.TryGetValue(workflowName, out Workflow workflow))
            {
                workflow = new() { name = workflowName };
                Workflow.workflows.Add(workflowName, workflow);
            }

            lineSp.ExtractRef(out string rulesStr, c => c != '}').SkipRef(1);
            string[] rules = rulesStr.Split(',');
            foreach (var ruleDesc in rules)
            {
                Workflow.Rule rule = new() { };
                workflow.rules.Add(rule);
                string[] rulesSplit = ruleDesc.Split(':');
                if (rulesSplit.Length == 1)
                {
                    rule.op = ' ';
                    rule.value = 0;
                    workflowName = rulesSplit[0];
                }
                else
                {
                    rule.op = rulesSplit[0].Contains('>') ? '>' : '<';
                    string[] assignment = rulesSplit[0].Split(rule.op);
                    rule.variableIndex = assignment[0] switch { "x" => 0, "m" => 1, "a" => 2, "s" => 3, _ => -1 };
                    rule.value = long.Parse(assignment[1]);
                    workflowName = rulesSplit[1];
                }

                if (!Workflow.workflows.TryGetValue(workflowName, out rule.followUp))
                {
                    rule.followUp = new() { name = workflowName };
                    Workflow.workflows.Add(workflowName, rule.followUp);
                }
            }
        }
    }
}

public class Workflow
{
    public struct Range
    {
        public long lower = 1;
        public long upper = 4000;

        public Range() {}
    }

    public class Rule
    {
        public char op;
        public long value;
        public int variableIndex;
        public Workflow? followUp;

        public void Do(Range[] range)
        {
            Console.Write($"->{variableIndex}{op}{value}");
            switch (op)
            {
                case '>':
                    range[variableIndex].lower = long.Max(value, range[variableIndex].lower);
                    followUp!.Do(range.ToArray());
                    break;

                case '<':
                    range[variableIndex].upper = long.Min(value, range[variableIndex].upper);
                    followUp!.Do(range.ToArray());
                    break;

                case ' ':
                    followUp!.Do(range.ToArray());
                    break;

                default:
                    Debug.Fail("Unknown op");
                    break;
            }
        }
    }

    public void Do(Range[] range)
    {
        if (name == "A")
        {
            for (var i = 0; i < range.Length; i++)
            {
                ranges[i].Add(range[i]);
            }

            return;
        }

        if (name == "R")
        {
            return;
        }
        
        Console.Write($"->{name}");
        foreach (var rule in rules)
        {
            rule.Do(range.ToArray());
        }
    }

    public static List<Range>[] ranges = [[], [], [], []];
    
    public static OrderedDictionary<string, Workflow> workflows = new();
    
    public string name;
    public List<Rule> rules = new();
}
