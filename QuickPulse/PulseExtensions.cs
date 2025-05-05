using QuickPulse.Bolts;

namespace QuickPulse;

public static class PulseExtensions
{
    public static T Run<T>(this Flow<T> flow, object input) =>
        flow(new Signal(input)).Value;

    public static T RunBoundPulse<T>(this Flow<T> boundPulse) =>
        boundPulse(new Signal(null!)).Value;
}