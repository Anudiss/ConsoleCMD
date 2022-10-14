using ConsoleCMD.Applications;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DropdownMenu _dropdownMenu;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateDropdownMenu()
        {
            _dropdownMenu = new DropdownMenu()
            {
                Items = new[] { "Руддщ", "aksdAWDJajsd", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2", "2" },
                MaxHeight = TB.ActualHeight,
                PlacementTarget = TB
            };

            _dropdownMenu.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                MessageBox.Show((string)args.AddedItems[0]);
            };
            
        }

        private void TB_KeyDown(object sender, KeyEventArgs e)
        {
            if (_dropdownMenu == null)
                CreateDropdownMenu();
            switch (e.Key)
            {
                case Key.Enter:
                    CommandsParse();
                    break;
                case Key.Tab:
                    Rect rect = TB.GetRectFromCharacterIndex(TB.CaretIndex);
                    _dropdownMenu.CustomMargin = new Thickness
                    {
                        Left = rect.X,
                        Bottom = 5
                    };
                    break;
                case Key.Escape:
                    _dropdownMenu.IsOpen = !_dropdownMenu.IsOpen;
                    break;
            }
        }

        private void CommandHint()
        {
            
        }

        private void CommandsParse()
        {
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

                Command cmd = Command.GetCommand(commandName);
                try
                {
                    cmd.Execute(args);
                }
                catch (ArgumentException)
                {
                    TB.Text += $"\n{cmd.Usage}";
                }
            }
        }
    }
}
