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
        Current?.Log(data);
    }
}