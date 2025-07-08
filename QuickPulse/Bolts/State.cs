namespace QuickPulse.Bolts;

public class State
{
    public State() { }
    public State(object value) { CurrentInput = value; }

    public object? CurrentInput { get; private set; }
    public T GetValue<T>() { return (T)CurrentInput!; }
    public State SetValue<T>(T value) { CurrentInput = value!; return this; }

    public readonly Dictionary<Type, object> Memory = [];

    public IArtery? CurrentArtery { get; private set; }
    public void SetArtery(IArtery artery)
    {
        CurrentArtery = artery;
    }
}
