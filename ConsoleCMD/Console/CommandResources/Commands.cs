using CommandParser.Command;
using ConsoleCMD.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Array = CommandParser.Command.Array;
using Tuple = CommandParser.Command.Tuple;

namespace ConsoleCMD.Console.CommandResources
{
    public static class Commands
    {
        public static readonly List<CommandEntity> CommandEntities = new()
        {
            new CommandEntity()
            {
                Synonyms = new[] { "cd" },
                Arguments = new[]
                {
                    new Argument()
                    {
                        Name = "DirectoryPath",
                        ArgumentType = Arguments.Path,
                        IsRequired = true
                    }
                },
                CommandExecutor = (args, flags) =>
                {
                    string path = args["DirectoryPath"] as string;
                    MessageBox.Show(path);
                    return true;
                }
            }
        };
    }
}
