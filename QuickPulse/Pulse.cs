using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static class Pulse
{
    public static Flow<TOut> Start<TOut>() =>
        state => Cask.Some(state, state.GetValue<TOut>());

    // public static Flow<Unit> Using(IArtery artery) =>
    //     state => { state.SetArtery(artery); return Cask.Empty(state); };

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

    public static Flow<Unit> FirstOf(params (Func<bool>, Func<Flow<Unit>>)[] data) =>
        state =>
        {
            foreach (var item in data)
            {
                if (item.Item1())
                {
                    return item.Item2()(state);
                }
            }
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

    public static Flow<Box<T>> Gather<T>() =>
        state =>
        {
            if (!state.Memory.TryGetValue(typeof(T), out var obj))
            {
                ComputerSays.No($"No value of type {typeof(T).Name} found.");
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

    public static Flow<Unit> ToFlow<T>(Flow<T> flow, T value) => // T[] input maybe ?
        state => { state.SetValue(value); flow(state); return Cask.Empty(state); };

    public static Flow<Unit> ToFlow<T>(Flow<T> flow, IEnumerable<T> values) =>
        state =>
        {
            foreach (var item in values)
                flow(state.SetValue(item));
            return Cask.Empty(state);
        };

    public static Flow<Unit> ToFlowIf<T>(bool flag, Flow<T> flow, Func<T> func) =>
        state =>
        {
            if (flag)
            {
                state.SetValue(func());
                flow(state);
                return Cask.Empty(state);
            }
            return Cask.Empty(state);
        };

    public static Flow<Unit> NoOp() => Cask.Empty;

    public static Flow<Unit> CaseOf() => Cask.Empty;
}
