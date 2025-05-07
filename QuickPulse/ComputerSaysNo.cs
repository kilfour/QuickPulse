namespace QuickPulse;

public class ComputerSaysNo : InvalidOperationException
{
    public ComputerSaysNo() : base() { }
    public ComputerSaysNo(string msg) : base(msg) { }
}

public static class ComputerSays
{
    public static void No()
    {
        throw new ComputerSaysNo();
    }

    public static void No(string msg)
    {
        throw new ComputerSaysNo(msg);
    }
}