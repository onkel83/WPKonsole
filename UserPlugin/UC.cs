using Helfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLoggingLibrary;
using WPRepo;

namespace Plugins
{
    public class UC
    {
        private readonly JsonRepository<User> _repository = JsonRepository<User>.Instance;
        private readonly FileLogger _logger = FileLogger.Instance;
        private bool _isRunning = true;
        private List<User> _data = [];

        public UC() {
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
                    case "q":
                        _isRunning = false;Konsole.GetUserInput("Danke für die Verwendung der Benutzer Verwaltung bei [W]inemp[P]roductions.\r\nBitte [Enter] Drücken."); break;
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
            Konsole.Line(50, "#");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[UC]", "Benutzer Verwaltung");
            Konsole.Menu(50, "#", "", "");
            Konsole.Line(50, "#");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "a", "Benutzer hinzufügen");
            Konsole.Menu(50, "#", "d", "Benutzer entfernen");
            Konsole.Menu(50, "#", "e", "Benutzer bearbeiten");
            Konsole.Menu(50, "#", "s", "Benutzer anzeigen");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "q", "Beenden");
            Konsole.Menu(50, "#", "", "");
            Konsole.Line(50, "#");
        }
        private static void ShowShowMenue()
        {
            Console.Clear();
            Konsole.Line(50, "#");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[UC]", "Benutzer Anzeigen");
            Konsole.Menu(50, "#", "", "");
            Konsole.Line(50, "#");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "a", "Alle Benutzer");
            Konsole.Menu(50, "#", "i", "Benutzer nach ID");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "q", "Beenden");
            Konsole.Menu(50, "#", "", "");
            Konsole.Line(50, "#");
        }
        private void ShowMenue()
        {
            bool isRunning = true;
            while (isRunning)
            {
                ShowShowMenue();
                switch (Konsole.GetUserInput("Bitte wählen sie")?.ToLower())
                {
                    case "a":
                        _data = _repository.GetData();
                        foreach(User u in _data)
                        {
                            Console.WriteLine(u.ToString());
                        }
                        var tmp = Konsole.GetUserInput("[q] = zurück zum Hauptmenue\r\n[Enter] = zurück ins Anzeige Menue\r\nBitte [q] oder [Enter] eingeben :");
                        if(string.IsNullOrEmpty(tmp))
                            break;
                        else if(tmp.Equals("q"))
                            isRunning = false;
                        break;
                    case "i":
                        _data = _repository.GetData();
                        var ui = Konsole.GetUserNotNullInput("Bitte Geben sie die ID an : ");
                        foreach (User u in _data)
                        {
                            if (u.ID.Equals(ui))
                            {
                                Console.WriteLine(u.ToString());
                            }
                        }
                        ui = Konsole.GetUserInput("[q] = zurück zum Hauptmenue\r\n[Enter] = zurück ins Anzeige Menue\r\nBitte [q] oder [Enter] eingeben :");
                        if (string.IsNullOrEmpty(ui))
                            break;
                        else if(ui.Equals("q"))
                            isRunning = false;
                        break;
                    case "q":
                        isRunning = false; Konsole.GetUserInput("Sie werden zurück in das Hauptmenue geleitet.\r\nBitte [Enter] Drücken."); break;
                    default:
                        _logger.Log(LogLevel.Warning, $"Befehl nicht bekannt");
                        break;
                }
            }
        }
        private void Add()
        {
            Console.Clear();
            Konsole.Line(50, "#");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[UC]", "Benutzer anlegen");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[q]", "Eingabe kann 'Jeder'zeit mit [q] beendet werden !");
            Konsole.Line(50, "#");
            string[] prompts = [
                "Bitte geben sie den Namen ein : ",
                "Bitte geben sie den Rolle ein : (0:Gast|1:Benutzer|2:Moderrator|3:Administrator)"
                ];
            string[]? result = Konsole.GetUserNotNullInput(prompts)??null;
            if(result != null)
            {
                foreach(string prompt in prompts)
                {
                    if (prompt.Equals("q"))
                    {
                        return;
                    }
                }
            }
            if(result != null && result.Length == prompts.Length)
            {
                User u = new() { ID = RepoHelper.GetLastID(_repository.GetData()).ToString(), Name = result[0], Rolle = result[1], UserID = RepoHelper.GenerateUserID(), Password = StringEncryptor.GenerateKey() };
                _repository.AddData(u) ;
                _logger.Log(LogLevel.Debug, $"Benutzer mit Folgenden Daten hinzugefügt : ID : {u.ID}, Name : {u.Name}, Rolle : {u.Rolle}, UserID : {u.UserID}, Password : {u.Password}");
                Console.WriteLine($"Daten : \r\nID : {u.ID}\r\nName : {u.Name}\r\nRolle : {u.Rolle}\r\nUserID : {u.UserID}\r\nPassword : {u.Password}\r\nBenutzer Erfolgreich hinzugefügt ! [Enter] zum fort fahren");
                Console.ReadLine();
            }
            else
            {
                _logger.Log(LogLevel.Warning, "Fehler beim erstellen eines Users! result[] stimmt nicht mit Prompts[] überein !");
            }
        }
        private void Delete()
        {
            Console.Clear();
            Konsole.Line(50, "#");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[UC]", "Benutzer Löschen");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[q]", "Eingabe kann 'Jeder'zeit mit [q] beendet werden !");
            Konsole.Line(50, "#");
            string id = Konsole.GetUserNotNullInput("Bitte geben sie die ID ein : ");
            if (Konsole.IsExitKey("q", id))
            {
                return;
            }
            User userToRemove = _repository.GetData().Find(u => u.ID == id)??new();
            if (userToRemove != null && (!string.IsNullOrEmpty(userToRemove.ID)))
            {
                _repository.GetData().Remove(userToRemove);
                Console.WriteLine("User removed.");
            }
            else
            {
                Console.WriteLine("User not found for removal.");
            }
        }
        private void Edit()
        {
            Console.Clear();
            Konsole.Line(50, "#");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[UC]", "Benutzer Bearbeiten");
            Konsole.Menu(50, "#", "", "");
            Konsole.Menu(50, "#", "[q]", "Eingabe kann 'Jeder'zeit mit [q] beendet werden !");
            Konsole.Line(50, "#");
            string id = Konsole.GetUserNotNullInput("Bitte geben sie die ID ein : ");
            if (Konsole.IsExitKey("q", id))
                return;
            User? userToUpdate = _repository.GetData().Find(u => u.ID == id);
            if (userToUpdate != null)
            {
                userToUpdate.Name = Konsole.GetUserInput("Bitte geben sie den Namen ein : ")??userToUpdate.Name;
                if (Konsole.IsExitKey("q", userToUpdate.Name))
                    return;
                userToUpdate.Rolle = Konsole.GetUserInput("Bitte geben sie den Rolle ein : (0:Gast|1:Benutzer|2:Moderrator|3:Administrator")??userToUpdate.Rolle;
                if (Konsole.IsExitKey("q", userToUpdate.Rolle))
                    return;
                userToUpdate.UserID = Konsole.GetUserInput("Bitte geben sie die UserID ein : (####-#)")??userToUpdate.UserID;
                if(Konsole.IsExitKey("q", userToUpdate.UserID))
                    return;
                userToUpdate.Password = Konsole.GetUserNotNullInput("Neues Passwort erstellen ?")?? userToUpdate.Password;
                if (Konsole.IsExitKey("q", userToUpdate.Password))
                    return;
                Console.WriteLine("User updated.");
            }
            else
            {
                Console.WriteLine("User not found for editing.");
            }
        }
    }
}
