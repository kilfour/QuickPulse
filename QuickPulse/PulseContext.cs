namespace QuickPulse;

public static class PulseContext
{
    [ThreadStatic]
    public static IArtery? Current;

    public static T SetCurrent<T>(T artery) where T : IArtery
    {
        Current = artery;
        return artery;
    }

    public static void Log(object data)
    {
        Current?.Flow(data);
    }
}