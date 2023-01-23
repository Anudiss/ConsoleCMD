using CommandParser;
using ConsoleCMD.Database;
using ConsoleCMD.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SysIO = System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;

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

            //MeasureFunctionExecutionTimes();

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

        private static void MeasureFunctionExecutionTimes()
        {
            var stopwatch = Stopwatch.StartNew();
            var physicalPaths = GetAllPathsInPhysicalDirectory(FileSystemDirectoryPath);
            stopwatch.Stop();
            MessageBox.Show($"Getting physical paths: {stopwatch.Elapsed.TotalSeconds}, paths count: {physicalPaths.Count()}");

            stopwatch = Stopwatch.StartNew();
            var dbPaths = GetAllPathsInDbDirectory(RootDirectory);
            stopwatch.Stop();
            MessageBox.Show($"Getting db paths: {stopwatch.Elapsed.TotalSeconds}, paths count: {dbPaths.Count()}");

            stopwatch = Stopwatch.StartNew();
            var dbPathsToDelete = dbPaths.AsParallel().Where(dbPath => physicalPaths.Contains(dbPath) == false);
            stopwatch.Stop();
            MessageBox.Show($"Getting db paths to delete: {stopwatch.Elapsed.TotalSeconds}, paths count: {dbPathsToDelete.Count()}");

            stopwatch = Stopwatch.StartNew();
            var dbPathsToCreate = physicalPaths.AsParallel().Where(physicalPath => dbPaths.Contains(physicalPath) == false);
            stopwatch.Stop();
            MessageBox.Show($"Getting db paths to create: {stopwatch.Elapsed.TotalSeconds}, paths count: {dbPathsToCreate.Count()}");

            stopwatch = Stopwatch.StartNew();
            dbPathsToDelete.ForEach(path => DeleteDbPathInParentDirectory(RootDirectory, path));
            stopwatch.Stop();
            MessageBox.Show($"Deleting db paths: {stopwatch.Elapsed.TotalSeconds}, paths deleted: {dbPathsToDelete.Count()}");

            stopwatch = Stopwatch.StartNew();
            dbPathsToCreate.ForEach(path => BuildDbPathInParentDirectory(RootDirectory, path));
            stopwatch.Stop();
            MessageBox.Show($"Creating db paths: {stopwatch.Elapsed.TotalSeconds}, paths created: {dbPathsToCreate.Count()}");
        }

        private static void ReflectPhysicalFileSystemStructureToDB()
        {
            var task1 = Task.Run(() => GetAllPathsInPhysicalDirectory(FileSystemDirectoryPath));
            var task2 = Task.Run(() => GetAllPathsInDbDirectory(RootDirectory));

            task1.Wait();
            task2.Wait();

            var physicalPaths = task1.Result;
            var dbPaths = task2.Result;

            var task3 = Task.Run(() =>
            {
                var dbPathsToDelete = dbPaths.Where(dbPath => physicalPaths.Contains(dbPath) == false);
                return dbPathsToDelete;
            });

            var task4 = Task.Run(() => {
                var dbPathsToCreate = physicalPaths.Where(physicalPath => dbPaths.Contains(physicalPath) == false);
                return dbPathsToCreate;
            });

            task3.Wait();
            task4.Wait();

            var dbPathsToDelete = task3.Result;
            var dbPathsToCreate = task4.Result;

            dbPathsToDelete.ForEach(path => DeleteDbPathInParentDirectory(RootDirectory, path));
            dbPathsToCreate.ForEach(path => BuildDbPathInParentDirectory(RootDirectory, path));
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

                var relativePath = path.Replace($"{FileSystemDirectoryPath}{SysIO.Path.DirectorySeparatorChar}", "");
                paths.Add(new Path { Kind = pathKind, Value = relativePath });

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

                path.Value = path.Value.Replace($"{FileSystemDirectoryName}{SysIO.Path.DirectorySeparatorChar}", "");
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

            Directory directoryToDelete;
            directoryToDelete = parentDirectory.SubDirectories
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

            var directory = parentDirectory.SubDirectories.FirstOrDefault(directory => directory.Name == headSlice);
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
