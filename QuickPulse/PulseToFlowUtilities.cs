using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, T> IntoFlow<T>(Flow<T> flow)
        => (s, v) => flow(s.SetValue(v));
    private static Action<State, T> IntoFactory<T>(Func<T, Flow<Unit>> flowFactory)
        => (s, v) => GetFlowFromFactory(flowFactory)(s.SetValue(v));
}