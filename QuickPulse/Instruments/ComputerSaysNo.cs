namespace QuickPulse.Instruments;

public class ComputerSaysNo(string msg) : InvalidOperationException(msg) { }

public static class ComputerSays
{
    public static void No(string msg) => throw new ComputerSaysNo(msg);
    public static T No<T>(string msg) => throw new ComputerSaysNo(msg);
}