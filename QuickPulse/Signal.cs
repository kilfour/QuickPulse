using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static class Signal
{
    public static Signal<T> From<T>(Flow<T> flow)
    {
        return new Signal<T>(flow);
    }

    public static Signal<T> Tracing<T>()
    {
        var flow =
            from start in Pulse.Start<T>()
            from _ in Pulse.Trace(start)
            select start;
        return new Signal<T>(flow);
    }
}

public class Signal<T>
{
    private readonly State state;
    private readonly Flow<T> flow;

    public Signal(Flow<T> flow)
    {
        state = new State();
        this.flow = flow;
    }

    public Signal<T> SetArtery(IArtery artery)
    {
        state.SetArtery(artery);
        return this;
    }

    public TArtery SetAndReturnArtery<TArtery>(TArtery artery) where TArtery : IArtery
    {
        state.SetArtery(artery);
        return artery;
    }

    public void Pulse(params T[] input)
    {

        if (input == null)
        {
            flow(state.SetValue<T>(default!));
            return;
        }
        if (!input.Any())
            return;
        Pulse((IEnumerable<T>)input);
    }

    public void Pulse(IEnumerable<T> inputs)
    {
        foreach (var item in inputs)
            flow(state.SetValue(item));// ← Re-invokes the entire flow
    }

    public void PulseUntil(Func<bool> shouldStop, T input)
    {
        var times = 0;
        while (!shouldStop())
        {
            Pulse(input);
            if (times >= 255)
                ComputerSays.No("You can only pulse a max of 256 times, using Pulse.Until");
            times++;
        }
    }

    public void Manipulate<TValue>(Func<TValue, TValue> update)
        where TValue : notnull, new()
    {
        var existing = (Box<TValue>)state.Memory[typeof(TValue)]!;
        var updated = update(existing.Value);
        existing.Value = updated;
    }

    public IDisposable Scoped<TValue>(Func<TValue, TValue> enter, Func<TValue, TValue> exit)
        where TValue : notnull, new()
    {
        var existing = (Box<TValue>)state.Memory[typeof(TValue)]!;
        existing.Value = enter(existing.Value);
        return new DisposableAction(() => { existing.Value = exit(existing.Value); });
    }
}
