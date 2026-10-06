namespace Hs.LangSense;
public static class Pattern
{
    public static ValuePattern Define(string name, string expression, ValueType type = ValueType.Unknown,
        params IRule[] rules) => new(name, expression, type, rules);
    public static PatternBuilder Create(string name) => PatternBuilder.Create(name);
    public static void Register(ValuePattern pattern) => PatternRegistry.Register(pattern);
}
