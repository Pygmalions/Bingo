namespace Bingo.One.Patterns;

public class RepeatAtLeast<TElement> : Pattern<TElement>
{
    public readonly Pattern<TElement> Condition;
    public readonly uint Times;

    public RepeatAtLeast(uint times, Pattern<TElement> condition)
    {
        Times = times;
        Condition = condition;
    }

    public override bool Match(Context<TElement> context)
    {
        var count = 0;
        while (Condition.Match(context))
            count++;

        return count >= Times;
    }

    public override bool Equivalent(Pattern<TElement> target)
    {
        if (target is RepeatAtLeast<TElement> pattern)
            return Times == pattern.Times;
        return false;
    }
}