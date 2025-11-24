using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Reference.B_MakeItFlow.ToFlow;

[DocFile]
public class PulseOnType
{
    [DocContent("**`Pulse.OnType(...)`** Executes a subflow over a value or collection, conditionally.")]
    [Fact]
    [DocExample(typeof(PulseOnType), nameof(Pulse_OnType_Flow))]
    public void Pulse_OnType()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From(Pulse_OnType_Flow())
            .SetArtery(collector)
            .Pulse(new Cat())
            .Pulse(new Dog())
            .Pulse(new Dog())
            .Pulse(new Cat());

        var result = collector.Values;
        Assert.Equal(2, result.Count);
        Assert.Equal("dog", result[0]);
        Assert.Equal("dog", result[1]);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<Animal> Pulse_OnType_Flow()
    {
        var dogFlow =
            from dog in Pulse.Start<Dog>()
            from _ in Pulse.Trace(dog.Name)
            select dog;
        var flow =
            from animal in Pulse.Start<Animal>()
            from _ in Pulse.OnType(dogFlow, () => animal)
            select animal;
        return flow;
    }

    [Fact]
    [DocContent("A factory method can also be used.")]
    [DocExample(typeof(PulseOnType), nameof(Pulse_OnType_Inline_Flow))]
    [DocContent("Typing the lamda expression (`(Dog a)`) avoids the need for the type variables.")]
    public void Pulse_OnType_Inline()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From(Pulse_OnType_Inline_Flow())
            .SetArtery(collector)
            .Pulse(new Cat())
            .Pulse(new Dog())
            .Pulse(new Dog())
            .Pulse(new Cat());

        var result = collector.Values;
        Assert.Equal(2, result.Count);
        Assert.Equal("dog", result[0]);
        Assert.Equal("dog", result[1]);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<Animal> Pulse_OnType_Inline_Flow()
    {
        var dogFlow =
            from dog in Pulse.Start<Dog>()
            from _ in Pulse.Trace(dog.Name)
            select dog;
        var flow =
            from animal in Pulse.Start<Animal>()
            from _ in Pulse.OnType((Dog a) => Pulse.Trace(a.Name), () => animal)
            select animal;
        return flow;
    }


    public abstract class Animal { public abstract string Name { get; } };
    public class Dog : Animal
    {
        public override string Name => "dog";
    }

    public class Cat : Animal
    {
        public override string Name => "cat";
    }
}