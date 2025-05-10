using QuickPulse.Arteries;

namespace QuickPulse.Tests.Ascii;

public class Tests
{

    [Fact]
    public void Go()
    {
        var diagram = new Diagram();
        diagram.AddEdge("1", "2");
        diagram.AddEdge("2", "3");
        diagram.AddEdge("1", "3");

        var lines = new AsciiRenderer(diagram).Render();

        var flow =
           from start in Pulse.Start<string>()
           from questions in Pulse.Trace(start)
           select start;

        var writer = new WriteDataToFile().ClearFile();

        Signal.From(flow)
            .SetArtery(writer)
            .Pulse(lines);
    }

    char[] boxDrawingChars = new[]
    {
        // Horizontal and vertical lines
        '─', '━', // light/heavy horizontal
        '│', '┃', // light/heavy vertical

        // Corners
        '┌', '┍', '┎', '┏', // top-left
        '┐', '┑', '┒', '┓', // top-right
        '└', '┕', '┖', '┗', // bottom-left
        '┘', '┙', '┚', '┛', // bottom-right

        // T-junctions
        '├', '┝', '┞', '┟', '┠', '┡', '┣', // left tees
        '┤', '┥', '┦', '┧', '┨', '┩', '┫', // right tees
        '┬', '┭', '┮', '┯', '┰', '┱', '┲', '┳', // top tees
        '┴', '┵', '┶', '┷', '┸', '┹', '┺', '┻', // bottom tees

        // Crosses
        '┼', '┽', '┾', '┿', '╂', '╋',

        // Double lines
        '═', '║',
        '╔', '╗', '╚', '╝',
        '╠', '╣', '╦', '╩', '╬',

        // Light double/dashed/rounded/etc
        '╭', '╮', '╯', '╰', // rounded corners
        '╱', '╲', // slashes (diagonal)
        '╳', '╴', '╵', '╶', '╷' // ends/crosses
    };

    char[] arrowChars = new[]
    {
        '→', '←', '↑', '↓',
        '↔', '↕', '⇒', '⇐', '⇑', '⇓', '⇔',
        '➝', '➞', '➔', '➤', '➣'
    };

    char[] treeChars = new[]
    {
        '│', '─', '└', '├', '┬', '┼', '┐', '┘', '┤'
    };

}