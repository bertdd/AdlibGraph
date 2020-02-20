using Adlib.Setup.Application;
using Adlib.Setup.Constants;
using System.Collections.Generic;

namespace DDigit.Graph
{
  internal class ApplicationList : NodeList
  {
    public void CreateEdges(DataSourceList datasources, MethodList methods, ScreenList screens, DatabaseList databases, FieldList fields, IndexList indexes)
    {
      foreach (var applicationNode in applications.Values)
      {
        applicationNode.Application.DataSourceList.ForEach((dataSourceInfo) =>
          AddDataSourceEdgeToApplicationNode(applicationNode, dataSourceInfo, datasources, methods, screens, databases, fields, indexes));
      }

      foreach (var applicationNode in applications.Values)
      {
        ColorEdges(applicationNode);
      }
    }

    void ColorEdges(AdlibNode node)
    {
      foreach (var edge in node.Edges)
      {
        edge.Traversed = true;
        var child = edge.Target;
        ColorEdges(child);
      }
    }

    void AddDataSourceEdgeToApplicationNode(ApplicationNode applicationNode, DataSourceInfo dataSourceInfo,
                                            DataSourceList datasources, MethodList methods, ScreenList screens, DatabaseList databases, FieldList fields, IndexList indexes)
    {
      var applicationInfo = applicationNode.Application;
      if (dataSourceInfo.DatabaseType == DatabaseType.NormalDatabase)
      {
        var dataSourceNode = datasources.FindDataSourceNode(applicationInfo, dataSourceInfo);

        AddEdge(applicationNode, AdlibEdgeType.HasDataSource, dataSourceNode);

        dataSourceInfo.ListScreenList.ForEach(screenName =>
          screens.LinkScreenToNode(applicationInfo, dataSourceNode, screenName, AdlibEdgeType.UsesListScreen));
        dataSourceInfo.DetailScreenList.ForEach(screenName =>
          screens.LinkScreenToNode(applicationInfo, dataSourceNode, screenName, AdlibEdgeType.UsesDetailScreen));
        dataSourceInfo.MethodList.ForEach(methodInfo =>
          methods.LinkMethodToNode(applicationInfo, dataSourceNode, methodInfo, screens));

        var databasePath = DatabasePath(applicationInfo, dataSourceInfo);

        screens.LinkScreensToDataSourceNode(applicationInfo, databases, fields, indexes, dataSourceInfo.DetailScreenList, databasePath);
        screens.LinkScreensToDataSourceNode(applicationInfo, databases, fields, indexes, dataSourceInfo.ListScreenList, databasePath);
      }
    }

    readonly SortedDictionary<string, ApplicationNode> applications = new SortedDictionary<string, ApplicationNode>();
    internal IEnumerable<ApplicationNode> Values => applications.Values;
    internal void Add(string path, ApplicationNode applicationNode) => applications[path] = applicationNode;
    internal int Count => applications.Count;
  }
}
