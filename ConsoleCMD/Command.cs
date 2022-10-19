﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace ConsoleCMD.Applications
{
    public class Command
    {
        public enum ReturnCode
        {
            Special, Success, Error
        }

        public static readonly Regex CommandRegex = new Regex(@"\s*(?<command>\S+)\s*(?<args>.+)?\s*", RegexOptions.Compiled);

        // Flags will be arguments, starting with -- or short -
        // There are ReturnCode.Special. We can do side actions when get this code

        public static Dictionary<string[], Command> CommandNames = new Dictionary<string[], Command>
        {
            { new[] { "help", "h" },
                new Command(
                    description: "Выводит список доступных команд",
                    usage: "help",
                    invalidArgsMessage: "",
                    argsValidator: (args) => true,
                    executor: (args) => {
                        return (ReturnCode.Success, string.Join("\n", CommandNames.Select(keyPair => $"{keyPair.Key[0]} - {keyPair.Value.Description}")));
                    }
                )
            },
            { new[] { "set_background_color", "set_bg_color", "set_bg_col" },
                new Command(
                    description: "Изменяет фоновый цвет консоли",
                    usage: "set_background_color <color>\nПример: set_background_color white",
                    invalidArgsMessage: "Команда принимает ровно один аргумент",
                    argsValidator: (args) => args.Length == 1,
                    executor: (args) => {
                        string color = args[0].ToLower();
                        if (!ConsoleComponent.SupportedColors.Contains(color))
                        {
                            return (ReturnCode.Error, "Неверно указан цвет");
                        }
                        Brush brush = new BrushConverter().ConvertFromString(color) as Brush;
                        ConsoleComponent.Instance.BackgroundColor = brush;
                        return (ReturnCode.Success, "");
                    }
                )
            },
            { new[] { "set_foreground_color", "set_fg_color", "set_fg_col" },
                new Command(
                    description: "Изменяет цвет текста консоли.",
                    usage: "set_foreground_color <color>\nПример: set_foreground_color black",
                    invalidArgsMessage: "Команда принимает ровно один аргумент",
                    argsValidator: (args) => args.Length == 1,
                    executor: (args) => {
                        string color = args[0].ToLower();
                        if (!ConsoleComponent.SupportedColors.Contains(color))
                        {
                            return (ReturnCode.Error, "Неверно указан цвет");
                        }
                        Brush brush = new BrushConverter().ConvertFromString(color) as Brush;
                        ConsoleComponent.Instance.ForegroundColor = brush;
                        return (ReturnCode.Success, "");
                    }
                )
            },
            { new[] { "echo" },
                new Command(
                    description: "Просто выводит переданный(е) аргумент(ы)",
                    usage: "echo arg1, arg2, ... , argN\nПример: echo Hello, world!",
                    invalidArgsMessage: "",
                    argsValidator: (args) => true,
                    executor: (args) => (ReturnCode.Success, string.Join(" ", args))
                )
            },
            { new[] { "clear_history", "clr_hist", "c_h" },
                new Command(
                    description: "Очищает историю ввода",
                    usage: "clear_history",
                    invalidArgsMessage: "",
                    argsValidator: (args) => true,
                    executor: (args) => { CommandHistory.Clear(); return (ReturnCode.Success, ""); }
                )
            },
            { new[] { "exit", "quit", "close", "shutdown" },
                new Command(
                    description: "Очищает историю ввода",
                    usage: "clear_history",
                    invalidArgsMessage: "",
                    argsValidator: (args) => true,
                    executor: (args) => { CommandHistory.Clear(); return (ReturnCode.Special, "shutdown"); }
                )
            },
        };

        public static bool IsCommandExist(string commandName) =>
            CommandNames.Any(keyPair => keyPair.Key.Contains(commandName));

        public static Command GetCommand(string commandName) =>
            CommandNames.First(keyPair => keyPair.Key.Contains(commandName)).Value;

        public delegate (ReturnCode, string) CommandExecutor(string[] args);
        public delegate bool CommandArgsValidator(string[] args);

        public string[] Names { get; }
        public string Description { get; }
        public string Usage { get; }
        public Argument[] Arguments { get; } 
        public Flag[] Flags { get; }
        public Regex Pattern { get; }
        public CommandExecutor Executor { get; }

        public Command(string[] names, string description, CommandExecutor executor, Argument[] arguments, Flag[] flags)
        {
            Names = names;
            Description = description;
            Usage = AssembleUsage(arguments);
            Arguments = arguments;
            Flags = flags;
            Executor = executor;
        }

        private string AssembleUsage(Argument[] arguments, Flag[] flags)
        {
            string command = $"{Names[0]}";
            foreach (var flag in flags)
            {
                command += $" -{flag.Name}:{flag.Type}";
            }
            foreach (var argument in arguments)
            {
                string arg = $"{argument.Name}:{argument.Type}";
                command += argument.IsRequired ? $" {arg}" : $" [{arg}]";
            }
            return command;
        }

        private Regex AssemblePattern()
        {

        }

        private bool ValidateArguments((Argument argument, object value)[] arguments)
        {
            throw new NotImplementedException();
        }

        public (ReturnCode, string) Execute(string[] args)
        {
            if (args.Length > 0 && (args.Contains("--help") || args.Contains("-h")))
            {
                return (ReturnCode.Success, Description + "\n" + Usage);
            }
            if (!ArgsValidator.Invoke(args))
                return (ReturnCode.Error, InvalidArgsMessage);
            return Executor.Invoke(args);
        }

        public (Argument argument, object value) ParseArguments()
        {
            throw new NotImplementedException();
        }
    }

    public struct Argument
    {
        public ArgumentType Type { get; set; }
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        
        public Argument(ArgumentType type, string name, bool isRequired)
        {
            Type = type;
            Name = name;
            IsRequired = isRequired;
        }
    }

    public struct Flag
    {
        public ArgumentType Type { get; set; }
        public string ShortName{ get; set; }
        public string FullName { get; set; }

        public Flag(ArgumentType type, string shortName, string fullName)
        {
            Type = type;
            ShortName = shortName;
            FullName = fullName;
        }
    }

    public enum ArgumentType
    {
        Int, Double, String, Bool, Color, Path
    }
}
