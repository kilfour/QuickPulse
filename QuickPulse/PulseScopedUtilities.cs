using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State> WithScope<TValue>(Func<TValue, TValue> enter, Flow<Flow> flow) =>
        state =>
        {
            var box = state.GetTheCell<TValue>();
            var prev = box.Value;
            box.Value = enter(prev);
            try { flow(state); }
            finally { box.Value = prev; }
        };
}