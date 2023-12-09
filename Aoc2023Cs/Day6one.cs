using System.Runtime.InteropServices;

namespace Aoc2023Cs;

public partial class Day6
{
   public struct Race
   {
      public int duration;
      public int record;
      
      public readonly int WaysToWin()
      {
         int waysToWin = 0;
         for (int tButtonDown = 0; tButtonDown < duration; ++tButtonDown)
         {
            int result = tButtonDown * (duration - tButtonDown);
            if (result > record)
            {
               ++waysToWin;
            }
         }
         return waysToWin;
      }
   }
   
   public static void Run(int part)
   {
      if (part == 1) RunOne(); else RunTwo();
   }
   
   public static void RunOne()
   {
      var lines = "6".ReadLinesArray(test: false);
      Span<char> timeLine = lines[0].AsSpan();
      Span<char> distanceLine = lines[1].AsSpan();

      var races = new List<Race>();
      
      timeLine = timeLine.After(':');
      while (timeLine.Length > 0)
      {
         Race race = new();
         timeLine.SkipWhiteRef().ExtractIntRef(out race.duration);
         races.Add(race);
      }
      
      distanceLine = distanceLine.After(':');
      foreach (ref Race race in CollectionsMarshal.AsSpan(races))
      {
         distanceLine.SkipWhiteRef().ExtractIntRef(out race.record);
      }

      int score = 1;
      foreach (Race race in races)
      {
         score *= race.WaysToWin();
      }
      
      Console.WriteLine($"Part One: {score}");
   }
}
