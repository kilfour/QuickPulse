using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class LocalFileLogSpike
{

    [Fact(Skip = "touches file system")]
    public void WriteHere()
    {
        var log = FileLog.WriteHere();
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(log)
            .Pulse([1, 2, 3]);
    }

    [Fact(Skip = "touches file system")]
    public void AppendHere()
    {
        var log = FileLog.AppendHere();
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(log)
            .Pulse([42, 43, 44]);
    }
}
