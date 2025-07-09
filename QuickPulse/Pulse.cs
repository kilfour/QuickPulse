using QuickPulse.Bolts;

namespace QuickPulse;

public static class Pulse
{
    public static Flow<TOut> Start<TOut>() =>
        state => Cask.Some(state, state.GetValue<TOut>());

    public static Flow<Unit> Trace(params object[] data) =>
        state =>
        {
            state.CurrentArtery?.Flow(data);
            return Cask.Empty(state);
        };

    public static Flow<Unit> TraceIf(bool flag, params object[] data) =>
        state =>
        {
            if (flag)
                state.CurrentArtery?.Flow(data);
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

    public static Flow<Unit> EffectIf(bool flag, Action action) =>
        state =>
        {
            if (flag)
                action();
            return Cask.Empty(state);
        };

    public static Flow<T> ToFlow<T>(Flow<T> flow, T value) => // T[] input maybe ?
        state => { state.SetValue(value); return flow(state); };

    public static Flow<T> ToFlow<T>(Flow<T> flow, IEnumerable<T> values) =>
        state =>
        {
            foreach (var item in values)
                flow(state.SetValue(item));
            return Cask.None<T>(state);
        };

    public static Flow<T> ToFlowIf<T>(bool flag, Flow<T> flow, Func<T> func) =>
        state =>
        {
            if (flag)
            {
                state.SetValue(func());
                return flow(state);
            }
            return Cask.None<T>(state);
        };

    public static Flow<Unit> NoOp() => Cask.Empty;
}
