namespace Bingo.One.Patterns;

public class AnyOf<TElement> : Pattern<TElement>
{
    public readonly IEnumerable<Pattern<TElement>> Conditions;

    public AnyOf(IEnumerable<Pattern<TElement>> conditions)
    {
        Conditions = conditions;
    }
    
    public override bool Match(Context<TElement> context)
    {
        return Conditions.Any(condition => condition.Match(context));
    }
}