using Adlib.Setup.Database;
using Adlib.Interfaces;

namespace DDigit.Graph
{
  internal class IndexNode : AdlibNode
  {
    public IndexNode(DatabaseInfo databaseInfo, IIndexInfo indexInfo) :
      base(IndexList.Path(databaseInfo, indexInfo), $"{indexInfo.IndexName} ({databaseInfo.BaseName})")
    {

    }

    internal IndexInfo Index { get; }
    public static NodeColors Color => NodeColors.LightBlue;
  }
}