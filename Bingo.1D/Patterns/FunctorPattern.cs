namespace Bingo.One.Patterns;

public class FunctorPattern<TElement> : Pattern<TElement>
{
    public Predicate<Context<TElement>> Functor;

    public FunctorPattern(Predicate<Context<TElement>> functor)
    {
        Functor = functor;
    }

    public override bool Match(Context<TElement> context) => Functor(context);

    public override bool Equivalent(Pattern<TElement> target)
    {
        if (target is FunctorPattern<TElement> pattern)
            return Functor == pattern.Functor;
        return false;
    }
}