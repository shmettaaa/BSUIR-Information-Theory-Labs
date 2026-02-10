using InformationTheoryLab1.MainLogic;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace InformationTheoryLab1
{
    /// <summary>
    /// Interaction logic for ColumnarMethodWindow.xaml
    /// </summary>
    public partial class ColumnarMethodWindow : MetroWindow
    {
        public ColumnarMethodWindow()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MainTextBox.IsEnabled = true;
            KeyTextBox.IsEnabled = true;
            ResultButton.IsEnabled = true;

            MainTextBox.Clear();
            KeyTextBox.Clear();
            ResultTextBlock.Text = "";

            if (EncryptRadioButton.IsChecked == true)
            {
                TextLabel.Content = "Введите исходный текст:";
                KeyLabel.Content = "Введите ключевое слово:";
                ResultButton.Content = "Шифровать";
            }
            else if (DecryptRadioButton.IsChecked == true)
            {
                TextLabel.Content = "Введите шифротекст:";
                KeyLabel.Content = "Введите ключевое слово:";
                ResultButton.Content = "Дешифровать";
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (EncryptRadioButton.IsChecked != true && DecryptRadioButton.IsChecked != true)
            {
                MessageBox.Show("Сначала выберите операцию (Шифровать или Дешифровать)");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);

                    if (lines.Length >= 1)
                        MainTextBox.Text = lines[0];

                    if (lines.Length >= 2)
                        KeyTextBox.Text = lines[1];
                }
                catch
                {
                    MessageBox.Show("Ошибка при чтении файла");
                }
            }
        }

        private void ResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (EncryptRadioButton.IsChecked == true)
            {
                string result = Columnar.Encrypt(MainTextBox.Text, KeyTextBox.Text);
                ResultTextBlock.Text = result;
            }
            else
            {
                string result = Columnar.Decrypt(MainTextBox.Text, KeyTextBox.Text);
                ResultTextBlock.Text = result;
            }
        }
    }
}