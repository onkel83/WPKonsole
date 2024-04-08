using WPInterfaces;
using WPLoggingLibrary;

namespace UserPlugin
{
    public class UserPlugin : IPlugin
    {
        private readonly FileLogger _logger = FileLogger.Instance;
        public string Name => "UserPlugin";
        public void Execute(string[] args)
        {
            switch (args.Length)
            {
                case 0: _logger.Log(LogLevel.Warning, "UserPlugin Execute mit zuwenig Argumenten aufgerufen!");break;
                case 1:
                    switch (args[0])
                    {
                        case "con":
                            _= new UC();
                            break;
                        case "ser":
                            _logger.Log(LogLevel.Info, "UserPlugin hat diesen Befehl noch nicht implementiert!");
                            break;
                        default:
                            _logger.Log(LogLevel.Warning, $"UserPlugin kennt den Befehl nicht ! {args[0]}");
                            break;
                    }
                    break;
                default:
                    _logger.Log(LogLevel.Warning, "UserPlugin Execute mit zu wenig oder zu vielen Argumenten aufgerufen!");
                    break;
            }
            _logger.Dispose();
        }
    }
}
