namespace QuickPulse.Bolts;

public interface IBeat<out TValue> { State State { get; } TValue Value { get; } }

public record Beat<TValue>(State State, TValue Value) : IBeat<TValue>;

public static class Beat
{
    public static Beat<TValue> Some<TValue>(State state, TValue value) => new(state, value);
    public static Beat<TValue> None<TValue>(State state) => new(state, default!);
    public static Beat<Flow> Empty(State state) => None<Flow>(state);
}
