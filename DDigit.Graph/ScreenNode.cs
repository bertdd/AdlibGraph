using Adlib.Setup.Screen;

namespace DDigit.Graph
{
  internal class ScreenNode : AdlibNode
  {
    public ScreenNode(ScreenInfo screenInfo) : base(ScreenPath(screenInfo), screenInfo.NeutralDescription)
    {
      Screen = screenInfo;
    }

    public static NodeColors Color => NodeColors.Yellow;
    static string ScreenPath(ScreenInfo screenInfo) => screenInfo.PhysicalPath.ToLower();
    public ScreenInfo Screen { get; }
  }
}
