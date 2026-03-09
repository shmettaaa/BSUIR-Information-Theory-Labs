using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InformationTheoryLab2
{
    public partial class MainWindow : Window
    {
        private bool isEncryptMode = true;
        private StreamCipher cipher = new StreamCipher();

        public MainWindow()
        {
            InitializeComponent();
            UpdateControls();
        }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            ClearAllFields();

            if (EncryptRadio.IsChecked == true)
            {
                isEncryptMode = true;
                InputLabel.Content = "Исходный текст (биты):";
                OutputLabel.Content = "Шифротекст (биты):";
                ActionButton.Content = "Шифровать";
            }
            else if (DecryptRadio.IsChecked == true)
            {
                isEncryptMode = false;
                InputLabel.Content = "Шифротекст (биты):";
                OutputLabel.Content = "Исходный текст (биты):";
                ActionButton.Content = "Дешифровать";
            }

            UpdateControls();
        }

        private void RegisterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = RegisterTextBox.Text;
            string filtered = new string(text.Where(c => c == '0' || c == '1').ToArray());

            if (text != filtered)
            {
                RegisterTextBox.Text = filtered;
                RegisterTextBox.SelectionStart = filtered.Length;
            }

            RegisterLengthLabel.Text = RegisterTextBox.Text.Length.ToString();
            UpdateControls();
        }

        private void ClearAllFields()
        {
            RegisterTextBox.Text = "";
            InputTextBox.Text = "";
            KeyTextBox.Text = "";
            OutputTextBox.Text = "";
            RegisterLengthLabel.Text = "0";
        }

        private void UpdateControls()
        {
            string reg = RegisterTextBox.Text ?? "";
            int regLen = reg.Length;
            bool regValid = regLen == 38 && reg.All(c => c == '0' || c == '1');

            string inputRaw = InputTextBox.Text ?? "";
            bool inputHasOnlyValidChars = inputRaw.All(c =>
                c == '0' || c == '1' ||
                c == ' ' || c == '\r' || c == '\n' || c == '\t');  

            bool inputNotEmpty = !string.IsNullOrWhiteSpace(inputRaw);  

            bool inputValid = inputNotEmpty && inputHasOnlyValidChars;

            bool modeSelected = EncryptRadio.IsChecked == true || DecryptRadio.IsChecked == true;

            bool canProceed = modeSelected && regValid && inputValid;

            ActionButton.IsEnabled = canProceed;

            OpenButton.IsEnabled = modeSelected;
            SaveButton.IsEnabled = modeSelected && !string.IsNullOrWhiteSpace(OutputTextBox.Text);

            InputTextBox.IsEnabled = modeSelected;
            RegisterTextBox.IsEnabled = modeSelected;

            KeyTextBox.IsEnabled = true;
            OutputTextBox.IsEnabled = true;
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateControls();  
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                byte[] bytes = File.ReadAllBytes(dlg.FileName);
                string bits = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                InputTextBox.Text = bits;
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OutputTextBox.Text)) return;

            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == true)
            {
                string bits = OutputTextBox.Text.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                if (bits.Length % 8 != 0)
                {
                    MessageBox.Show("Длина бит не кратна 8!", "Ошибка");
                    return;
                }

                byte[] bytes = new byte[bits.Length / 8];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(bits.Substring(i * 8, 8), 2);
                }
                File.WriteAllBytes(dlg.FileName, bytes);
            }
        }

        private void EncryptDecrypt_Click(object sender, RoutedEventArgs e)
        {
            string registerState = RegisterTextBox.Text;
            cipher.SetInitialState(registerState);

            string inputBits = InputTextBox.Text.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");

            if (string.IsNullOrEmpty(inputBits))
            {
                MessageBox.Show("Введите или загрузите биты для обработки.");
                return;
            }

            var (result, key) = cipher.Process(inputBits);

            OutputTextBox.Text = result;
            KeyTextBox.Text = key;

            UpdateControls(); 
        }
    }
}