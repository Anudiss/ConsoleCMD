using CommandParser;
using ConsoleCMD.Database;
using ConsoleCMD.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SysIO = System.IO;

namespace ConsoleCMD.FileSystem
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
            InitializePhysicalWorkingDirectoryStructure();

            InitializeDbRootDirectory();

            ReflectPhysicalFileSystemStructureToDB();
            
            DatabaseContext.Entities.SaveChanges();
        }

        private static void InitializePhysicalWorkingDirectoryStructure()
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

        private static void InitializeDbRootDirectory()
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
            var physicalPaths = GetAllPathsInPhysicalDirectory(FileSystemDirectoryPath);
            var relativePhysicalPaths = physicalPaths.Select(path =>
            {
                path.Value = path.Value.Replace($"{FileSystemDirectoryPath}", "");
                return path;
            });
            //SysIO.File.WriteAllText(@"C:\Users\Ильназ\Documents\ToyOS\Физические пути.txt",
            //    string.Join("\n", relativePhysicalPaths.Select(p => p.Value)));

            var dbPaths = GetAllPathsInDbDirectory(RootDirectory);
            var relativeDbPaths = dbPaths.Select(path =>
            {
                path.Value = path.Value.Replace($@"{FileSystemDirectoryName}\", "");
                return path;
            });
            //SysIO.File.WriteAllText(@"C:\Users\Ильназ\Documents\ToyOS\Пути в БД.txt",
            //    string.Join("\n", relativeDbPaths.Select(p => p.Value)));

            var dbPathsToDelete = relativeDbPaths.Where(dbPath => relativePhysicalPaths.Contains(dbPath) == false);
            //SysIO.File.WriteAllText(@"C:\Users\Ильназ\Documents\ToyOS\Пути для удаления из БД.txt",
            //    string.Join("\n", dbPathsToDelete.Select(p => p.Value)));
            dbPathsToDelete.ForEach(path => DeleteDbPathInParentDirectory(RootDirectory, path));

            // Нужно для отладки
            //DatabaseContext.Entities.SaveChanges();

            var dbPathsAfterDeletion = GetAllPathsInDbDirectory(RootDirectory);
            var relativeDbPathsAfterDeletion = dbPathsAfterDeletion.Select(path =>
            {
                path.Value = path.Value.Replace($@"{FileSystemDirectoryName}\", "");
                return path;
            });
            //SysIO.File.WriteAllText(@"C:\Users\Ильназ\Documents\ToyOS\Пути в БД после удаления.txt",
            //    string.Join("\n", relativeDbPathsAfterDeletion.Select(p => p.Value)));

            var dbPathsToCreate = relativePhysicalPaths.Where(physicalPath => relativeDbPaths.Contains(physicalPath) == false);
            //SysIO.File.WriteAllText(@"C:\Users\Ильназ\Documents\ToyOS\Пути для добавления в БД.txt",
            //    string.Join("\n", dbPathsToCreate.Select(p => p.Value)));
            dbPathsToCreate.ForEach(path => BuildDbPathInParentDirectory(RootDirectory, path));

            // Нужно для отладки
            //DatabaseContext.Entities.SaveChanges();

            var dbPathsAfterCreation = GetAllPathsInDbDirectory(RootDirectory);
            var relativeDbPathsAfterCreation = dbPathsAfterCreation.Select(path =>
            {
                path.Value = path.Value.Replace($@"{FileSystemDirectoryName}\", "");
                return path;
            });
            //SysIO.File.WriteAllText(@"C:\Users\Ильназ\Documents\ToyOS\Пути в БД после добавления.txt",
            //    string.Join("\n", relativeDbPathsAfterCreation.Select(p => p.Value)));
        }

        private static IEnumerable<Path> GetAllPathsInPhysicalDirectory(string directoryPath)
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

                paths.Add(new Path { Kind = pathKind, Value = path });

                if (pathKind == PathKind.Directory)
                    paths.AddRange(GetAllPathsInPhysicalDirectory(path));
            });

            return paths;
        }

        private static IEnumerable<Path> GetAllPathsInDbDirectory(Directory baseDirectory)
        {
            var paths = new List<Path>();

            baseDirectory.Children.ForEach(@object =>
            {
                var path = PathFromDbDirectoryToObject(baseDirectory, @object);

                paths.Add(path);

                if (@object is Directory directory)
                {
                    var isEndDirectory = directory.Children.Any() == false;
                    if (isEndDirectory)
                        return;
                    
                    paths.AddRange(GetAllPathsInDbDirectory(directory));
                }
            });

            return paths;
        }

        /// <summary>
        /// Вычисляет путь от директории до объекта,
        /// Ограничения:
        ///  fromDir и obj не должны быть одним и тем же объектом
        ///  fromDir должен находиться выше в иерархии, чем obj
        /// </summary>
        /// <param name="parentDirectory">Родительская директория</param>
        /// <param name="object">Дочерний объект в иерархии от родителя</param>
        /// <returns>Путь от директории до объекта</returns>
        private static Path PathFromDbDirectoryToObject(Directory parentDirectory, IFileSystemObject @object)
        {
            if (parentDirectory == @object)
                throw new ArgumentException("Cannot build path from the object to the same object");

            PathKind pathKind;
            string objectName;
            Directory objectParent;

            if (@object is File file)
            {
                pathKind = PathKind.File;
                objectName = file.FullName;
                objectParent = file.Parent;
            }
            else
            {
                var dir = @object as Directory;

                pathKind = PathKind.Directory;
                objectName = dir.Name;
                objectParent = dir.Parent;
            }

            var path = objectName;
            while (objectParent != null)
            {
                path = SysIO.Path.Combine(objectParent.Name, path);

                objectParent = objectParent.Parent;
                if (objectParent == parentDirectory)
                    break;
            }

            return new Path { Value = path, Kind = pathKind };
        }


        private static void DeleteDbPathInParentDirectory(Directory parentDirectory, Path path)
        {
            var headSlice = path.Slices.First();

            PathKind sliceKind = path.Kind == PathKind.File && path.Slices.Count() == 1 ? PathKind.File : PathKind.Directory;

            if (sliceKind == PathKind.File)
            {
                string fileName = SysIO.Path.GetFileNameWithoutExtension(headSlice),
                       fileExtension = SysIO.Path.GetExtension(headSlice);

                fileExtension = fileExtension != string.Empty ? fileExtension[1..] : "";

                var fileToDelete = parentDirectory.Files
                    .First(file => file.Parent == parentDirectory && file.FullName == headSlice);

                DatabaseContext.Entities.Files.Local.Remove(fileToDelete);

                return;
            }

            var directoryToDelete = parentDirectory.SubDirectories
                .FirstOrDefault(directory => directory.Name == headSlice);

            if (directoryToDelete == null)
                return;

            path.Slices = path.Slices.Skip(1);

            var isLastSlice = path.Slices.Any() == false;
            if (isLastSlice)
                DatabaseContext.Entities.Directories.Local.Remove(directoryToDelete);
            else
                DeleteDbPathInParentDirectory(directoryToDelete, path);
        }

        private static void BuildDbPathInParentDirectory(Directory parentDirectory, Path path)
        {
            var headSlice = path.Slices.First();

            PathKind sliceKind = path.Kind == PathKind.File && path.Slices.Count() == 1 ? PathKind.File : PathKind.Directory;

            if (sliceKind == PathKind.File)
            {
                string fileName = SysIO.Path.GetFileNameWithoutExtension(headSlice),
                       fileExtension = SysIO.Path.GetExtension(headSlice);

                fileExtension = fileExtension != string.Empty ? fileExtension[1..] : "";

                Extension extension = null;
                if (fileExtension != null)
                {
                    extension = DatabaseContext.Entities.Extensions.Local.FirstOrDefault(e => e.Name == fileExtension);
                    if (extension == null)
                    {
                        extension = new Extension { Name = fileExtension.ToLower(), Icon = null };
                        DatabaseContext.Entities.Extensions.Local.Add(extension);
                    }
                }

                DatabaseContext.Entities.Files.Local
                    .Add(new File { Parent = parentDirectory, Name = fileName, Extension = extension });

                return;
            }

            Directory directory = parentDirectory.SubDirectories.FirstOrDefault(directory => directory.Name == headSlice);
            if (directory == null)
            {
                directory = new Directory { Parent = parentDirectory, Name = headSlice };
                DatabaseContext.Entities.Directories.Local.Add(directory);
            }

            path.Slices = path.Slices.Skip(1);

            var isLastSlice = path.Slices.Any() == false;
            if (isLastSlice)
                return;

            BuildDbPathInParentDirectory(directory, path);
        }
    }
}
