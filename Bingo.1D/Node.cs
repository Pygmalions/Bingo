namespace Bingo.One;

internal class Node<TElement>
{
    public readonly Pattern<TElement> Pattern;
    
    public Node(Pattern<TElement> pattern)
    {
        Pattern = pattern;
    }

    /// <summary>
    /// Patterns followed by this pattern.
    /// </summary>
    public readonly List<Node<TElement>> Next = new();
    
    /// <summary>
    /// Generator to use if the elements satisfied this pattern.
    /// </summary>
    public Func<TElement[], object>? Generator;
}