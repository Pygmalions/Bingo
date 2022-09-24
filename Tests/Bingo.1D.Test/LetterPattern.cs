namespace Bingo.One.Test;

public class LetterPattern : Pattern<char>
{
    public readonly char Letter;

    public LetterPattern(char letter)
    {
        Letter = letter;
    }
    
    public override bool Match(Context<char> context)
    {
        if (context.Current != Letter) 
            return false;
        context.Consume();
        return true;

    }

    public override bool Equivalent(Pattern<char> target)
    {
        if (target is LetterPattern pattern)
            return Letter == pattern.Letter;
        return false;
    }
}