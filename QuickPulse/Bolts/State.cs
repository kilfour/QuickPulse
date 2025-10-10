using QuickPulse.Instruments;

namespace QuickPulse.Bolts;

public class State
{
    public State() { }
    public State(object value) { CurrentInput = value; }

    public object? CurrentInput { get; private set; }
    public T GetValue<T>() { return (T)CurrentInput!; }
    public State SetValue<T>(T value) { CurrentInput = value!; return this; }

    public IArtery? CurrentArtery { get; private set; }
    public void SetArtery(IArtery artery)
    {
        if (artery == null)
            ComputerSays.No("The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?");
        CurrentArtery = artery;
    }

    private readonly Dictionary<Type, IArtery> heart = [];
    public void Graft<TArtery>(TArtery artery) where TArtery : IArtery
    {
        heart[typeof(TArtery)] = artery;
    }

    public IArtery GetArtery<TArtery>() where TArtery : IArtery
    {
        if (typeof(TArtery) == CurrentArtery!.GetType())
            return CurrentArtery;
        if (heart.ContainsKey(typeof(TArtery)))
            return heart[typeof(TArtery)];
        return null!; // todo Computer Says No 
    }

    private readonly Dictionary<Type, object> Memory = [];
    public Box<TValue> GetTheBox<TValue>()
    {
        if (!Memory.TryGetValue(typeof(TValue), out var obj))
            ComputerSays.No($"No value of type {typeof(TValue).Name} found.");
        return (Box<TValue>)obj!;
    }

    public Box<TValue> GetTheBox<TValue>(TValue value)
    {
        if (!Memory.TryGetValue(typeof(TValue), out var obj))
        {
            var box = new Box<TValue>(value);
            Memory[typeof(TValue)] = box;
            return box;
        }
        return (Box<TValue>)obj!;
    }

    public Box<TValue> GetTheBox<TValue>(Func<TValue> factory)
    {
        if (!Memory.TryGetValue(typeof(TValue), out var obj))
        {
            var box = new Box<TValue>(factory());
            Memory[typeof(TValue)] = box;
            return box;
        }
        return (Box<TValue>)obj!;
    }

    public bool FlowRanDry { get; private set; }
    public void StopFlowing() { FlowRanDry = true; }
}
