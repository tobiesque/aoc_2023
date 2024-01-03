using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day20
{
    public static void Run()
    {
        string[] lines = Day.Str.ReadLinesArray(test: false);

        
        Machines machines = new(lines);
        machines.WriteCsv("machines.csv");
        return;
        
        Console.WriteLine(machines);

        if (Day.PartOne)
        {
            machines.PressButtons(1000);
            Console.WriteLine($"{machines.counterLow} / {machines.counterHigh}");
            long result = machines.counterHigh * machines.counterLow;
            Console.WriteLine($"Part {Day.PartStr}: {result}");
            return;
        }

        Machines.verbose = false;
        // machines.WriteDot("day20.dot");
        
        // find patterns
        machines.PressButtons(100000000);
    }

    public class Machines
    {
        public static Machines machines;
        public long counterHigh = 0;
        public long counterLow = 0;
        public static bool verbose = true;

        public struct Transition
        {
            public Module from;
            public bool high;
            public Module to;
        }
        public List<Transition> sends = new();
        
        public class Module
        {
            public string name;
            public Module[] destinations;
            public Module[] inputs;

            public virtual void ReceivePulse(bool high, Module from)
            {
                if (verbose)
                {
                    string lowHighName = high ? "-high" : "-low";
                    Console.WriteLine($"{from.name} {lowHighName} -> {name}");
                }
            }

            public void SendPulse(bool high)
            {
                machines.sends.AddRange(destinations.Select(d => new Transition(){ from = this, high = high, to = d }));
            }

            public virtual void Reset() {}
        }

        public class FlipFlop : Module
        {
            public bool state = false;
            public int bitIndex;

            public bool hasPattern = false;
            public int patternZeroes = 0;
            public int patternOnes = 0;
            public bool lastState = true;

            public bool WorkOutPattern()
            {
                if (hasPattern) return false;
                if (state != lastState)
                {
                    if ((patternOnes > 0) && (patternZeroes > 0))
                    {
                        hasPattern = true;
                        return true;
                    }
                }
                
                if (!state)
                {
                    ++patternZeroes;
                }
                else
                {
                    ++patternOnes;
                }

                lastState = state;
                return false;
            }

            public string PatternToString() => string.Format("{0," + bitIndex + "} {1}{2}", 
                                                             "^",
                                                             '0'.Replicate(patternZeroes), 
                                                             '1'.Replicate(patternOnes));
            
            public string PatternToString2()
            {
                return $"{bitIndex}: {patternZeroes}-{patternOnes}";
            }

            public string StateToString() => state ? "on" : "off";

            public override void ReceivePulse(bool high, Module from)
            {
                base.ReceivePulse(high, from);
                if (high) return;
                
                state = !state;
                if (state) machines.state |= 1UL << bitIndex;
                else machines.state &= ~(1UL << bitIndex);
                SendPulse(state);
            }

            public override void Reset()
            {
                state = false;
            }
        }

        public class Conjunction : Module
        {
            public override void ReceivePulse(bool high, Module from)
            {
                base.ReceivePulse(high, from);
                if (high) pulseMemory.Add(from); else pulseMemory.Remove(from);
                bool sendLow = (pulseMemory.Count == inputs.Length);
                SendPulse(!sendLow);
            }

            public override void Reset()
            {
                pulseMemory.Clear();
            }
            
            public HashSet<Module> pulseMemory = new();
        }

        public class Broadcaster : Module
        {
            public override void ReceivePulse(bool high, Module from)
            {
                base.ReceivePulse(high, from);
                SendPulse(high);
            }
        }
        
        public class Output : Module
        {
        }
        
        public SortedDictionary<string, Module> modules;
        public Broadcaster broadcaster;
        public Module button = new () { name = "button" };
        public Module rx;

        public void Reset()
        {
            foreach (var module in modules.Values) module.Reset();
        }

        public int workedOutPatterns = 0;
        public void WorkOutPatterns()
        {
            foreach (FlipFlop flipFlop in modules.Values.OfType<FlipFlop>())
            {
                if (flipFlop.WorkOutPattern())
                {
                    ++workedOutPatterns;
                    Console.WriteLine(flipFlop.PatternToString());
                }
            }
        }
        
        public bool PressButton()
        {
            int counterHighSingle = 0;
            int counterLowSingle = 0;
            
            broadcaster.ReceivePulse(false, button);
            ++counterLowSingle;
            
            while (sends.Count > 0)
            {
                var sends2 = sends.ToArray();
                sends.Clear();
                foreach (var send in sends2)
                {
                    if ((send.to == rx) && !send.high)
                    {
                        return true;
                    }
                    send.to.ReceivePulse(send.high, send.from);
                    if (send.high) ++counterHighSingle; else ++counterLowSingle;
                }
            }
            
            counterHigh += counterHighSingle;
            counterLow += counterLowSingle;
            if (verbose)
            {
                Console.WriteLine($"Signals: {counterHighSingle} high, {counterLowSingle} low" +
                                         $" ({counterHigh} high, {counterLow} low)");
            }

            return false;
        }

        public ulong state = 0;
        
        public void PressButtons(long n)
        {
            ulong startState = state;
            ulong lastState = state;
            if (verbose)
            {
                Console.WriteLine($">>> {FlipFlopsToString()} <<< \n");
            }

            PrintState(state, 0);

            var startC = ConjunctionStates();
            Console.WriteLine($"Conjunctions: {ConjunctionsToString()}");
            
            if (!Day.PartOne && false)
            {
                WorkOutPatterns();
            }

            for (long i = 1; i <= n; ++i)
            {
                PressButton();
                PrintState(state, i);
                Console.WriteLine($"Conjunctions: {ConjunctionsToString()}");

                if (startC.SequenceEqual(ConjunctionStates()))
                {
                    Console.WriteLine("CCycle");
                }
                
                if (!Day.PartOne && false)
                {
                    WorkOutPatterns();
                    if (workedOutPatterns == 48)
                    {
                        foreach (var flipFlop in modules.Values.OfType<FlipFlop>())
                        {
                            Console.WriteLine(flipFlop.PatternToString2());
                        }
                        return;
                    }
                }

                if (state == startState)
                {
                    Console.WriteLine($"Cycle at {i}");
                }

                if (verbose)
                {
                    Console.WriteLine($">>> {FlipFlopsToString()} <<< \n");
                }
                
                lastState = state;
            }
        }

        public void PrintState(ulong states, long i)
        {
            string binary = $"{Convert.ToString((long)states, 2),48}".Replace(' ', '0');
            Console.WriteLine($"{binary} / {states} ({i})");
        }

        public Machines(string[] lines)
        {
            machines = this;
            modules = new();
            foreach (var line in lines)
            {
                Module module;
                string[] decl = line.Replace(" ", "").Split("->");
                if (decl[0].StartsWith("%"))
                {
                    module = new FlipFlop() { name = decl[0][1..] };

                }
                else if (decl[0].StartsWith("&"))
                {
                    module = new Conjunction() { name = decl[0][1..] };
                }
                else
                {
                    Debug.Assert(decl[0] == "broadcaster");
                    broadcaster = new Broadcaster() { name = decl[0] };
                    module = broadcaster;
                }

                modules.Add(module.name, module);
            }

            foreach (var line in lines)
            {
                string[] decl = line.Replace(" ", "").Split("->");
                string name = decl[0].Trim('%').Trim('&');
                Module module = modules[name];

                string[] destinations = decl[1].Split(',');
                foreach (var dName in destinations)
                {
                    if (!modules.TryGetValue(dName, out Module destination))
                    {
                        destination = new Output() { name = dName, destinations = [] };
                        modules[dName] = destination;
                    }
                }

                module.destinations = destinations.Select(d => modules[d]).ToArray();
            }

            foreach (var module in modules.Values)
            {
                module.inputs = modules.Values.Where(m => m.destinations.Contains(module)).ToArray();
            }

            int i = 0;
            foreach (var conjunction in modules.Values.OfType<FlipFlop>())
            {
                conjunction.bitIndex = 48 - i;
                ++i;
            }
            
            rx = modules.Values.First(m => m.name == "rx");
            // Console.WriteLine($"rx is a {rx.GetType().ToString()} with {rx.inputs.Length} inputs");

            if (!Day.PartOne)
            {
                var flipFlopStates = FlipFlopStates();
                Debug.Assert(flipFlopStates.Length == 48);
            }
        }

        public int[] ConjunctionStates()
        {
            return modules.Values.OfType<Conjunction>().Select(m => m.inputs.Length - m.pulseMemory.Count).ToArray();
        }
        
        public string ConjunctionsToString()
        {
            return string.Join(", ", modules.Values.OfType<Conjunction>().Select(m => 
                                        (m.pulseMemory.Count).Bin(4)));
        }
        
        public BitArray FlipFlopStates()
        {
            return new BitArray(modules.Values.OfType<FlipFlop>().Select(m => m.state).ToArray());
        }
        
        public string FlipFlopsToString()
        {
            return string.Join(", ", modules.Values.OfType<FlipFlop>().Select(m => $"({m.name}: {m.StateToString()})"));
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (Module module in modules.Values)
            {
                sb.AppendLine($"{module.name} -> {module.destinations.Select(d => d.name).MakeList()}");
            }
            return sb.ToString();
        }
        
        public void WriteCsv(string filename)
        {
            StringBuilder sb = new();
            sb.AppendLine("name_from,name_to");
            foreach (Module from in modules.Values)
            {
                foreach (Module to in from.destinations)
                {
                    sb.AppendLine($"\t{from.name},{to.name}");
                }
            }
            File.WriteAllText(filename, sb.ToString());
        }
    }
}