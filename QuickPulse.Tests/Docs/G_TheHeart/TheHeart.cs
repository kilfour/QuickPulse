using QuickPulse.Arteries;
using QuickPulse.Explains;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Docs.G_TheHeart;

[DocFile]
[DocContent("> Hunting for Flows.\n\n")]
[DocContent(
@"The Heart is the typed Artery registry: it remembers where pulses can go, based on Artery type, and lets you target them deliberately.  
It is *not* an output by itself. 
")]
public class TheHeart
{
    [Fact]
    [DocHeader("The Main Artery")]
    [DocContent(
@"There is *always* exactly one Main Artery. It is the default outflow for a signal. Use `Signal.SetArtery(...)` to set it.  
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` emissions flow into it.  ")]
    [DocExample(typeof(TheHeart), nameof(Signal_set_Artery_example))]
    public void Signal_set_Artery()
    {
        var stringSink = Text.Capture();
        Signal_set_Artery_example(stringSink);
        Assert.Equal("42", stringSink.Content());
    }

    [CodeSnippet]
    private static void Signal_set_Artery_example(IArtery stringSink)
    {
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(stringSink) // <= 'stringSink' is now the Main Artery
            .Pulse(42);
    }

    [Fact]
    [DocContent(
@"`Signal.SetAndReturnArtery(...)` Similar, but returns the Artery you pass in (useful for quick wiring):")]
    [DocExample(typeof(TheHeart), nameof(Signal_set_and_return_Artery_example))]
    public void Signal_set_and_return_Artery()
    {
        Assert.IsType<StringSink>(Signal_set_and_return_Artery_example());
    }

    [CodeSnippet]
    [CodeRemove("return ")]
    private static StringSink Signal_set_and_return_Artery_example()
    {
        return Signal.From<int>(a => Pulse.Trace(a)).SetAndReturnArtery(Text.Capture());
    }

    [Fact]
    [DocContent(@"Setting an Artery on a signal that already has one **replaces** the previous Artery.  ")]
    [DocExample(typeof(TheHeart), nameof(Signal_setting_Artery_twice_example))]
    public void Signal_setting_Artery_twice()
    {
        var (stringSink, caulfield) = Signal_setting_Artery_twice_example();
        Assert.Equal("42", stringSink.Content());
        Assert.Equal("43", caulfield.Content());
    }

    [CodeSnippet]
    [CodeRemove("return (stringSink, caulfield);")]
    private static (StringSink stringSink, StringSink caulfield) Signal_setting_Artery_twice_example()
    {
        var stringSink = Text.Capture();
        var caulfield = Text.Capture();
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(stringSink)
            .Pulse(42)
            .SetArtery(caulfield)
            .Pulse(43);
        // stringSink.Content()    => "42"
        // caulfield.Content()     => "43"
        return (stringSink, caulfield);
    }

    [Fact]
    [DocContent(
@"- Trying to set the Main Artery to null throws:  
    > The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?")]
    public void Signal_setting_null_Artery_throws()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.From<int>(a => Pulse.Trace(a)).SetArtery(null!));
        Assert.Equal("The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?", ex.Message);
    }

    [Fact]
    public void Signal_setting_and_return_null_Artery_throws()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.From<int>(a => Pulse.Trace(a)).SetAndReturnArtery<StringSink>(null!));
        Assert.Equal("The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?", ex.Message);
    }

    [DocHeader("Grafting Extra Arteries")]
    [DocContent(
@"Apart from pulsing flows through the Main Artery, QuickPulse allows you to redirect flows to additional Arteries.  
There are various situations where this is useful.  
In the following section we will discuss how to set up one particular use case:
'Adding a diagnostic trace to an existing flow.' 

Suppose we have the following flow: ")]
    [DocExample(typeof(TheHeart), nameof(Grafting_starting_flow))]
    [CodeSnippet]

    private Flow<char> Grafting_starting_flow()
    {
        return
            from ch in Pulse.Start<char>()
            from depth in Pulse.Prime(() => -1)
            from _ in Pulse.TraceIf(depth >= 0, () => ch)
            from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
            from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
            select ch;
    }

    [DocContent(
@"This is a simple flow that returns the text between braces, even if there are other braces inside said text.  
**An Example**:")]
    [DocExample(typeof(TheHeart), nameof(Grafting_starting_flow_usage))]
    [CodeSnippet]
    [CodeRemove("return")]
    [CodeReplace("Grafting_starting_flow()", "flow")]
    private Signal<char> Grafting_starting_flow_usage()
    {
        var stringSink = Text.Capture();
        return
        Signal.From(Grafting_starting_flow())
            .SetArtery(stringSink)
            .Pulse("{ a { b } c }");
    }

    [Fact]
    [DocContent(
@"Unfortunately the result of this is ` a { b } c }` and really, we want it to be ` a { b } c `.  ")]
    public void Grafting_bad_flow()
    {
        Assert.Equal(" a { b } c }", Grafting_starting_flow_usage().GetArtery<StringSink>().Content());
    }


    [DocContent(
@"
So let's try and find out what's going on.  

First we define a new typed Artery:")]
    [DocExample(typeof(Diagnostic))]
    [DocContent("Then we *Graft* it onto the Heart through the `Signal.Graft(...)` method.")]
    [DocExample(typeof(TheHeart), nameof(Grafting_inspected_flow_usage))]
    [CodeSnippet]
    [CodeRemove("return")]
    [CodeReplace("Grafting_inspected_flow()", "flow")]
    private Signal<char> Grafting_inspected_flow_usage()
    {
        var stringSink = Text.Capture();
        var diagnostic = new Diagnostic();
        return
        Signal.From(Grafting_inspected_flow())
            .SetArtery(stringSink)
            .Graft(diagnostic)
            .Pulse("{ a { b } c }");
    }

    [CodeExample]
    public class Diagnostic : Collector<string> { }

    [DocContent(
@"In this case, we could just Graft a `Collector<string>`, but creating a derived class expresses our intent much better.

Lastly we add a `Pulse.TraceTo<TArtery>(...)` to the flow:
")]
    [DocExample(typeof(TheHeart), nameof(Grafting_inspected_flow))]
    [CodeSnippet]
    [CodeReplace("return", "var flow = ")]
    private Flow<char> Grafting_inspected_flow()
    {
        return
            from ch in Pulse.Start<char>()
            from depth in Pulse.Prime(() => -1)
            let enter = depth
            let emit = depth >= 0
            from _ in Pulse.TraceIf(emit, () => ch)
            from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
            from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
            from exit in Pulse.Draw<int>()
            from diag in Pulse.TraceTo<Diagnostic>(
                $"char='{ch}', enter={enter}, emit={emit}, exit={exit}")
            select ch;
    }

    [Fact]
    [DocContent(
@"When executing this, the `StringSink` Artery contains the same as before, but now we have the following in the `Diagnostic` Artery:")]
    [DocExample(typeof(TheHeart), nameof(Grafting_checking_diagnostics_expected))]
    [DocContent("We can now use this information to correct the original flow")]
    public void Grafting_checking_diagnostics()
    {
        var collector = Grafting_inspected_flow_usage().GetArtery<Diagnostic>();
        Assert.Equal(Grafting_checking_diagnostics_expected(), collector.Values);
    }


    [CodeSnippet]
    [CodeRemove("return")]
    private static List<string> Grafting_checking_diagnostics_expected()
    {
        return
        [
            "char='{', enter=-1, emit=False, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='a', enter=0, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='{', enter=0, emit=True, exit=1",
            "char=' ', enter=1, emit=True, exit=1",
            "char='b', enter=1, emit=True, exit=1",
            "char=' ', enter=1, emit=True, exit=1",
            "char='}', enter=1, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='c', enter=0, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='}', enter=0, emit=True, exit=-1"
        ];
    }


    [Fact]
    [DocHeader("GetArtery")]
    [DocContent(
@"`Signal.GetArtery<TArtery>(...)` can be used to retrieve the current `IArtery` set on the signal.  
")]
    [DocExample(typeof(TheHeart), nameof(Signal_get_Artery_example))]
    public void Signal_get_Artery()
    {
        var stringSink = Signal_get_Artery_example();
        Assert.Equal("42", stringSink.Content());
    }

    [CodeSnippet]
    [CodeRemove("return stringSink;")]
    private static StringSink Signal_get_Artery_example()
    {
        var stringSink =
            Signal.From<int>(a => Pulse.Trace(a))
                .SetArtery(Text.Capture())
                .Pulse(42)
                .GetArtery<StringSink>(); // <=
        // stringSink.Content() => "42"
        return stringSink;
    }

    [DocContent(
@"`Signal.GetArtery<TArtery>(...)` throws if trying to retrieve a concrete type of `IArtery` that the heart is unaware of.
    ")]
    [Fact]
    public void Signal_get_Artery_throws_if_wrong_typed_retrieved()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.From(Pulse.NoOp()).SetArtery(Text.Capture()).GetArtery<FileLogArtery>());
        var lines = ex.Message.Split(Environment.NewLine);
        Assert.Equal("No IArtery of type 'FileLogArtery' set on the current Signal.", lines[0]);
        Assert.Equal("Main IArtery is of type 'StringSink'.", lines[1]);
    }
}
