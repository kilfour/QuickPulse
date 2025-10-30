using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Reads the current value of type TCell from memory without changing it. Use to recall state stored by Prime or Manipulate.
    /// </summary>
    public static Flow<TCell> Draw<TCell>() => s => Beat.Some(s, s.GetTheCell<TCell>().Value);

    /// <summary>
    /// Reads a value derived from a stored box type without altering state. 
    /// Use to extract a specific field or projection from previously primed or manipulated memory.
    /// </summary>
    public static Flow<TValue> Draw<TCell, TValue>(Func<TCell, TValue> func) =>
        s => Beat.Some(s, func(s.GetTheCell<TCell>().Value));
}
