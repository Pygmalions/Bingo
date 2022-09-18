namespace Bingo.One;

internal class Session<TElement>
{
    /// <summary>
    /// Context for this iterator to use.
    /// </summary>
    public readonly Context<TElement> Context;

    /// <summary>
    /// Root pattern for this iterator to start with.
    /// </summary>
    private readonly Pattern<TElement> _root;
    
    /// <summary>
    /// Generator to use when the matcher has no available path.
    /// </summary>
    public Func<TElement[], object>? Generator { get; set; }

    /// <summary>
    /// Next pattern waiting to be checked.
    /// </summary>
    public Pattern<TElement> Position { get; private set; }

    /// <summary>
    /// Generated elements.
    /// </summary>
    public readonly List<object> Productions = new();

    /// <summary>
    /// Patterns to continue.
    /// </summary>
    private readonly Stack<Pattern<TElement>> _continuations = new();
    
    public Session(Pattern<TElement> root, Source<TElement> source)
    {
        _root = root;
        Position = _root;
        Context = new Context<TElement>(source);
        // Get the first element ready.
        source.Consume();
    }

    public void Move()
    {
        if (Position.Match(Context))
        {
            if (Position.Continuations.Count > 1)
                Context.Mark();
            foreach (var continuation in Position.Continuations)
            {
                _continuations.Push(continuation);
            }

            Generator = Position.Generator ?? Generator;
        }
        else
        {
            Context.Rollback();
        }

        if (_continuations.TryPop(out var next))
        {
            Position = next;
            return;
        }
        
        // The current consumption being 0 means that no pattern can consume this element.
        if (Context.Consumption == 0)
        {
            // Consume this element to prevent it from entering the next loop.
            Context.Consume();
        }
        if (Generator != null)
            Productions.Add(Generator(Context.Commit()));
        // Discard the element sequence.
        else Context.Commit();
        
        Position = _root;
        Generator = _root.Generator;
    }
}