using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Bolts;

public class State
{
    public State() { }
    public State(object value) { CurrentInput = value; }

    public object? CurrentInput { get; private set; }
    public T GetValue<T>() { return (T)CurrentInput!; }
    public State SetValue<T>(T value) { CurrentInput = value!; return this; }

    public IArtery CurrentArtery { get; private set; } = Install.Shunt;
    public void SetArtery(IArtery artery)
        => CurrentArtery = artery
            ?? ComputerSays.No<IArtery>("The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?");

    private readonly Dictionary<Type, IArtery> heart = [];
    public void Graft<TArtery>(TArtery artery) where TArtery : IArtery
    {
        heart[typeof(TArtery)] = artery;
    }

    public TArtery GetArtery<TArtery>() where TArtery : IArtery
    {
        if (CurrentArtery is TArtery current)
            return current;

        if (heart.TryGetValue(typeof(TArtery), out var artery))
            return (TArtery)artery;

        return ComputerSays.No<TArtery>(
            $"No IArtery of type '{typeof(TArtery).Name}' set on the current Signal.{Environment.NewLine}" +
            $"Main IArtery is of type '{CurrentArtery.GetType().Name}'.");
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
