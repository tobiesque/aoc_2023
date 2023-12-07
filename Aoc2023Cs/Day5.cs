using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day5
{
    public class Type(int id, string name = "")
    {
        public int id = id;
        public string name = name;
    }
    
    public class RangeMap
    {
        public Type sourceType { get; set; }
        public int source;
        public int destination;
        public int size;
    }

    public class Spot
    {
        public List<int> seeds = new();
        public Util.MultiMap<Type, RangeMap> ranges = new();
        public Dictionary<int, Type> types = new();

        public Type location;

        public Type AddType(int typeId, string name = "")
        {
            Type type = new(typeId, name);
            types.Add(typeId, type);
            ranges.MultiAdd(type, new RangeMap());
            return type;
        }

        public RangeMap AddRangeMap(int typeId, int[] entries)
        {
            RangeMap rangeMap = new()
                {
                    sourceType = types[typeId],
                    source = entries[0],
                    destination = entries[1],
                    size = entries[2]
                };
            ranges.MultiAdd(rangeMap.sourceType, rangeMap);
            return rangeMap;
        }

        public Type NextType(Type type) => types[type.id + 1];

        public int MapToLocation(Type type, int value)
        {
            if (type == location) return value;
            
            foreach (var rangeMap in ranges[type])
            {
                if (Util.Between(value, rangeMap.source, rangeMap.size))
                {
                    return MapToLocation(NextType(type), value - rangeMap.source + rangeMap.destination);
                }
            }
            
            return -1;
        }
    }
    public static void Run()
    {
        var lines = Util.ReadLines("5", test: true);

        Spot spot = new();

        // read seeds
        string seedsLine = lines.First()!.After(':');
        lines = lines.Skip(1);
        while (seedsLine.Length > 0)
        {
            seedsLine = seedsLine.ExtractSpacedInt(out int seedAmount);
            spot.seeds.Add(seedAmount);
        }
        lines = lines.SkipEmptyLines();

        // read mappings
        int typeId = 0;
        while (lines.Any())
        {
            // read map header
            string mapLine = lines.First()!;
            mapLine.ExtractString(out string mapDescriptor);
            string[] descriptors = mapDescriptor.Split('-');
            spot.AddType(typeId, descriptors[0]);
            if (descriptors[2] == "location")
            {
                spot.location = spot.AddType(typeId + 1, descriptors[2]);
            }
            ++typeId;

            // read map range entries
            lines = lines.Skip(1);
            while (lines.Any() && !string.IsNullOrWhiteSpace(lines.First()!))
            {
                var entries = lines.First()!.Split(' ').Select(int.Parse);
                spot.AddRangeMap(typeId, entries.ToArray());
                lines = lines.Skip(1);
            }
            lines = lines.Skip(1);
        }

        foreach (Type type in spot.types.Values)
        {
            Console.WriteLine($"{type.id} {type.name}");
        }
    }
}