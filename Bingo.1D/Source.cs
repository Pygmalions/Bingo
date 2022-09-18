namespace Bingo.One;

public abstract class Source<TElement>
{
    public TElement? Current { get; set; }

    /// <summary>
    /// Whether the source is empty or not.
    /// </summary>
    public bool Empty { get; protected set; }

    /// <summary>
    /// Buffered elements, which mainly came from the transaction rollback.
    /// </summary>
    private readonly Stack<TElement> _bufferedElements = new();
    /// <summary>
    /// Consumed elements of the current transaction.
    /// </summary>
    private readonly Stack<TElement> _consumedElements = new();

    /// <summary>
    /// Count of consumed elements in this transaction.
    /// </summary>
    public int ConsumedCount => _consumedElements.Count;

    /// <summary>
    /// Count of buffered elements in this transaction.
    /// </summary>
    public int BufferedCount => _bufferedElements.Count;
    
    /// <summary>
    /// Implement this method to provide elements.
    /// </summary>
    /// <returns></returns>
    protected abstract bool Acquire(out TElement? element);

    private bool _initialized = false;

    /// <summary>
    /// Store the current element if it is not null and this source has been initialized,
    /// then acquire a new element from the source.
    /// </summary>
    public bool Consume()
    {
        // Store the current element into the transaction if this source has initialized.
        if (_initialized && Current is not null)
        {
            _consumedElements.Push(Current);
        }
        _initialized = true;

        // Try to acquire an element from the buffer.
        if (_bufferedElements.TryPop(out var element))
        {
            Current = element;
            return true;
        }

        // Acquire an element from the Acquire(...) method.
        if (Acquire(out element))
        {
            Current = element;
            return true;
        }
        // The empty flag will be set to true every time the Acquire(...) method returns a false.
        Empty = true;
        return false;
    }

    /// <summary>
    /// Return the elements of the current transaction into the buffer queue,
    /// which can be consumed again. 
    /// </summary>
    /// <returns>Count of effected consumed elements.</returns>
    public int Rollback(int count = 1)
    {
        if (_initialized || _consumedElements.Count == 0)
            return 0;
        if (Current != null)
            _bufferedElements.Push(Current);
        for (var index = 0; index < count; index++)
        {
            if (!_consumedElements.TryPop(out var element))
            {
                count = index;
                break;
            }
            _bufferedElements.Push(element);
        }
        Current = _bufferedElements.Pop();

        return count;
    }
    
    /// <summary>
    /// Return the elements queue of the current transaction,
    /// and then start a new transaction.
    /// </summary>
    /// <returns>Queue of committed elements.</returns>
    public TElement[] Commit()
    {
        var length = _consumedElements.Count;
        var sequence = new TElement[length];
        for (var index = 1; _consumedElements.TryPop(out var element); index++)
            sequence[length - index] = element;
        return sequence;
    }
}