using WPInterfaces;
using WPLoggingLibrary;
using WPRepo;

namespace Plugins
{
    public enum UserRolle
    {
        Gast = 0,
        Benutzer = 1,
        Disponent = 2,
        Administrator = 3
    }

    public class User : IData
    {
        private readonly FileLogger _logger = FileLogger.Instance;

        private string? iD;
        private string userID = string.Empty;
        private string? name;
        private string? password = string.Empty;
        private UserRolle rolle = 0;

        public string ID { get => iD ?? "0"; set => iD = value; }
        public string Name { get => name ?? "TestUser"; set => name = value; }
        public string Rolle
        {
            get => rolle.ToString();
            set
            {
                if (Enum.TryParse(value, out UserRolle parsedRolle))
                {
                    rolle = parsedRolle;
                }
                else
                {
                    _logger.Log(LogLevel.Warning, "Userrolle ist nicht bekannt! Gast gesetzt für die Sitzung");
                    rolle = UserRolle.Gast;
                }
            }
        }
        public string UserID { get => RepoHelper.IsUserIDValid(userID) ? userID : UserID = string.Empty; set => userID = RepoHelper.GenerateUserID(); }
        public string Password { get => password??string.Empty; set => password = value; }


        public override string ToString()
        {
            return $"{ID};{Name};{Rolle};{UserID}";
        }
    }
}