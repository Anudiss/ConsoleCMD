using CommandParser.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
                        ArgumentType = Arguments.String,
                        IsRequired = true
                    }
                },
                CommandExecutor = (args, flags) =>
                {
                    MessageBox.Show($"{string.Join(", ", args.Select(arg => arg.Value))}");
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
                        ArgumentType = Arguments.Int,
                        IsRequired = false
                    }
                },
                CommandExecutor = (args, flags) =>
                {
                    MessageBox.Show($"{string.Join(", ", args.Select(arg => arg.Value))}");
                }
            }
        };
    }
}
