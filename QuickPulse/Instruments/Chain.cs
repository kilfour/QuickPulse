namespace QuickPulse.Instruments;

public static class Chain
{
    public static TResult It<TResult>(Action action, TResult result) { action(); return result; }
}