using CommandParser;
using ConsoleCMD.Resources.Connection;
using ConsoleCMD.Resources.Connection.PartialClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using SysIO = System.IO;

namespace ConsoleCMD
{
    public static class FileSystemManager
    {
        public static string WorkingDirectoryName { get; private set; }
        public static string FileSystemDirectoryName { get; private set; }
        public static string ApplicationsDirectoryName { get; private set; }

        public static string WorkingDirectoryPath { get; private set; }
        public static string FileSystemDirectoryPath { get; private set; }
        public static string ApplicationsDirectoryPath { get; private set; }

        public static Directory RootDirectory { get; private set; }
        public static Directory CurrentDirectory
        {
            get => NavigationComponent.CurrentDirectory;
            set => NavigationComponent.CurrentDirectory = value;
        }

        public static void InitializeFileSystem()
        {
            Initialize_Physical_WorkingDirectoryStructure();

            Initialize_DB_RootDirectory();

            ReflectPhysicalFileSystemStructureToDB();

            DatabaseContext.Entities.SaveChanges();
        }
        
        private static void Initialize_Physical_WorkingDirectoryStructure()
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

        private static void Initialize_DB_RootDirectory()
        {
            RootDirectory = DatabaseContext.Entities.Directories.Local.FirstOrDefault(d => d.ParentId == null);
            if (RootDirectory == null)
            {
                RootDirectory = new Directory
                {
                    Parent = null,
                    Name = FileSystemDirectoryName
                };
                DatabaseContext.Entities.Directories.Local.Add(RootDirectory);
            }
            else if (RootDirectory.Name != FileSystemDirectoryName)
            {
                RootDirectory.Name = FileSystemDirectoryName;
            }

            CurrentDirectory = RootDirectory;
        }

        private static void ReflectPhysicalFileSystemStructureToDB()
        {
            var physicalEntryPaths = GetAllEntryPathsFromPhysicalDirectory(FileSystemDirectoryPath);
            //MessageBox.Show(string.Join("\n", physicalEntryPaths), "Физические пути");

            var relativePhysicalEntryPaths = physicalEntryPaths.Select(entryPath =>
                entryPath.Replace($"{FileSystemDirectoryPath}\\", "")
            );
            //MessageBox.Show(string.Join("\n", relativePhysicalEntryPaths), "Относительные физические пути");

            var dbEntryPaths = GetAllEntryPathsFromDBDirectory(RootDirectory);
            //MessageBox.Show(string.Join("\n", dbEntryPaths), "Пути из БД");

            dbEntryPaths.ForEach(dbEntryPath =>
            {
                // Если есть путь в БД, но его нет физически, удалить его из БД
                if (relativePhysicalEntryPaths.Contains(dbEntryPath) == false)
                {
                    var entries = SplitPathToEntries(dbEntryPath);

                    DeleteFileSystemObjectByPathEntriesFromDB(RootDirectory, entries);
                }
            });
            //MessageBox.Show(string.Join("\n", GetAllEntryPathsFromDBDirectory(RootDirectory)), "Пути в БД, после удаления из неё записей, которых нет физически");


            relativePhysicalEntryPaths.ForEach(physicalEntryPath =>
            {
                // Если есть физический путь, но он не отражён в БД, отразить его в БД
                if (dbEntryPaths.Contains(physicalEntryPath) == false)
                {
                    var entries = SplitPathToEntries(physicalEntryPath);

                    WriteFileSystemObjectByPathEntriesToDB(RootDirectory, entries);
                }
            });
            //MessageBox.Show(string.Join("\n", GetAllEntryPathsFromDBDirectory(RootDirectory)), "Пути в БД, после добавления недостающих физических записей");
        }

        private static IEnumerable<string> SplitPathToEntries(string path)
            => path.Split(new char[] { SysIO.Path.DirectorySeparatorChar },
                    StringSplitOptions.RemoveEmptyEntries);

        private static IEnumerable<string> GetAllEntryPathsFromPhysicalDirectory(string dirPath)
        {
            var entryPaths = new List<string>();
            foreach (var entry in SysIO.Directory.GetFileSystemEntries(dirPath))
            {
                var entryPath = SysIO.Path.Combine(dirPath, entry);
                entryPaths.Add(entryPath);

                // if it is directory
                if (SysIO.Directory.Exists(entryPath))
                    entryPaths.AddRange(GetAllEntryPathsFromPhysicalDirectory(entryPath));
            }
            return entryPaths;
        }

