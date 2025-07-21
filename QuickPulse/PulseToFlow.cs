using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static partial class Pulse
{
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

    public static Flow<Unit> ToFlowIf<T, TBox>(Func<TBox, bool> predicate, Flow<T> flow, Func<T> func) =>
        state =>
        {
            if (CheckInTheBox(predicate, state))
            {
                state.SetValue(func());
                flow(state);
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
        };// Func<T, bool> predicate  (CheckInTheBox(predicate, state))

    public static Flow<Unit> ToFlowIf<T, TBox>(Func<TBox, bool> predicate, Flow<T> flow, Func<IEnumerable<T>> func) =>
        state =>
        {
            if (CheckInTheBox(predicate, state))
            {
                var values = func();
                foreach (var item in values)
                    flow(state.SetValue(item));
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
}