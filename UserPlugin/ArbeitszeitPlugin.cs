using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserPlugin;
using WPInterfaces;
using WPLoggingLibrary;

namespace Plugins
{
    public class ArbeitszeitPlugin : IPlugin
    {
        private readonly FileLogger _logger = FileLogger.Instance;

        public string Name => "ArbeitszeitPlugin";

        public void Execute(string[] args)
        {
            switch (args.Length)
            {
                case 0: _logger.Log(LogLevel.Warning, "ArbeitszeitPlugin Execute mit zuwenig Argumenten aufgerufen!"); break;
                case 1:
                    switch (args[0])
                    {
                        case "con":
                            _= new AZC();
                            break;
                        case "ser":
                            _logger.Log(LogLevel.Info, "ArbeitszeitPlugin hat diesen Befehl noch nicht implementiert!");
                            break;
                        default:
                            _logger.Log(LogLevel.Warning, $"UserPlugin kennt den Befehl nicht ! {args[0]}");
                            break;
                    }
                    break;
                default:
                    _logger.Log(LogLevel.Warning, "ArbeitszeitPlugin Execute mit zu vielen Argumenten aufgerufen!");
                    break;
            }
            _logger.Dispose();
        }
    }
}
