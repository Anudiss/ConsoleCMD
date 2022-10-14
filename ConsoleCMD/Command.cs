using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConsoleCMD.Applications
{
    public class Command
    {
        public static Dictionary<string[], Command> CommandNames = new Dictionary<string[], Command>
        {
            { new[] { "color", "col", "c" }, new Command(
                                                         description: "Меняет цвет, но ничего не делает",
                                                         usage: "color <color>",
                                                         executor: (args) => { MessageBox.Show(string.Join(", ", args)); },
                                                         validator: (args) => args?.Length == 1) }
        };

        public static Command GetCommand(string commandName)
        {
            if (CommandNames.All(keyPair => keyPair.Key.Contains(commandName) == false))
                return null;

            return CommandNames.First(keyPair => keyPair.Key.Contains(commandName)).Value;
        }
        public const string Pattern = @"\s*(?<command>\w+)\s*(?<args>.+)?";

        public delegate void CommandExecutor(string[] args);
        public delegate bool CommandValidator(string[] args);

        public string Description { get; }
        public string Usage { get; }
        public CommandExecutor Executor { get; }
        public CommandValidator Validator { get; }

        public Command(string description, string usage, CommandExecutor executor, CommandValidator validator)
        {
            Description = description;
            Usage = usage;
            Executor = executor;
            Validator = validator;
        }

        public void Execute(string[] args)
        {
            if (Validate(args) == false)
            {
                // ОШИБКА НАХУЙ
                throw new ArgumentException();
            }
            else
                Executor?.Invoke(args);
        }

        public bool Validate(string[] args) => Validator.Invoke(args);
    }
}
