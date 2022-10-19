using ConsoleCMD.Applications;
using System.Linq;
using System.Windows;

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

            Console.Focus();
        }
    }
}
