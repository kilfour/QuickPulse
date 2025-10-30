using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, TValue> IntoFlow<TValue>(Flow<TValue> flow)
        => (s, v) => flow(s.SetValue(v));
    private static Action<State, TValue> IntoFactory<TValue>(Func<TValue, Flow<Flow>> flowFactory)
        => (s, v) => GetFlowFromFactory(flowFactory)(s.SetValue(v));
}