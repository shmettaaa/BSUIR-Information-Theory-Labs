using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace InformationTheoryLab3
{
    public partial class MainWindow : Window
    {
        private RabinCipher cipher;
        private bool isEncryptMode = true;
        private string currentInputFilePath;
        private string currentOutputFilePath;
        private BigInteger lastN;

        public MainWindow()
        {
            InitializeComponent();
            cipher = new RabinCipher();
            UpdateControls();
        }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            isEncryptMode = EncryptRadio.IsChecked == true;
            ClearFields();
            UpdateControls();
        }

        private void Parameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateParameters();
            UpdateControls();
        }

        private void ValidateParameters()
        {
            bool valid = true;
            StringBuilder errors = new StringBuilder();

            if (!BigInteger.TryParse(PTextBox.Text, out BigInteger p))
            {
                errors.AppendLine("p должно быть целым числом.");
                valid = false;
            }
            else if (!IsPrime(p))
            {
                errors.AppendLine("p не является простым числом.");
                valid = false;
            }
            else if (p % 4 != 3)
            {
                errors.AppendLine("p ≡ 3 (mod 4) не выполняется.");
                valid = false;
            }

            if (!BigInteger.TryParse(QTextBox.Text, out BigInteger q))
            {
                errors.AppendLine("q должно быть целым числом.");
                valid = false;
            }
            else if (!IsPrime(q))
            {
                errors.AppendLine("q не является простым числом.");
                valid = false;
            }
            else if (q % 4 != 3)
            {
                errors.AppendLine("q ≡ 3 (mod 4) не выполняется.");
                valid = false;
            }

            if (BigInteger.TryParse(PTextBox.Text, out p) && BigInteger.TryParse(QTextBox.Text, out q))
            {
                if (p == q)
                {
                    errors.AppendLine("p и q должны быть различными.");
                    valid = false;
                }
                else
                {
                    lastN = p * q;

                    MaxBLabel.Text = $"Максимальное значение b: 1 ≤ b ≤ {lastN - 1}   (n = p·q = {lastN})";

                    if (!BigInteger.TryParse(BTextBox.Text, out BigInteger b))
                    {
                        errors.AppendLine("b должно быть целым числом.");
                        valid = false;
                    }
                    else if (b <= 0 || b >= lastN)
                    {
                        errors.AppendLine($"b должно быть в интервале (0, {lastN}).");
                        valid = false;
                    }

                    if (lastN <= 255)
                    {
                        errors.AppendLine($"n = p*q = {lastN} <= 255. Для однозначной расшифровки байтов рекомендуется n > 255.");
                    }
                }
            }
            else
            {
                MaxBLabel.Text = "Максимальное значение b будет показано после ввода p и q";
            }

            StatusLabel.Text = valid ? "Параметры корректны." : errors.ToString();
            StatusLabel.Foreground = valid ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
        }

        private bool IsPrime(BigInteger num)
        {
            if (num < 2) return false;
            if (num == 2) return true;
            if (num % 2 == 0) return false;
            for (BigInteger i = 3; i * i <= num; i += 2)
                if (num % i == 0) return false;
            return true;
        }

        private void UpdateControls()
        {
            bool paramsValid = StatusLabel.Text.Contains("корректны");
            bool modeSelected = EncryptRadio.IsChecked == true || DecryptRadio.IsChecked == true;

            ActionButton.IsEnabled = paramsValid && modeSelected;
            OpenButton.IsEnabled = modeSelected;
            SaveButton.IsEnabled = modeSelected && !string.IsNullOrWhiteSpace(NumbersTextBox.Text);
        }

        private void ClearFields()
        {
            NumbersTextBox.Text = "";
            LogTextBox.Text = "";
            MaxBLabel.Text = "Максимальное значение b будет показано после ввода p и q";
            currentInputFilePath = null;
            currentOutputFilePath = null;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (isEncryptMode)
            {
                dlg.Title = "Выберите файл для шифрования";
                dlg.Filter = "Все файлы (*.*)|*.*";
            }
            else
            {
                dlg.Title = "Выберите зашифрованный текстовый файл (с числами)";
                dlg.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            }

            if (dlg.ShowDialog() == true)
            {
                currentInputFilePath = dlg.FileName;
                LogTextBox.AppendText($"Открыт файл: {currentInputFilePath}\n");
            }
        }

        private void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NumbersTextBox.Text)) return;

            SaveFileDialog dlg = new SaveFileDialog();
            if (isEncryptMode)
                dlg.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            else
                dlg.Filter = "Все файлы (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                currentOutputFilePath = dlg.FileName;
                try
                {
                    if (isEncryptMode)
                    {
                        File.WriteAllText(currentOutputFilePath, NumbersTextBox.Text);
                        LogTextBox.AppendText($"Зашифрованные числа сохранены в: {currentOutputFilePath}\n");
                    }
                    else
                    {
                        byte[] data = ConvertHexOrNumbersToBytes(NumbersTextBox.Text);
                        File.WriteAllBytes(currentOutputFilePath, data);
                        LogTextBox.AppendText($"Расшифрованный файл сохранён: {currentOutputFilePath}\n");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private byte[] ConvertHexOrNumbersToBytes(string text)
        {
            string[] parts = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] result = new byte[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (!byte.TryParse(parts[i], out byte b))
                    throw new FormatException($"Неверный формат: {parts[i]}");
                result[i] = b;
            }
            return result;
        }

        private async void Execute_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentInputFilePath))
            {
                MessageBox.Show("Сначала выберите файл с помощью кнопки 'Открыть файл'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!BigInteger.TryParse(PTextBox.Text, out BigInteger p) ||
                !BigInteger.TryParse(QTextBox.Text, out BigInteger q) ||
                !BigInteger.TryParse(BTextBox.Text, out BigInteger b))
            {
                MessageBox.Show("Некорректные параметры. Проверьте ввод.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            cipher.SetParameters(p, q, b);

            try
            {
                if (isEncryptMode)
                {
                    byte[] fileData = File.ReadAllBytes(currentInputFilePath);
                    var encryptedNumbers = cipher.Encrypt(fileData);

                    string numbersText = string.Join(" ", encryptedNumbers);
                    NumbersTextBox.Text = numbersText;

                    LogTextBox.AppendText($"Шифрование завершено. Обработано байт: {fileData.Length}\n");
                }
                else
                {

                    string content = File.ReadAllText(currentInputFilePath);
                    string[] parts = content.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    BigInteger[] numbers = parts.Select(BigInteger.Parse).ToArray();

                    byte[] decryptedBytes = cipher.Decrypt(numbers);

                    string byteValues = string.Join(" ", decryptedBytes.Select(x => x.ToString()));
                    NumbersTextBox.Text = byteValues;

                    LogTextBox.AppendText($"Дешифрование завершено. Получено байт: {decryptedBytes.Length}\n");
                }

                UpdateControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LogTextBox.AppendText($"Ошибка: {ex.Message}\n");
            }
        }
    }
}