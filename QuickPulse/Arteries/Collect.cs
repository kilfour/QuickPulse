namespace QuickPulse.Arteries;

/// <summary>
/// Provides factory methods for creating collector arteries that record all values emitted by a flow.
/// </summary>
public static class Collect
{
    /// <summary>
    /// Creates a new <see cref="Collector{T}"/> that gathers every absorbed value into an internal list.
    /// Use for testing, inspection, or verifying emitted flow output.
    /// </summary>
    public static Collector<T> ValuesOf<T>() => new();
}

/// <summary>
/// An artery that collects every absorbed value in sequence for later inspection.
/// </summary>
public class Collector<T> : IArtery
{
    /// <summary>
    /// The collected values in the order they were absorbed.
    /// </summary>
    public readonly List<T> Values = [];

    /// <summary>
    /// Absorbs incoming data and appends each item to the <see cref="Values"/> list.
    /// Use to verify or inspect emitted values from a flow.
    /// </summary>
    public void Absorb(params object[] data)
    {
        foreach (var item in data)
            Values.Add((T)item);
    }
}
