using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Adlib.Database.IO;
using Adlib.Setup.Application;
using Adlib.Setup.Constants;
using Adlib.Setup.Database;
using Adlib.Interfaces;
using Adlib.Database;

namespace DDigit.Graph
{
  public class NodesCollection
  {
    public void LoadNodes(string folder)
    {
      LoadObjects(folder);
    }

    void LoadObjects(string folder)
    {
      var directoryInfo = new DirectoryInfo(folder);

      LoadScreens(directoryInfo, storage);
      LoadDatabases(directoryInfo, storage);
      LoadApplications(directoryInfo, storage);

      foreach (var subFolder in directoryInfo.GetDirectories())
      {
        LoadObjects(subFolder.FullName);
      }
    }

    IDatabaseEngine OpenDatabase(string path, string user = null, bool useTransactions = true)
    {
      var databaseEngine = DatabaseEngine.CreateDatabaseEngine(path, user ?? Environment.UserName);
      databaseEngine.UseTransactions = useTransactions;
      return databaseEngine;
    }

    public void DeleteIndexes()
    {
      var deleteIndexes = new List<IIndexInfo>();
      foreach (var node in Indexes.Values)
      {
        bool used = false;
        foreach (var reverseEdge in node.ReverseEdges)
        {
          var sourceNode = reverseEdge.Source;
          foreach (var forwardEdge in sourceNode.Edges)
          {
            if (forwardEdge.Target == node && forwardEdge.Traversed)
            {
              used = true;
              break;
            }
          }
          if (used)
          {
            break;
          }
        }
        if (!used)
        {
          deleteIndexes.Add(node.Index);
        }
      }

      var deleteFields = new List<IFieldInfo>();
      foreach (var node in Fields.Values)
      {
        bool used = false;
        foreach (var reverseEdge in node.ReverseEdges)
        {
          var sourceNode = reverseEdge.Source;
          foreach (var forwardEdge in sourceNode.Edges)
          {
            if (forwardEdge.Target == node && forwardEdge.Traversed)
            {
              used = true;
              break;
            }
          }
          if (used)
          {
            break;
          }
        }
        if (!used && node.Field.Tag != "%0")
        {
          deleteFields.Add(node.Field);
        }
      }


      var updateDatabases = new List<IAdlibDatabaseInfo>();
      foreach (var indexInfo in deleteIndexes)
      {
        var databaseInfo = indexInfo.DatabaseInfo;
        using (DatabaseEngine databaseEngine = (DatabaseEngine)OpenDatabase(databaseInfo.PhysicalPath))
        {
          databaseInfo.IndexList.Remove(indexInfo);
          databaseEngine.DeleteIndexTable(indexInfo);
          if (!updateDatabases.Contains(databaseInfo))
          {
            updateDatabases.Add(databaseInfo);
          }
          IndexDeleted?.Invoke(this, new IndexDeletedEventArgs(indexInfo.TableName));
        }
      }

      foreach (var fieldInfo in deleteFields)
      {
        var databaseInfo = (DatabaseInfo)fieldInfo.DatabaseInfo;
        databaseInfo.Delete(fieldInfo);
        if (!updateDatabases.Contains(databaseInfo))
        {
          updateDatabases.Add(databaseInfo);
        }
        FieldDeleted?.Invoke(this, new FieldDeletedEventArgs(fieldInfo.Name));
      }

      foreach (var databaseInfo in updateDatabases)
      {
        databaseInfo.Save();
      }
    }

    public void DeleteScreens()
    {
      foreach (var node in Screens.Values)
      {
        if (node.ReverseEdges.Count == 0)
        {
          File.Delete(node.Path);
          ScreenDeleted?.Invoke(this, new ScreenDeletedEventArgs(node.Path));
        }
      }
    }

    public event EventHandler<ScreenDeletedEventArgs> ScreenDeleted;
    public event EventHandler<IndexDeletedEventArgs> IndexDeleted;
    public event EventHandler<FieldDeletedEventArgs> FieldDeleted;

    void LoadApplications(DirectoryInfo directoryInfo, Storage storage)
    {
      var fileInfo = directoryInfo.GetFiles("adlib.pbk");
      foreach (var file in fileInfo)
      {
        var path = file.FullName.ToLower();
        var applicationInfo = new ApplicationInfo(path, storage);
        var applicationNode = new ApplicationNode(path, applicationInfo);
        Applications.Add(path, applicationNode);

        foreach (var dataSourceInfo in applicationInfo.DataSourceList)
        {
          if (dataSourceInfo.DatabaseType == DatabaseType.NormalDatabase)
          {
            var dataSourceNode = new DataSourceNode(applicationInfo, dataSourceInfo);
            Datasources.Add(dataSourceNode.Path, dataSourceNode);
          }
        }
      }
    }

    static readonly XNamespace dgmlNS = "http://schemas.microsoft.com/vs/2009/dgml";

