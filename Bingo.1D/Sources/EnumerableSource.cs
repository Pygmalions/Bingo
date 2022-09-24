namespace Bingo.One.Sources;

public class EnumerableSource<TElement> : Source<TElement>
{
    private readonly IEnumerable<TElement> _container;
    private IEnumerator<TElement> _enumerator;

    public EnumerableSource(IEnumerable<TElement> container)
    {
        _container = container;
        _enumerator = _container.GetEnumerator();
    }

    public void Reset()
    {
        _enumerator = _container.GetEnumerator();
    }

    protected override bool Acquire(ref TElement? element)
    {
        if (!_enumerator.MoveNext())
            return false;
        element = _enumerator.Current;
        return true;
    }
}