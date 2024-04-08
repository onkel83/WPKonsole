
using WPLoggingLibrary;

Console.WriteLine("Hello, World!");
FileLogger.Instance.Log(LogLevel.Debug, "Test Eintrag!");
FileLogger.Instance.Dispose();
Console.WriteLine("Test Eintrag erstellt!");
Console.ReadLine();