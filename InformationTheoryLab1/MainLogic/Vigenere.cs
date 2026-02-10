using System;
using System.Collections.Generic;
using System.Text;

namespace InformationTheoryLab1.MainLogic
{
    internal static class Vigenere
    {
        public static string GenerateKey(string text, string shortKey)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(shortKey))
                return "";

            StringBuilder generatedKey = new StringBuilder();
            int textIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (i < shortKey.Length)
                {
                    generatedKey.Append(shortKey[i]);
                }
                else
                {
                    generatedKey.Append(text[textIndex]);
                    textIndex++;
                }
            }

            return generatedKey.ToString();
        }

        public static string Encrypt(string text, string key)
        {
            //Реализация шифрования Виженера
            return "Зашифрованный текст";
        }

        public static string Decrypt(string text, string key)
        {
            //Реализация дешифрования Виженера
            return "Расшифрованный текст";
        }
    }

}
