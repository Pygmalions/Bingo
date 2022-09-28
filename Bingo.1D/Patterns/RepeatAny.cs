namespace Bingo.One.Patterns;

/// <summary>
/// Try to match the condition for any times (includes 0),
/// which means that this pattern will always match.
/// </summary>
public class RepeatAny<TElement> : Pattern<TElement>
{
    public readonly Pattern<TElement> Condition;

    public RepeatAny(Pattern<TElement> condition)
    {
        Condition = condition;
    }

    public override bool Match(Context<TElement> context)
    {
        while (Condition.Match(context))
        {}
        return true;
    }

    public override bool Equivalent(Pattern<TElement> target) => target is RepeatAny<TElement>;
}