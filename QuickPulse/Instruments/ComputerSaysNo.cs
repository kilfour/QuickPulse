namespace QuickPulse.Instruments;

public class ComputerSaysNo : InvalidOperationException
{
    public ComputerSaysNo(string msg) : base(msg) { }
}

public static class ComputerSays
{
    public static void No(string msg)
    {
        throw new ComputerSaysNo(msg);
    }
}