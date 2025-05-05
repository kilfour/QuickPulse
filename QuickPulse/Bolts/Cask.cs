namespace QuickPulse.Bolts;

public record Cask<TValue>(TValue Value);

public static class Cask
{
    public static Cask<TValue> Some<TValue>(TValue value) => new(value);
    public static Cask<TValue> None<TValue>() => new(default!);
    public static Cask<Unit> Empty() => None<Unit>();
    public static Cask<Unit> AcidOnly() => Some(Unit.Instance);
    public static bool IsSome<T>(Cask<T> result) => result.Value is not null;
    public static bool IsNone<T>(Cask<T> result) => result.Value is null;
}
