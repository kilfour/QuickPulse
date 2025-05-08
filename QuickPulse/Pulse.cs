using QuickPulse.Bolts;

namespace QuickPulse;

public static class Pulse
{
    public static Flow<TOut> Start<TOut>() =>
        state => Cask.Some(state, state.GetValue<TOut>());

    public static Flow<TOut> Start<TOut>(TOut value) =>
        state => Cask.Some(state, value);

    public static Flow<Unit> Using(IPulser pulser) =>
        state => { state.SetPulser(pulser); return Cask.Empty(state); };

    public static Unit Stop { get { return Unit.Instance; } }

    public static Flow<Unit> TraceIf(bool flag, params object[] data) =>
        state =>
        {
            if (flag)
                state.CurrentPulser?.Monitor(data);
            return Cask.Empty(state);
        };

    public static Flow<Unit> TraceIf(bool flag, Action action) =>
        state =>
        {
            if (flag)
                action();
            return Cask.Empty(state);
        };

    public static Flow<Unit> TraceIf(Func<bool> predicate, Action action) =>
        state =>
        {
            if (predicate())
                action();
            return Cask.Empty(state);
        };

    public static Flow<Unit> Trace(Action action) =>
        state =>
        {
            action();
            return Cask.Empty(state);
        };

    public static Flow<Unit> Trace(params object[] data) =>
        state =>
        {
            state.CurrentPulser?.Monitor(data);
            return Cask.Empty(state);
        };

    public static Flow<Box<T>> Gather<T>(T value) =>
        state =>
        {
            if (!state.Memory.TryGetValue(typeof(T), out var obj))
            {
                var box = new Box<T>(value);
                state.Memory[typeof(T)] = box;
                return Cask.Some(state, box);
            }
            return Cask.Some(state, (Box<T>)obj!);
        };
    public static Flow<Unit> Effect(Action action) =>
        state => { action(); return Cask.Empty(state); };

    public static Flow<Unit> NoOp() => state => Cask.Empty(state);

    public static Flow<Unit> Batched<T>(this Flow<Unit> flow, IEnumerable<T> items) =>
    state =>
        {
            foreach (var item in items)
            {
                state.SetValue(item);
                flow(state);
            }
            return Cask.Empty(state);
        };
}