using System.Reflection;

int day = 19;
int part = 2;



string binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location!)!;
Directory.SetCurrentDirectory(binPath);

Type? dayClass = Type.GetType($"Aoc2023Cs.Day{day}");
if (dayClass == null)
{
    string partStr = (part == 1) ? "one" : "two";
    dayClass = Type.GetType($"Aoc2023Cs.Day{day}{partStr}")!;
}
MethodInfo runMethod = dayClass.GetMethod("Run")!;
object?[]? argument = ( runMethod.GetParameters().Length != 0 ) ? new [] { (object?)part } : null;
runMethod.Invoke(null, argument );
