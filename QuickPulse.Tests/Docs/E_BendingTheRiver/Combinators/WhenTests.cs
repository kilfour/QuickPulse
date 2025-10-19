using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.E_BendingTheRiver.Combinators;


public class WhenTests
{
    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.When(input % 2 == 0, Pulse.Trace("even"))
            select input;
    }

    [Fact]
    [DocExample(typeof(WhenTests), nameof(When_flow))]
    public void When()
    {
        Assert.Equal(BendingTheRiver.Default_result_values(), BendingTheRiver.GetResult(When_flow()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow_factory()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.When(input % 2 == 0, () => Pulse.Trace("even"))
            select input;
    }

    [Fact]
    [DocHeader("Flow Factory Version", 1)]
    [DocExample(typeof(WhenTests), nameof(When_flow_factory))]
    public void When_factory()
    {
        Assert.Equal(BendingTheRiver.Default_result_values(), BendingTheRiver.GetResult(When_flow_factory()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow_state()
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.When<int>(a => input % a == 0, Pulse.Trace("even"))
            select input;
    }

    [Fact]
    [DocHeader("Using State", 1)]
    [DocExample(typeof(WhenTests), nameof(When_flow_state))]
    public void When_state()
    {
        Assert.Equal(BendingTheRiver.Default_result_values(), BendingTheRiver.GetResult(When_flow_state()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow_state_factory()
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.When<int>(a => input % a == 0, () => Pulse.Trace("even"))
            select input;
    }

    [Fact]
    [DocHeader("Using State: Flow Factory Version", 1)]
    [DocExample(typeof(WhenTests), nameof(When_flow_state_factory))]
    [DocContent(
@"> **Why the factory overloads?**  
> `When(..., () => flow)` and many other `Pulse` methods using the factory flow pattern defer creating the subflow/value until the condition is true, avoiding work and allocations on the false path.")]
    public void When_state_factory()
    {
        Assert.Equal(BendingTheRiver.Default_result_values(), BendingTheRiver.GetResult(When_flow_state_factory()));
    }
}