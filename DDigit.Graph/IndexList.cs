using Adlib.Interfaces;
using System.Collections.Generic;

namespace DDigit.Graph
{
  internal class IndexList : NodeList
  {
    public List<IndexNode> FindIndexNodes(IAdlibDatabaseInfo databaseInfo, string tag)
    {
      var result = new List<IndexNode>();
      foreach (var indexInfo in databaseInfo.IndexList)
      {
        foreach (var indexTag in indexInfo.IndexTags)
        {
          if (indexTag.Text == tag)
          {
            if (indexes.TryGetValue(Path(databaseInfo, indexInfo), out IndexNode node))
            {
              result.Add(node);
            }
          }
        }
      }
      return result;
    }

    public int UnusedCount
    {
      get
      {
        int count = 0;
        foreach (var node in indexes.Values)
        {
          if (node.ReverseEdges.Count == 0)
          {
            count++;
          }
        }
        return count;
      }
    }

    readonly SortedDictionary<string, IndexNode> indexes = new SortedDictionary<string, IndexNode>();
    public IEnumerable<IndexNode> Values => indexes.Values;
    public int Count => indexes.Count;
    public static string Path(IAdlibDatabaseInfo databaseInfo, IIndexInfo indexInfo) => $"{DatabaseNode.DatabasePath(databaseInfo)}\\index\\{indexInfo.TableName}";
    internal void Add(string path, IndexNode indexNode) => indexes[path] = indexNode;
  }
}
