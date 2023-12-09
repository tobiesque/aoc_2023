using System.Reflection;

int day = 7;
int part = 1;

Type dayClass = Type.GetType($"Aoc2023Cs.Day{day}")!;
MethodInfo runMethod = dayClass.GetMethod("Run")!;
object?[]? argument = ( runMethod.GetParameters().Length != 0 ) ? new [] { (object?)part } : null;
runMethod.Invoke(null, argument );
