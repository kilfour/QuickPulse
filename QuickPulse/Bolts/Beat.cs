namespace QuickPulse.Bolts;

public record Beat<TValue>(State State, TValue Value);

public static class Beat
{
    public static Beat<TValue> Some<TValue>(State state, TValue value) => new(state, value);
    public static Beat<TValue> None<TValue>(State state) => new(state, default!);
    public static Beat<Flow> Empty(State state) => None<Flow>(state);
    public static bool IsSome<T>(Beat<T> result) => result.Value is not null;
    public static bool IsNone<T>(Beat<T> result) => result.Value is null;
}
