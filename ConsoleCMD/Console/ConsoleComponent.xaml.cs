using CommandParser.Command;
using ConsoleCMD.Console.CommandResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ConsoleCMD.Console
{
    /// <summary>
    /// Логика взаимодействия для ConsoleComponent.xaml
    /// </summary>
    public partial class ConsoleComponent : UserControl
    {
        public HintsBoxComponent HintsBox { get; set; }

        #region Команды управления консолью
        public static RelayCommand OpenHintsBoxCommand { get; set; }
        public static RelayCommand CloseHintsBoxCommand { get; set; }
        public static RelayCommand ExecuteCommand { get; set; }
        #endregion
        #region Конструктор
        /// <summary>
        /// Конструктор
        /// </summary>
        public ConsoleComponent()
        {
            InitializeConsoleCommands();

            InitializeComponent();
        }
        #endregion
        #region Методы для работы с консолью
        #region Методы работы с подсказками
        #region OpenHintsBox
        /// <summary>
        /// Метод открытия подсказок
        /// </summary>
        private void OpenHintsBox()
        {
            HintsBox.Open();
            ChangeHints();
        }
        #endregion
        #region CloseHintsBox
        /// <summary>
        /// Метод закрытия подсказок
        /// </summary>
        private void CloseHintsBox() =>
            HintsBox.Close();
        #endregion
        #region ChangeHints
        /// <summary>
        /// Метод изменения подсказок в HintsBoxComponent
        /// </summary>
        private void ChangeHints()
        {
            if (!HintsBox.IsOpen)
                return;

            string currentCommand = GetEditingCommand();
            if (currentCommand == null)
            {
                HintsBox.Items = Commands.CommandEntities.ToArray();
                return;
            }

            HintsBox.Items = Commands.CommandEntities.Where(entity => entity.Synonyms.Any(e => e.StartsWith(new string(currentCommand.TakeWhile(c => c != ' ').ToArray())))).ToArray();
        }
        #endregion
        #region GetEditingCommand
        /// <summary>
        /// Метод, получения текущей команды
        /// </summary>
        /// <returns>Команда, на которой находится курсор</returns>
        private string GetEditingCommand()
        {
            Match match = Regex.Match(Console.Text.Trim(), @"(?:(?<Command>[^;]+);?)+");
            foreach (Capture capture in match.Groups["Command"].Captures)
                if (capture.Index <= Console.CaretIndex && capture.Index + capture.Length >= Console.CaretIndex)
                    return capture.Value.Trim();
            return null;
        }
        #endregion
        #endregion
        #region InitializeConsoleCommands
        /// <summary>
        /// Метод для инициализации команд
        /// </summary>
        private void InitializeConsoleCommands()
        {
            OpenHintsBoxCommand = new RelayCommand((arg) => OpenHintsBox(), (arg) => !HintsBox.IsOpen);
            CloseHintsBoxCommand = new RelayCommand((arg) => CloseHintsBox(), (arg) => HintsBox.IsOpen);
            ExecuteCommand = new RelayCommand((arg) => ExecuteCommands(), (arg) => true);
        }
        #endregion
        #region Clear
        /// <summary>
        /// Метод очистки TextBox
        /// </summary>
        private void Clear() => Console.Text = "";
        #endregion
        #endregion
        #region Методы для работы с командами
        #region ParseCommands
        /// <summary>
        /// Метод парсинга введённых команд
        /// </summary>
        /// <returns>Перечисление сущностей команд</returns>
        private IEnumerable<Command> ParseCommands()
        {
            string[] commands = Console.Text.Split(';');
            foreach (var command in commands)
            {
                if (command.Trim() == string.Empty)
                    continue;

                yield return Commands.CommandEntities.ParseCommand(command);
            }
        }
        #endregion
        #region ExecuteCommands
        /// <summary>
        /// Метод выполнения введённых команд
        /// </summary>
        private void ExecuteCommands()
        {
            try
            {
                foreach (Command command in ParseCommands())
                    command.Execute();
                Clear();
                CloseHintsBox();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion
        #endregion
        #region События
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HintsBox = new HintsBoxComponent()
            {
                PlacementTarget = Console,
                Placement = PlacementMode.Top,
                IsOpen = false
            };
        }

        private void PreviewConsoleKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void OnConsoleTextChanged(object sender, TextChangedEventArgs e) =>
            ChangeHints();

        private void OnCaretPositionChanged(object sender, RoutedEventArgs e) =>
            ChangeHints();
        #endregion
    }
    #region RelayCommand
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public Action<object> _execute;
        public Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) != false;

        public void Execute(object parameter) => _execute?.Invoke(parameter);
    }
    #endregion
}
