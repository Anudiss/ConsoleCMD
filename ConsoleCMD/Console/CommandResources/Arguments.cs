using CommandParser.Command;
using System.Text.RegularExpressions;

namespace ConsoleCMD.Console.CommandResources
{
    public static partial class Arguments
    {
        public static readonly ArgumentType Path = new()
        {
            Name = "Path",
            ArgumentParsingRegex = PathRegex(),
            Parse = (input) => input.Trim()
        };

        [GeneratedRegex(@"^(?:(?:(?:\~)[\\\/])?(?:(?:\.{1,2})[\\\/])*)(?:(?:[^\\\/\'\<\>\|\:\?\u0022]+)[\\\/])*(?:[^\\\/\'\<\>\|\:\?\u0022]+)$", RegexOptions.Compiled)]
        private static partial Regex PathRegex();
    }
}
