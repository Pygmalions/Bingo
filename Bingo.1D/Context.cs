namespace Bingo.One;

public class Context<TElement>
{
    public Context(Source<TElement> source)
    {
        _source = source;
    }

    /// <summary>
    /// Bound element source.
    /// </summary>
    private readonly Source<TElement> _source;
    
    /// <summary>
    /// Current element.
    /// </summary>
    public TElement? Current => _source.Current;

    /// <summary>
    /// Count of elements consumed by current pattern.
    /// </summary>
    public int Consumption => _source.ConsumedCount;
    
    /// <summary>
    /// History of counts of consumed elements.
    /// </summary>
    private readonly Stack<int> _milestones = new();

    /// <summary>
    /// Mark a milestone for this context to rollback.
    /// </summary>
    internal void Mark()
    {
        if (Consumption > 0)
        {
            _milestones.Push(Consumption);
        }
    }
    
    /// <summary>
    /// Add the current element to the sequence and acquire a new one from the source.
    /// </summary>
    public void Consume()
    {
        _source.Consume();
    }

    /// <summary>
    /// Commit the current element sequence.
    /// </summary>
    /// <returns></returns>
    internal TElement[] Commit() => _source.Commit();

    /// <summary>
    /// Rollback to the nearest milestone.
    /// </summary>
    internal void Rollback()
    {
        if (_milestones.TryPop(out var consumption))
            _source.Rollback(consumption);
    }

    /// <summary>
    /// Additional data of this context.
    /// </summary>
    private readonly Dictionary<string, object> _data = new();

    /// <summary>
    /// Additional data indexer.
    /// </summary>
    /// <param name="name">Data item name.</param>
    public object? this[string name]
    {
        get => _data.TryGetValue(name, out var data) ? data : null;
        set
        {
            if (value != null)
                _data[name] = value;
            else _data.Remove(name);
        }
    }
}