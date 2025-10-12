namespace QuickPulse.Arteries;

/// <summary>
/// Factory for creating latch arteries that remember only the most recent absorbed value.
/// </summary>
public static class TheLatch
{
    /// <summary>
    /// Creates a new latch that holds the last value it absorbed. Use when only the final output matters.
    /// </summary>
    public static Latch<T> Holds<T>() => new();
}

/// <summary>
/// An artery that stores the most recently absorbed value, overwriting previous ones.
/// </summary>
public class Latch<T> : IArtery
{
    private T? value;

    /// <summary>
    /// The last value absorbed by this latch.
    /// </summary>
    public T? Q => value;

    /// <summary>
    /// Absorbs incoming data and updates the stored value to the last element. Use to capture the latest flow output.
    /// </summary>
    public void Absorb(params object[] data) => value = (T)data.Last();
}
