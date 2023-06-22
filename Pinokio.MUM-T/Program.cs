// See https://aka.ms/new-console-template for more information
using Pinokio.Core;
using System.Diagnostics;
using System.Reflection.Metadata;

LogHandler.LogInfoHandle += Console.WriteLine;
LogHandler.LogErrorHandle += Console.WriteLine;
Console.WriteLine("Hello, World!");


static bool LoadLexicon()
{
    LogHandler.AddLog(LogLevel.Info, "Start to Load Lexicon");

    if (GetZMQPort(out int zmqPort))
    {
        var sw = new Stopwatch();
        sw.Start();
        document = new RLDocument(zmqPort);
        document.GenerateMapModel("SMT");
        document.LoadMapData(printDot);
        LogHandler.AddLog(LogLevel.Info, "");
        sw.Stop();

        LogHandler.AddLog(LogLevel.Info, $"Finish to Load Models({sw.ElapsedMilliseconds})");
        return true;
    }

    return false;
}