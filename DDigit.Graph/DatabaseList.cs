using Adlib.Database.IO;
using Adlib.Interfaces;
using Adlib.Setup;
using Adlib.Setup.Database;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDigit.Graph
{
  public class DatabaseList : NodeList
  {
    internal void CreateEdges(FieldList fields, IndexList indexes, ScreenList screens)
    {
      foreach (var database in databases.Values)
      {
        var databaseInfo = database.Database;
        var databaseNode = FindDatabaseNode(databaseInfo);
        if (databaseNode != null)
        {
          foreach (var fieldInfo in databaseInfo.FieldInfoCollection.Values)
          {
            var fieldNode = fields.FindFieldNode(databaseInfo, fieldInfo.Tag);
            if (fieldInfo.IsLinked)
            {
              LinkFieldNodeToDatabaseNode(databaseNode, fieldNode);
              var linkedFieldNode = fields.FindFieldNode(databaseInfo, fieldInfo.LinkRefTag);
              if (fieldNode != null && linkedFieldNode != null)
              {
                AddEdge(fieldNode, AdlibEdgeType.UsesLinkRef, linkedFieldNode);
                foreach (var indexNode in indexes.FindIndexNodes(databaseInfo, fieldInfo.LinkRefTag))
                {
                  AddEdge(linkedFieldNode, AdlibEdgeType.IndexedIn, indexNode);
                }
              }
              screens.LinkScreenToNode(databaseInfo, fieldNode, fieldInfo.LinkScreen, AdlibEdgeType.UsesLinkScreen);
              screens.LinkScreenToNode(databaseInfo, fieldNode, fieldInfo.ZoomScreen, AdlibEdgeType.UsesZoomScreen);
              screens.LinkScreenToNode(databaseInfo, fieldNode, fieldInfo.EditScreen, AdlibEdgeType.UsesEditScreen);
            }

            if (!fieldInfo.IsLinkRef)
            {
              LinkFieldNodeToDatabaseNode(databaseNode, fieldNode);
              foreach (var indexNode in indexes.FindIndexNodes(databaseInfo, fieldInfo.Tag))
              {
                AddEdge(fieldNode, AdlibEdgeType.IndexedIn, indexNode);
              }
            }
          }
        }
      }
    }

    void LinkFieldNodeToDatabaseNode(AdlibNode databaseNode, AdlibNode fieldNode)
    {
      if (databaseNode != null && fieldNode != null)
      {
        AddEdge(databaseNode, AdlibEdgeType.HasField, fieldNode);
      }
    }

    internal void LoadDatabases(DirectoryInfo directoryInfo, Storage storage, FieldList fields, IndexList indexes)
    {
      var fileInfo = directoryInfo.GetFiles("*.inf");
      foreach (var file in fileInfo)
      {
        if (file.FullName.EndsWith(".inf", StringComparison.CurrentCultureIgnoreCase))
        {
          var databaseInfo = new DatabaseInfo(new AdlibPath(file.FullName), storage);
          var databaseNode = new DatabaseNode(databaseInfo);
          databases[databaseNode.Path] = databaseNode;

          foreach (var fieldInfo in databaseInfo.FieldInfoCollection.Values)
          {
            var fieldNode = new FieldNode(databaseInfo, fieldInfo);
            fields.Add(fieldNode.Path, fieldNode);
          }

          foreach (var indexInfo in databaseInfo.IndexList)
          {
            if (indexInfo.FirstIndexTag != "%0" && indexInfo.IndexName != "wordlist")
            {
              var indexNode = new IndexNode(databaseInfo, indexInfo);
              indexes.Add(indexNode.Path, indexNode);
            }
          }
        }
      }
    }

    DatabaseNode FindDatabaseNode(DatabaseInfo databaseInfo) => databases.TryGetValue(DatabaseNode.DatabasePath(databaseInfo), out DatabaseNode node) ? node : null;
    internal DatabaseNode this[string path] => databases[path];
    internal IEnumerable<DatabaseNode> Values => databases.Values;
    public int Count => databases.Count;

    readonly SortedDictionary<string, DatabaseNode> databases = new SortedDictionary<string, DatabaseNode>();

  }
}
