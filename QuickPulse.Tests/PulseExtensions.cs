using System.Runtime.CompilerServices;
using QuickPulse.Arteries;

namespace QuickPulse.Tests;


// TODO: make this a first class citizen, maybe in QuickPulse.Show
public static class PulseExtensions
{
    public static T PulseToQuickLog<T>(
        this T item,
        [CallerMemberName] string testName = "",
        [CallerFilePath] string callerPath = "")
    {
        var dir = Path.GetDirectoryName(callerPath)!;
        var fullPath = Path.Combine(dir, $"{testName}.log");
        Signal.From<string>(a => Pulse.Trace(a))
           .SetArtery(FileLog.Write(fullPath));
        //.Pulse(Please.AllowMe().ToAddSomeClass().IntroduceThis(item!, false));
        return item;
    }
}