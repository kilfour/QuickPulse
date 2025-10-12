using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Reads the current value of type T from memory without changing it. Use to recall state stored by Prime or Manipulate.
    /// </summary>
    public static Flow<T> Draw<T>() => s => Cask.Some(s, s.GetTheBox<T>().Value);
}
