using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class LocalFileLogSpike
{

    [Fact(Skip = "touches file system")]
    public void WriteHere()
    {
        var log = FileLog.WriteHere();
        Signal.From(
                from anInt in Pulse.Start<int>()
                from trace in Pulse.Trace(anInt)
                select anInt)
            .SetArtery(log)
            .Pulse([1, 2, 3]);
    }

    [Fact(Skip = "touches file system")]
    public void AppendHere()
    {
        var log = FileLog.AppendHere();
        Signal.From(
                from anInt in Pulse.Start<int>()
                from trace in Pulse.Trace(anInt)
                select anInt)
            .SetArtery(log)
            .Pulse([42, 43, 44]);
    }
}
