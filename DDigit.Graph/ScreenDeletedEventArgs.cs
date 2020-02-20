using System;

namespace DDigit.Graph

{
  public class ScreenDeletedEventArgs : EventArgs
  {
    public ScreenDeletedEventArgs(string path)
    {
      Path = path;
    }

    public string Path { get; }
  }
}