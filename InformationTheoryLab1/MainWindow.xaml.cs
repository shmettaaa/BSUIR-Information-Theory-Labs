using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
namespace InformationTheoryLab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ColumnarMethodButton_Click(object sender, RoutedEventArgs e)
        {
            var columnarMethodWindow = new ColumnarMethodWindow();  
            columnarMethodWindow.Owner = this;               
            this.IsEnabled = false;                   
            columnarMethodWindow.ShowDialog();               
            this.IsEnabled = true;                    
        }

        private void VigenereButton_Click(object sender, RoutedEventArgs e)
        {
            var VigenereMethodWindow = new VigenereMethodWindow();
            VigenereMethodWindow.Owner = this;
            this.IsEnabled = false;
            VigenereMethodWindow.ShowDialog();
            this.IsEnabled = true;
        }
    }
}