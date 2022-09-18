namespace Bingo.One.Patterns;

public class Sequence<TElement> : Pattern<TElement>
{
    public Sequence() {}

    public Sequence(params Pattern<TElement>[] patterns)
    {
        Continuations.AddRange(patterns);
    }

    public override bool Match(Context<TElement> context) => true;
}