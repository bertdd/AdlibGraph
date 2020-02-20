using Adlib.Setup.Application;
using System.Collections.Generic;

namespace DDigit.Graph
{
  public class MethodList : NodeList
  {
    internal void LinkMethodToNode(ApplicationInfo applicationInfo, AdlibNode dataSourceNode, MethodInfo methodInfo, ScreenList screens)
    {
      var methodNode = new MethodNode(dataSourceNode.Path, methodInfo);

      methods[methodNode.Path] = methodNode;
      AddEdge(dataSourceNode, AdlibEdgeType.HasMethod, methodNode);

      methodInfo.ListScreenList.ForEach((screen) =>
        screens.LinkScreenToNode(applicationInfo, dataSourceNode, screen, AdlibEdgeType.UsesListScreen));

      methodInfo.DetailScreenList.ForEach((screen) =>
       screens.LinkScreenToNode(applicationInfo, dataSourceNode, screen, AdlibEdgeType.UsesListScreen));
    }

    public int UnusedCount
    {
      get
      {
        int count = 0;
        foreach (var node in methods.Values)
        {
          if (node.ReverseEdges.Count == 0)
          {
            count++;
          }
        }
        return count;
      }
    }

    readonly SortedDictionary<string, MethodNode> methods = new SortedDictionary<string, MethodNode>();
    internal IEnumerable<MethodNode> Values => methods.Values;
    public int Count => methods.Count;
  }
}
