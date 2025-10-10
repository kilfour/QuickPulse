using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static class Signal
{
    public static Signal<T> From<T>(Flow<T> flow) => new(flow);

    public static Signal<T> From<T>(Func<T, Flow<Unit>> flowFactory) =>
        new(Pulse.GetFlowFromFactory(flowFactory));

    public static Signal<T> Tracing<T>() =>
        new(Pulse.GetFlowFromFactory<T>(a => Pulse.Trace(a!)));

    public static Signal<T> ToFile<T>(string? maybeFileName = null) =>
        Tracing<T>().SetArtery(TheLedger.Records(maybeFileName));
}

public class Signal<T>(Flow<T> flow)
{
    private readonly State state = new();
    private readonly Flow<T> flow = flow;

    public TArtery GetArtery<TArtery>() where TArtery : class, IArtery
    {
        var artery = state.GetArtery<TArtery>();
        if (artery == null) ComputerSays.No("No IArtery set on the current Signal.");
        var typedArtery = artery as TArtery;
        if (typedArtery == null) ComputerSays.No($"IArtery set on the current Signal is of type '{artery!.GetType().Name}' not '{typeof(TArtery).Name}'.");
        return typedArtery!;
    }

    public Signal<T> SetArtery(IArtery artery) => Chain.It(() => state.SetArtery(artery), this);

    public TArtery SetAndReturnArtery<TArtery>(TArtery artery) where TArtery : IArtery =>
        Chain.It(() => state.SetArtery(artery), artery);

    public bool FlowRanDry => state.FlowRanDry;

    public Signal<T> Pulse(T value) => Pulse(Single(value));

    public Signal<T> Pulse(IEnumerable<T> inputs)
    {
        if (state.CurrentArtery == null)
            ComputerSays.No("The Heart flatlined. No Main Artery. Did you forget to call SetArtery(...) ?");
        if (FlowRanDry) return this;
        foreach (var item in inputs)
        {
            flow(state.SetValue(item));
            if (FlowRanDry) break;
        }
        return this;
    }

    private static IEnumerable<T> Single(T value) { yield return value; }

    public Signal<T> Graft<TArtery>(TArtery artery) where TArtery : IArtery
    {
        state.Graft(artery);
        return this;
    }

    public Signal<T> FlatLine(Flow<Unit> finisher)
    {
        finisher(state);
        return this;
    }
}
