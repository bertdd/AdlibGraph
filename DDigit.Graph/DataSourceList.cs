using Adlib.Setup.Application;
using System.Collections.Generic;

namespace DDigit.Graph
{
  internal class DataSourceList : NodeList
  {
    internal DataSourceNode FindDataSourceNode(ApplicationInfo applicationInfo, DataSourceInfo dataSourceInfo) => datasources[Path(applicationInfo, dataSourceInfo)];

    public static string Path(ApplicationInfo applicationInfo, DataSourceInfo dataSourceInfo) =>
      $"{ApplicationPath(applicationInfo)}\\{dataSourceInfo.TextList[0].Text}";

    readonly SortedDictionary<string, DataSourceNode> datasources = new SortedDictionary<string, DataSourceNode>();
    public IEnumerable<DataSourceNode> Values => datasources.Values;
    internal void Add(string path, DataSourceNode datasourceNode) => datasources[path] = datasourceNode;
  }
}
