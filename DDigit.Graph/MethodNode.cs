using Adlib.Setup;
using Adlib.Setup.Application;

namespace DDigit.Graph
{
  internal class MethodNode : AdlibNode
  {
    public MethodNode(string parentPath, MethodInfo methodInfo) :
      base(parentPath + '\\' + methodInfo.TextList[0].Text, methodInfo.TextList[0].Text)
    {
      MethodInfo = methodInfo;
    }

    public static NodeColors Color => NodeColors.LightGreen;
    internal AdlibObject MethodInfo { get; }
  }
}
