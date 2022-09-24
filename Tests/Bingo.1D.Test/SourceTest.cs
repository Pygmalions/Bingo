using Bingo.One.Sources;
using NUnit.Framework;

namespace Bingo.One.Test;

[TestFixture]
public class SourceTest
{
    [Test]
    public void ConsumeFunction()
    {
        const string text = "Hello, world!";
        var source = new EnumerableSource<char>(text);
        source.Consume();
        while (source.Valid)
            source.Consume();
        var result = new string(source.Commit());
        Assert.AreEqual(text, result);
    }
    
    [Test]
    public void RollbackFunction()
    {
        const string text = "Hello, world!";
        var source = new EnumerableSource<char>(text);
        source.Consume();
        for (var index = 0; index < 5; index++)
            source.Consume();
        Assert.AreEqual(',', source.Current);
        source.Rollback(1);
        Assert.AreEqual('o', source.Current);
        source.Consume();
        source.Rollback(5);
        Assert.AreEqual('H', source.Current);
        Assert.IsEmpty(source.Commit());
    }
}