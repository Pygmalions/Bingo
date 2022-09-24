using Bingo.One.Sources;
using NUnit.Framework;

namespace Bingo.One.Test;

[TestFixture]
public class ContextTest
{
    [Test]
    public void CommitTest()
    {
        const string text = "Hello, world!";
        var context = new Context<char>(new EnumerableSource<char>(text));
        while (!context.Empty)
            context.Consume();
        var result = new string(context.Commit());
        Assert.AreEqual(text, result);
    }
    
    [Test]
    public void RollbackTest()
    {
        const string text = "Hello, world!";
        var context = new Context<char>(new EnumerableSource<char>(text));
        context.Consume();
        context.Consume();
        Assert.AreEqual('l', context.Current);
        context.Rollback();
        Assert.AreEqual('H', context.Current);
        Assert.IsEmpty(context.Commit());
    }
    
    [Test]
    public void CheckpointFunctionTest()
    {
        const string text = "Hello, world!";
        var context = new Context<char>(new EnumerableSource<char>(text));
        for (var index = 0; index < 7; index++)
            context.Consume();
        Assert.AreEqual('w', context.Current);
        context.AddCheckpoint();
        context.Consume();
        context.Consume();
        context.Consume();
        Assert.AreEqual('l', context.Current);
        context.Rollback();
        Assert.AreEqual('w', context.Current);
        Assert.AreEqual(context.Commit(), text[..7]);
    }
    
    [Test]
    public void CheckpointRemovingTest()
    {
        const string text = "Hello, world!";
        var context = new Context<char>(new EnumerableSource<char>(text));
        for (var index = 0; index < 2; index++)
            context.Consume();
        Assert.AreEqual('l', context.Current);
        context.AddCheckpoint();
        for (var index = 0; index < 3; index++)
            context.Consume();
        Assert.AreEqual(',', context.Current);
        context.RemoveCheckpoint();
        context.Rollback();
        Assert.AreEqual('H', context.Current);
        Assert.IsEmpty(context.Commit());
    }
}