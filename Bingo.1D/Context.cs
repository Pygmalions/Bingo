namespace Bingo.One;

public class Context<TElement>
{
    private readonly Source<TElement> _source;

    public Context(Source<TElement> source)
    {
        _source = source;
        // Prepare the first element.
        _source.Consume();
    }

    /// <summary>
    /// The current element to process.
    /// </summary>
    public TElement Current => _source.Current;

    /// <summary>
    /// If true, the current element is valid and accessible, otherwise false.
    /// </summary>
    public bool Empty => !_source.Valid;

    public int ConsumedCount => _source.ConsumedCount;

    public int BufferedCount => _source.BufferedCount;

    /// <summary>
    /// Count of consumed elements.
    /// </summary>
    private int _consumption;

    /// <summary>
    /// Stack of consumptions in every checkpoint.
    /// </summary>
    private readonly Stack<int> _checkpoints = new();

    /// <summary>
    /// Add a checkpoint at the current position.
    /// </summary>
    public void AddCheckpoint()
    {
        _checkpoints.Push(_consumption);
        _consumption = 0;
    }

    /// <summary>
    /// Remove the nearest checkpoint.
    /// </summary>
    public void RemoveCheckpoint()
    {
        if (_checkpoints.TryPop(out var consumption))
            _consumption += consumption;
    }

    /// <summary>
    /// Consume the current element and get the next one.
    /// </summary>
    /// <returns></returns>
    public bool Consume()
    {
        if (!_source.Consume())
            return false;
        _consumption++;
        return true;
    }

    /// <summary>
    /// Commit all consumed elements, ignore and clear all checkpoints.
    /// </summary>
    /// <returns>Consumed elements.</returns>
    public TElement[] Commit()
    {
        _consumption = 0;
        _checkpoints.Clear();
        return _source.Commit();
    }

    /// <summary>
    /// Rollback to the nearest checkpoint.
    /// </summary>
    public void Rollback()
    {
        _source.Rollback(_consumption);
        _consumption = _checkpoints.TryPop(out var consumption) ? consumption : 0;
    }
}