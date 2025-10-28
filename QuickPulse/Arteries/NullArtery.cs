namespace QuickPulse.Arteries;

/// <summary>
/// Provides access to the built-in <see cref="NullArtery"/>,
/// an inert artery that silently absorbs all output.
/// </summary>
public static class Install
{
    /// <summary>
    /// The default inert artery that silently absorbs all data.
    /// Use when no output is required.
    /// </summary>
    public static readonly IArtery Null = new NullArtery();
}

/// <summary>
/// An inert artery that ignores all absorbed data.
/// Implements the Null Object pattern for flows without outputs.
/// </summary>
public class NullArtery : IArtery
{
    /// <summary>
    /// Accepts data and discards it silently.
    /// Use to safely disable output without altering flow logic.
    /// </summary>
    public void Absorb(params object[] data) { }
}
