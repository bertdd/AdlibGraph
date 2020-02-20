using Adlib.Setup;
using Adlib.Setup.Application;
using Adlib.Setup.Constants;
using Adlib.Setup.Screen;
using Adlib.Interfaces;
using System.Collections.Generic;
using System.IO;
using System;

namespace DDigit.Graph
{
  public class ScreenList : NodeList
  {
    internal void LinkScreenToNode(TopAdlibObject adlibObject, AdlibNode node, string screen, AdlibEdgeType edgeType)
    {
      if (!string.IsNullOrWhiteSpace(screen))
      {
        var path = ScreenPath(adlibObject, screen);
        if (screens.ContainsKey(path))
        {
          AddEdge(node, edgeType, screens[path]);
        }
      }
    }

    internal void LinkScreensToDataSourceNode(ApplicationInfo applicationInfo, DatabaseList databases, FieldList fields, IndexList indexes,
                                              Adlib.Setup.ScreenList screenList, string databasePath)
    {
      foreach (var screen in screenList)
      {
        if (!string.IsNullOrWhiteSpace(screen))
        {
          string path = ScreenPath(applicationInfo, screen);
          if (screens.ContainsKey(path))
          {
            var screenNode = screens[path];
            if (screenNode != null && screenNode.Screen != null)
            {
              // Link the fields that are used on the screen to the screen
              foreach (var field in screenNode.Screen.FieldList)
              {

                if (field.Type == ScreenObjectType.Data || field.Type == ScreenObjectType.HtmlField || field.Type == ScreenObjectType.Image)
                {
                  var fieldNode = fields.FindFieldNode(databasePath + '\\' + field.Tag);
                  if (fieldNode != null)
                  {
                    AddEdge(screenNode, AdlibEdgeType.ScreenUsesField, fieldNode);

                    var databaseNode = databases[databasePath];
                    AddEdge(fieldNode, AdlibEdgeType.FieldOf, databaseNode);

                    foreach (var indexNode in indexes.FindIndexNodes(fieldNode.Field.DatabaseInfo, fieldNode.Field.Tag))
                    {
                      AddEdge(fieldNode, AdlibEdgeType.IndexedIn, indexNode);
                      AddEdge(indexNode, AdlibEdgeType.IndexOf, databaseNode);
                    }

                    if (fieldNode.Field.IsLinked)
                    {
                      // this is a linked screen, add the linkref field and create an edge to the database
                      var fieldInfo = fieldNode.Field;
                      var linkedDatabasePath = DatabaseNode.DatabasePath(fieldInfo.LinkedDatabaseInfo);
                      var linkedDatabaseNode = databases[linkedDatabasePath];

                      var linkRefTag = fieldInfo.LinkRefTag;
                      if (!string.IsNullOrWhiteSpace(linkRefTag))
                      {
                        var linkRefFieldPath = FieldNode.FieldPath(fieldInfo.DatabaseInfo, linkRefTag);
                        var linkRefFieldNode = fields.FindFieldNode(linkRefFieldPath);
                        if (linkRefFieldNode == null)
                        {
                          Console.WriteLine($"Application ERROR: link ref tag '{linkRefTag}' not defined in database '{fieldInfo.DatabaseInfo.BaseName}'");
                        }
                        else
                        {
                          AddEdge(fieldNode, AdlibEdgeType.UsesLinkRef, linkRefFieldNode);
                          AddEdge(linkRefFieldNode, AdlibEdgeType.FieldOf, linkedDatabaseNode);

                          foreach (var indexNode in indexes.FindIndexNodes(linkRefFieldNode.Field.DatabaseInfo, linkRefFieldNode.Field.Tag))
                          {
                            AddEdge(linkRefFieldNode, AdlibEdgeType.IndexedIn, indexNode);
                            AddEdge(indexNode, AdlibEdgeType.IndexOf, linkedDatabaseNode);
                          }
                        }
                      }

                      // we also need to add the term field for the linked database
                      var linkIndexTag = fieldInfo.LinkIndexTag;
                      if (!string.IsNullOrWhiteSpace(linkIndexTag))
                      {
                        var linkIndexFieldPath = fieldNode.FieldPath(applicationInfo, fieldInfo.LinkedDatabaseInfo, linkIndexTag);
                        var linkIndexFieldNode = fields.FindFieldNode(linkIndexFieldPath);
                        if (linkIndexFieldNode != null)
                        {
                          AddEdge(fieldNode, AdlibEdgeType.UsesLinkField, linkIndexFieldNode);

                          // create an edge from the linkindexfield to the database node
                          AddEdge(linkIndexFieldNode, AdlibEdgeType.FieldOf, linkedDatabaseNode);

                          foreach (var indexNode in indexes.FindIndexNodes(linkIndexFieldNode.Field.DatabaseInfo, linkIndexFieldNode.Field.Tag))
                          {
                            AddEdge(linkIndexFieldNode, AdlibEdgeType.IndexedIn, indexNode);
                            AddEdge(indexNode, AdlibEdgeType.IndexOf, linkedDatabaseNode);
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    internal void LoadScreens(DirectoryInfo directoryInfo, IIOAdapter storage)
    {
      foreach (var file in directoryInfo.GetFiles("*.fmt"))
      {
        var screenNode = new ScreenNode(new ScreenInfo(file.FullName, storage));
        screens[screenNode.Path] = screenNode;
      }
    }

    public int UnusedCount
    {
      get
      {
        int count = 0;
        foreach (var node in screens.Values)
        {
          if (node.ReverseEdges.Count == 0)
          {
            count++;
          }
        }
        return count;
      }
    }

    string ScreenPath(TopAdlibObject adlibObject, string screenPath) => ScreenPath(Path.Combine(Path.GetDirectoryName(adlibObject.PhysicalPath), screenPath));
    string ScreenPath(string absolutePath) => AddExtension(new FileInfo(absolutePath).FullName.ToLower(), ".fmt");
    readonly SortedDictionary<string, ScreenNode> screens = new SortedDictionary<string, ScreenNode>();
    internal IEnumerable<ScreenNode> Values => screens.Values;
    internal int Count => screens.Count;
  }
}

