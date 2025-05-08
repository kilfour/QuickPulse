namespace QuickPulse.Bolts;

public class State
{
    public State() { }
    public State(object value) { CurrentInput = value; }

    public object? CurrentInput { get; private set; }
    public T GetValue<T>() { return (T)CurrentInput!; }
    public void SetValue<T>(T value) { CurrentInput = value!; }

    public readonly Dictionary<Type, object> Memory = [];

    public IPulser? CurrentPulser { get; private set; }
    public void SetPulser(IPulser pulser)
    {
        CurrentPulser = pulser;
    }
}
