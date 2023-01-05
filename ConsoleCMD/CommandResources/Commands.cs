using CommandParser.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Array = CommandParser.Command.Array;
using Tuple = CommandParser.Command.Tuple;

namespace ConsoleCMD.CommandResources
{
    public static class Commands
    {
        public static List<CommandEntity> CommandEntities = new List<CommandEntity>()
        {
            new CommandEntity()
            {
                Synonyms = new[] { "print" },
                Arguments = new[]
                {
                    new Argument()
                    {
                        Name = "text",
                        ArgumentType = BaseArgumentTypes.String,
                        IsRequired = true
                    }
                },
                CommandExecutor = (args, flags) =>
                {
                    MessageBox.Show($"print: {string.Join(", ", args.Select(arg => arg.Value))}");
                    return null;
                }
            },
            new CommandEntity()
            {
                Synonyms = new[] { "print123" },
                Arguments = new[]
                {
                    new Argument()
                    {
                        Name = "Da",
                        ArgumentType = new Array(BaseArgumentTypes.Int)
                        {
                            Name = "IntArray"
                        }
                    }
                },
                CommandExecutor = (args, flags) =>
                {
                    MessageBox.Show($"print123: {string.Join(", ", args.Select(arg => arg.Value))}");
                    return null;
                }
            }
        };
    }
}
