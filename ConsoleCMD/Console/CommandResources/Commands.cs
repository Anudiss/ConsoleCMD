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
            },
            //new CommandEntity()
            //{
            //    Synonyms = new[] { "start_application" },
            //    Arguments = new[]
            //    {
            //        new Argument()
            //        {
            //            Name = "applicationName",
            //            ArgumentType = BaseArgumentTypes.String,
            //            IsRequired = true
            //        }
            //    },
            //    CommandExecutor = (args, flags) =>
            //    {
            //        var appName = args["applicationName"] as string;

            //        if (App.ApplicationManager.IsApplicationExist(appName) == false)
            //            return "Ошибка: приложение с таким названием не установлено";

            //        App.ApplicationManager.StartApplication(appName);

            //        return null;
            //    }
            //},
            //new CommandEntity()
            //{
            //    Synonyms = new[] { "stop_application" },
            //    Arguments = new[]
            //    {
            //        new Argument()
            //        {
            //            Name = "applicationName",
            //            ArgumentType = BaseArgumentTypes.String,
            //            IsRequired = true
            //        }
            //    },
            //    CommandExecutor = (args, flags) =>
            //    {
            //        var appName = args["applicationName"] as string;

            //        if (App.ApplicationManager.IsApplicationExist(appName) == false)
            //            return "Ошибка: приложение с таким названием не установлено";
            //        else if (App.ApplicationManager.IsApplicationRunning(appName) == false)
            //            return "Ошибка: приложение не запущено";
                    
            //        App.ApplicationManager.StopApplication(appName);
                    
            //        return null;
            //    }
            //}
        };
    }
}
