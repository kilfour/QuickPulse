using QuickPulse.Arteries;
using QuickPulse.Explains;
using QuickPulse.Instruments;
using QuickPulse.Show;

namespace QuickPulse.Tests.Docs.TheHuntingHeart;

[DocFile]
[DocContent("> Hunting for Flows.\n\n")]
[DocContent(
@"The Heart is the typed Artery registry: it remembers where pulses can go, based on Artery type, and lets you target them deliberately.  
It is *not* an output by itself. 
")]

public class TheHeart
{
    [Fact(Skip = "broke doc")]
    public void CreateDoc()
    {
        Explain.This<TheHeart>("the-heart.md");
    }

    [Fact]
    [DocHeader("The Main Artery")]
    [DocContent(
@"There is *always* exactly one Main Artery. It is the default outflow for a signal.  

**`Signal.SetArtery(...)`** sets the **Main Artery**.  
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` emissions flow into it.  ")]
    [DocCodeExample(typeof(TheHeart), nameof(Signal_set_Artery_example))]
    public void Signal_set_Artery()
    {
        var holden = TheString.Catcher();
        Signal_set_Artery_example(holden);
        Assert.Equal("42", holden.Whispers());
    }

    [DocSnippet]
    private void Signal_set_Artery_example(IArtery holden)
    {
        Signal.Tracing<int>()
            .SetArtery(holden) // <= 'holden' is now the Main Artery
            .Pulse(42);
    }

    [Fact]
    [DocContent(
@"**`Signal.SetAndReturnArtery(...)`** Similar, but returns the Artery you pass in (useful for quick wiring):")]
    [DocCodeExample(typeof(TheHeart), nameof(Signal_set_and_return_Artery_example))]
    public void Signal_set_and_return_Artery()
    {
        Assert.IsType<Holden>(Signal_set_and_return_Artery_example());
    }

    [DocSnippet]
    [DocReplace("return", "")]
    private Holden Signal_set_and_return_Artery_example()
    {
        return Signal.Tracing<int>().SetAndReturnArtery(TheString.Catcher());
    }

    [Fact]
    [DocContent(@"Setting an Artery on a signal that already has one **replaces** the previous Artery.  ")]
    [DocCodeExample(typeof(TheHeart), nameof(Signal_setting_Artery_twice))]
    [DocSnippet]
    public void Signal_setting_Artery_twice()
    {
        var holden = TheString.Catcher();
        var caulfield = TheString.Catcher();
        Signal.Tracing<int>()
            .SetArtery(holden)
            .Pulse(42)
            .SetArtery(caulfield)
            .Pulse(43);
        Assert.Equal("42", holden.Whispers());
        Assert.Equal("43", caulfield.Whispers());
    }

    [Fact]
    [DocHeader("Safeties:", 2)]
    [DocContent(
@"- Trying to set the Main Artery to null throws:  
    > The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?")]
    public void Signal_setting_null_Artery_throws()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.Tracing<int>().SetArtery(null!));
        Assert.Equal("The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?", ex.Message);
    }

    [Fact]
    public void Signal_setting_and_return_null_Artery_throws()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.Tracing<int>().SetAndReturnArtery<Holden>(null!));
        Assert.Equal("The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?", ex.Message);
    }

    [Fact]
    [DocContent(
@"- Pulsing without setting the Main Artery throws:  
    > The Heart flatlined. No Main Artery. Did you forget to call SetArtery(...) ?")]
    public void Signal_pulse_without_main_Artery_throws()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.Tracing<int>().Pulse(42));
        Assert.Equal("The Heart flatlined. No Main Artery. Did you forget to call SetArtery(...) ?", ex.Message);
    }

    [DocHeader("Grafting Arteries")] // ungraft
    [DocContent(
@"Apart from pulsing flows through the Main Artery, QuickPulse allows you to redirect flows to additional Arteries.  
There are various situations where this is useful.  
In the following section we will discuss how to set up one particular use case:
'Adding a diagnostic trace to an existing flow.' 

Suppose we have the following flow: ")]
    [DocCodeExample(typeof(TheHeart), nameof(Grafting_starting_flow))]
    [DocSnippet]

    private Flow<char> Grafting_starting_flow()
    {
        return
            from ch in Pulse.Start<char>()
            from depth in Pulse.Gather(-1)
            from _ in Pulse.TraceIf(depth.Value >= 0, () => ch)
            from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
            from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
            select ch;
    }

    [DocContent(
@"This is a simple flow that returns the text between braces, even if there are other braces inside said text.  
**An Example**:")]
    [DocCodeExample(typeof(TheHeart), nameof(Grafting_starting_flow_usage))]
    [DocSnippet]
    [DocReplace("return", "")]
    [DocReplace("Grafting_starting_flow()", "flow")]
    private Signal<char> Grafting_starting_flow_usage()
    {
        var holden = TheString.Catcher();
        return
        Signal.From(Grafting_starting_flow())
            .SetArtery(holden)
            .Pulse("{ a { b } c }");
    }

    [Fact]
    [DocContent(
@"Unfortunately the result of this is ` a { b } c }` and really, we want it to be ` a { b } c `.  ")]
    public void Grafting_bad_flow()
    {
        Assert.Equal(" a { b } c }", Grafting_starting_flow_usage().GetArtery<Holden>().Whispers());
    }


    [DocContent(
@"
So let's try and find out what's going on.  

First we define a new typed Artery:")]
    [DocCodeExample(typeof(Diagnostic))]
    [DocContent("Then we *Graft* it onto the Heart through the `Signal.Graft(...)` method.")]
    [DocCodeExample(typeof(TheHeart), nameof(Grafting_inspected_flow_usage))]
    [DocSnippet]
    [DocReplace("return", "")]
    [DocReplace("Grafting_inspected_flow()", "flow")]
    private Signal<char> Grafting_inspected_flow_usage()
    {
        var holden = TheString.Catcher();
        var diagnostic = new Diagnostic();
        return
        Signal.From(Grafting_inspected_flow())
            .SetArtery(holden)
            .Graft(diagnostic)
            .Pulse("{ a { b } c }");
    }

    [DocExample]
    public class Diagnostic : TheCollector<string> { }

    [DocContent(
@"In this case, we could just Graft the `TheCollector<string>`, but creating a derived class explains our intent much better.

Lastly we add a `Pulse.TraceTo<TArtery>(...)` to the flow:
")]
    [DocCodeExample(typeof(TheHeart), nameof(Grafting_inspected_flow))]
    [DocSnippet]
    [DocReplace("return", "var flow = ")]
    private Flow<char> Grafting_inspected_flow()
    {
        return
            from ch in Pulse.Start<char>()
            from depth in Pulse.Gather(-1)
            let enter = depth.Value
            let emit = depth.Value >= 0
            from _ in Pulse.TraceIf(emit, () => ch)
            from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
            from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
            let exit = depth.Value
            from diag in Pulse.TraceTo<Diagnostic>(
                $"char='{ch}', enter={enter}, emit={emit}, exit={exit}")
            select ch;
    }

    [Fact]
    [DocContent(
@"When executing this, the `Holden` Artery contains the same as before, but now we have the following in the `Diagnostic` Artery:")]
    [DocCodeExample(typeof(TheHeart), nameof(Grafting_checking_diagnostics_expected))]
    [DocContent("We can now use this information to correct the original flow")]
    public void Grafting_checking_diagnostics()
    {
        var collector = Grafting_inspected_flow_usage().GetArtery<Diagnostic>();
        Assert.Equal(Grafting_checking_diagnostics_expected(), collector.TheExhibit);
    }


    [DocSnippet]
    [DocReplace("return", "")]
    private List<string> Grafting_checking_diagnostics_expected()
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
@"**`Signal.GetArtery<TArtery>(...)`** can be used to retrieve the current `IArtery` set on the signal.  
")]
    [DocCodeExample(typeof(TheHeart), nameof(Signal_get_Artery))]
    [DocSnippet]
    public void Signal_get_Artery()
    {
        var holden =
            Signal.Tracing<int>()
                .SetArtery(TheString.Catcher())
                .Pulse(42)
                .GetArtery<Holden>();
        Assert.Equal("42", holden.Whispers());
    }

    [Fact]
    [DocHeader("Safeties:", 2)]
    [DocContent(
@"- If no Main Artery exists `GetArtery` throws:  
    ...")] //> The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?

    public void Signal_get_Artery_throws_if_no_Artery_set()
    {
        var ex = Assert.Throws<NullReferenceException>(() => Signal.Tracing<int>().GetArtery<TheCollector<int>>());
        Assert.Equal("Object reference not set to an instance of an object.", ex.Message);
    }

    //     [Doc(Order = Chapters.Signalling + "-7.1-1", Caption = "", Content =
    // @"**`Signal.GetArtery<TArtery>(...)`** throws if trying to retrieve the wrong type of `IArtery`.
    // ")]
    //     [Fact]
    //     public void Signal_get_Artery_throws_if_wrong_typed_retrieved()
    //     {
    //         var ex = Assert.Throws<ComputerSaysNo>(() => Signal.Tracing<int>().SetArtery(TheString.Catcher()).GetArtery<TheCollector<int>>());
    //         Assert.Equal("IArtery set on the current Signal is of type 'Holden' not 'TheCollector`1'.", ex.Message);
    //     }
}

