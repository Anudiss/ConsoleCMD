using System;
using System.Text.RegularExpressions;

namespace DirectoryParsing
{
    public class Program
    {

        public static readonly string   Separator            = @"\\\/";
        public static readonly string   NonAcceptableSymbols = $@"{Separator}'\<\>\|\:\?\u0022";
        public static readonly string[] BeginningSymbols     = new[] { @$"\~[{Separator}]", @$"\.{{1,2}}[{Separator}]" };

        public static readonly string   BeginningRegex   = $@"(?<Beginning>(?:{BeginningSymbols[1]})?(?:{BeginningSymbols[1]})*)";
        public static readonly string   DirectoriesRegex = $@"(?:(?<Path>[^{NonAcceptableSymbols}]+)[{Separator}])*";
        public static readonly string   DestinationRegex = $@"(?<Destination>[^{NonAcceptableSymbols}]+)";

        public static readonly Regex PathRegex = new($@"^{BeginningRegex}{DirectoriesRegex}{DestinationRegex}$", RegexOptions.Compiled);

        public static void Main(string[] args)
        {

        }

        public static bool TryParsePath(string strPath)
        {
            return true;
        }
    }

    public abstract class FileSystemObject
    {
        public string Name { get; set; } = default!;
        public Directory Parent { get; set; } = default!;
    }

    public class Directory : FileSystemObject
    {
        public IEnumerable<Directory> Directories { get; set; } = default!;
        public IEnumerable<File> Files { get; set; } = default!;
        public IEnumerable<FileSystemObject> Children = default!;
    }

    public class File : FileSystemObject
    {
        public string Extension { get; set; } = default!;
    }
}