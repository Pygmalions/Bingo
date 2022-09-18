namespace Bingo.One;

public abstract class Pattern<TElement>
{
    /// <summary>
    /// Continuations followed by this pattern.
    /// </summary>
    public readonly List<Pattern<TElement>> Continuations = new();

    /// <summary>
    /// Generator to generate new element with the matched sequence.
    /// </summary>
    public Func<TElement[], object>? Generator;

    /// <summary>
    /// Match with the specified context.
    /// </summary>
    /// <param name="context">Context to use.</param>
    /// <returns>
    /// True if this pattern can match one or more elements in the context;
    /// otherwise false.
    /// </returns>
    public abstract bool Match(Context<TElement> context);

    /// <summary>
    /// Check whether this pattern is equivalent to the specified pattern.
    /// This method will return false by default. <br/>
    /// This method is used for merging pattern sequences in the pattern tree.
    /// </summary>
    /// <param name="target">The target pattern to compare.</param>
    /// <returns>
    /// True if this pattern is equivalent to the specified pattern,
    /// otherwise false.
    /// </returns>
    public virtual bool Equivalent(Pattern<TElement> target) => false;
}