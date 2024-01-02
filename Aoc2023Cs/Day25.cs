using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aoc2023Cs;

public class Day25
{
    public static void Run()
    {
        var lines = Day.Str.ReadLinesArray(test: false);
        Graph graph = new(lines);
        
        // graph.WritePy();
        // graph.WriteCsv();
        
        // three edges determined by using graph tools and trying different algorithms
        graph.Remove("vnm", "qpp");
        graph.Remove("vkp", "kfr");
        graph.Remove("rhk", "bff");

        Graph vnm = new Graph(graph.nodes["vnm"]);
        Graph qpp = new Graph(graph.nodes["qpp"]);
        
        Console.WriteLine(vnm.Size);
        Console.WriteLine(qpp.Size);
        
        Console.WriteLine($"Part One: {vnm.Size * qpp.Size}");
        
        // Console.WriteLine(graph);
    }
    
    public class Graph
    {
        public class Node
        {
            public string name;
            public int id;
            
            public HashSet<Node> connections = new();
        }

        public class Connection(Node source, Node target)
        {
            public Node from = source;
            public Node to = target;

            public override int GetHashCode()
            {
                return HashCode.Combine(RuntimeHelpers.GetHashCode(from), RuntimeHelpers.GetHashCode(to));
            }

            public Connection Inverse() => new(to, from);
        }

        public SortedDictionary<string, Node> nodes = new();
        public HashSet<Connection> connections = new();

        public Graph(string[] lines)
        {
            int i = 0;
            foreach (var line in lines)
            {
                string name = line.Split(':')[0].Trim();
                Node node = new() { name = name, id = i };
                nodes.Add(name, node);
                ++i;
            }
            
            foreach (var line in lines)
            {
                var split = line.Split(':');
                string name = split[0].Trim();
                Node from = nodes[name];
                string[] names = split[1].Trim().Split(' ');
                foreach (var connection in names)
                {
                    if (!nodes.TryGetValue(connection, out var to))
                    {
                        to = new() { name = connection, id = i };
                        nodes.Add(connection, to);
                        ++i;
                    }
                    from.connections.Add(to);
                    to.connections.Add(from);
                    connections.Add(new Connection(from, to));
                }
            }
        }

        public Graph(Node node)
        {
            AddR(node);
        }

        public void AddR(Node node)
        {
            if (!nodes.TryAdd(node.name, node))
            {
                return;
            }

            foreach (Node connected in node.connections)
            {
                AddR(connected);
            }
        }

        public void Remove(string from, string to)
        {
            Node fromNode = nodes[from];
            Node toNode = nodes[to];
            bool success = fromNode.connections.Remove(toNode);
            success &= toNode.connections.Remove(fromNode);
            Debug.Assert(success);
        }

        public int Size => nodes.Count;
        
        public void WritePy()
        {
            StringBuilder sb = new();
            sb.AppendLine("data = {");
            foreach (var node in nodes.Values)
            {
                string connected = node.connections.Select(c => c.name).MakeList("\", \"");
                sb.Append($"    \"{node.name}\": [ ");
                sb.Append($"\"{connected}\"");
                sb.AppendLine(" ],");
            }
            sb.AppendLine("}");
            File.WriteAllText("nodes.py", sb.ToString());
        }
        
        public void WriteCsv()
        {
            StringBuilder sb = new();
            sb.AppendLine("id,name");
            foreach (var node in nodes.Values)
            {
                sb.AppendLine($"{node.id},{node.name}");
            }
            File.WriteAllText("nodes.csv", sb.ToString());

            sb.Clear();
            sb.AppendLine("id_from,id_to");
            foreach (var connection in connections)
            {
                sb.AppendLine($"{connection.from.id},{connection.to.id}");
                sb.AppendLine($"{connection.to.id},{connection.from.id}");
            }
            File.WriteAllText("connections.csv", sb.ToString());
        }
        
        public void WriteDot(string filename)
        {
            StringBuilder sb = new();
            sb.AppendLine("digraph day20 {");
            sb.AppendLine("\tnode [fontsize=25.0 width=0.5];");
            foreach (var connection in connections)
            {
                sb.AppendLine($"{connection.from.name}->{connection.to.name}");
            }
            sb.AppendLine("}");
            File.WriteAllText(filename, sb.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (var connected in connections)
            {
                sb.AppendLine($"{connected.from.name}<->{connected.to.name}");
            }
            sb.AppendLine($"{connections.Count} connections");
            return sb.ToString();
        }
    }
    
}