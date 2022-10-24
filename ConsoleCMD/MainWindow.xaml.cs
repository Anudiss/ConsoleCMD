using ConsoleCMD.Applications;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using ConsoleCMD.Pages;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new DesktopPage());
        }
    }
}
