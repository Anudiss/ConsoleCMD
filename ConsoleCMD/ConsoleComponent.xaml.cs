using ConsoleCMD.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для Console.xaml
    /// </summary>
    public partial class ConsoleComponent : UserControl
    {
        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(ConsoleComponent), new PropertyMetadata(Brushes.Black));

        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Brush), typeof(ConsoleComponent), new PropertyMetadata(Brushes.White));

        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(ConsoleComponent), new PropertyMetadata(""));

        public static readonly string[] SupportedColors =
            typeof(Brushes).GetProperties()
                            .Select(p => new { p.Name, Brush = p.GetValue(null) as Brush })
                            .ToArray()
                            .Select(v => v.Name.ToLower())
                            .ToArray();

        private bool isCommandExecuted = false;

        public static ConsoleComponent Instance;
        
        public ConsoleComponent()
        {
            InitializeComponent();
            Instance = this;
        }

        public void Write(string s)
        {
            int prevSelectionStart = TBConsole.SelectionStart;
            TBConsole.Text += s;
            TBConsole.SelectionStart = prevSelectionStart + s.Length + 2;
        }
        public void WriteLine(string s) => Write(s + "\n");
        public void Clear() => TBConsole.Text = "";
        public bool IsEmpty() => string.IsNullOrWhiteSpace(TBConsole.Text);

        public new bool Focus()
        {
            return TBConsole.Focus();
        }

        private void TBConsole_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (isCommandExecuted)
            {
                isCommandExecuted = false;
                Clear();
                return;
            }
            
            if (e.Key != Key.Enter
                && e.Key != Key.Tab
                && e.Key != Key.Escape
                && e.Key != Key.Up
                && e.Key != Key.Down) return;

            StatusText = "";
            e.Handled = true;

            bool isShiftPressed = Keyboard.Modifiers == ModifierKeys.Shift;
            switch (e.Key)
            {
                case Key.Enter:
                    if (HintsBox.IsOpen) SubstituteCommand();
                    else TryParseAndExecuteCommands();
                    HintsBox.Close();
                    break;
                case Key.Tab:
                    if (HintsBox.IsOpen)
                        HintsBox.SelectedItemIndex += isShiftPressed ? -1 : 1;
                    else OpenHintsBox();
                    break;
                case Key.Escape:
                    if (HintsBox.IsOpen) HintsBox.Close();
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

        private void OpenHintsBox()
        {
            int selectedIndex = HintsBox.SelectedItemIndex;
            LoadHints();
            if (HintsBox.Items is null)
            {
                HintsBox.SelectedItemIndex = 0;
                return;
            }
            HintsBox.SelectedItemIndex = selectedIndex;
            MoveHintsBox();
            HintsBox.Open();
            StatusText = "";
        }

        private void MoveHintsBox()
        {
            Rect rect = TBConsole.GetRectFromCharacterIndex(TBConsole.CaretIndex);
            if (rect.X + HintsBox.Width >= ActualWidth)
               HintsBox.Position = new Point(ActualWidth - HintsBox.Width + 6, 0);
            else
               HintsBox.Position = HintsBox.Position = new Point(rect.X, 0);
        }

        private (int, string) GetCommandWithArgsCurrentlyBeingEdited()
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
            (int index, string editingCommandWithArgs) = GetCommandWithArgsCurrentlyBeingEdited();
            Match match = Command.CommandRegex.Match(editingCommandWithArgs);
            if (!match.Success)
            {
                int lastPosition = TBConsole.SelectionStart;
                string value = HintsBox.LB.SelectedValue as string;
                TBConsole.Text = TBConsole.Text.Insert(lastPosition, value);
                TBConsole.SelectionStart = lastPosition + value.Length + 1;
                HintsBox.Close();
                return;
            }

            Group command = match.Groups["command"];

            string selectedCommand = HintsBox.LB.SelectedValue as string,
                saved = TBConsole.Text.Substring(0, index + command.Index);
            TBConsole.Text = saved + Regex.Replace(TBConsole.Text, $"^({saved}){command.Value}", selectedCommand);
            TBConsole.SelectionStart = saved.Length + selectedCommand.Length + 1;
        }

        private string[] GetHints()
        {
            (int index, string editingCommandWithArgs) = GetCommandWithArgsCurrentlyBeingEdited();
            if (string.IsNullOrWhiteSpace(editingCommandWithArgs))
                return Command.Commands.Select(cmd => cmd.Names[0])
                                           .ToArray();

            Match match = Command.CommandRegex.Match(editingCommandWithArgs);
            if (!match.Success)
            {
                HintsBox.Close();
                return null;
            }
            string command = match.Groups["command"].Value;
            return Command.Commands.Where(cmd => cmd.Names.Any(name => Regex.IsMatch(name, $"^({command}).*")))
                                   .Select(cmd => cmd.Names[0])
                                   .ToArray();
        }

        private void LoadHints()
        {
            string[] hints = GetHints();
            if (hints is null || hints.Length == 0)
            {
                HintsBox.Close();
                StatusText = "Нет подсказок";
                return;
            }
            HintsBox.Items = hints;
            StatusText = "";
        }

        private List<(string, string[])> ParseAndGetCommandsAndArgs()
        {
            string[] commandsWithArgs = TBConsole.Text.Split(';');
            if (string.IsNullOrWhiteSpace(commandsWithArgs.Last()))
                commandsWithArgs = commandsWithArgs.Take(commandsWithArgs.Length - 1).ToArray();

            CommandHistory.AddHistoryLine(TBConsole.Text);

            List<(string, string[])> commandsAndArgs = new List<(string, string[])>();
            foreach (string command in commandsWithArgs)
            {
                Match match = Command.CommandRegex.Match(command);
                if (!match.Success)
                {
                    WriteLine($"Ошибка: команды \"{command}\" не существует.");
                    continue;
                }
                string commandName = match.Groups["command"].Value;
                if (!Command.IsCommandExist(commandName))
                {
                    WriteLine($"Ошибка: команды \"{commandName}\" не существует.");
                    continue;
                }
                string[] commandArgs = match.Groups["args"].Success ? match.Groups["args"].Value.Trim().Split(' ') : new string[] { };
                commandsAndArgs.Add((commandName, commandArgs));
            }
            return commandsAndArgs.Count == 0 ? null : commandsAndArgs;
        }

        private void ExecuteCommand(Command command, string[] args)
        {
            (Command.ReturnCode code, string output) = command.Execute("");
            if (code == Command.ReturnCode.Special)
            {
                if (output == "shutdown")
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else if (code == Command.ReturnCode.Error)
                WriteLine("Ошибка: " + output);
            else if (!string.IsNullOrWhiteSpace(output))
                WriteLine(output);
        }

        private void TryParseAndExecuteCommands()
        {
            if (string.IsNullOrWhiteSpace(TBConsole.Text))
                Clear();
            if (IsEmpty())
                return;

            isCommandExecuted = true;

            string[] splittedCommands = TBConsole.Text.Trim().Split(';');
            
            foreach (var splittedCommand in splittedCommands)
            {
                Match match = Command.CommandRegex.Match(splittedCommand);
                string commandName = match.Groups["command"].Value;
                if (Command.IsCommandExist(commandName) == false)
                {
                    MessageBox.Show($"Такой команды не существует {commandName}");
                    continue;
                }

                Command command = Command.GetCommand(commandName);
                var result = command.Execute(splittedCommand);
                MessageBox.Show($"{result}");
            }
        }

        private void TBConsole_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (HintsBox.IsOpen)
            {
                MoveHintsBox();
                LoadHints();
            }
        }

        private void ConsoleComponent_Loaded(object sender, RoutedEventArgs e)
        {
            HintsBox.MaxHeight = ActualHeight;
            HintsBox.PlacementTarget = this;
            
            CommandHistory.OnHistoryLineChanged += (historyLine) =>
            {
                Clear();
                Write(historyLine);
            };
        }

    }
}
