namespace Bingo.One.Patterns;

public class RepeatAtMost<TElement> : Pattern<TElement>
{
    public readonly Pattern<TElement> Condition;
    public readonly uint Times;

    public RepeatAtMost(uint times, Pattern<TElement> condition)
    {
        Times = times;
        Condition = condition;
    }

    public override bool Match(Context<TElement> context)
    {
        var count = 0;
        while (Condition.Match(context))
        {
            count++;
            if (count > Times)
                break;
        }
            

        return count <= Times;
    }

    public override bool Equivalent(Pattern<TElement> target)
    {
        if (target is RepeatAtMost<TElement> pattern)
            return Times == pattern.Times;
        return false;
    }
}