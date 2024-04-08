﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UserPlugin
{
    public static class Konsole
    {
        public static void Line(int length, string sign)
        {
            for (int i = 0; i < length; i++)
            {
                Console.Write(sign);
            }
            Console.WriteLine();
        }

        public static void Menu(int length, string sign, string key, string name)
        {
            // Berechne die erforderliche Länge für die Zeile
            int requiredLength = 4 + key.Length + name.Length; // 4 = spaces and separators

            // Überprüfe, ob die angegebene Länge ausreichend ist
            if (length < requiredLength)
            {
                // Erhöhe die Länge, um genügend Platz für key und name zu schaffen
                length = requiredLength;
            }

            // Berechne die verbleibende Länge für Leerzeichen
            int remainingLength = length - requiredLength;

            // Gebe die Zeile aus
            Console.Write(sign);
            Console.Write("   ");
            Console.Write(key);
            Console.Write("   ");
            Console.Write(name);
            for (int i = 0; i < remainingLength; i++)
            {
                Console.Write(" ");
            }
            Console.Write(" ");
            Console.WriteLine(sign);
        }

        public static string? GetUserInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public static string GetUserNotNullInput(string prompt)
        {
            string? userInput;
            do
            {
                userInput = GetUserInput(prompt);
            } while (string.IsNullOrEmpty(userInput));
            return userInput;
        }

        public static bool IsExitKey(string key, string value)
        {
            if (key.Equals(value))
            {
                return true;
            }
            return false;
        }
    }
}
