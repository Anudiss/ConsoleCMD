using CommandParser.Command;
using ConsoleCMD.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Array = CommandParser.Command.Array;
using Tuple = CommandParser.Command.TupleType;
using ConsoleCMD.Database;

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

                    if (DatabaseManager.TryGetDirectory(path, out var directory))
                        NavigationComponent.CurrentDirectory = directory;

                    return true;
                }
            }
        };
    }
}
