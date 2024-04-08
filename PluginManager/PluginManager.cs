using System.Reflection;
using System.Runtime.InteropServices;
using WPInterfaces;
using WPLoggingLibrary;

namespace PluginManager
{
    public sealed class PluginManager
    {
        private static readonly Lazy<PluginManager> _instance = new(() => new PluginManager());
        private readonly List<IPlugin> _loadedPlugins;
        private readonly FileLogger _logger;

        public static PluginManager Instance => _instance.Value;

        private PluginManager()
        {
            _loadedPlugins = [];
            _logger = FileLogger.Instance;
        }

        ~PluginManager()
        {
            Dispose();
        }

        public void ExecutePlugin(string pluginName, string[] args)
        {
            foreach(var t in _loadedPlugins)
            {
                if (t.Name.Equals(pluginName))
                {
                    t.Execute(args);
                    return;
                }
            }
        }

        public void LoadPlugins(string pluginDirectory)
        {
            var dllPaths = Directory.GetFiles(pluginDirectory, "*.dll");
            LoadPluginsFromPaths(dllPaths);
        }

        public void LoadPlugins(IEnumerable<string> dllPaths)
        {
            LoadPluginsFromPaths(dllPaths);
        }

        public void LoadPluginByName(string pluginName, string pluginDirectory)
        {
            var pluginPath = Directory.GetFiles(pluginDirectory, "*.dll")
                                       .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p).Equals(pluginName, StringComparison.OrdinalIgnoreCase));
            if (pluginPath != null)
            {
                LoadPluginFromPath(pluginPath);
            }
            else
            {
                LogWarning($"Plugin '{pluginName}' not found in directory '{pluginDirectory}'.");
            }
        }

        public void UnloadPluginByName(string pluginName)
        {
            var pluginToRemove = _loadedPlugins.FirstOrDefault(p => p.Name.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
            if (pluginToRemove != null)
            {
                _loadedPlugins.Remove(pluginToRemove);
                LogInfo($"Plugin '{pluginName}' unloaded.");
            }
            else
            {
                LogWarning($"Plugin '{pluginName}' is not loaded.");
            }
        }

        public void UnloadAllPlugins()
        {
            _loadedPlugins.Clear();
            LogDebug("All plugins unloaded.");
        }

        private void LoadPluginsFromPaths(IEnumerable<string> dllPaths)
        {
            foreach (var dllPath in dllPaths)
            {
                LoadPluginFromPath(dllPath);
            }
        }

        private void LoadPluginFromPath(string dllPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                if (assembly != null)
                {
                    var pluginTypes = assembly.GetTypes()
                                              .Where(type => typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

                    foreach (var type in pluginTypes)
                    {
                        var pluginObject = Activator.CreateInstance(type);
                        if (pluginObject != null && pluginObject is IPlugin plugin)
                        {
                            _loadedPlugins.Add(plugin);
                            LogDebug($"Plugin '{plugin.Name}' loaded.");
                        }
                    }
                }
                else
                {
                    LogError($"Assembly could not be loaded: {dllPath}");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error loading plugins from '{dllPath}': {ex.Message}");
            }
        }

        private void LogDebug(string message)
        {
            _logger?.Log(LogLevel.Debug, message);
        }

        private void LogInfo(string message)
        {
            _logger?.Log(LogLevel.Info, message);
        }

        private void LogWarning(string message)
        {
            _logger?.Log(LogLevel.Warning, message);
        }

        private void LogError(string message)
        {
            _logger?.Log(LogLevel.Error, message);
        }

        public void Dispose()
        {
            _logger?.Dispose();
        }
    }
}
