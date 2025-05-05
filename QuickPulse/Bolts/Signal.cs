namespace QuickPulse.Bolts;

public class Signal
{
    public object CurrentInput { get; private set; }
    public Signal(object value) { CurrentInput = value; }
    public T GetValue<T>()
    {
        return (T)CurrentInput;
    }
}
