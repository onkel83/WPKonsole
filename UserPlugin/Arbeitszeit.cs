using System.Globalization;
using WPInterfaces;
using WPLoggingLibrary;

namespace Plugins
{
    public class Arbeitszeit : IData
    {
        private readonly FileLogger _logger = FileLogger.Instance;

        string _ID = string.Empty;
        string _UserID = string.Empty;
        DateTime _Start = DateTime.Now;
        DateTime _Ende = DateTime.Now;
        double _Pause = 0;

        public string ID { get => _ID; set => _ID = value; }
        public string UserID { get => _UserID; set => _UserID = value; }
        public string Start { get => DateTimeToString(_Start); set => _Start = StringToDateTime(value); }
        public string Ende { get => DateTimeToString(_Ende); set => _Ende = StringToDateTime(value); }
        public string Pause { get => DoubleToString(_Pause); set => _Pause = StringToDouble(value); }
        public string WorkTime { get => ((_Ende - _Start).TotalHours - _Pause).ToString(); }

        private double StringToDouble(string value)
        {
            double result = 0;
            try { result = (double.TryParse(value, out result)) ? result : 0;}catch (Exception ex) { _logger.Log(LogLevel.Error, $"Fehler beim Wandeln von : {value} in ein double!\r\n{ex.Message}"); }
            return result;
        }
        private DateTime StringToDateTime(string value)
        {
            DateTime result = DateTime.Now;
            try { result = DateTime.ParseExact(value, "HH:mm dd.MM.yyyy", CultureInfo.InvariantCulture); } catch(Exception ex){ _logger.Log(LogLevel.Warning, $"Fehler beim wandeln der DateTime : {value}\r\n{ex.Message}"); }
            return result;
        }
        private string DoubleToString(double value)
        {
            try { return value.ToString(); }catch(Exception ex) { _logger.Log(LogLevel.Warning, $"Fehler beim wandeln von Double : {value}\r\n{ex.Message}"); }
            return string.Empty; 
        }
        private string DateTimeToString(DateTime value)
        {
            try { return value.ToString("HH:mm dd.MM.yyyy"); }catch(Exception ex) { _logger.Log(LogLevel.Warning, $"Fehler beim wandeln von DateTime : {value}\r\n{ex.Message}"); }    
            return string.Empty; 
        }

        public override string ToString()
        {
            return $"{ID};{UserID};{Start};{Ende};{Pause};{WorkTime}";
        }
    }
}
