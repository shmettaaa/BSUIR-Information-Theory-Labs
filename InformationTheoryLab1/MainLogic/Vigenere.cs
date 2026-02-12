using System;
using System.Collections.Generic;
using System.Text;

namespace InformationTheoryLab1.MainLogic
{
    internal static class Vigenere
    {
        private static readonly string RussianAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        public static string GenerateKey(string text, string shortKey)
        {
            string cleanShortKey = CleanKey(shortKey);
            if (string.IsNullOrEmpty(cleanShortKey)) return "";

            int lettersCount = 0;
            foreach (char c in text)
                if (IsRussianLetter(c))
                    lettersCount++;

            return GenerateKey(text, cleanShortKey, lettersCount);
        }

        private static bool IsRussianLetter(char c)
        {
            return (c >= 'А' && c <= 'Я') || c == 'Ё';
        }

        private static string CleanKey(string key)
        {
            StringBuilder cleaned = new StringBuilder();
            foreach (char c in key)
            {
                if (IsRussianLetter(c))
                    cleaned.Append(c);
            }
            return cleaned.ToString();
        }

        private static int GetLetterIndex(char c)
        {
            if (c >= 'А' && c <= 'Е')
                return c - 'А';
            if (c == 'Ё')
                return 6; 
            if (c >= 'Ж' && c <= 'Я')
                return c - 'Ж' + 7; 
            return 0;
        }

        public static string GenerateKey(string text, string shortKey, int lettersCount)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(shortKey))
                return "";

            StringBuilder generatedKey = new StringBuilder();
            int textIndex = 0;
            int keyIndex = 0;

            while (generatedKey.Length < lettersCount)
            {
                if (keyIndex < shortKey.Length)
                {
                    generatedKey.Append(shortKey[keyIndex]);
                    keyIndex++;
                }
                else
                {
                    while (textIndex < text.Length && !IsRussianLetter(text[textIndex]))
                        textIndex++;

                    if (textIndex < text.Length)
                    {
                        generatedKey.Append(text[textIndex]);
                        textIndex++;
                    }
                }
            }

            return generatedKey.ToString();
        }

        public static string Encrypt(string text, string key)
        {
            string cleanKey = CleanKey(key);
            if (string.IsNullOrEmpty(cleanKey)) return text;

            List<char> letters = new List<char>();
            Dictionary<int, char> nonLetters = new Dictionary<int, char>();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (IsRussianLetter(c))
                    letters.Add(c);
                else
                    nonLetters[i] = c;
            }

            if (letters.Count == 0) return text;

            string generatedKey = GenerateKey(text, cleanKey, letters.Count);

            StringBuilder encrypted = new StringBuilder();

            for (int i = 0; i < letters.Count; i++)
            {
                char textChar = letters[i];
                char keyChar = generatedKey[i];

                int textIndex = GetLetterIndex(textChar);
                int keyIndex = GetLetterIndex(keyChar);

                int encryptedIndex = (textIndex + keyIndex) % RussianAlphabet.Length;
                encrypted.Append(RussianAlphabet[encryptedIndex]);
            }

            StringBuilder result = new StringBuilder();
            int encryptedIndexPtr = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (nonLetters.ContainsKey(i))
                    result.Append(nonLetters[i]);
                else
                    result.Append(encrypted[encryptedIndexPtr++]);
            }

            return result.ToString();
        }

        public static string Decrypt(string text, string key)
        {
            string cleanKey = CleanKey(key);
            if (string.IsNullOrEmpty(cleanKey)) return text;

            List<char> letters = new List<char>();
            Dictionary<int, char> nonLetters = new Dictionary<int, char>();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (IsRussianLetter(c))
                    letters.Add(c);
                else
                    nonLetters[i] = c;
            }

            if (letters.Count == 0) return text;

            StringBuilder decrypted = new StringBuilder();
            StringBuilder generatedKey = new StringBuilder(cleanKey);

            for (int i = 0; i < letters.Count; i++)
            {
                char encryptedChar = letters[i];
                char keyChar = generatedKey[i];

                int encryptedIndex = GetLetterIndex(encryptedChar);
                int keyIndex = GetLetterIndex(keyChar);

                int decryptedIndex = (encryptedIndex - keyIndex + RussianAlphabet.Length) % RussianAlphabet.Length;
                char decryptedChar = RussianAlphabet[decryptedIndex];
                decrypted.Append(decryptedChar);

                if (generatedKey.Length < letters.Count)
                    generatedKey.Append(decryptedChar);
            }

            StringBuilder result = new StringBuilder();
            int decryptedIndexPtr = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (nonLetters.ContainsKey(i))
                    result.Append(nonLetters[i]);
                else
                    result.Append(decrypted[decryptedIndexPtr++]);
            }

            return result.ToString();
        }
    }
}