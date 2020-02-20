using Adlib.Setup.Application;

namespace DDigit.Graph
{
  internal class DataSourceNode : AdlibNode
  {
    public DataSourceNode(ApplicationInfo applicationInfo, DataSourceInfo dataSourceInfo) :
      base(DataSourceList.Path(applicationInfo, dataSourceInfo), dataSourceInfo.TextList[0].Text)
    {
      DataSource = dataSourceInfo;
    }

    internal DataSourceInfo DataSource { get; }
    public static NodeColors Color => NodeColors.Orange;
  }
}
