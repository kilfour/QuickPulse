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

    public static Flow<Unit> Scoped<TValue>(Func<TValue, TValue> enter, Flow<Unit> flow) =>
        state =>
            {
                var existing = (Box<TValue>)state.Memory[typeof(TValue)]!;
                var existingValue = existing.Value;
                existing.Value = enter(existing.Value);
                flow(state);
                existing.Value = existingValue;
                return Cask.Empty(state);
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



    private static Flow<T> GetFlowFromFactory<T>(Func<T, Flow<Unit>> flowFactory)
    {
        return from i in Start<T>() from _ in flowFactory(i) select i;
    }

    public static Flow<Unit> ToFlow<T>(Flow<T> flow, T value) =>
        state => { flow(state.SetValue(value)); return Cask.Empty(state); };

    public static Flow<Unit> ToFlow<T>(Flow<T> flow, IEnumerable<T> values) =>
        state =>
        {
            foreach (var item in values)
                flow(state.SetValue(item));
            return Cask.Empty(state);
        };

    public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, T value) =>
        state =>
        {
            var flow = GetFlowFromFactory(flowFactory);
            flow(state.SetValue(value));
            return Cask.Empty(state);
        };

    public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, IEnumerable<T> values) =>
        state =>
        {
            var flow = GetFlowFromFactory(flowFactory);
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

    public static Flow<Unit> ToFlowIf<T>(bool flag, Flow<T> flow, Func<IEnumerable<T>> func) =>
        state =>
        {
            if (flag)
            {
                var values = func();
                foreach (var item in values)
                    flow(state.SetValue(item));
                return Cask.Empty(state);
            }
            return Cask.Empty(state);
        };

    public static Flow<Unit> ToFlowIf<T>(bool flag, Func<T, Flow<Unit>> flowFactory, Func<T> func) =>
        state =>
        {
            if (flag)
            {
                state.SetValue(func());
                var flow = GetFlowFromFactory(flowFactory);
                flow(state);
                return Cask.Empty(state);
            }
            return Cask.Empty(state);
        };

    public static Flow<Unit> ToFlowIf<T>(bool flag, Func<T, Flow<Unit>> flowFactory, Func<IEnumerable<T>> func) =>
        state =>
        {
            if (flag)
            {
                var values = func();
                var flow = GetFlowFromFactory(flowFactory);
                foreach (var item in values)
                    flow(state.SetValue(item));
                return Cask.Empty(state);
            }
            return Cask.Empty(state);
        };

    public static Flow<Unit> When(bool flag, Flow<Unit> flow) =>
        state =>
        {
            if (flag)
            {
                flow(state);
                return Cask.Empty(state);
            }
            return Cask.Empty(state);
        };

    public static Flow<Unit> NoOp() => Cask.Empty;

    public static Flow<TResult> Then<TSource, TResult>(this Flow<TSource> flow, Flow<TResult> next)
    {
        return flow.SelectMany(_ => next);
    }
}
