namespace QuickPulse.Bolts;

public class Cell<T>(T value)
{
    public T Value { get; set; } = value;
}