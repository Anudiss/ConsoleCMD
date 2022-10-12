using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void TB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                e.Handled = true;
                return;
            }
            string[] commands = TB.Text.Split(';');
            // command example: color red; 
            Regex commandRegex = new Regex(@"^(?<command>\w+)\s*(?<args>.+)?");
            foreach (string command in commands)
            {
                Match match = commandRegex.Match(command);
                if (!match.Success)
                {
                    TB.Text += "Ошибка в команде.";
                    break;
                }
                string commandName = match.Groups["command"].Value;
                string[] args = null;
                if (match.Groups["args"].Success)
                    args = match.Groups["args"].Value.Split(' ');
                
            }
        }
    }
}
