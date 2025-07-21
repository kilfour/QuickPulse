using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static partial class Pulse
{
    private static Flow<T> GetFlowFromFactory<T>(Func<T, Flow<Unit>> flowFactory)
    {
        return from i in Start<T>() from _ in flowFactory(i) select i;
    }

    private static Box<T> GetTheBox<T>(State state)
    {
        if (!state.Memory.TryGetValue(typeof(T), out var obj))
        {
            ComputerSays.No($"No value of type {typeof(T).Name} found.");
        }
        return (Box<T>)obj!;
    }

    private static bool CheckInTheBox<T>(Func<T, bool> predicate, State state)
    {
        return predicate(GetTheBox<T>(state).Value);
    }
}