using QuickPulse.Bolts;

namespace QuickPulse.Diagnostics;

public static class PulseContext
{
    [ThreadStatic]
    public static IPulse? Current;

    public static T SetCurrent<T>(T pulse) where T : IPulse
    {
        Current = pulse;
        return pulse;
    }

    public static void Log(object data)
    {
        Current?.Monitor(data);
    }

    public static void FromFlow<T>(this Flow<T> flow)
    {
        Current = ToPulse(flow);
    }

    public static IPulse ToPulse<T>(this Flow<T> flow)
    {
        return new DiagnosticPulse(a => flow(new State(a)));
    }
}
