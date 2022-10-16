using ConsoleCMD.Applications;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
                            .Select(p => new { Name = p.Name, Brush = p.GetValue(null) as Brush })
                            .ToArray()
                            .Select(v => v.Name.ToLower())
                            .ToArray();


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

        private void Window_Loaded(object sender, RoutedEventArgs e) => InitializeDropDownMenu();

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
            TBConsole.Text = saved + Regex.Replace(TBConsole.Text, $"{saved}{command.Value}", selectedCommand);
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

        private void TryParseAndExecuteCommands()
        {
            _commandExecuted = true;
            string[] commands = TBConsole.Text.Split(';');
            CommandHistory.AddHistoryLine(TBConsole.Text);
            CommandHistory.Reset();
            foreach (string command in commands)
            {
                Match match = Command.CommandRegex.Match(command);
                if (!match.Success)
                {
                    MessageBox.Show("Такой команды не существует.");
                    break;
                }
                string commandName = match.Groups["command"].Value;
                if (!Command.IsCommandExist(commandName))
                {
                    MessageBox.Show("Такой команды не существует.");
                    break;
                }
                string[] args = match.Groups["args"].Success ? match.Groups["args"].Value.Trim().Split(' ') : new string[] { };
                ExecuteCommand(Command.GetCommand(commandName), args);
            }
        }

        private void ExecuteCommand(Command command, string[] args)
        {
            (Command.ReturnCode code, string output) = command.Execute(args);
            if (code == Command.ReturnCode.Error)
                output = $"Ошибка: {output}";
            if (output.Trim() == string.Empty)
            {
                TBConsole.Text = "";
                return;
            }
            int prevSelectionStart = TBConsole.SelectionStart;
            TBConsole.Text += $"\n{output}";
            TBConsole.SelectionStart = prevSelectionStart + output.Length + 2;
        }

        private void TBConsole_KeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftPressed = Keyboard.Modifiers == ModifierKeys.Shift;
            if (_commandExecuted)
            {
                _commandExecuted = false;
                TBConsole.Text = "";
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    StatusLine.Text = "";
                    if (DDM.IsOpen)
                        SubstituteCommand();
                    else
                        TryParseAndExecuteCommands();
                    DDM.IsOpen = false;
                    break;
                case Key.Tab:
                    StatusLine.Text = "";
                    if (DDM.IsOpen)
                        DDM.SelectedItemIndex += isShiftPressed ? -1 : 1;
                    OpenDropdownMenu();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    StatusLine.Text = "";
                    DDM.IsOpen = false;
                    break;
                case Key.Up:
                    if (CommandHistory.SelectedLine == 0)
                        CommandHistory.SetCurrentHistoryLine(TBConsole.Text);
                    CommandHistory.MoveNext();
                    e.Handled = true;
                    break;
                case Key.Down:
                    CommandHistory.MovePrevious();
                    e.Handled = true;
                    break;
            }
        }

        private void TBConsole_TextChanged(object sender, TextChangedEventArgs e)
        {
            MoveDropdownMenu();
            if (DDM.IsOpen)
                LoadHints();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) => MoveDropdownMenu();
        private void Window_LocationChanged(object sender, EventArgs e) => MoveDropdownMenu();
    }
}
