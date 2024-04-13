using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPInterfaces;

namespace WPRepo
{
    public static class RepoHelper
    {
        private static readonly Random _random = new();

        public static string GenerateUserID()
        {
            return $"{_random.Next(0, 10)}{_random.Next(0, 10)}{_random.Next(0, 10)}{_random.Next(0, 10)}-{_random.Next(0, 10)}";
        }

        public static bool IsUserIDValid(string userID)
        {
            // Überprüfen, ob der String das erwartete Format hat (####-#)
            if (userID.Length != 6 || userID[4] != '-')
            {
                return false;
            }

            // Überprüfen, ob die ersten vier Zeichen Zahlen sind
            for (int i = 0; i < 4; i++)
            {
                if (!char.IsDigit(userID[i]))
                {
                    return false;
                }
            }

            // Überprüfen, ob das letzte Zeichen eine Zahl ist
            if (!char.IsDigit(userID[5]))
            {
                return false;
            }

            return true;
        }

        public static List<T> UpdateIDs<T>(List<T> value) where T : IData
        {
            // Sortiere die Liste nach IDs
            value.Sort((x, y) => Convert.ToInt32(x.ID).CompareTo(Convert.ToInt32(y.ID)));

            // Überprüfe auf doppelte IDs
            bool hasDuplicates = value.Select(item => item.ID).Distinct().Count() != value.Count;

            // Wenn doppelte IDs gefunden wurden, neu zuweisen
            if (hasDuplicates)
            {
                int newID = 1;
                foreach (var item in value)
                {
                    item.ID = newID.ToString();
                    newID++;
                }
            }

            return value;
        }

        public static int GetLastID<T>(List<T> value) where T : IData
        {
            List<T> updatedList = UpdateIDs(value); // Aktualisiere die IDs, um sicherzustellen, dass keine Duplikate vorhanden sind

            // Finde die höchste vorhandene ID und gib sie um eins erhöht zurück
            int lastID = 0;
            foreach (var item in updatedList)
            {
                int itemID = Convert.ToInt32(item.ID);
                if (itemID > lastID)
                {
                    lastID = itemID;
                }
            }
            return lastID + 1;
        }

        public static List<T> GetUpdatedList<T>(List<T> value) where T : IData
        {
            // Aktualisiere die IDs, um sicherzustellen, dass keine Duplikate vorhanden sind
            List<T> updatedList = UpdateIDs(value);

            return updatedList;
        }
    }
}
