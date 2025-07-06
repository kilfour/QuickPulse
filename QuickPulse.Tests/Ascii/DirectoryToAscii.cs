namespace QuickPulse.Tests.Ascii;

public static class DirectoryToAscii
{
    private class Node
    {
        public List<Node> Children { get; init; } = [];

        public string Label { get; set; } = string.Empty;
    }

    public static string FromDirectory(string rootPath)
    {
        var root = new Node { Label = new DirectoryInfo(rootPath).Name + "/" };
        AddChildren(root, rootPath);
        return Sculpt(root);
    }

    private static void AddChildren(Node node, string path)
    {
        foreach (var directory in Directory.GetDirectories(path))
        {
            var child = new Node { Label = Path.GetFileName(directory) + "/" };
            node.Children.Add(child);
            AddChildren(child, directory);
        }

        foreach (var file in Directory.GetFiles(path))
        {
            var child = new Node { Label = Path.GetFileName(file) };
            node.Children.Add(child);
        }
    }

    private static string Sculpt(Node root)
    {
        var lines = new List<string>();
        var stack = new Stack<(Node Node, int Depth, bool IsLast, string Indent)>();
        bool isFirst = true;
        stack.Push((root, 0, true, ""));
        while (stack.Count > 0)
        {
            var (node, depth, isLast, indent) = stack.Pop();
            if (isFirst)
            {
                lines.Add(node.Label); // No prefix for the root
                isFirst = false;
            }
            else
            {
                var prefix = isLast ? "└── " : "├── ";
                lines.Add($"{indent}{prefix}{node.Label}");
            }
            string childIndent = indent + (isLast ? "    " : "│   ");
            for (int i = node.Children.Count() - 1; i >= 0; i--)
            {
                var child = node.Children[i];
                if (child != null)
                {
                    bool isLastChild = i == node.Children.Count - 1;
                    stack.Push((child, depth + 1, isLastChild, childIndent));
                }
            }
        }
        return string.Join("\n", lines);
    }
}