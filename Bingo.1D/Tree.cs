using Bingo.One.Patterns;

namespace Bingo.One;

public class Tree<TElement>
{
    public readonly Pattern<TElement> Root = new Sequence<TElement>();

    public List<object> Parse(Source<TElement> source)
    {
        var session = new Session<TElement>(Root, source);

        while (!source.Empty)
        {
            session.Move();
        }
        
        return session.Productions;
    }

    public void Merge(IEnumerable<Pattern<TElement>> patterns)
    {
        var position = Root;
        foreach (var pattern in patterns)
        {
            var found = false;
            foreach (var continuation in position.Continuations)
            {
                if (!continuation.Equivalent(pattern))
                    continue;
                position = continuation;
                found = true;
                break;
            }
            if (found)
                continue;
            position.Continuations.Add(pattern);
            position = pattern;
        }
    }
}