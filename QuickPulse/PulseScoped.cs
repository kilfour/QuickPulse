namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Temporarily replaces a stored value for the duration of the given flow, then restores it. Use to test or modify state in isolation.
    /// </summary>
    public static Flow<Flow> Scoped<TValue>(Func<TValue, TValue> enter, Flow<Flow> flow) =>
        Runnel(Always, WithScope(enter, flow));
}