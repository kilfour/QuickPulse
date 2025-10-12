namespace QuickPulse.Arteries;

/// <summary>
/// Factory for creating collector arteries that record everything passing through a flow.
/// </summary>
public static class TheCollector
{
    /// <summary>
    /// Creates a new collector artery that gathers all absorbed values into an internal list. Use for testing or inspection.
    /// </summary>
    public static Collector<T> Exhibits<T>() => new();
}

/// <summary>
/// An artery that collects every absorbed value into an exhibit list for later inspection.
/// </summary>
public class Collector<T> : IArtery
{
    /// <summary>
    /// The collected values in the order they were absorbed.
    /// </summary>
    public readonly List<T> TheExhibit = [];

    /// <summary>
    /// Absorbs incoming data and adds it to the exhibit list. Use to verify emitted values from a flow.
    /// </summary>
    public void Absorb(params object[] data)
    {
        foreach (var item in data)
            TheExhibit.Add((T)item);
    }
}
