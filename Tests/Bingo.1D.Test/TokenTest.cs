using System;
using System.Linq;
using Bingo.One.Sources;
using NUnit.Framework;

namespace Bingo.One.Test;

public class TokenTest
{
    public struct Token
    {
        public string Text;
        public string Name;

        public override string ToString()
        {
            return $"({Name}) {Text}";
        }
    }

    [Test]
    public void BasicTest()
    {
        var tree = new Tree<char>();

        tree.Root.Generator = (text) => new Token()
        {
            Text = new string(text),
            Name = "Token"
        };
        
        tree.Merge(new []
        {
            new LetterPattern('a'),
            new LetterPattern('s'),
            new LetterPattern('y'),
            new LetterPattern('n'),
            new LetterPattern('c')
            {
                Generator = (text) => new Token()
                {
                    Text = new string(text),
                    Name = "async"
                }
            }
        });
        
        tree.Merge(new []
        {
            new LetterPattern('a'),
            new LetterPattern('w'),
            new LetterPattern('a'),
            new LetterPattern('i'),
            new LetterPattern('t')
            {
                Generator = (text) => new Token()
                {
                    Text = new string(text),
                    Name = "await"
                }
            }
        });
        
        tree.Merge(new []
        {
            new LetterPattern(' ')
            {
                Generator = (text) => new Token()
                {
                    Text = new string(text),
                    Name = "space"
                }
            }
        });
        
        var tokens = tree.Parse(new EnumerableSource<char>("async await this is hello"))
            .Cast<Token>().ToList();
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

        return;
    }
}