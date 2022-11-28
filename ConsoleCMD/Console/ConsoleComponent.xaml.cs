using CommandParser.Command;
using CommandParser.Hints;
using ConsoleCMD.CommandResources;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ConsoleCMD.Console
{
    /// <summary>
    /// Логика взаимодействия для ConsoleComponent.xaml
    /// </summary>
    public partial class ConsoleComponent : UserControl
    {
        /*
         * Сама задумка такая
         * Ты пишешь команду, он сразу парсит её (ещё не придумал в какой момент будет парсить)
         * Из-за уже спарсеной команды можно вытащить всё
         * Перемещаясь курсором можно увидеть всплывающее окно типа ошибка или какой тип данных тут используется, имя аргумента
         * Также хочу добавить авто рефакторинг команды
         */
        public static readonly Regex CommandRegex = new Regex(@"((?<Command>[^\;]+)\;?)+", RegexOptions.Compiled);
        public static readonly Regex ArgumentsRegex = new Regex(@"(\S+)*", RegexOptions.Compiled);

        #region ConsoleCommands
        public static ConsoleCommand OpenHintBoxCommand { get; set; }
        public static ConsoleCommand CloseHintBoxCommand { get; set; }
        #endregion

        public IEnumerable<CommandEntity> Commands { get; set; }

        public HintsBoxComponent HintsBox { get; set; }

        public ConsoleComponent()
        {
            InitializeCommands();

            InitializeComponent();
        }

        private void InitializeCommands()
        {
            OpenHintBoxCommand = new ConsoleCommand((param) => HintsBox.Open(), (param) => !HintsBox.IsOpen);
            CloseHintBoxCommand = new ConsoleCommand((param) => HintsBox.Close(), (param) => HintsBox.IsOpen);
        }

        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            HighlightCommand();
        }

        /*
         * Главная суть вот такая
         * 
         * Если мы добавляет Run и ебенем ему стили, после будем вводить туда текст, то он тоже будет
         * с этими стилями
         * 
         * То есть нужно каким-то хуем распознать в какой команде мы находимся,
         * на какие-то конченные кнопки событие ёбнуть, чтоб создавало новый Run и определяло
         * что мы вводим
         * 
         * Если стереть весь текст из Run'а, то Run удаляется (Я проверял)
         * (Кроме первого, чтоб было куда текст вводить)
         */

        private void HighlightCommand()
        {
            string text = new TextRange(TBConsole.Document.ContentStart, TBConsole.Document.ContentEnd).Text;
            CommandContainer.Inlines.Clear();
            string[] commands = text.Split(';');
            foreach (var command in commands)
            {
                if (command.Trim() == string.Empty)
                    continue;

                Span span = new Span();

                var tokens = CommandParser.Command.CommandParser.DoLexicalAnalyze(command.Trim());
                string commandName = tokens.First();

                span.Inlines.Add(new Run($" {commandName}")
                {
                    Foreground = HintColorScheme.ColorScheme[HintPart.CommandName]
                });

                foreach (var token in tokens.Skip(1))
                {
                    span.Inlines.Add(new Run($" {token}")
                    {
                        Foreground = HintColorScheme.ColorScheme[token.StartsWith("-") ? HintPart.Flag : HintPart.RequiredArgument]
                    });
                }

                CommandContainer.Inlines.Add(span);
                CommandContainer.Inlines.Add(";");
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Validate()
        {
            throw new NotImplementedException();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HintsBox = new HintsBoxComponent
            {
                PlacementTarget = this,
                IsOpen = false,
                Items = CommandResources.Commands.CommandEntities.ToArray()
            };
        }

        private void MoveHintsBox()
        {
            /*if (!HintsBox.IsOpen)
                return;

            Rect rect = TBConsole.GetRectFromCharacterIndex(TBConsole.CaretIndex);
            if (rect.X + HintsBox.Width >= ActualWidth)
                HintsBox.Position = new Point(ActualWidth - HintsBox.Width + 6, 0);
            else
                HintsBox.Position = HintsBox.Position = new Point(rect.X, 0);*/
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e) =>
            MoveHintsBox();
    }
    #region ConsoleCommand
    public class ConsoleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public ConsoleCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute.Invoke(parameter);
        public void Execute(object parameter) => _execute.Invoke(parameter);
    }
    #endregion
    #region CommandRange
    /// <summary>
    /// Класс одной команды в виде строки
    /// </summary>
    public class CommandRange
    {
        /// <summary>
        /// Начало команды
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Длина команды
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Сама команда
        /// </summary>
        public string Command { get; set; }

        public MatchCollection Arguments =>
            ConsoleComponent.ArgumentsRegex.Matches(Command);

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="commandCapture">Часть выражения, содержащее команду</param>
        public CommandRange(Capture commandCapture)
        {
            Start = commandCapture.Index;
            Length = commandCapture.Length;
            Command = commandCapture.Value;
        }

        public Capture EditingArgument(int caretPosition)
        {
            foreach (Match match in Arguments)
            {
                if (Start + match.Index <= caretPosition && Start + match.Index + match.Length >= caretPosition)
                    return match;
            }
            return null;
        }
    }
    #endregion
}
