using System.Runtime.InteropServices;

namespace Aoc2023Cs;

public class Day6one
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
   
   public static void Run()
   {
      string[] lines = Util.ReadLines("6", test: false).ToArray();
      string timeLine = lines[0];
      string distanceLine = lines[1];

      var races = new List<Race>();
      
      timeLine = timeLine.After(':');
      while (timeLine.Any())
      {
         Race race = new();
         timeLine = timeLine.SkipSpaces().ExtractInt(out race.duration);
         races.Add(race);
      }
      
      distanceLine = distanceLine.After(':');
      foreach (ref Race race in CollectionsMarshal.AsSpan(races))
      {
         distanceLine = distanceLine.SkipSpaces().ExtractInt(out race.record);
      }

      int score = 1;
      foreach (Race race in races)
      {
         score *= race.WaysToWin();
      }
      
      Console.WriteLine($"Part One: {score}");
   }
}
