﻿using System.Diagnostics;
using System.Formats.Asn1;
using Rock.Collections;

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
            GeneratePartOne.Generate();
            return;
        }

        if (partOne)
        {
            foreach (var part_ in Part.parts) part_.in_();
            Console.WriteLine($"Part One: {Part.score}");
            return;
        }

        // part two
        CreaateWorkflows();
        DoVariables();
    }

    public static void RunWorkflows()
    {
        Workflow workflow = Workflow.workflows["in"];
        workflow.Do();
    }
    
    public static void CreaateWorkflows()
    {
        string[] lines = "19".ReadLinesArray(test: true);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith('{')) continue;
            
            Span<char> lineSp = line.AsSpan();
            lineSp.ExtractRef(out string workflowName, c => c != '{').SkipRef(1);

            // Console.WriteLine($"Workflow {workflowName}");

            if (!Workflow.workflows.TryGetValue(workflowName, out Workflow workflow))
            {
                workflow = new() { name = workflowName };
                Workflow.workflows.Add(workflowName, workflow);
            }

            lineSp.ExtractRef(out string rulesStr, c => c != '}').SkipRef(1);
            string[] rules = rulesStr.Split(',');
            foreach (var ruleDesc in rules)
            {
                // Console.WriteLine($"    {ruleDesc}");

                Workflow.Rule rule = new() { };
                workflow.rules.Add(rule);
                string[] rulesSplit = ruleDesc.Split(':');
                if (rulesSplit.Length == 1)
                {
                    rule.op = ' ';
                    rule.value = 0;
                    rule.followWorkflow = rulesSplit[0];
                }
                else
                {
                    rule.op = rulesSplit[0].Contains('>') ? '>' : '<';
                    string[] assignment = rulesSplit[0].Split(rule.op);
                    rule.variable = assignment[0];
                    rule.value = long.Parse(assignment[1]);
                    rule.followWorkflow = rulesSplit[1];
                }
            }
        }
    }

    public static void DoVariables()
    {
        long resultPartOne = 0;
        
        string[] lines = "19".ReadLinesArray(test: true);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (!line.StartsWith('{')) continue;

            Workflow.result = 0;            
            Span<char> lineSp = line.AsSpan();
            
            Workflow.variables.Clear();
            lineSp.SkipRef(1).ExtractRef(out string defsStr, c => c != '}');
            string[] defs = defsStr.Split(',');
            foreach (var def in defs)
            {
                string[] defDecl = def.Split('=');
                string variable = defDecl[0];
                long value = int.Parse(defDecl[1]);
                Workflow.variables[variable] = value;
            }

            RunWorkflows();
            
            resultPartOne += Workflow.result;
        }
        
        Console.WriteLine($"Part One: {resultPartOne}");
    }
}

public class Workflow
{
    public static long result = 0;
    
    public class Rule
    {
        public string variable;
        public char op; // >, <, ' '
        public long value;
        public string followWorkflow;

        public string Do()
        {
            long variableValue = !string.IsNullOrWhiteSpace(variable) ? variables[variable] : 0;
            switch (op)
            {
                case '>':
                    if (variableValue > value) return followWorkflow;
                    break;
                
                case '<':
                    if (variableValue < value) return followWorkflow;
                    break;
                
                case ' ':
                    return followWorkflow;
                
                default:
                    Debug.Fail("Unknown op");
                    break;
            }

            return "";
        }
    }

    public bool Do()
    {
        Console.WriteLine($"->{name}");
        foreach (var rule in rules)
        {
            string followup = rule.Do();
            if (followup == "")
            {
                continue;
            }
            if (followup == "A")
            {
                result += variables.Values.Sum();
                Console.WriteLine($"  ->Accepted");
                return true;
            }
            if (followup == "R")
            {
                Console.WriteLine($"  ->Rejected");
                return true;
            }
                
            Workflow workflow = workflows[followup];
            if (workflow.Do())
            {
                return true;
            }
        }

        return false;
    }

    public static OrderedDictionary<string, Workflow> workflows = new();
    public static Dictionary<string, long> variables = new();
    
    public string name;
    public List<Rule> rules = new();
}
