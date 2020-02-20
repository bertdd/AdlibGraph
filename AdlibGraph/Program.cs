using DDigit.Graph;

using System;
using System.Diagnostics;
using System.IO;

namespace AdlibGraph
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("[AdlibGraph Version 1.0]");
      if (args.Length < 2)
      {
        Console.WriteLine("Usage: AdlibGraph applicationFolder dgmlFile");
        return;
      }

      var adlibNodes = new NodesCollection();

      try
      {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        adlibNodes.LoadNodes(args[0]);

        adlibNodes.CreateDatabaseEdges();
        adlibNodes.CreateApplicationEdges();
        stopWatch.Stop();

        Console.WriteLine($"Data for '{args[0]}' loaded in {stopWatch.ElapsedMilliseconds} mS.\n");
        const int columnWidth = 8;

        Console.WriteLine($"               {"Nodes",columnWidth} {"Unused",columnWidth} {"%",columnWidth}");
        Console.WriteLine($"Applications : {adlibNodes.ApplicationCount,columnWidth:#} {0,columnWidth:#}");
        Console.WriteLine($"Methods      : {adlibNodes.MethodCount,columnWidth:#} {adlibNodes.UnusedMethodsCount,columnWidth:#}");
        Console.WriteLine($"Databases    : {adlibNodes.DatabaseCount,columnWidth:#} {adlibNodes.UnusedDatabaseCount,columnWidth:#} {adlibNodes.UnusedDatabaseCount / (double)adlibNodes.DatabaseCount,columnWidth:P1}");
        Console.WriteLine($"Screens      : {adlibNodes.ScreenCount,columnWidth:#} {adlibNodes.UnusedScreensCount,columnWidth:#} {adlibNodes.UnusedScreensCount / (double)adlibNodes.ScreenCount,columnWidth:P1}");
        Console.WriteLine($"Fields       : {adlibNodes.FieldCount,columnWidth:#} {adlibNodes.UnusedFieldsCount,columnWidth:#} {adlibNodes.UnusedFieldsCount / (double)adlibNodes.FieldCount,columnWidth:P1}");
        Console.WriteLine($"Indexes      : {adlibNodes.IndexCount,columnWidth:#} {adlibNodes.UnusedIndexesCount,columnWidth:#} {adlibNodes.UnusedIndexesCount / (double)adlibNodes.IndexCount,columnWidth:P1}");

        Console.WriteLine();
        Console.WriteLine($"Writing {args[1]}");
        adlibNodes.SaveDgml(args[1]);
        Console.WriteLine("Done");

        //adlibNodes.DeleteIndexes();
        //adlibNodes.DeleteScreens();
      }
      catch (DirectoryNotFoundException ex)
      {
        Console.WriteLine($"Error: {ex.Message}");
      }
    }
  }
}
