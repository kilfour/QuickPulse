namespace QuickPulse.Tests._Tools;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class DocAttribute : Attribute, IEquatable<DocAttribute>
{
    public string Order { get; init; }
    public string Caption { get; init; }
    public string Content { get; init; }

    public DocAttribute(string order = "", string caption = "", string content = "")
    {
        Order = order;
        Caption = caption;
        Content = content;
    }

    public bool Equals(DocAttribute? other) =>
        other is not null &&
        Order == other.Order &&
        Caption == other.Caption &&
        Content == other.Content;

    public override bool Equals(object? obj) =>
        obj is DocAttribute other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(Order, Caption, Content);
}
