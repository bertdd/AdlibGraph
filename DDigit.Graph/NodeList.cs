using Adlib.Setup.Application;
using System.Collections.Generic;
using System.IO;

namespace DDigit.Graph
{
  public abstract class NodeList
  {
    protected static string DatabasePath(ApplicationInfo applicationInfo, DataSourceInfo dataSourceInfo) =>
     new FileInfo(Path.Combine(Path.GetDirectoryName(applicationInfo.PhysicalPath), dataSourceInfo.Directory) +
                             Path.DirectorySeparatorChar + dataSourceInfo.DatabaseName + ".inf").FullName.ToLower();
    protected static string ApplicationPath(ApplicationInfo applicationInfo) => applicationInfo.PhysicalPath.ToLower();

    internal static void AddEdge(AdlibNode source, AdlibEdgeType type, AdlibNode target)
    {
      var edge = new AdlibEdge(source, type, target);
      AddSingleEdge(source.Edges, edge);
      AddSingleEdge(target.ReverseEdges, edge);
    }

    protected static string AddExtension(string fileName, string extension) => fileName.EndsWith(extension) ? fileName : fileName + extension;

    protected static void AddSingleEdge(List<AdlibEdge> edges, AdlibEdge edge)
    {
      foreach (var existingEdge in edges)
      {
        if (edge.Source == existingEdge.Source && edge.Target == existingEdge.Target && edge.EdgeType == existingEdge.EdgeType)
        {
          return;
        }
      }
      edges.Add(edge);
    }
  }
}
