using Adlib.Setup.Application;

namespace DDigit.Graph
{
  internal class ApplicationNode : AdlibNode
  {
    internal ApplicationNode(string path, ApplicationInfo applicationInfo) : base(path, applicationInfo.TextList[0].Text)
    {
      Application = applicationInfo;
    }

    public static NodeColors Color => NodeColors.Red;
    public ApplicationInfo Application { get; }
  }
}