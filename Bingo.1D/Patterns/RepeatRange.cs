namespace Bingo.One.Patterns;

public class RepeatRange<TElement> : Pattern<TElement>
{
    public readonly Pattern<TElement> Condition;
    public readonly uint LowerBound; 
    public readonly uint UpperBound;

    public RepeatRange(uint lowerBound, uint upperBound, Pattern<TElement> condition)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
        Condition = condition;
    }

    public override bool Match(Context<TElement> context)
    {
        var count = 0;
        while (Condition.Match(context))
        {
            count++;
            if (count > UpperBound)
                break;
        }

        return count >= LowerBound && count <= UpperBound;
    }

    public override bool Equivalent(Pattern<TElement> target)
    {
        if (target is RepeatRange<TElement> pattern)
            return LowerBound == pattern.LowerBound && UpperBound == pattern.UpperBound;
        return false;
    }
}