    public void SaveDgml(string fileName)
    {
      var dgml = new XDocument();
      var root = new XElement(dgmlNS + "DirectedGraph", new XAttribute("Title", "Adlib dependencies"));

      var DGMLNodes = new XElement(dgmlNS + "Nodes");
      var DGMLCategories = new XElement(dgmlNS + "Categories");
      var DGMLLinks = new XElement(dgmlNS + "Links");

      root.Add(DGMLNodes, DGMLCategories, DGMLLinks);
      dgml.Add(root);

      foreach (var node in Applications.Values)
      {
        DGMLNodes.Add(CreateDGMLNode(node));
        AddDGMLLinks(node, DGMLNodes, DGMLLinks);
      }

      foreach (var node in Databases.Values)
      {
        DGMLNodes.Add(CreateDGMLNode(node));
      }

      foreach (var node in Datasources.Values)
      {
        DGMLNodes.Add(CreateDGMLNode(node));
      }

      foreach (var node in Screens.Values)
      {
        DGMLNodes.Add(
         new XElement(dgmlNS + "Node",
         new XAttribute("Id", node.Path),
         new XAttribute("Label", $"{node.Name} ({node.Screen.FileName})"),
         new XAttribute("Category", node.GetType().Name)));
      }

      foreach (var node in Fields.Values)
      {
        DGMLNodes.Add(
         new XElement(dgmlNS + "Node",
         new XAttribute("Id", node.Path),
         new XAttribute("Label", $"{node.Name} ({node.Field.Tag})"),
         new XAttribute("Category", node.GetType().Name)));
      }

      foreach (var node in Indexes.Values)
      {
        DGMLNodes.Add(CreateDGMLNode(node));
      }

      foreach (var node in Methods.Values)
      {
        DGMLNodes.Add(CreateDGMLNode(node));
      }

      foreach (var node in Applications.Values)
      {
        AddDGMLLinks(node, DGMLNodes, DGMLLinks);
      }

      foreach (var node in Databases.Values)
      {
        bool linked = false;
        foreach (var edge in node.Edges)
        {
          if (edge.EdgeType == AdlibEdgeType.UsesDatabase)
          {
            linked = true;
            break;
          }
        }
        if (!linked)
        {
          AddDGMLLinks(node, DGMLNodes, DGMLLinks);
        }
      }

      foreach (var type in new Type[]
      {
        typeof(ApplicationNode),
        typeof(DatabaseNode),
        typeof(MethodNode),
        typeof(DataSourceNode),
        typeof(ScreenNode),
        typeof(IndexNode),
        typeof(FieldNode)
      })
      {
        var fieldInfo = type.GetProperty("Color");
        var color = (NodeColors)fieldInfo.GetValue(null);
        var objectType = type.Name;

        DGMLCategories.Add(new XElement(dgmlNS + "Category",
         new XAttribute("Id", objectType),
         new XAttribute("Background", color),
         new XAttribute("Label", objectType))
        );
      };


      DGMLCategories.Add(new XElement(dgmlNS + "Category",
        new XAttribute("Id", "Traversed"),
        new XAttribute("Background", "Red")));

      dgml.Save(fileName);
    }

    void AddDGMLLinks(AdlibNode node, XElement dGMLNodes, XElement dGMLLinks)
    {
      foreach (var edge in node.Edges)
      {
        var childNode = edge.Target;
        dGMLLinks.Add(CreateDGMLLink(edge));
        AddDGMLLinks(childNode, dGMLNodes, dGMLLinks);
      }
    }

    XElement CreateDGMLLink(AdlibEdge edge)
    {
      var element = new XElement(dgmlNS + "Link",
        new XAttribute("Source", edge.Source.Path),
        new XAttribute("Target", edge.Target.Path),
        new XAttribute("Label", edge.EdgeType)
      );
      if (edge.Traversed)
      {
        element.Add(new XAttribute("Category", "Traversed"));
      }
      return element;
    }

    static XElement CreateDGMLNode(AdlibNode node) =>
      new XElement(dgmlNS + "Node",
        new XAttribute("Id", node.Path),
        new XAttribute("Label", node.Name),
        new XAttribute("Category", node.GetType().Name));


    public void CreateApplicationEdges() => Applications.CreateEdges(Datasources, Methods, Screens, Databases, Fields, Indexes);

    public void CreateDatabaseEdges() => Databases.CreateEdges(Fields, Indexes, Screens);

    void LoadDatabases(DirectoryInfo directoryInfo, Storage storage) =>
      Databases.LoadDatabases(directoryInfo, storage, Fields, Indexes);

    void LoadScreens(DirectoryInfo directoryInfo, Storage storage) => Screens.LoadScreens(directoryInfo, storage);

    internal FieldList Fields { get; } = new FieldList();
    internal DataSourceList Datasources { get; } = new DataSourceList();
    internal ApplicationList Applications { get; } = new ApplicationList();
    internal IndexList Indexes { get; } = new IndexList();
    public MethodList Methods { get; } = new MethodList();
    public ScreenList Screens { get; } = new ScreenList();
    public DatabaseList Databases { get; } = new DatabaseList();
    public int ApplicationCount => Applications.Count;
    public int DatabaseCount => Databases.Count;
    public int ScreenCount => Screens.Count;
    public int MethodCount => Methods.Count;
    public int FieldCount => Fields.Count;
    public int IndexCount => Indexes.Count;


    public int UnusedDatabaseCount
    {
      get
      {
        int count = 0;
        foreach (var node in Databases.Values)
        {
          if (node.ReverseEdges.Count == 0)
          {
            count++;
          }
        }
        return count;
      }
    }

    public int UnusedScreensCount => Screens.UnusedCount;
    public int UnusedMethodsCount => Methods.UnusedCount;
    public int UnusedFieldsCount => Fields.UnusedCount;
    public int UnusedIndexesCount => Indexes.UnusedCount;

    readonly Storage storage = new Storage();

  }
}
