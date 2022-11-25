using ConsoleCMD.CommandResources;
using System;
using System.Linq;
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
        #region BackgroundColor
        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(ConsoleComponent), new PropertyMetadata(Brushes.Black));
        #endregion
        #region ForegroundColor
        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Brush), typeof(ConsoleComponent), new PropertyMetadata(Brushes.White));
        #endregion
        #region StatusText
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
        #endregion

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
                    
                    break;
                case Key.Tab:
                    if (HintsBox.IsOpen)
                        HintsBox.SelectedItemIndex += isShiftPressed ? -1 : 1;
                    else OpenHintsBox();
                    break;
                case Key.Escape:
                    CloseHintsBox();
                    break;
                case Key.Up:
                    ShowPreviousHistoryLine();
                    break;
                case Key.Down:
                    ShowNextHitstoryLine();
                    break;
            }
        }
        #region History
        private void ShowNextHitstoryLine()
        {
            CommandHistory.MovePrevious();
        }
        private void ShowPreviousHistoryLine()
        {
            if (CommandHistory.SelectedLine == 0)
                CommandHistory.SetCurrentHistoryLine(TBConsole.Text);
            CommandHistory.MoveNext();
        }
        #endregion
        #region Hints box
        private void OpenHintsBox()
        {
            ChangeHints();
            HintsBox.Open();
        }

        private void CloseHintsBox()
        {
            if (HintsBox.IsOpen) HintsBox.Close();
        }

        private void MoveHintsBox()
        {
            Rect rect = TBConsole.GetRectFromCharacterIndex(TBConsole.CaretIndex);
            if (rect.X + HintsBox.Width >= ActualWidth)
               HintsBox.Position = new Point(ActualWidth - HintsBox.Width + 6, 0);
            else
               HintsBox.Position = HintsBox.Position = new Point(rect.X, 0);
        }

        private void ChangeHints()
        {
            string editingCommand = GetCommandWithArgsCurrentlyBeingEdited();
            string commandName = editingCommand.Trim().ToLower().Split(' ')[0];
            HintsBox.Items = Commands.CommandEntities.Where(entity => entity.Synonyms.Any(synonym => synonym.ToLower().StartsWith(commandName)))
                                                     .ToArray();
        }
        #endregion

        private string GetCommandWithArgsCurrentlyBeingEdited()
        {
            string[] commands = TBConsole.Text.Split(';');
            int offset = 0;
            for (int i = 0; i < commands.Length; i++)
            {
                int length = commands[i].Length;
                if (TBConsole.CaretIndex >= offset && TBConsole.CaretIndex <= offset + length)
                    return commands[i];
                offset += length + 1;
            }
            return "";
        }

        private void OnConsoleTextChanged(object sender, TextChangedEventArgs e)
        {
            if (HintsBox.IsOpen)
            {
                ChangeHints();
                MoveHintsBox();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
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
