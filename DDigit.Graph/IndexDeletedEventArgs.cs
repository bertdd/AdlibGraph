using System;

namespace DDigit.Graph
{
  public class IndexDeletedEventArgs : EventArgs
  {
    public IndexDeletedEventArgs(string tableName)
    {
      TableName = tableName;
    }

    public string TableName { get; private set; }
  }
}