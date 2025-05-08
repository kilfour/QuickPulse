using QuickPulse.Bolts;

namespace QuickPulse;

public static class FlowExtensions
{
    public static T Run<T>(this Flow<T> flow, object input) =>
        flow(new State(input)).Value;

    public static T RunWith<T>(this Flow<T> flow, IPulser pulser, object input)
    {
        var state = new State(input);
        state.SetPulser(pulser);
        return flow(state).Value;
    }

    public static T RunWith<T>(this Flow<T> flow, IPulser pulser)
    {
        var state = new State(null!);
        state.SetPulser(pulser);
        return flow(state).Value;
    }

    public static T RunBoundPulse<T>(this Flow<T> boundPulse) =>
        boundPulse(new State(null!)).Value;
}

public interface IPulser { public void Monitor(params object[] data); }