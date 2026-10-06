namespace Hs.LangSense;
public interface IValueMiddleware
{
    ValueResult<string> Invoke(ValuePipelineContext context, Func<ValuePipelineContext, ValueResult<string>> next);
}
public sealed class ValuePipelineContext
{
    public object? Raw { get; init; }
    public string Value { get; set; } = ""; public ValueResult<string>? Result { get; set; }
    public Dictionary<string, object?> Items { get; } = []; public ValuePipelineContext(object? raw) { Raw = raw; Value = Convert.ToString(raw) ?? ""; }
}
public sealed class ValuePipeline
{
    private readonly List<IValueMiddleware> _middlewares = []; public ValuePipeline Use(IValueMiddleware m) { _middlewares.Add(m); return this; }
    public ValueResult<string> Execute(object? value) { var c = new ValuePipelineContext(value); Func<ValuePipelineContext, ValueResult<string>> terminal = x => { x.Result = SmartValue.Analyze(x.Value); return x.Result; }; foreach (var m in _middlewares.AsEnumerable().Reverse()) { var next = terminal; terminal = ctx => m.Invoke(ctx, next); } return terminal(c); }
}
public sealed class NormalizeMiddleware : IValueMiddleware
{
    public ValueResult<string> Invoke(ValuePipelineContext c, Func<ValuePipelineContext, ValueResult<string>> next) { c.Value = Normalizer.Normalize(c.Value); return next(c); }
}
public sealed class ValidationMiddleware(IRule[] rules) : IValueMiddleware
{
    public ValueResult<string> Invoke(ValuePipelineContext c, Func<ValuePipelineContext, ValueResult<string>> next)
    {
        var r = next(c); var issues = rules.Select(x => x.Validate(c.Value)).Where(x => x != null).Cast<ValidationIssue>().ToList(); r.Errors.AddRange(issues); r.IsValid = r.IsValid && issues.Count == 0; r.Success = r.IsValid; return r;
    }
}
public sealed class SensitiveScrubMiddleware : IValueMiddleware
{
    public ValueResult<string> Invoke(ValuePipelineContext c, Func<ValuePipelineContext, ValueResult<string>> next)
    {
        c.Value = SensitiveData.Mask(c.Value); return next(c);
    }
}
public sealed class CustomMiddleware(Func<ValuePipelineContext, ValuePipelineContext> transform) : IValueMiddleware
{
    public ValueResult<string> Invoke(ValuePipelineContext c, Func<ValuePipelineContext, ValueResult<string>> next) => next(transform(c));
}
