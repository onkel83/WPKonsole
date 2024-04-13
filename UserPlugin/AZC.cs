using UserPlugin;
using WPLoggingLibrary;
using WPRepo;

namespace Plugins
{
    public class AZC
    {
        private readonly JsonRepository<Arbeitszeit> _repository = JsonRepository<Arbeitszeit>.Instance;
        private readonly FileLogger _logger = FileLogger.Instance;
        private bool _isRunning = true;
        private List<Arbeitszeit> _data = [];
        private string _userID = RepoHelper.GenerateUserID();

        public string UserID {
            get => RepoHelper.IsUserIDValid(_userID)?_userID:RepoHelper.GenerateUserID();
            set => _userID = RepoHelper.IsUserIDValid(value) ? value : RepoHelper.GenerateUserID();
        }

        public AZC()
        {
            Run();
        }

        private void Run()
        {

            while (_isRunning)
            {
                ShowMainMenue();
                switch (Konsole.GetUserInput("Bitte wählen sie")?.ToLower())
                {
                    case "a":
                        Add();
                        break;
                    case "d":
                        Delete();
                        break;
                    case "e":
                        Edit();
                        break;
                    case "s":
                        ShowMenue();
                        break;
                    case "v":
                        ShowSettingsMenue();
                        break;
                    case "q":
                        _isRunning = false; Konsole.GetUserInput("Danke für die Verwendung der Arbeitszeit Verwaltung bei [W]inemp[P]roductions.\r\nBitte [Enter] Drücken."); break;
                    default:
                        _logger.Log(LogLevel.Warning, $"Befehl nicht bekannt");
                        break;
                }
            }
            _repository.Dispose();
            _logger.Dispose();
        }
        private static void ShowMainMenue()
        {
            Console.Clear();
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[AZC]", "Arbeitszeit Verwaltung");
            Konsole.Menu(128, "#", "", "");
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "a", "Arbeitszeit hinzufügen");
            Konsole.Menu(128, "#", "d", "Arbeitszeit entfernen");
            Konsole.Menu(128, "#", "e", "Arbeitszeit bearbeiten");
            Konsole.Menu(128, "#", "s", "Arbeitszeit anzeigen");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "v", "Verwalten");
            Konsole.Menu(128, "#", "q", "Beenden");
            Konsole.Menu(128, "#", "", "");
            Konsole.Line(128, "#");
        }
        private static void ShowShowSettingsMenue()
        {
            Console.Clear();
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[AZC]", "Arbeitszeit Einstellungen");
            Konsole.Menu(128, "#", "", "(Funktionen zur Zeit nicht Implementiert)");
            Konsole.Menu(128, "#", "", "");
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "i", "Daten Importieren");
            Konsole.Menu(128, "#", "e", "Daten Exportieren");
            Konsole.Menu(128, "#", "r", "Daten Reparieren");
            Konsole.Menu(128, "#", "u", "UserID wechseln");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "q", "Beenden");
            Konsole.Menu(128, "#", "", "");
            Konsole.Line(128, "#");
        }
        private static void ShowShowMenue()
        {
            Console.Clear();
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[AZC]", "Arbeitszeit Anzeigen");
            Konsole.Menu(128, "#", "", "");
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "a", "Alle Arbeitszeiten");
            Konsole.Menu(128, "#", "i", "Arbeitszeiten nach User");
            Konsole.Menu(128, "#", "m", "Arbeitszeiten nach Monat (aktuelles Jahr)");
            Konsole.Menu(128, "#", "j", "Arbeitszeiten nach Jahr (alle Monate)");
            Konsole.Menu(128, "#", "b", "Arbeitszeiten zwischen 2 Zeiträumen");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "q", "Beenden");
            Konsole.Menu(128, "#", "", "");
            Konsole.Line(128, "#");
        }
        private static void ShowSettingsMenue()
        {
            bool isRunning = true;
            ShowShowSettingsMenue();
            while (isRunning)
            {
                switch(Konsole.GetUserNotNullInput("Bitte Geben sie ihre auswahl ein : "))
                {
                    case "i":
                        //daten Import (XmlFile/JsonFile)
                        break;
                    case "e":
                        //daten Exportieren (XmlFile/JsonFile)
                        break;
                    case "r":
                        //ID's neusortieren
                        break;
                    case "u":
                        //UserID ändern
                        break;
                    case "q":
                        isRunning = false; break;
                    default:
                        break;
                }
            }

        }
        private void ShowMenue()
        {
            var result = new List<Arbeitszeit>();
            string ui = string.Empty;
            bool isRunning = true;
            while (isRunning)
            {
                ShowShowMenue();
                switch (Konsole.GetUserInput("Bitte wählen sie")?.ToLower())
                {
                    case "a":
                        _data = _repository.GetData();
                        foreach (Arbeitszeit u in _data)
                        {
                            Console.WriteLine(u.ToString());
                        }
                        isRunning = AskFooter("[q] = zurück zum Hauptmenue\r\n[Enter] = zurück ins Anzeige Menue\r\nBitte [q] oder [Enter] eingeben :");break;
                    case "i":
                        _data = _repository.GetData();
                        ui = Konsole.GetUserNotNullInput("Bitte Geben sie die UserID an : ");
                        if (!string.IsNullOrEmpty(ui) && ui != "q")
                        {
                            result = _data.FindAll(x => x.UserID == ui);
                            foreach(var a in result)
                            {
                                Console.WriteLine(a);
                            }
                        }
                        isRunning = AskFooter("[q] = zurück zum Hauptmenue\r\n[Enter] = zurück ins Anzeige Menue\r\nBitte [q] oder [Enter] eingeben :");break;
                    case "m":
                        _data = _repository.GetData();
                        ui = Konsole.GetUserNotNullInput("Bitte geben sie den Monat ein : (als Zahl : Zweistellig)");
                        if(!string.IsNullOrEmpty(ui) && ui != "q")
                        {
                            result = _data.FindAll(x => Convert.ToDateTime(x.Start).Month == Convert.ToInt32(ui));
                            foreach(var a in result)
                            {
                                Console.WriteLine(a);
                            }
                        }
                        isRunning = AskFooter("[q] = zurück zum Hauptmenue\r\n[Enter] = zurück ins Anzeige Menue\r\nBitte [q] oder [Enter] eingeben :");break;
                    case "j":
                        _data = _repository.GetData();
                        ui = Konsole.GetUserNotNullInput("Bitte geben sie das Jahr ein : (als Zahl : Vierstellig)");
                        if (!string.IsNullOrEmpty(ui) && ui != "q")
                        {
                            result = _data.FindAll(x => Convert.ToDateTime(x.Start).Year == Convert.ToInt32(ui));
                            foreach (var a in result)
                            {
                                Console.WriteLine(a);
                            }
                        }
                        isRunning = AskFooter("[q] = zurück zum Hauptmenue\r\n[Enter] = zurück ins Anzeige Menue\r\nBitte [q] oder [Enter] eingeben :"); break;
                    case "b":
                        _data = _repository.GetData();
                        string[] prompts = ["Bitte geben sie das Startdatum an : (dd.MM.yyyy)", "Bitte geben sie das Enddatum an : (dd.MM.yyyy"];
                        string[] results = Konsole.GetUserNotNullInput(prompts)??[];
                        if(results != null && results.Length >= 2)
                        {
                            result = _data.FindAll(x => Convert.ToDateTime(x.Start) >= Convert.ToDateTime(results[0]));
                            result = result.FindAll(x => Convert.ToDateTime(x.Ende) <= Convert.ToDateTime(results[1]));
                            foreach(var a in result) {
                                Console.WriteLine(a);
                            }
                        }
                        isRunning = AskFooter("[q] = zurück zum Hauptmenue\r\n[Enter] = zurück ins Anzeige Menue\r\nBitte [q] oder [Enter] eingeben :");break;
                    case "q":
                        isRunning = false; Konsole.GetUserInput("Sie werden zurück in das Hauptmenue geleitet.\r\nBitte [Enter] Drücken."); break;
                    default:
                        _logger.Log(LogLevel.Warning, $"Befehl nicht bekannt");
                        break;
                }
            }
        }
        private static bool AskFooter(string prompt)
        {
            var value = Konsole.GetUserNotNullInput(prompt);
            if (string.IsNullOrEmpty(value))
                return true;
            else if (value.Equals("q"))
                return false;
            return true;
        }
        private void Add()
        {
            Console.Clear();
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[AZC]", "Arbeitszeit anlegen");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[q]", "Eingabe kann 'Jeder'zeit mit [q] beendet werden !");
            Konsole.Line(128, "#");
            string[] prompts = [
                "Bitte geben sie die Startzeit und das Startdatum ein : (HH:mm dd.MM.yyyy)",
                "Bitte geben sie die Endzeit   und das Enddatum ein   : (HH:mm dd.MM.yyyy)",
                "Bitte geben sie die Pausenzeit ein : (#,# wobei 1,0 = 1 Stunde)"
                ];
            string[]? result = Konsole.GetUserNotNullInput(prompts)??null;
            if (result != null)
            {
                foreach (string prompt in prompts)
                {
                    if (prompt.Equals("q"))
                    {
                        return;
                    }
                }
            }
            if (result != null && result.Length == prompts.Length)
            {
                Arbeitszeit value = new() { ID = RepoHelper.GetLastID(_repository.GetData()).ToString(), UserID = UserID, Start = result[0], Ende = result[1], Pause = result[2] };
                _repository.AddData(value);
                _logger.Log(LogLevel.Info, $"Arbeitszeit mit folgenden Daten hinzugefügt : {value}");
                Console.WriteLine($"Daten : \r\nID : {value.ID}\r\nUserID : {value.UserID}\r\nStart : {value.Start}\r\nEnde : {value.Ende}\r\nPause : {value.Pause}\r\nArbeitszeit : {value.WorkTime}\r\n Arbeitszeit mit diesen Daten hinzugefügt ! [Enter] zum fort fahren");
                Console.ReadLine();
                return;
            }
        }
        private void Delete()
        {
            Console.Clear();
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[AZC]", "Arbeitszeit Löschen");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[q]", "Eingabe kann 'Jeder'zeit mit [q] beendet werden !");
            Konsole.Line(128, "#");
            string id = Konsole.GetUserNotNullInput("Bitte geben sie die ID ein : ");
            if (Konsole.IsExitKey("q", id))
            {
                return;
            }
            Arbeitszeit userToRemove = _repository.GetData().Find(u => u.ID == id)??new();
            if (userToRemove != null && (!string.IsNullOrEmpty(userToRemove.ID)))
            {
                _repository.GetData().Remove(userToRemove);
                Console.WriteLine("Worktime removed. Press [Enter] for Continue");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Worktime not found for removal. Press [Enter] for Continue");
                Console.ReadLine();
            }
        }
        private void Edit()
        {
            Console.Clear();
            Konsole.Line(128, "#");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[AZC]", "Arbeitszeit Bearbeiten");
            Konsole.Menu(128, "#", "", "(Achtung momentan können keine UserID's angepasst werden)");
            Konsole.Menu(128, "#", "", "");
            Konsole.Menu(128, "#", "[q]", "Eingabe kann 'Jeder'zeit mit [q] beendet werden !");
            Konsole.Line(128, "#");
            string[] prompts = [
                    "Bitte geben sie die ID ein : "
                ];
            string[] results = Konsole.GetUserNotNullInput(prompts)??[];
            int i = 0;
            if(results.Length > 0)
            {
                Arbeitszeit? userToUpdate = _repository.GetData().Find(u => u.ID == results[0]);
                if(userToUpdate == null)
                {
                    Console.WriteLine("Arbeitszeit nicht gefunden! weiter mit [Enter]");
                    _logger.Log(LogLevel.Warning, $"Arbeitszeit mit der ID : {results[0]} nicht gefunden!");
                    Console.ReadLine();
                    return;
                }
                Arbeitszeit old = userToUpdate;
                prompts = [
                        "Bitte geben sie die Startzeit ein [hh:mm dd.MM.yyyy] und [ ] für den alten Wert",
                        "Bitte geben sie die Endzeit ein [hh:mm dd.MM.yyyy] und [ ] für den alten Wert",
                        "Bitte geben sie die Pausenzeit [0.0f] ein und [ ] für den alten Wert"
                    ];
                results = Konsole.GetUserInput(prompts)??[];
                if(results != null && results.Length == 3 && userToUpdate != null)
                {
                    foreach(string s in results)
                    {
                        if (s.Equals("q"))
                        {
                            return;
                        }else if (string.IsNullOrEmpty(s))
                        {
                            switch (i)
                            {
                                case 0:
                                    results[0] = userToUpdate.Start;break;
                                case 1:
                                    results[1] = userToUpdate.Ende;break;
                                case 2:
                                    results[2] = userToUpdate.Pause;break;
                                default: break;
                            }
                        }
                        i++;
                    }
                    _repository.GetData().Remove(old);
                    _repository.AddData(userToUpdate);
                    _logger.Log(LogLevel.Info, $"Arbeitszeit: :|: {old} :|: mit folgenden Daten geändert: :|: {userToUpdate} :|:");
                    Console.WriteLine($"Daten : Original|Geändert\r\nID : {old.ID}|{userToUpdate.ID}\r\nUserID : {old.UserID}|{userToUpdate.UserID}\r\nStartzeit/datum : {old.Start}|{userToUpdate.Start}");
                    Console.WriteLine($"Endzeit/datum : {old.Ende}|{userToUpdate.Ende}\r\nPausenzeit : {old.Pause}|{userToUpdate.Pause}\r\n Arbeitzeit : {old.WorkTime}|{userToUpdate.WorkTime}\r\n Drücken sie [Enter] zum fort fahren!");
                    Console.ReadLine();
                }
            }

        }
    }
}