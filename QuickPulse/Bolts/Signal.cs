namespace QuickPulse.Bolts;


public class Signal<T>
{
    private readonly State state;
    private readonly Flow<Unit> flow;

    public Signal(/*State state, */Flow<Unit> flow)
    {
        this.state = new State();
        this.flow = flow;
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

    public void Pulse(T input)
    {
        state.SetValue(input);
        flow(state);
    }
}

public static class Signal
{
    public static Signal<T> From<T>(Flow<Unit> flow)
    {
        return new Signal<T>(flow);
    }
}

// public static class SignalGatherExtensions
// {
//     public static void Manipulate<T>(this State state, Func<T, T> update)
//         where T : notnull, new()
//     {
//         if (!state.Memory.TryGetValue(typeof(T), out var obj))
//         {
//             var Box = new Box<T>(new T());
//             state.Memory[typeof(T)] = obj!;
//         }
//         var existing = (Box<T>)state.Memory[typeof(T)]!;
//         existing.Value = update(existing.Value);
//     }
// }
