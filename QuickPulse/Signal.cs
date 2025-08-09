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
        Tracing<T>().SetArtery(WriteData.ToFile(maybeFileName));
}

public class Signal<T>
{
    private readonly State state;
    private readonly Flow<T> flow;

    public Signal(Flow<T> flow) { state = new State(); this.flow = flow; }

    public Signal<T> ChainIt(Action action) => Chain.It(action, this);

    public TArtery GetArtery<TArtery>() where TArtery : class, IArtery
    {
        var artery = state.CurrentArtery;
        if (artery == null) ComputerSays.No("No IArtery set on the current Signal.");
        var typedArtery = artery as TArtery;
        if (typedArtery == null) ComputerSays.No($"IArtery set on the current Signal is of type '{artery!.GetType().Name}' not '{typeof(TArtery).Name}'.");
        return typedArtery!;
    }

    public Signal<T> SetArtery(IArtery artery) => ChainIt(() => state.SetArtery(artery));

    public TArtery SetAndReturnArtery<TArtery>(TArtery artery) where TArtery : IArtery =>
        Chain.It(() => state.SetArtery(artery), artery);

    public bool FlowRanDry => state.FlowRanDry;

    public Signal<T> Pulse(T value)
    {
        if (FlowRanDry) return this;
        flow(state.SetValue(value));
        return this;
    }

    public Signal<T> Pulse(IEnumerable<T> inputs)
    {
        foreach (var item in inputs)
        {
            if (FlowRanDry) break;
            flow(state.SetValue(item));
            if (FlowRanDry) break;
        }
        return this;
    }
}