        private static IEnumerable<string> GetAllEntryPathsFromDBDirectory(Directory dir)
        {
            var paths = new List<string>();

            DatabaseContext.Entities.Files.Local.ForEach(f =>
            {
                paths.Add(GetPathFromDirectoryToFileSystemObject(dir, f));
            });

            var endDirectories = DatabaseContext.Entities.Directories.Local.Where(d => d.Children.Count() == 0);
            endDirectories.ForEach(d => {
                if (dir != d)
                    paths.Add(GetPathFromDirectoryToFileSystemObject(dir, d));
            });

            return paths;
        }

        private static string GetPathFromDirectoryToFileSystemObject(Directory fromDir, IFileSystemObject fsObj)
        {
            if (fromDir == fsObj)
            {
                // TODO:
                // Возможно, здесь стоит вызвать исключение
                return "";
            }

            string name = "";
            Directory parent = null;
            
            if (fsObj is File file)
            {
                name = file.FullName;
                parent = file.Parent;
            }
            else if (fsObj is Directory dir)
            {
                name = dir.Name;
                parent = dir.Parent;
            }

            var path = name;
            while (parent != null && parent != fromDir)
            {
                path = SysIO.Path.Combine(parent.Name, path);
                parent = parent.Parent;
            }
            
            return path;
        }

        private static void DeleteFileSystemObjectByPathEntriesFromDB(Directory parentDir, IEnumerable<string> entries)
        {
            var childEntry = entries.First();

            if (childEntry.Contains('.'))
            {
                // File
                string fileName = childEntry.Split('.')[0],
                       fileExtension = childEntry.Split('.')[1];

                var file = DatabaseContext.Entities.Files.Local.First(f => f.ParentId == parentDir.Id
                                                                     && f.Name == fileName
                                                                     && f.Extension.Name == fileExtension);
                DatabaseContext.Entities.Files.Local.Remove(file);

                // Завершить, поскольку, если это файл - это всегда конечная запись в массиве записей
                return;
            }
            else
            {
                // Directory

                Directory childDir = parentDir.SubDirectories.First(d => d.Name == childEntry);

                // Директорию нужно удалить, только если это конечная директория
                if (entries.Count() == 1)
                {
                    DatabaseContext.Entities.Directories.Local.Remove(childDir);
                    return;
                }

                DeleteFileSystemObjectByPathEntriesFromDB(childDir, entries.Skip(1));
            }
        }
        
        private static void WriteFileSystemObjectByPathEntriesToDB(Directory parentDir, IEnumerable<string> entries)
        {
            var childEntry = entries.First();
            if (childEntry.Contains('.'))
            {
                // File

                string fileName = childEntry.Split('.')[0],
                       fileExtension = childEntry.Split('.')[1];

                var extension = DatabaseContext.Entities.Extensions.Local.FirstOrDefault(e => e.Name == fileExtension);
                if (extension == null)
                {
                    extension = new Extension
                    {
                        Name = fileExtension,
                        Icon = null
                    };
                    DatabaseContext.Entities.Extensions.Local.Add(extension);
                }

                DatabaseContext.Entities.Files.Local.Add(
                    new File
                    {
                        Parent = parentDir,
                        Name = fileName,
                        Extension = extension
                    }
                );

                // Завершить, поскольку, если это файл - это всегда конечная запись в массиве записей
                return;
            }
            else
            {
                // Directory

                Directory childDir = parentDir.SubDirectories.FirstOrDefault(d => d.Name == childEntry);
                if (childDir == null)
                {
                    childDir = new Directory
                    {
                        Parent = parentDir,
                        Name = childEntry
                    };
                    DatabaseContext.Entities.Directories.Local.Add(childDir);
                }

                if (entries.Count() > 1)
                {
                    WriteFileSystemObjectByPathEntriesToDB(childDir, entries.Skip(1));
                }
            }
        }

        public static void OpenDirectory(string name)
        {

        }
    }
}
