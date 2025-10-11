using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static class Signal
{
    public static Signal<T> From<T>(Flow<T> flow) => new(flow);

    public static Signal<T> From<T>(Func<T, Flow<Unit>> flowFactory) =>
        new(Pulse.GetFlowFromFactory(flowFactory));
}

public class Signal<T>(Flow<T> flow)
{
    private readonly State state = new();
    private readonly Flow<T> flow = flow;

    public bool FlowRanDry => state.FlowRanDry;

    public Signal<T> Pulse()
        => typeof(T) == typeof(Unit)
            ? Pulse((T)(object)Unit.Instance)
            : ComputerSays.No<Signal<T>>("Pulse() without arguments only allowed for Signal<Unit>.");

    private static IEnumerable<T> Single(T value) { yield return value; }

    public Signal<T> Pulse(T value) => Pulse(Single(value));

    public Signal<T> Pulse(IEnumerable<T> inputs)
    {
        if (FlowRanDry) return this;
        foreach (var item in inputs)
        {
            flow(state.SetValue(item));
            if (FlowRanDry) break;
        }
        return this;
    }

    public Signal<T> SetArtery(IArtery artery) => Chain.It(() => state.SetArtery(artery), this);

    public TArtery SetAndReturnArtery<TArtery>(TArtery artery) where TArtery : IArtery =>
        Chain.It(() => state.SetArtery(artery), artery);

    public TArtery GetArtery<TArtery>() where TArtery : class, IArtery
        => state.GetArtery<TArtery>();

    public Signal<T> Graft<TArtery>(TArtery artery) where TArtery : IArtery
        => Chain.It(() => state.Graft(artery), this);

    public Signal<T> FlatLine(Flow<Unit> finisher)
        => Chain.It(() => finisher(state), this);
}
