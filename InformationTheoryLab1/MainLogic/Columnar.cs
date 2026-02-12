using System;
using System.Collections.Generic;
using System.Text;

namespace InformationTheoryLab1.MainLogic
{
    internal static class Columnar
    {
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
        private static int CompareRussianLetters(char x, char y)
        {

            int codeX = GetRussianOrder(x);
            int codeY = GetRussianOrder(y);

            return codeX.CompareTo(codeY);
        }

        private static int GetRussianOrder(char c)
        {
            if (c >= 'А' && c <= 'Е')
                return c - 'А' + 1;
            if (c == 'Ё')
                return 7;
            if (c >= 'Ж' && c <= 'Я')
                return c - 'Ж' + 8;
            return 0; 
        }
        private static int[] GetColumnOrder(string key)
        {
            int n = key.Length;
            int[] order = new int[n];

            var pairs = new (char letter, int index)[n];
            for (int i = 0; i < n; i++)
            {
                pairs[i] = (key[i], i);
            }

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (CompareRussianLetters(pairs[j].letter, pairs[j + 1].letter) > 0)
                    {
                       
                        var temp = pairs[j];
                        pairs[j] = pairs[j + 1];
                        pairs[j + 1] = temp;
                    }
                    
                }
            }

            for (int i = 0; i < n; i++)
            {
                order[pairs[i].index] = i + 1;
            }

            return order;
        }

        public static string Encrypt(string text, string key)
        {
            string cleanKey = CleanKey(key);
            if (string.IsNullOrEmpty(cleanKey)) return text;

            List<char> letters = new List<char>();
            Dictionary<int, char> nonLetters = new Dictionary<int, char>();

            for (int i = 0; i < text.Length; i++)
            {
                if (IsRussianLetter(text[i]))
                    letters.Add(text[i]);
                else
                    nonLetters[i] = text[i];
            }

            if (letters.Count == 0) return text;

            int[] order = GetColumnOrder(cleanKey);
            int cols = cleanKey.Length;
            int rows = letters.Count / cols;
            if (letters.Count % cols != 0)  
                rows++;
            char[,] table = new char[rows, cols];
            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (index < letters.Count)
                        table[i, j] = letters[index++];
                    else
                        table[i, j] = '\0'; 
                }
            }

            StringBuilder encrypted = new StringBuilder();

            for (int num = 1; num <= cols; num++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (order[j] == num)
                    {
                        for (int i = 0; i < rows; i++)
                        {
                            if (table[i, j] != '\0')
                                encrypted.Append(table[i, j]);
                        }
                        break;
                    }
                }
            }

            StringBuilder result = new StringBuilder();
            int letterIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (nonLetters.ContainsKey(i))
                    result.Append(nonLetters[i]);
                else
                    result.Append(encrypted[letterIndex++]);
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
                if (IsRussianLetter(text[i]))
                    letters.Add(text[i]);
                else
                    nonLetters[i] = text[i];
            }

            if (letters.Count == 0) return text;

            int[] order = GetColumnOrder(cleanKey);
            int cols = cleanKey.Length;
            int rows = (letters.Count + cols - 1) / cols;

            int fullCols = letters.Count % cols; 
            if (fullCols == 0) fullCols = cols;  

            int[] colLengths = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                colLengths[j] = (j < fullCols) ? rows : rows - 1;
            }

            char?[,] table = new char?[rows, cols];

            int letterIndex = 0;

            for (int num = 1; num <= cols; num++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (order[j] == num)
                    {
                        for (int i = 0; i < colLengths[j]; i++)
                        {
                            table[i, j] = letters[letterIndex++];
                        }
                        break;
                    }
                }
            }

            StringBuilder decrypted = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (table[i, j].HasValue)
                        decrypted.Append(table[i, j]);
                }
            }

            StringBuilder result = new StringBuilder();
            int decryptedIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (nonLetters.ContainsKey(i))
                    result.Append(nonLetters[i]);
                else
                    result.Append(decrypted[decryptedIndex++]);
            }

            return result.ToString();
        }
    }
}