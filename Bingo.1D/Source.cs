namespace Bingo.One;

public abstract class Source<TElement>
{
    /// <summary>
    /// Whether the current element is valid or not.
    /// </summary>
    public bool Valid { get; protected set; }

    /// <summary>
    /// Current element. It is nullable only when TElement is a reference type.
    /// </summary>
    private TElement? _current;

    /// <summary>
    /// Consumed elements.
    /// </summary>
    private readonly Stack<TElement> _consumedElements = new();

    public int ConsumedCount => _consumedElements.Count;
    
    /// <summary>
    /// Buffered elements.
    /// </summary>
    private readonly Stack<TElement> _bufferedElements = new();

    public int BufferedCount => _bufferedElements.Count;
    
    /// <summary>
    /// The current element to process.
    /// </summary>
    public TElement Current
    {
        get
        {
            if (_current != null && Valid)
                return _current;
            throw new InvalidOperationException("Current element is invalid.");
        }
    }

    protected abstract bool Acquire(ref TElement? element);

    /// <summary>
    /// Commit and pick out the current element sequence.
    /// </summary>
    /// <returns>Consumed elements sequence.</returns>
    public TElement[] Commit()
    {
        var length = _consumedElements.Count;
        var sequence = new TElement[length];
        for (var index = 1; index <= length; index++)
        {
            sequence[length - index] = _consumedElements.Pop();
        }

        return sequence;
    }

    /// <summary>
    /// Rollback the current element sequence.
    /// </summary>
    /// <param name="count">
    /// Count of consumed elements to rollback.
    /// If it is negative, then all consumed elements will be rolled back.
    /// </param>
    /// <returns>Count of effected elements.</returns>
    public int Rollback(int count = -1)
    {
        if (count == 0)
            return 0;
        if (count < 0)
            count = _consumedElements.Count;
        for (var index = 0; index < count; index++)
        {
            if (!_consumedElements.TryPop(out var element))
            {
                count = index;
                break;
            }
            _bufferedElements.Push(element);
        }

        // Update the current element while not push the former one into the stack.
        Valid = false;
        Consume();
        return count;
    }

    /// <summary>
    /// Consume the current element and get the next element.
    /// </summary>
    public bool Consume()
    {
        if (Valid && _current != null)
            _consumedElements.Push(_current);
        var valid = false;
        
        if (_bufferedElements.TryPop(out var element))
        {
            _current = element;
            valid = true;
        } else if (Acquire(ref _current))
        {
            valid = true;
        }
        else
        {
            _current = default;
        }

        Valid = valid;
        return valid;
    }
}