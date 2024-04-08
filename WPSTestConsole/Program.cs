using PluginManager;

var plugins = PluginManager.PluginManager.Instance;
plugins.LoadPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));
plugins.ExecutePlugin("UserPlugin", ["con"]);
plugins.Dispose();
Console.WriteLine("Ende des Programms!");
Console.ReadLine();