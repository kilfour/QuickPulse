namespace QuickPulse.Bolts;

public class Box<T>(T value)
{
    public T Value { get; set; } = value;
}