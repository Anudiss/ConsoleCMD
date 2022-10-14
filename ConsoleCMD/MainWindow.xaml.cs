using ConsoleCMD.Applications;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                Items = new[] { "Руддщ", "KASJdawkd", "2", "3", "42", "12", "1232", "4322", "532", "asdw2", "gasd2" },
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
            bool shift = Keyboard.Modifiers == ModifierKeys.Shift;

            switch (e.Key)
            {
                case Key.Enter:
                    if (_dropdownMenu.IsOpen)
                        SubstituteCommand();
                    else
                        CommandsParse();

                    _dropdownMenu.IsOpen = false;
                    break;
                case Key.Tab:
                    OpenDropdownMenu();
                    if (_dropdownMenu.IsOpen)
                        _dropdownMenu.SelectedItemIndex += shift ? -1 : 1;
                    e.Handled = true;
                    break;
                case Key.Escape:
                    _dropdownMenu.IsOpen = false;
                    break;
            }
        }

        private void MoveDropdownMenu()
        {
            if (_dropdownMenu.IsOpen == false)
                return;
            Rect rect = TB.GetRectFromCharacterIndex(TB.CaretIndex);
            if (rect.X + _dropdownMenu.Width >= Grid.ActualWidth)
                _dropdownMenu.Position = new Point(Grid.ActualWidth - _dropdownMenu.Width + 8, 0);
            else
                _dropdownMenu.Position = new Point(rect.X, 0);
        }

        private void OpenDropdownMenu()
        {
            _dropdownMenu.IsOpen = true;
            MoveDropdownMenu();
        }

        private (int, string) GetEditingCommand()
        {
            string[] commands = TB.Text.Split(';');
            int sum = 0;
            int cursor = TB.CaretIndex;
            for (int i = 0; i < commands.Length; i++)
            {
                int length = commands[i].Length;
                if (cursor >= sum && cursor <= sum + length)
                    return (sum, commands[i]);
                sum += length + 1;
            }
            return default;
        }

        private void SubstituteCommand()
        {
            string command = _dropdownMenu.LB.SelectedValue as string;
            (int index, string fullCommand) = GetEditingCommand();

            Match match = Regex.Match(fullCommand, @"\s*(?<command>\w+)\s*(?<args>.+)?");
            if (match.Success == false)
                return;

            Group cmd = match.Groups["command"];
            string save = TB.Text.Substring(0, index + cmd.Index);
            TB.Text = save + Regex.Replace(TB.Text, $"{save}{cmd.Value}", command);
            TB.SelectionStart = save.Length + command.Length + 1;
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CreateDropdownMenu();
        }

        private void TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            MoveDropdownMenu();
        }
    }
}
