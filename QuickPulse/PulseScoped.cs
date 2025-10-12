namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Temporarily replaces a stored value for the duration of the given flow, then restores it. Use to test or modify state in isolation.
    /// </summary>
    public static Flow<Unit> Scoped<TValue>(Func<TValue, TValue> enter, Flow<Unit> flow) =>
        Runnel(Always, WithScope(enter, flow));
}