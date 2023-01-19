using CommandParser.Hints;
using System.Collections.Generic;
using System.Windows.Media;

namespace ConsoleCMD.Console
{
    public static class HintColorScheme
    {
        public static readonly Dictionary<HintPart, SolidColorBrush> ColorScheme = new Dictionary<HintPart, SolidColorBrush>()
        {
            { HintPart.CommandName, new SolidColorBrush(Colors.Black) },
            { HintPart.RequiredArgument, new SolidColorBrush(Colors.Blue) },
            { HintPart.NotRequiredArgument, new SolidColorBrush(Colors.LightBlue) },
            { HintPart.ArgumentType, new SolidColorBrush(Colors.IndianRed) }
        };
    }
}
