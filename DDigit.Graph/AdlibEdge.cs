namespace DDigit.Graph
{
  public class AdlibEdge
  {
    internal AdlibEdge(AdlibNode source, AdlibEdgeType type, AdlibNode target)
    {
      Source = source;
      EdgeType = type;
      Target = target;
    }

    internal AdlibNode Source { get; }
    internal AdlibNode Target { get; }
    internal AdlibEdgeType EdgeType { get; }
    internal bool Traversed { get; set; }
    public override string ToString() => $"{Source} {EdgeType} {Target}";
  }
}
