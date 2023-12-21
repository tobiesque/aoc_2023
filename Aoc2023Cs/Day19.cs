﻿using System.Diagnostics;
using Rock.Collections;

namespace Aoc2023Cs;

public class Day19
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);

        if (partOne) { Day19_PartOne.Run(); return; }
        
        string[] lines = "19".ReadLinesArray(test: true);
        CreateWorkflows(lines);
        DoVariables(lines);
    }

    public static void RunWorkflows()
    {
        Workflow workflow = Workflow.workflows["in"];
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
                    rule.variable = assignment[0];
                    rule.value = long.Parse(assignment[1]);
                    workflowName = rulesSplit[1];
                }
                
                if (!Workflow.workflows.TryGetValue(workflowName, out rule.followUp))
                {
                    rule.followUp = new() { name = workflowName };
                    Workflow.workflows.Add(workflowName, rule.followUp);
                }
                rule.followUp.backlinks.Add(workflow);
            }
        }
    }

    public static void DoVariables(string[] lines)
    {
        long resultPartOne = 0;
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (!line.StartsWith('{')) continue;

            Console.WriteLine($"{line}");
            
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
        public enum Op { Greater, Lesser, None };

        public char op;
        public string variable;
        public long value;
        public Workflow followUp;
    }

    public class RuleGreater : Rule
    {
        
    }

    public class RuleLesser : Rule
    {
        
    }

    public class RuleFallback : Rule
    {
        
    }

    public static Dictionary<string, long> variables = new();
    
    public static OrderedDictionary<string, Workflow> workflows = new();
    
    public string name;
    public List<Rule> rules = new();
    public List<Workflow> backlinks = new();
}
