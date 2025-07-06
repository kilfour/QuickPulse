namespace QuickPulse.Tests.Ascii;

public class AsciiRenderer
{
    private readonly Diagram _diagram;
    private readonly Dictionary<string, (int x, int y)> _positions = new();

    private const int BoxWidth = 12;
    private const int BoxHeight = 3;
    private const int HSpacing = 4;
    private const int VSpacing = 2;

    public AsciiRenderer(Diagram diagram)
    {
        _diagram = diagram;

        // Naive horizontal row layout
        int x = 0, y = 0;
        foreach (var node in diagram.Nodes)
            _positions[node.Name] = (x++, y++);
    }

    public IEnumerable<string> Render()
    {
        const int width = 120, height = 30;
        var buffer = Enumerable.Range(0, height).Select(_ => new string(' ', width).ToCharArray()).ToArray();

        // Draw nodes
        foreach (var (name, (cx, cy)) in _positions)
        {
            int x = cx * (BoxWidth + HSpacing);
            int y = cy * (BoxHeight + VSpacing);
            Put(buffer, x, y, "+------------+");
            Put(buffer, x, y + 1, $"|{name.PadCenter(12)}|");
            Put(buffer, x, y + 2, "+------------+");
        }

        // Draw edges
        foreach (var edge in _diagram.Edges)
        {
            var (fx, fy) = _positions[edge.From.Name];
            var (tx, ty) = _positions[edge.To.Name];

            int fromX = fx * (BoxWidth + HSpacing) + BoxWidth;
            int fromY = fy * (BoxHeight + VSpacing) + 1;

            int toX = tx * (BoxWidth + HSpacing);
            int toY = ty * (BoxHeight + VSpacing) + 1;

            if (fromY == toY)
            {
                // Horizontal line
                for (int x = fromX + 1; x < toX; x++)
                    buffer[fromY][x] = '─';

                if (edge.Label != null)
                    Put(buffer, (fromX + toX) / 2 - edge.Label.Length / 2, fromY - 1, edge.Label);
            }
            else
            {
                // Basic vertical then horizontal connection
                int midX = fromX + 2;
                int dir = Math.Sign(toY - fromY);

                for (int y = fromY + dir; y != toY; y += dir)
                    buffer[y][midX] = '│';

                int hStart = Math.Min(midX, toX);
                int hEnd = Math.Max(midX, toX);
                for (int x = hStart; x <= hEnd; x++)
                    buffer[toY][x] = '─';

                if (edge.Label != null)
                    Put(buffer, (hStart + hEnd) / 2 - edge.Label.Length / 2, toY - 1, edge.Label);
            }
        }

        // Output
        foreach (var row in buffer)
            yield return new string(row);
    }

    private void Put(char[][] buf, int x, int y, string s)
    {
        if (y < 0 || y >= buf.Length) return;
        for (int i = 0; i < s.Length && x + i < buf[0].Length; i++)
            buf[y][x + i] = s[i];
    }
}

public static class StringExtensions
{
    public static string PadCenter(this string s, int width)
    {
        int padding = Math.Max(0, width - s.Length);
        int padLeft = padding / 2;
        int padRight = padding - padLeft;
        return new string(' ', padLeft) + s + new string(' ', padRight);
    }
}