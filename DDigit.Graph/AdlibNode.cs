using System.Collections.Generic;

namespace DDigit.Graph
{
  internal abstract class AdlibNode
  {
    internal AdlibNode(string path, string name)
    {
      Path = path;
      Name = name;
    }

    internal string Path { get; private set; }
    internal string Name { get; private set; }
    public override string ToString() => $"'{Name}'";

    public List<AdlibEdge> Edges = new List<AdlibEdge>();
    public List<AdlibEdge> ReverseEdges = new List<AdlibEdge>();
  }
}