using ConsoleCMD.Applications;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;

        }

        private void InitializeDropDownMenu()
        {
            
        }


        

        private void TBConsole_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
