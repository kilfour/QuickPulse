namespace QuickPulse.Tests.Ascii;

public class Diagram
{
    public List<Node> Nodes { get; } = new();
    public List<Edge> Edges { get; } = new();

    public Node GetOrAddNode(string name)
    {
        var node = Nodes.FirstOrDefault(n => n.Name == name);
        if (node == null)
        {
            node = new Node(name);
            Nodes.Add(node);
        }
        return node;
    }

    public void AddEdge(string from, string to, string? label = null)
    {
        var fromNode = GetOrAddNode(from);
        var toNode = GetOrAddNode(to);
        Edges.Add(new Edge(fromNode, toNode, label));
    }
}

public abstract record Element;
public record Node(string Name) : Element;

public record Edge(Node From, Node To, string? Label) : Element;
