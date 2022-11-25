using CommandParser.Command;
using System.Text.RegularExpressions;

namespace ConsoleCMD.CommandResources
{
    public static class Arguments
    {
        public static ArgumentType Int = new ArgumentType()
        {
            Name = "Int",
            ArgumentParsingRegex = new Regex(@"\d+", RegexOptions.Compiled),
            Parse = (arg) => int.Parse(arg),
            Validator = (arg) => true
        };

        public static ArgumentType String = new ArgumentType()
        {
            Name = "Text",
            ArgumentParsingRegex = new Regex(@".+", RegexOptions.Compiled),
            Parse = (arg) => arg.ToString(),
            Validator = (arg) => true
        };
    }
}
