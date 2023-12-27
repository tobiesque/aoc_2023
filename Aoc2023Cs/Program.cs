using System.Reflection;
using Aoc2023Cs;

string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location!)!;
Directory.SetCurrentDirectory(binPath);

Type? dayClass = Type.GetType($"Aoc2023Cs.Day{Day.day}");
if (dayClass == null)
{
    dayClass = Type.GetType($"Aoc2023Cs.Day{Day.day}{Day.PartStr.ToLower()}")!;
}
MethodInfo runMethod = dayClass.GetMethod("Run")!;
object?[]? argument = ( runMethod.GetParameters().Length != 0 ) ? new [] { (object?)Day.part } : null;
runMethod.Invoke(null, argument );
