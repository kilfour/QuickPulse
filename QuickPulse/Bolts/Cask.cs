namespace QuickPulse.Bolts;

public record Cask<TValue>(State state, TValue Value);

public static class Cask
{
    public static Cask<TValue> Some<TValue>(State state, TValue value) => new(state, value);
    public static Cask<TValue> None<TValue>(State state) => new(state, default!);
    public static Cask<Flow> Empty(State state) => None<Flow>(state);
    public static bool IsSome<T>(Cask<T> result) => result.Value is not null;
    public static bool IsNone<T>(Cask<T> result) => result.Value is null;
}
