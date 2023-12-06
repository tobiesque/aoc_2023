namespace Aoc2023Cs;

public class Day6two
{
    public struct Race
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
   
    public static void Run()
    {
        string[] lines = Util.ReadLines("6", test: false).ToArray();
        string duration = String.Concat(lines[0].Where(char.IsDigit));
        string record = String.Concat(lines[1].Where(char.IsDigit));
        Race race = new() { duration = ulong.Parse(duration), record = ulong.Parse(record) }; 
        ulong score = race.WaysToWin();
      
        Console.WriteLine($"Part Two: {score}");
    }
}