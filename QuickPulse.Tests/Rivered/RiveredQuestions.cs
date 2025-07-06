using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Rivered;

public class RiveredQuestions
{
    [Fact(Skip = "demo")]
    public void All_questions()
    {
        var json =
            from intAndTextAndBool in Pulse.Start<((int, string), bool)>()
            let intAndText = intAndTextAndBool.Item1
            let escaped = intAndText.Item2.Replace("\"", "\\\"")
            let comma = !intAndTextAndBool.Item2 ? ", " : "[ "
            from lb in Pulse.Trace($"{comma}{{ \"id\": {intAndText.Item1}, \"text\": \"{escaped}\" }}")
            select intAndTextAndBool;
        // { id : Int
        // , text : String

        // -- , tags : List String
        // -- , note : String
        // , asked : Bool
        // }
        var elm =
            from intAndTextAndBool in Pulse.Start<((int, string), bool)>()
            let intAndText = intAndTextAndBool.Item1
            let escaped = intAndText.Item2.Replace("\"", "\\\"")
            let comma = !intAndTextAndBool.Item2 ? ", " : "[ "
            from lb in Pulse.Trace($"{comma}{{ id = {intAndText.Item1}, text = \"{escaped}\", asked = False }}")
            select intAndTextAndBool;

        var question =
            from input in Pulse.Start<string>()
            from isFirstQuestion in Pulse.Gather(true)
            from trimmed in Pulse.Gather("") // reuse later for tail
            from _ in Pulse.Effect(() => trimmed.Value = input.Trim())
            let i = trimmed.Value.TakeWhile(char.IsDigit).Count()
            let hasDot = i > 0 && i < trimmed.Value.Length && trimmed.Value[i] == '.'
            let numberText = i > 0 ? trimmed.Value.Substring(0, i) : null
            let rest = i + 1 < trimmed.Value.Length
                ? new string(trimmed.Value.Skip(i + 1).ToArray()).Trim().Replace("*", "")
                : ""
            let numberAndTextOrNull = int.TryParse(numberText, out var number)
                ? new (int, string)?((number, rest))
                : null
            let isQuestion = numberAndTextOrNull != null
            from flowed in Pulse.ToFlowIf(isQuestion, elm, () => (numberAndTextOrNull.Value, isFirstQuestion.Value))
            from effect in Pulse.EffectIf(isQuestion, () => isFirstQuestion.Value = false)
            select input;

        var flow =
            from start in Pulse.Start<string[]>()
            from questions in Pulse.ToFlow(question, start)
            from rb in Pulse.Trace("]")
            select start;

        var whereWeAre = "/QuickPulse.Tests/Rivered/";
        var path = SolutionLocator.FindSolutionRoot() + whereWeAre + "MyQuestions.md";
        var writer = new WriteDataToFile(whereWeAre + "Questions.json", true).ClearFile();

        Signal.From(flow)
            .SetArtery(writer)
            .Pulse(File.ReadAllLines(path));
    }
}