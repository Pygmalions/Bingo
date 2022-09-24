using System.Linq;
using Bingo.One.Sources;
using NUnit.Framework;

namespace Bingo.One.Test;

[TestFixture]
public class TreeTest
{
    [Test]
    public void FullMatch()
    {
        var tree = new Tree<char>();
        string Generator(char[] letters) => new(letters);
        tree.Merge(new Pattern<char>[]
        {
            new LetterPattern('a'),
            new LetterPattern('s'),
            new LetterPattern(' ')
        }, Generator);
        
        tree.Merge(new Pattern<char>[]
        {
            new LetterPattern('a'),
            new LetterPattern('s'),
            new LetterPattern('y'),
            new LetterPattern('n'),
            new LetterPattern('c'),
            new LetterPattern(' ')
        }, Generator);
        
        tree.Merge(new Pattern<char>[]
        {
            new LetterPattern('a'),
            new LetterPattern('w'),
            new LetterPattern('a'),
            new LetterPattern('i'),
            new LetterPattern('t'),
            new LetterPattern(' ')
        }, Generator);

        var tokens = tree.Parse(new EnumerableSource<char>("as async await "))
            .Cast<string>().ToList();
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual("as ", tokens[0]);
        Assert.AreEqual("async ", tokens[1]);
        Assert.AreEqual("await ", tokens[2]);
    }
    
    [Test]
    public void TailEscapeTest()
    {
        var tree = new Tree<char>();
        string Generator(char[] letters) => new(letters);
        tree.Fallback = Generator;
        tree.Merge(new Pattern<char>[]
        {
            new LetterPattern('a'),
            new LetterPattern('s'),
            new LetterPattern(' ')
        }, Generator);
        
        tree.Merge(new Pattern<char>[]
        {
            new LetterPattern('a'),
            new LetterPattern('s'),
            new LetterPattern('y'),
            new LetterPattern('n'),
            new LetterPattern('c'),
            new LetterPattern(' ')
        }, Generator);
        
        tree.Merge(new Pattern<char>[]
        {
            new LetterPattern('a'),
            new LetterPattern('w'),
            new LetterPattern('a'),
            new LetterPattern('i'),
            new LetterPattern('t'),
            new LetterPattern(' ')
        }, Generator);

        var tokens = tree.Parse(new EnumerableSource<char>("as async await i"))
            .Cast<string>().ToList();
        Assert.AreEqual(4, tokens.Count);
        Assert.AreEqual("as ", tokens[0]);
        Assert.AreEqual("async ", tokens[1]);
        Assert.AreEqual("await ", tokens[2]);
        Assert.AreEqual("i", tokens[3]);
    }
}