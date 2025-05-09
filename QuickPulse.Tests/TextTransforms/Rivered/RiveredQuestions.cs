using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.TextTransforms.Rivered;
public class RiveredQuestions
{
    [Fact(Skip = "Demo")]
    public void All_questions()
    {
        var json =
            from intAndTextAndBool in Pulse.Start<((int, string), bool)>()
            let intAndText = intAndTextAndBool.Item1
            let escaped = intAndText.Item2.Replace("\"", "\\\"")
            let comma = !intAndTextAndBool.Item2 ? ", " : "[ "
            from lb in Pulse.Trace($"{comma}{{ \"id\": {intAndText.Item1}, \"text\": \"{escaped}\" }}")
            select intAndTextAndBool;

        var question =
            from line in Pulse.Start<string>()
            from isFirstQuestion in Pulse.Gather(true)
            let trimmed = line.Trim()
            let numberAndTextOrNull = GetLeadingNumberIfFollowedByDot(line)
            let isQuestion = numberAndTextOrNull != null
            from flowed in Pulse.ToFlowIf(isQuestion, json, () => (numberAndTextOrNull.Value, isFirstQuestion.Value))
            from effect in Pulse.EffectIf(isQuestion, () => isFirstQuestion.Value = false)
            select line;

        var flow =
            from start in Pulse.Start<string[]>()
            from questions in Pulse.ToFlow(question, start)
            from rb in Pulse.Trace("]")
            select start;

        var whereWeAre = "/QuickPulse.Tests/TextTransforms/Rivered/";
        var path = SolutionLocator.FindSolutionRoot() + whereWeAre + "MyQuestions.md";
        var writer = new WriteDataToFile(whereWeAre + "Questions.json").HardCodedPath().ClearFile();

        Signal.From(flow)
            .SetArtery(writer)
            .Pulse(File.ReadAllLines(path));
    }


    (int, string)? GetLeadingNumberIfFollowedByDott()
    {
        var charFlow =
            from ch in Pulse.Start<char>()
            select ch;

        var strFlow =
            from str in Pulse.Start<string>()
            from x in Pulse.Gather(() =>
            {
                int i = 0;
                while (i < str.Length && char.IsDigit(str[i]))
                {
                    i++;
                }
            })
            select str;

        var flow =
            from str in Pulse.Start<string>()
            let valid = !string.IsNullOrEmpty(str)
            select str;

        return null;
    }

    (int, string)? GetLeadingNumberIfFollowedByDot(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;
        int i = 0;
        while (i < input.Length && char.IsDigit(input[i]))
        {
            i++;
        }
        if (i > 0 && i < input.Length && input[i] == '.')
        {
            // Parse the leading number
            string numberPart = input.Substring(0, i);
            if (int.TryParse(numberPart, out int number))
            {
                return (number, new string(input.Skip(i + 1).ToArray()).Trim().Replace("*", ""));
            }
        }
        return null;
    }
}

public static class Ext
{
    public static string Right(this string s, int length)
    {
        if (string.IsNullOrEmpty(s) || length <= 0)
            return string.Empty;

        return s.Length <= length ? s : s.Substring(s.Length - length);
    }
}