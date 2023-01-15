using ConsoleCMD.Resources.Connection;
using System.Linq;
using SysIO = System.IO;

namespace ConsoleCMD.FileSystem
{
    public static class FileSystemManager
    {
        public static Directory Root { get; }

        static FileSystemManager() =>
            Root = DatabaseContext.Entities.Directories.Local.First(directory => directory.Parent == null);
        
        //public static void BuildFileSystemTree()
        //{
        //    var mainDir = Properties.Settings.Default.MainDirectory;

        //    var fileSystemRootDirPath = Path.Combine(mainDir, "FileSystemRoot");

        //    Walk(fileSystemRootDirPath, null);
        //}

        //public static void Walk(string dirPath, int? parentDirID)
        //{
        //    if (IsDirectory(dirPath))
        //    {
        //        // This is dir
        //        var dirName = Path.GetDirectoryName(dirPath);

        //        foreach (var entry in System.IO.Directory.GetFileSystemEntries(dirPath))
        //        {
        //            var entryPath = Path.Combine(dirPath, entry);
        //            Walk(entryPath, )
        //        }
        //        // Check: is dir exist in DB ?
        //        //if (DatabaseContext.Entities.Directory.Any(f => f.Name == dirName))
        //        //{
        //        //    DatabaseContext.Entities.Directory.Add(
        //        //        new Resources.Connection.Directory
        //        //        {
        //        //            Parent =
        //        //            Name = dirName
        //        //        }
        //        //    );
        //        //}
        //    }
        //    else
        //    {
        //        // This is file
        //        var fileName = entry;
        //        // Check: is file exist in DB?

        //        if (DatabaseContext.Entities.File.Any(f => f.Name == fileName))
        //        {
        //            DatabaseContext.Entities.File.Add(
        //                new Resources.Connection.File
        //                {
        //                    Name = fileName,
        //                    Extension = Path.GetExtension(fileName),
        //                    Directory =

        //                }
        //            );
        //        }
        //    }

            
        //}

        //public static void Walk(string dirPath, int? parentDirID)
        //{
        //    if(IsDirectory(dirPath))
        //    {
        //        if (parentDirID == null)
        //        {
        //            // This is root directory
        //            foreach (var entry in System.IO.Directory.GetFileSystemEntries(dirPath))
        //            {
        //                if (entry.)
        //            }
        //        }

        //        // Directory
        //        var dirName = Path.GetDirectoryName(dirPath);

        //        var isDirExist = DatabaseContext.Entities.Directory.Any(d => d.Name == dirName);

        //        if (isDirExist == false)
        //        {
        //            var parentDirName = System.IO.Directory.GetParent(dirPath).Name;

        //            var newDir = new Resources.Connection.Directory
        //            {
        //                Parent = parentDirID,
        //                Name = dirName
        //            };

        //            DatabaseContext.Entities.Directory.Add(newDir);
        //        }
        //    }
        //    else
        //    {
        //        // File
        //    }
        //}
/*
        public static bool IsMainDirectoryExist()
        {
            
        }

        public static void ValidateMainDirectoryByPath()
        {

        }

        public void F()
        {
            if (Properties.Settings.Default.MainDirectoryPath != null)
            {
                var fileSystemDirectoryPath
                    = Path.Combine(Properties.Settings.Default.MainDirectoryPath,
                        Properties.Settings.Default.FileSystemDirectoryName);

                var applicationsSystemDirectoryPath
                    = Path.Combine(Properties.Settings.Default.MainDirectoryPath,
                        Properties.Settings.Default.ApplicationsDirectoryName);

                SysIO.Directory.CreateDirectory(Properties.Settings.Default.MainDirectoryPath);
                SysIO.Directory.CreateDirectory(fileSystemDirectoryPath);
                SysIO.Directory.CreateDirectory(applicationsSystemDirectoryPath);
            }
            else
            {
                // создание папок
            }
        }

*/

        public static bool IsDirectory(string path)
            => (SysIO.File.GetAttributes(path) & SysIO.FileAttributes.Directory) == SysIO.FileAttributes.Directory;
    
    }
}
