using System;
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

        public static readonly Dictionary<ArgumentType, ArgumentData> ArgumentDataSet = new Dictionary<ArgumentType, ArgumentData>()
        {
            { ArgumentType.Int, new ArgumentData( @"(\d+)", (value) => int.Parse(value)) },
            { ArgumentType.Double, new ArgumentData( @"(\d+)([,\.]\d+)?", (value) => double.Parse(value)) },
            { ArgumentType.String, new ArgumentData( @"(\S+)", (value) => value) },
            { ArgumentType.Bool, new ArgumentData( @"([10]|(true)|(false)|(yes)|(no))|(y)|(n)", (value) => new[] { "1", "true", "y", "yes" }.Contains(value)) },
            { ArgumentType.Color, new ArgumentData( $@"(?<Name>{string.Join("|", ConsoleComponent.SupportedColors)})|(#(?<HEX>[0-9A-Fa-f]{{6}}))", 
                                                    (value) => ColorConverter.ConvertFromString(value)) },
            { ArgumentType.Path, new ArgumentData( @"(\d+)", (value) => value) }
        };

        public static readonly Regex CommandRegex = new Regex(@"\s*(?<command>\S+)\s*(?<args>.+)?\s*", RegexOptions.Compiled);

        // Flags will be arguments, starting with -- or short -
        // There are ReturnCode.Special. We can do side actions when get this code

        public static List<Command> CommandNames = new List<Command>
        {
            new Command(
                new[] { "help", "h" },
                description: "Выводит список доступных команд",
                executor: (args) => {
                    return (ReturnCode.Success, string.Join("\n", CommandNames.Select(command => $"{command.Names[0]} - {command.Description}")));
                },
                arguments: null,
                flags: null
            ),
            new Command(
                new[] { "set_background_color", "set_bg_color", "set_bg_col" },
                description: "Изменяет фоновый цвет консоли",
                executor: (args) => {
                    string color = args[0].ToLower();
                    if (!ConsoleComponent.SupportedColors.Contains(color))
                    {
                        return (ReturnCode.Error, "Неверно указан цвет");
                    }
                    Brush brush = new BrushConverter().ConvertFromString(color) as Brush;
                    ConsoleComponent.Instance.BackgroundColor = brush;
                    return (ReturnCode.Success, "");
                },
                arguments: null,
                flags: null
            ),
            new Command(
                new[] { "set_foreground_color", "set_fg_color", "set_fg_col" },
                description: "Изменяет цвет текста консоли.",
                executor: (args) => {
                    string color = args[0].ToLower();
                    if (!ConsoleComponent.SupportedColors.Contains(color))
                    {
                        return (ReturnCode.Error, "Неверно указан цвет");
                    }
                    Brush brush = new BrushConverter().ConvertFromString(color) as Brush;
                    ConsoleComponent.Instance.ForegroundColor = brush;
                    return (ReturnCode.Success, "");
                },
                arguments: null,
                flags: null
            ),
            new Command(
                new[] { "echo" },
                description: "Просто выводит переданный(е) аргумент(ы)",
                executor: (args) => (ReturnCode.Success, string.Join(" ", args)),
                arguments: null,
                flags: null
            ),
            new Command(
                new[] { "clear_history", "clr_hist", "c_h" },
                description: "Очищает историю ввода",
                executor: (args) => { CommandHistory.Clear(); return (ReturnCode.Success, ""); },
                arguments: null,
                flags: null
            ),
            new Command(
                new[] { "exit", "quit", "close", "shutdown" },
                description: "Очищает историю ввода",
                executor: (args) => { CommandHistory.Clear(); return (ReturnCode.Special, "shutdown"); },
                arguments: null,
                flags: null
            )
        };

        public static bool IsCommandExist(string commandName) =>
            CommandNames.Any(command => command.Names.Contains(commandName));

        public static Command GetCommand(string commandName) =>
            CommandNames.First(command => command.Names.Contains(commandName));

        public delegate (ReturnCode, string) CommandExecutor(string[] args);
        public delegate bool CommandArgsValidator(string[] args);
        public delegate object ArgumentParser(string value);

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
            Usage = AssembleUsage(arguments, flags);
            Pattern = AssemblePattern(arguments);
            Arguments = arguments;
            Flags = flags;
            Executor = executor;
        }

        private string AssembleUsage(Argument[] arguments, Flag[] flags)
        {
            string command = $"{Names[0]}";
            if (flags != null)
                foreach (var flag in flags)
                    command += $" -{flag.FullName}:{flag.Type}";
            
            if (arguments == null)
                return command;

            foreach (var argument in arguments)
            {
                string arg = $"{argument.Name}:{argument.Type}";
                command += argument.IsRequired ? $" {arg}" : $" [{arg}]";
            }
            return command;
        }

        private Regex AssemblePattern(Argument[] arguments)
        {
            string pattern = $@"(?<CommandName>{string.Join("|", Names)})\s*";
            if (arguments == null)
                return new Regex(pattern, RegexOptions.Compiled);

            pattern += string.Join(@"\s+", arguments.Select(arg => $"(?<{arg.Name}>{ArgumentDataSet[arg.Type].Pattern})"));

            return new Regex(pattern, RegexOptions.Compiled);
        }

        private bool ValidateCommand(string input) => Pattern.IsMatch(input);
        private bool ValidateCommand(string input, out Match match) => (match = Pattern.Match(input)).Success;

        public (ReturnCode, string) Execute()
        {
            
        }

        public (Argument argument, object value)[] ParseArguments(string input)
        {
            if (ValidateCommand(input, out Match match) == false)
                return null;

            var values = new List<(Argument argument, object value)>();

            foreach (var arg in Arguments)
                values.Add((arg, ParseArgument(arg, match.Groups[arg.Name].Value)));

            return values.ToArray();
        }

        private object ParseArgument(Argument argument, string value) => ArgumentDataSet[argument.Type].ArgumentParser(value);
    }

    public struct Argument
    {
        public ArgumentType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        
        public Argument(ArgumentType type, string name, string description, bool isRequired)
        {
            Type = type;
            Name = name;
            Description = description;
            IsRequired = isRequired;
        }
    }

    public struct Flag
    {
        public ArgumentType Type { get; set; }
        public string ShortName{ get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }

        public Flag(ArgumentType type, string shortName, string fullName, string description)
        {
            Type = type;
            ShortName = shortName;
            FullName = fullName;
            Description = description;
        }
    }

    public enum ArgumentType
    {
        Int, Double, String, Bool, Color, Path
    }

    public struct ArgumentData
    {
        public string Pattern { get; set; }
        public Command.ArgumentParser ArgumentParser { get; set; }

        public ArgumentData(string pattern, Command.ArgumentParser argumentParser)
        {
            Pattern = pattern;
            ArgumentParser = argumentParser;
        }

        public void Deconstruct(out string pattern, out Command.ArgumentParser argumentParser)
        {
            pattern = Pattern;
            argumentParser = ArgumentParser;
        }
    }
}
