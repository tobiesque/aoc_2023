using System.Runtime.InteropServices;

namespace Aoc2023Cs;

public partial class Day6
{
    public struct Race2
    {
        public ulong duration;
        public ulong record;
      
        public readonly ulong WaysToWin()
        {
            ulong waysToWin = 0;
            for (ulong tButtonDown = 0; tButtonDown < duration; ++tButtonDown)
            {
                ulong result = tButtonDown * (duration - tButtonDown);
                if (result > record)
                {
                    ++waysToWin;
                }
            }
            return waysToWin;
        }
    }
   
    public static void RunTwo()
    {
        var lines = "6".ReadLinesArray(test: false);
        lines[0].AsSpan().ExtractInt(out ulong duration);
        lines[1].AsSpan().ExtractInt(out ulong record);
        Race2 race = new() { duration = duration, record = record }; 
        ulong score = race.WaysToWin();
      
        Console.WriteLine($"Part Two: {score}");
    }
}