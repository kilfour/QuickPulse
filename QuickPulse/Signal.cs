using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static class Signal
{
    public static Signal<T> From<T>(Flow<T> flow)
    {
        return new Signal<T>(flow);
    }

    public static Signal<T> Tracing<T>()
    {
        var flow =
            from start in Pulse.Start<T>()
            from _ in Pulse.Trace(start)
            select start;
        return new Signal<T>(flow);
    }

    public static Signal<T> ToFile<T>(string? maybeFileName = null)
    {
        return Tracing<T>().SetArtery(WriteData.ToFile(maybeFileName));
    }
}

public class Signal<T>
{
    private readonly State state;
    private readonly Flow<T> flow;

    public Signal(Flow<T> flow)
    {
        state = new State();
        this.flow = flow;
    }

    public TArtery GetArtery<TArtery>() where TArtery : class, IArtery
    {
        var artery = state.CurrentArtery;
        if (artery == null) ComputerSays.No("No IArtery set on the current Signal.");
        var typedArtery = artery as TArtery;
        if (typedArtery == null) ComputerSays.No($"IArtery set on the current Signal is of type '{artery!.GetType().Name}' not '{typeof(TArtery).Name}'.");
        return typedArtery!;
    }

    public Signal<T> SetArtery(IArtery artery)
    {
        state.SetArtery(artery);
        return this;
    }

    public TArtery SetAndReturnArtery<TArtery>(TArtery artery) where TArtery : IArtery
    {
        state.SetArtery(artery);
        return artery;
    }

    public Signal<T> Pulse(params T[] input)
    {
        if (input == null)
        {
            flow(state.SetValue<T>(default!));
            return this;
        }
        if (!input.Any())
            return this;
        Pulse((IEnumerable<T>)input);
        return this;
    }

    public Signal<T> Pulse(IEnumerable<T> inputs)
    {
        foreach (var item in inputs)
            flow(state.SetValue(item));// <= Re-invokes the entire flow
        return this;
    }

    public Signal<T> PulseMultiple(int times, T input)
    {
        for (int i = 0; i < times; i++)
        {
            Pulse(input);
        }
        return this;
    }

    public Signal<T> PulseUntil(Func<bool> shouldStop, T input)
    {
        var times = 0;
        while (!shouldStop())
        {
            Pulse(input);
            if (times >= 255)
                ComputerSays.No("You can only pulse a max of 256 times, using Pulse.Until");
            times++;
        }
        return this;
    }
    public Signal<T> PulseMultipleUntil(int times, Func<bool> shouldStop, T input)
    {
        for (int i = 0; i < times; i++)
        {
            if (shouldStop())
                break;
            Pulse(input);
        }
        return this;
    }

    public Signal<T> Manipulate<TValue>(Func<TValue, TValue> update)
        where TValue : notnull, new()
    {
        var existing = (Box<TValue>)state.Memory[typeof(TValue)]!;
        var updated = update(existing.Value);
        existing.Value = updated;
        return this;
    }

    public IDisposable Scoped<TValue>(Func<TValue, TValue> enter, Func<TValue, TValue> exit)
        where TValue : notnull, new()
    {
        var existing = (Box<TValue>)state.Memory[typeof(TValue)]!;
        existing.Value = enter(existing.Value);
        return new DisposableAction(() => { existing.Value = exit(existing.Value); });
    }

    public Signal<T> Then<TNext>(Flow<TNext> next)
    {
        SetArtery(new Pipe<TNext>(next, state.CurrentArtery!));
        return this;
        // if (state.CurrentArtery is not ComposedPipe<T> pipe)
        // {
        //     pipe = new ComposedPipe<T>(() => state.CurrentArtery!);
        //     SetArtery(pipe);
        // }

        // pipe.AddFlow(next.Select(x => (object)x));
        // return this;
    }
}

public class Pipe<T> : IArtery
{
    private readonly Signal<T> signal;

    public Pipe(Flow<T> flow, IArtery target)
    {
        signal = Signal.From(flow).SetArtery(target);
    }

    public void Flow(params object[] data)
    {
        // if (data.Length != 1 || data[0] is not T input)
        // throw new InvalidOperationException($"Pipe expected a single value of type {typeof(T)}, but got: {string.Join(", ", data.Select(d => d?.GetType().Name ?? "null"))}");

        if (data is [T input])
        {
            signal.Pulse(input);
        }
    }
}

// public class ComposedPipe<TIn> : IArtery
// {
//     private readonly List<Flow<object>> _flowChain = new();
//     private readonly Func<IArtery> _getTarget;

//     public ComposedPipe(Func<IArtery> getTarget)
//     {
//         _getTarget = getTarget;
//     }

//     public void AddFlow<T>(Flow<T> flow)
//     {
//         _flowChain.Add(flow.Select(x => (object)x));
//     }

//     public void Flow(params object[] data)
//     {
//         if (data.Length != 1) throw new InvalidOperationException();

//         object current = data[0];
//         foreach (var flow in _flowChain)
//         {
//             var signal = Signal.From(flow).SetArtery(TheString.Catcher()); // optional
//             current = signal.Pulse(current).FirstOrDefault(); // only use first result per stage
//         }

//         _getTarget().Flow(current);
//     }
// }
