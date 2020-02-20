using Adlib.Interfaces;
using Adlib.Setup.Application;
using Adlib.Setup.Database;
using System.IO;

namespace DDigit.Graph
{
  internal class FieldNode : AdlibNode
  {
    public FieldNode(DatabaseInfo databaseInfo, IFieldInfo fieldInfo) :
      base(FieldPath(databaseInfo, fieldInfo.Tag), fieldInfo.Name)
    {
      Field = fieldInfo;
    }

    internal FieldNode(string path, string name) : base(path, name)
    {

    }


    public static string FieldPath(IAdlibDatabaseInfo databaseInfo, string tag) => $"{DatabaseNode.DatabasePath(databaseInfo)}\\{tag}";

    public string FieldPath(ApplicationInfo applicationInfo, IAdlibDatabaseInfo databaseInfo, string tag)
    {
      var combinedPath = System.IO.Path.Combine(applicationInfo.PhysicalPath, databaseInfo.PhysicalPath);
      var fileInfo = new FileInfo(combinedPath);
      return $"{fileInfo.FullName.ToLower()}\\{tag}";
    }

    public IFieldInfo Field { get; }
    public static NodeColors Color => NodeColors.DarkGreen;
  }
}
