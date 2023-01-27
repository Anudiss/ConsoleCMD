using CommandParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysIO = System.IO;

namespace ConsoleCMD.FileSystem
{
    static class PhysicalManager
    {
        public static string WorkingDirectoryName { get; private set; }
        public static string FileSystemDirectoryName { get; private set; }
        public static string ApplicationsDirectoryName { get; private set; }

        public static string WorkingDirectoryPath { get; private set; }
        public static string FileSystemDirectoryPath { get; private set; }
        public static string ApplicationsDirectoryPath { get; private set; }

        public static void Initialize()
        {
            var userDocsDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Кажется, что это не нужно:
            // Properties.Settings.Default.UserDocumentsDirectoryPath = userDocsDirPath;

            WorkingDirectoryName = Properties.Settings.Default.WorkingDirectoryName;
            FileSystemDirectoryName = Properties.Settings.Default.FileSystemDirectoryName;
            ApplicationsDirectoryName = Properties.Settings.Default.ApplicationsDirectoryName;

            WorkingDirectoryPath = SysIO.Path.Combine(userDocsDirPath, WorkingDirectoryName);
            FileSystemDirectoryPath = SysIO.Path.Combine(WorkingDirectoryPath, FileSystemDirectoryName);
            ApplicationsDirectoryPath = SysIO.Path.Combine(WorkingDirectoryPath, ApplicationsDirectoryName);

            SysIO.Directory.CreateDirectory(WorkingDirectoryPath);
            SysIO.Directory.CreateDirectory(FileSystemDirectoryPath);
            SysIO.Directory.CreateDirectory(ApplicationsDirectoryPath);
        }

        public static IEnumerable<Path> GetAllPathsRelativeToDirectory(string directoryPath)
        {
            var paths = new List<Path>();

            var childPaths = SysIO.Directory.GetFileSystemEntries(directoryPath);
            childPaths.ForEach(path =>
            {
                PathKind pathKind;
                if (SysIO.File.Exists(path))
                    pathKind = PathKind.File;
                else
                    pathKind = PathKind.Directory;

                if (pathKind == PathKind.File)
                {
                    var fileExtension = SysIO.Path.GetExtension(path);
                    if (fileExtension != "")
                        path = path.Replace(fileExtension, fileExtension.ToLower());
                }

                var relativePath = path.Replace($"{FileSystemDirectoryPath}", "");
                paths.Add(new Path { Kind = pathKind, Value = relativePath });

                if (pathKind == PathKind.Directory)
                    paths.AddRange(GetAllPathsRelativeToDirectory(path));
            });

            return paths;
        }
    }
}
