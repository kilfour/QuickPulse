using QuickPulse.Arteries;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Emits the given objects into the specified grafted artery. Use to direct traces to a custom or secondary output channel.
    /// </summary>
    public static Flow<Flow> TraceTo<TArtery>(params object[] data) where TArtery : IArtery =>
        Runnel(Always, _ => data, IntoGraftedArtery<TArtery>());
}
