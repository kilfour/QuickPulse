using QuickPulse.Instruments;

namespace QuickPulse.Bolts;

public static class Signal
{
    public static Signal<T> From<T>(Flow<Unit> flow)
    {
        return new Signal<T>(flow);
    }
}

public class Signal<T>
{
    private readonly State state;
    private readonly Flow<Unit> flow;

    public Signal(Flow<Unit> flow)
    {
        state = new State();
        this.flow = flow;
    }

    public Signal<T> SetArtery(IArtery artery)
    {
        state.SetArtery(artery);
        return this;
    }

    public void Pulse()
    {
        flow(state);
    }

    public void Pulse(T input)
    {
        state.SetValue(input);
        flow(state);
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
