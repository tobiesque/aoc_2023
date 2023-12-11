int day = 8;
int part = 2;









Type dayClass = Type.GetType($"Aoc2023Cs.Day{day}")!;
if (dayClass == null)
{
    string partStr = (part == 1) ? "one" : "two";
    dayClass = Type.GetType($"Aoc2023Cs.Day{day}{partStr}")!;
}
System.Reflection.MethodInfo runMethod = dayClass.GetMethod("Run")!;
object?[]? argument = ( runMethod.GetParameters().Length != 0 ) ? new [] { (object?)part } : null;
runMethod.Invoke(null, argument );
