using Adlib.Interfaces;
using Adlib.Setup.Database;
using System.IO;

namespace DDigit.Graph
{
  internal class DatabaseNode : AdlibNode
  {
    internal DatabaseNode(DatabaseInfo databaseInfo) : base(DatabasePath(databaseInfo), databaseInfo.BaseName)
    {
      Database = databaseInfo;
    }

    public static NodeColors Color => NodeColors.Blue;
    internal DatabaseInfo Database { get; }
    internal static string DatabasePath(IAdlibDatabaseInfo databaseInfo)
    {
      var fileInfo = new FileInfo(databaseInfo.PhysicalPath);
      return fileInfo.FullName.ToLower();
    }
  }
}
