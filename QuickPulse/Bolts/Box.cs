namespace QuickPulse.Bolts;

public class Box<T>
{
    public T Value { get; set; }
    public Box(T value) { Value = value; }
}