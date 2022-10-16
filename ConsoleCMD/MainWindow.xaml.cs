using ConsoleCMD.Applications;
using System;
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

        public Brush ConsoleBackgroundColor
        {
            get { return (Brush)GetValue(ConsoleBackgroundProperty); }
            set { SetValue(ConsoleBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ConsoleBackgroundProperty =
            DependencyProperty.Register("ConsoleBackgroundColor", typeof(Brush), typeof(MainWindow), new PropertyMetadata(Brushes.Black));

        public Brush ConsoleForegroundColor
        {
            get { return (Brush)GetValue(ConsoleForegroundProperty); }
            set { SetValue(ConsoleForegroundProperty, value); }
        }
        public static readonly DependencyProperty ConsoleForegroundProperty =
            DependencyProperty.Register("ConsoleForegroundColor", typeof(Brush), typeof(MainWindow), new PropertyMetadata(Brushes.White));

        public static readonly string[] ConsoleSupportedColors =
            typeof(Brushes).GetProperties()
                            .Select(p => new { p.Name, Brush = p.GetValue(null) as Brush })
                            .ToArray()
                            .Select(v => v.Name.ToLower())
                            .ToArray();

        private void ConsoleWrite(string s)
        {
            int prevSelectionStart = TBConsole.SelectionStart;
            TBConsole.Text += s;
            TBConsole.SelectionStart = prevSelectionStart + s.Length + 2;
        }
        private void ConsoleWriteLine(string s) => ConsoleWrite($"{s}\n");
        private void ConsoleClear() => TBConsole.Text = "";
        private bool ConsoleIsEmpty() => String.IsNullOrWhiteSpace(TBConsole.Text);

        private bool _commandExecuted = false;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;

            ConsoleBackgroundColor = Brushes.Black;
            ConsoleForegroundColor = Brushes.White;

            TBConsole.Focus();
        }

        private void InitializeDropDownMenu()
        {
            DDM.MaxHeight = TBConsole.ActualHeight;
            DDM.PlacementTarget = TBConsole;

            CommandHistory.OnHistoryLineChanged += (historyLine) =>
            {
                TBConsole.Text = historyLine;
                TBConsole.CaretIndex = TBConsole.Text.Length;
            };
        }


        private void OpenDropdownMenu()
        {
            int selectedIndex = DDM.SelectedItemIndex;
            LoadHints();
            if (DDM.Items is null)
            {
                DDM.SelectedItemIndex = 0;
                return;
            }
            DDM.SelectedItemIndex = selectedIndex;
            MoveDropdownMenu();
            DDM.IsOpen = true;
            StatusLine.Text = "";
        }

        private void MoveDropdownMenu()
        {
            if (!DDM.IsOpen) return;
            Rect rect = TBConsole.GetRectFromCharacterIndex(TBConsole.CaretIndex);
            if (rect.X + DDM.Width >= Grid.ActualWidth)
                DDM.Position = new Point(Grid.ActualWidth - DDM.Width + 6, 0);
            else
                DDM.Position = new Point(rect.X, 0);
        }

        private (int, string) GetEditingCommandWithArgs()
        {
            string[] commands = TBConsole.Text.Split(';');
            int offset = 0;
            for (int i = 0; i < commands.Length; i++)
            {
                int length = commands[i].Length;
                if (TBConsole.CaretIndex >= offset && TBConsole.CaretIndex <= offset + length)
                    return (offset, commands[i]);
                offset += length + 1;
            }
            return (0, "");
        }

        private void SubstituteCommand()
        {
            (int index, string editingCommandWithArgs) = GetEditingCommandWithArgs();
            Match match = Command.CommandRegex.Match(editingCommandWithArgs);
            if (!match.Success)
            {
                int lastPosition = TBConsole.SelectionStart;
                string value = DDM.LB.SelectedValue as string;
                TBConsole.Text = TBConsole.Text.Insert(lastPosition, value);
                TBConsole.SelectionStart = lastPosition + value.Length + 1;
                DDM.IsOpen = false;
                return;
            }

            Group command = match.Groups["command"];

            string selectedCommand = DDM.LB.SelectedValue as string,
                saved = TBConsole.Text.Substring(0, index + command.Index);
            TBConsole.Text = saved + Regex.Replace(TBConsole.Text, $"^({saved}){command.Value}", selectedCommand);
            TBConsole.SelectionStart = saved.Length + selectedCommand.Length + 1;
        }

        private string[] GetHints()
        {
            (int index, string editingCommandWithArgs) = GetEditingCommandWithArgs();
            if (editingCommandWithArgs.Trim() == "")
                return Command.CommandNames.Keys.Select(keys => keys.First())
                                                .ToArray();

            Match match = Command.CommandRegex.Match(editingCommandWithArgs);
            if (!match.Success)
            {
                DDM.IsOpen = false;
                return null;
            }
            string command = match.Groups["command"].Value;
            return Command.CommandNames.Keys.Where(keys => keys.Any(key => Regex.IsMatch(key, $"{command}.*")))
                                            .Select(keys => keys.First()).ToArray();
        }

        private void LoadHints()
        {
            string[] hints = GetHints();
            if (hints is null || hints.Length == 0)
            {
                DDM.IsOpen = false;
                StatusLine.Text = $"Нет подсказок";
                return;
            }
            DDM.Items = hints;
            StatusLine.Text = "";
        }

        private List<(string, string[])> ParseAndGetCommandsAndArgs()
        {
            string[] commandsWithArgs = TBConsole.Text.Split(';');
            if (commandsWithArgs.Last() == string.Empty)
                commandsWithArgs = commandsWithArgs.Take(commandsWithArgs.Length - 1).ToArray();

            CommandHistory.AddHistoryLine(TBConsole.Text);
            CommandHistory.Reset();

            List<(string, string[])> commandsAndArgs = new List<(string, string[])>();
            foreach (string command in commandsWithArgs)
            {
                Match match = Command.CommandRegex.Match(command);
                if (!match.Success)
                {
                    ConsoleWriteLine($"Ошибка: команды \"{command}\" не существует.");
                    continue;
                }
                string commandName = match.Groups["command"].Value;
                if (!Command.IsCommandExist(commandName))
                {
                    ConsoleWriteLine($"Ошибка: команды \"{commandName}\" не существует.");
                    continue;
                }
                string[] commandArgs = match.Groups["args"].Success ? match.Groups["args"].Value.Trim().Split(' ') : new string[] { };
                commandsAndArgs.Add((commandName, commandArgs));
            }
            return commandsAndArgs.Count == 0 ? null : commandsAndArgs;
        }

        private void ExecuteCommand(Command command, string[] args)
        {
            (Command.ReturnCode code, string output) = command.Execute(args);
            if (code == Command.ReturnCode.Error)
                ConsoleWriteLine("Ошибка: " + output);
            else if (!String.IsNullOrWhiteSpace(output))
                ConsoleWriteLine(output);
        }

        private void TryParseAndExecuteCommands()
        {
            if (String.IsNullOrWhiteSpace(TBConsole.Text))
                ConsoleClear();
            if (ConsoleIsEmpty())
                return;

            _commandExecuted = true;

            List<(string, string[])> commandsAndArgs = ParseAndGetCommandsAndArgs();

            if (commandsAndArgs is null)
                return;

            commandsAndArgs.ForEach(pair =>
            {
                Command command = Command.GetCommand(pair.Item1);
                string[] args = pair.Item2;
                ExecuteCommand(command, args);
            });
        }

        private void TBConsole_TextChanged(object sender, TextChangedEventArgs e)
        {
            MoveDropdownMenu();
            if (DDM.IsOpen) LoadHints();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) => MoveDropdownMenu();
        private void Window_Loaded(object sender, RoutedEventArgs e) => InitializeDropDownMenu();
        private void Window_LocationChanged(object sender, EventArgs e) => MoveDropdownMenu();

        private void TBConsole_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftPressed = Keyboard.Modifiers == ModifierKeys.Shift;
            if (_commandExecuted)
            {
                _commandExecuted = false;
                ConsoleClear();
                return;
            }

            if (new[] { Key.Enter, Key.Tab, Key.Escape, Key.Up, Key.Down }.Contains(e.Key) == false)
                return;

            StatusLine.Text = "";
            e.Handled = true;

            switch (e.Key)
            {
                case Key.Enter:
                    if (DDM.IsOpen)
                        SubstituteCommand();
                    else
                        TryParseAndExecuteCommands();
                    DDM.IsOpen = false;
                    break;
                case Key.Tab:
                    if (DDM.IsOpen)
                        DDM.SelectedItemIndex += isShiftPressed ? -1 : 1;
                    OpenDropdownMenu();
                    break;
                case Key.Escape:
                    DDM.IsOpen = false;
                    break;
                case Key.Up:
                    if (CommandHistory.SelectedLine == 0)
                        CommandHistory.SetCurrentHistoryLine(TBConsole.Text);
                    CommandHistory.MoveNext();
                    break;
                case Key.Down:
                    CommandHistory.MovePrevious();
                    break;
            }

        }
    }
}
