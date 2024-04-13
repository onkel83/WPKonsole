using PluginManager;

var plugins = PluginManager.PluginManager.Instance;
plugins.LoadPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));
bool isRunning = true;
Console.WriteLine("Willkommen beim Testprogramm by [W]inemp[P]roductions");
Console.WriteLine("Folgende Kommandos sind bekannt :");
Console.WriteLine("[azc] = Arbeitszeit Verwaltung");
Console.WriteLine("[uc] = User Verwaltung");
Console.WriteLine("[quit] = Beenden");
while (isRunning)
{
    switch (Console.ReadLine()?.ToLower())
    {
        case "azc":
            plugins.ExecutePlugin("ArbeitszeitPlugin", ["con"]);break;
        case "uc":
            plugins.ExecutePlugin("USerPlugin", ["con"]); break;
        case "quit":
            isRunning = false; break;
        default: break;
    }
}
plugins.Dispose();
Console.WriteLine("Ende des Programms!");
Console.ReadLine();