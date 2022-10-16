using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ConsoleCMD.Applications
{
    public class Command
    {
        public enum ReturnCode
        {
            Success = 0, Error
        }

        public static readonly Regex CommandRegex = new Regex(@"\s*(?<command>\w+)\s*(?<args>.+)?\s*", RegexOptions.Compiled);

        public static Dictionary<string[], Command> CommandNames = new Dictionary<string[], Command>
        {
            { new[] { "help", "h" },
                new Command(
                    description: "Выводит список доступных команд",
                    usage: "help\n",
                    invalidArgsMessage: "",
                    argsValidator: (args) => true,
                    executor: (args) => {
                        string commandsList = "";
                        foreach (string[] commandNames in CommandNames.Keys)
                        {
                            commandsList += commandNames[0] + "\n";
                        }
                        return (ReturnCode.Success, commandsList);
                    }
                )
            },
            { new[] { "set_background_color", "set_bg_color", "set_bg_col" },
                new Command(
                    description: "Изменяет фоновый цвет консоли.",
                    usage: "set_background_color <color>\nПример: set_background_color white",
                    invalidArgsMessage: "Команда принимает ровно один аргумент.",
                    argsValidator: (args) => args.Length == 1,
                    executor: (args) => {
                        string color = args[0].ToLower();
                        MessageBox.Show(color);
                        if (!MainWindow.ConsoleSupportedColors.Contains(color))
                        {
                            return (ReturnCode.Error, "Неверно указан цвет.");
                        }
                        Brush brush = new BrushConverter().ConvertFromString(color) as Brush;
                        MainWindow.AppWindow.ConsoleBackgroundColor = brush;
                        return (ReturnCode.Success, "");
                    }
                )
            },
            { new[] { "set_foreground_color", "set_fg_color", "set_fg_col" },
                new Command(
                    description: "Изменяет цвет текста консоли.",
                    usage: "set_foreground_color <color>\nПример: set_foreground_color black",
                    invalidArgsMessage: "Команда принимает ровно один аргумент.",
                    argsValidator: (args) => args.Length == 1,
                    executor: (args) => {
                        string color = args[0].ToLower();
                        if (!MainWindow.ConsoleSupportedColors.Contains(color))
                        {
                            return (ReturnCode.Error, "Неверно указан цвет.");
                        }
                        Brush brush = new BrushConverter().ConvertFromString(color) as Brush;
                        MainWindow.AppWindow.ConsoleForegroundColor = brush;
                        return (ReturnCode.Success, "");
                    }
                )
            },
            { new[] { "echo" },
                new Command(
                    description: "Просто выводит переданный(е) аргумент(ы).",
                    usage: "echo [<arg1>,<arg2>,...,<argn>]\nПример: echo Hello, world!",
                    invalidArgsMessage: "",
                    argsValidator: (args) => true,
                    executor: (args) => (ReturnCode.Success, string.Join(" ", args))
                )
            },
            { new[] { "clearhistory", "clearh", "ch" },
                new Command(
                    description: "Отчищает историю ввода",
                    usage: "clearhistory",
                    invalidArgsMessage: "",
                    argsValidator: (args) => true,
                    executor: (args) => { CommandHistory.Clear(); return (ReturnCode.Success, ""); }
                )
            }
        };

        public static bool IsCommandExist(string commandName) =>
            CommandNames.Any(keyPair => keyPair.Key.Contains(commandName));

        public static Command GetCommand(string commandName) =>
            CommandNames.First(keyPair => keyPair.Key.Contains(commandName)).Value;

        public delegate (ReturnCode, string) CommandExecutor(string[] args);
        public delegate bool CommandArgsValidator(string[] args);

        public string Description { get; }
        public string Usage { get; }
        public string InvalidArgsMessage { get; }
        public CommandExecutor Executor { get; }
        public CommandArgsValidator ArgsValidator { get; }

        public Command(string description, string usage, string invalidArgsMessage,
            CommandExecutor executor, CommandArgsValidator argsValidator)
        {
            Description = description;
            Usage = usage;
            InvalidArgsMessage = invalidArgsMessage;
            Executor = executor;
            ArgsValidator = argsValidator;
        }

        public (ReturnCode, string) Execute(string[] args)
        {
            if (args.Length > 0 && (args[0].ToLower() == "help" || args[0].ToLower() == "h"))
            {
                return (ReturnCode.Success, Usage);
            }
            if (!ArgsValidator.Invoke(args))
                return (ReturnCode.Error, InvalidArgsMessage);
            return Executor.Invoke(args);
        }
    }
}
