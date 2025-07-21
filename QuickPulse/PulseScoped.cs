using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> Scoped<TValue>(Func<TValue, TValue> enter, Flow<Unit> flow) =>
        state =>
            {
                var existing = (Box<TValue>)state.Memory[typeof(TValue)]!;
                var existingValue = existing.Value;
                existing.Value = enter(existing.Value);
                flow(state);
                existing.Value = existingValue;
                return Cask.Empty(state);
            };
}