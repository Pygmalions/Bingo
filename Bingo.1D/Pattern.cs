namespace Bingo.One;

public abstract class Pattern<TElement>
{
    public abstract bool Match(Context<TElement> context);

    public virtual bool Equivalent(Pattern<TElement> target) => false;
}