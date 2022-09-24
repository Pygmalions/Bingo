namespace Bingo.One.Patterns;

public class AllOf<TElement> : Pattern<TElement>
{
    public readonly IEnumerable<Pattern<TElement>> Conditions;

    public AllOf(IEnumerable<Pattern<TElement>> conditions)
    {
        Conditions = conditions;
    }

    public override bool Match(Context<TElement> context)
    {
        return Conditions.All(condition => condition.Match(context));
    }
}