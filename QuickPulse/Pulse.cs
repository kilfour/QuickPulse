using QuickPulse.Bolts;

namespace QuickPulse;

public static class Pulse
{
    public static Flow<TOut> From<TOut>() =>
        state => Cask.Some(state.GetValue<TOut>());

    public static Flow<TOut> From<TOut>(TOut value) =>
        _ => Cask.Some(value);

    public static Flow<TOut> Shape<TOut>(Func<TOut> func) =>
        _ => Cask.Some(func());
}
