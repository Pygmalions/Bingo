using Bingo.One.Patterns;

namespace Bingo.One;

public class Tree<TElement>
{
    private readonly Node<TElement> _root = new(new FunctorPattern<TElement>(_ => true));

    public Func<TElement[], object>? Fallback
    {
        get => _root.Generator;
        set => _root.Generator = value;
    }

    public void Merge(IEnumerable<Pattern<TElement>> patterns, Func<TElement[], object> generator)
    {
        var position = _root;
        foreach (var pattern in patterns)
        {
            var found = false;
            foreach (var branch in position.Next)
            {
                if (!branch.Pattern.Equivalent(pattern))
                    continue;
                found = true;
                position = branch;
                break;
            }

            if (found)
                continue;
            var next = new Node<TElement>(pattern);
            position.Next.Add(next);
            position = next;
        }

        position.Generator = generator;
    }
    
    public List<object> Parse(Source<TElement> source)
    {
        var productions = new List<object>();

        var position = _root;
        var generator = _root.Generator;
        var nodes = new Stack<Node<TElement>>();
        var context = new Context<TElement>(source);

        void Generate()
        {
            var sequence = context.Commit();
            if (generator != null)
                productions!.Add(generator(sequence));
        }

        void GoToRoot()
        {
            nodes.Clear();
            position = _root;
            generator = _root.Generator;
        }
        
        while (!context.Empty)
        {
            if (position.Pattern.Match(context))
            {
                generator = position.Generator;
                // Meet the end of this pattern branch.
                if (position.Next.Count == 0)
                {
                    Generate();
                    GoToRoot();
                    continue;
                }
                for (var index = position.Next.Count - 1; index > 0; index--)
                {
                    // Add enough checkpoints for alternative branches.
                    context.AddCheckpoint();
                    nodes.Push(position.Next[index]);
                }
                nodes.Push(position.Next[0]);
            }
            else
                // Rollback to the previous checkpoint.
                context.Rollback();

            // Check whether this iteration reaches the end of all possible branches.
            if (nodes.TryPop(out var next))
            {
                position = next;
                continue;
            }
            
            // This current element is not consumed by any pattern.
            if (context.ConsumedCount == 0)
                context.Consume();
            Generate();

            // Go back to the root.
            GoToRoot();
        }
        // Handle the remained tail sequence.
        if (context.ConsumedCount > 0)
            Generate();

        return productions;
    }
}