using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Hosting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace ConsoleCMD.Applications
{
    public class Command
    {
        public enum ReturnCode
        {
            Special, Success, Error, InvalidArguments
        }

        public static readonly Dictionary<ArgumentType, ArgumentDataSetValue> ArgumentDataSet = new Dictionary<ArgumentType, ArgumentDataSetValue>()
        {
            { ArgumentType.Int, new ArgumentDataSetValue( @"(\d+)", (value) => int.Parse(value)) },
            { ArgumentType.Double, new ArgumentDataSetValue( @"(\d+)([,\.]\d+)?", (value) => double.Parse(value.Replace('.', ','))) },
            { ArgumentType.String, new ArgumentDataSetValue( @"(\S+)", (value) => value) },
            { ArgumentType.Bool, new ArgumentDataSetValue( @"([10]|(true)|(false)|(yes)|(no))|(y)|(n)", (value) => new[] { "1", "true", "y", "yes" }.Contains(value)) },
            { ArgumentType.Color, new ArgumentDataSetValue( $@"(?<Name>{string.Join("|", ConsoleComponent.SupportedColors)})|(#(?<HEX>[0-9A-Fa-f]{{6}}))", 
                                                    (value) => ColorConverter.ConvertFromString(value)) },
            { ArgumentType.Path, new ArgumentDataSetValue( @"(\d+)", (value) => value) }
        };

        public static readonly Regex CommandRegex = new Regex(@"\s*(?<command>\S+)\s*(?<args>.+)?\s*", RegexOptions.Compiled);
        public static readonly Regex FlagRegex = new Regex(@"\-{1,2}(?<flagName>\S+)", RegexOptions.Compiled);

        // Flags will be arguments, starting with -- or short -
        // There are ReturnCode.Special. We can do side actions when get this code

        public static List<Command> Commands = new List<Command>
        {
            new Command(
                    names: new[] { "bg", "set_bg", "set_background_color" },
                    description: "Устанавливает цвет фона консоли",
                    executor: (args) =>
                    {
                        ConsoleComponent.Instance.BackgroundColor = new SolidColorBrush((Color)args["Color"]);
                        return (ReturnCode.Success, "");
                    },
                    arguments: new[] { new Argument(ArgumentType.Color, "Color", "Background color", true) },
                    flags: null
                ),
            new Command(
                    names: new[] { "fg", "set_fg", "set_foreground_color" },
                    description: "Устанавливает цвет текста консоли",
                    executor: (args) =>
                    {
                        ConsoleComponent.Instance.ForegroundColor = new SolidColorBrush((Color)args["Color"]);
                        return (ReturnCode.Success, "");
                    },
                    arguments: new[] { new Argument(ArgumentType.Color, "Color", "Foreground color", true) },
                    flags: null
                )
        };

        public static bool IsCommandExist(string commandName) =>
            Commands.Any(command => command.Names.Contains(commandName));

        public static Command GetCommand(string commandName) =>
            Commands.First(command => command.Names.Contains(commandName));

        public delegate (ReturnCode, string) CommandExecutor(Arguments arguments);
        public delegate bool CommandArgsValidator(string[] args);
        public delegate object ArgumentParser(string value);

        public string[] Names { get; }
        public string Description { get; }
        public string Usage { get; }
        public Argument[] Arguments { get; } 
        public Flag[] Flags { get; }
        public Regex FullCommandPattern { get; }
        public Regex ArgumentsPattern { get; }
        public Regex FlagPattern { get; }
        public CommandExecutor Executor { get; }

        public Command(string[] names, string description, CommandExecutor executor, Argument[] arguments, Flag[] flags)
        {
            Names = names;
            Description = description;
            Arguments = arguments;
            Flags = flags;
            Usage = AssembleUsage(arguments, flags);
            FullCommandPattern = AssemblePattern(arguments);
            FlagPattern = AssembleFlags(flags);
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
            string pattern = $@"(?<CommandName>{string.Join("|", Names)})\s+";
            if (arguments == null)
                return new Regex(pattern, RegexOptions.Compiled);

            pattern += string.Join(@"\s+", arguments.Select(arg => $"(?<{arg.Name}>{ArgumentDataSet[arg.Type].Pattern})"));

            return new Regex(pattern, RegexOptions.Compiled);
        }

        private Regex AssembleFlags(Flag[] flags) => 
            new Regex(string.Join("|", flags.Select(flag => $@"(?<{flag.FullName}>\-{flag.ShortName}|\-{{2}}{flag.FullName})")));

        private bool ValidateCommand(string input) => FullCommandPattern.IsMatch(input);
        private bool ValidateCommand(string input, out Match match) => (match = FullCommandPattern.Match(input)).Success;

        public (ReturnCode, string) Execute(string input)
        {
            if (ValidateCommand(input) == false)
                return (ReturnCode.InvalidArguments, "Неверно введены аргументы");

            Arguments arguments = new Arguments()
            {
                Data = ParseArguments(input).ToArray()
            };
            return ((ReturnCode, string))(Executor?.Invoke(arguments));
        }

        private (Argument argument, object value)[] ParseArguments(string input)
        {
            Match match = FullCommandPattern.Match(input);
            var values = new List<(Argument argument, object value)>();

            foreach (var arg in Arguments)
                values.Add((arg, ParseArgument(arg, 
                                               match.Groups[arg.Name].Value))
                                               );

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

        public static bool operator ==(Argument arg1, Argument arg2)
        {
            return arg1.Type == arg2.Type &&
                   arg1.Name == arg2.Name &&
                   arg1.Description == arg2.Description &&
                   arg1.IsRequired == arg2.IsRequired;
        }

        public static bool operator !=(Argument arg1, Argument arg2)
        {
            return arg1.Type != arg2.Type ||
                   arg1.Name != arg2.Name ||
                   arg1.Description != arg2.Description ||
                   arg1.IsRequired != arg2.IsRequired;
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

    public struct ArgumentDataSetValue
    {
        public string Pattern { get; set; }
        public Command.ArgumentParser ArgumentParser { get; set; }

        public ArgumentDataSetValue(string pattern, Command.ArgumentParser argumentParser)
        {
            Pattern = pattern;
            ArgumentParser = argumentParser;
        }
    }

    public struct Arguments
    {
        public (Argument argument, object value)[] Data { get; set; }

        public Arguments(params (Argument, object)[] values) => Data = values;

        public object this[string argumentName] => Data.First(e => e.argument.Name == argumentName).value;
        public object this[Argument arg] => Data.First(e => e.argument == arg).value;
    }
}
