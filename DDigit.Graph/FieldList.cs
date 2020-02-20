using Adlib.Setup.Database;
using System.Collections.Generic;

namespace DDigit.Graph
{
  internal class FieldList : NodeList
  {
    public int UnusedCount
    {
      get
      {
        int count = 0;
        foreach (var node in fields.Values)
        {
          if (node.ReverseEdges.Count == 0)
          {
            count++;
          }
        }
        return count;
      }
    }

    public AdlibNode FindFieldNode(DatabaseInfo databaseInfo, string tag) =>
    fields.TryGetValue(FieldNode.FieldPath(databaseInfo, tag), out FieldNode fieldNode) ? fieldNode : null;

    readonly SortedDictionary<string, FieldNode> fields = new SortedDictionary<string, FieldNode>();
    public IEnumerable<FieldNode> Values => fields.Values;
    public int Count => fields.Count;
    internal FieldNode FindFieldNode(string path) => fields.TryGetValue(path, out FieldNode node) ? node : null;
    internal void Add(string fieldPath, FieldNode fieldNode) => fields[fieldPath] = fieldNode;
  }
}
