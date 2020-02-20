using System;

namespace DDigit.Graph
{
  public class FieldDeletedEventArgs : EventArgs
  {

    public FieldDeletedEventArgs(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}