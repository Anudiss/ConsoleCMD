using CommandParser;
using ConsoleCMD.Database;
using ConsoleCMD.Database.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Windows.UI;
using SysIO = System.IO;

namespace ConsoleCMD.FileSystem
{
    public static class DatabaseManager
    {
        #region Properties

        public const string AbsolutePathBeginning = "~";

        public static readonly char PathSeparator = SysIO.Path.DirectorySeparatorChar;

        public static Directory RootDirectory { get; private set; }

        public static Directory CurrentDirectory
        {
            get => NavigationComponent.CurrentDirectory;
            set => NavigationComponent.CurrentDirectory = value;
        }

        #endregion Properties

        public static void Initialize()
        {
            PhysicalManager.Initialize();

            InitializeRootDirectory();

            ReflectPhysicalFileSystemStructureInDatabase();

            DatabaseContext.Entities.SaveChanges();
        }

        #region Interface methods

        

        #region Getting file system object, directory and file

        public static bool TryGetDirectory(string strPath, out Directory foundDirectory)
        {
            TryGetFileSystemObject(strPath, out var fileSystemObject);

            foundDirectory = null;
            if (fileSystemObject is Directory dir)
                foundDirectory = dir;

            return foundDirectory != null;

        }

        public static bool TryGetFile(string strPath, out File foundFile)
        {
            TryGetFile(strPath, out var fileSystemObject);

            foundFile = null;
            if (fileSystemObject is File file)
                foundFile = file;

            return foundFile != null;
        }

        public static bool TryGetFileSystemObject(string strPath, out IFileSystemObject foundFsObject)
        {
            var pathSlices = strPath.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            Directory currentDirectory;
            if (pathSlices.First() == AbsolutePathBeginning)
                currentDirectory = RootDirectory;
            else
                currentDirectory = CurrentDirectory;

            foundFsObject = null;
            foreach (var slice in pathSlices.Skip(1)) {
                if (slice == "..")
                {
                    if (currentDirectory.Parent == null)
                    {
                        foundFsObject = null;
                        return false;
                    }
                    currentDirectory = currentDirectory.Parent;
                    foundFsObject = currentDirectory;
                }
                else
                {
                    foundFsObject = currentDirectory.Children
                        .FirstOrDefault(child => child is Directory dir && dir.Name == slice
                            || child is File file && file.FullName == slice);
                    
                    if (foundFsObject == null)
                        return false;

                    if (foundFsObject is Directory dir)
                        currentDirectory = dir;
                }
            }

            return true;
        }

        #endregion Getting file system object, directory and file

        #endregion Interface methods

        #region Service methods

        private static void InitializeRootDirectory()
        {
            RootDirectory = DatabaseContext.Entities.Directories.Local
                .FirstOrDefault(d => d.ParentId == null);
            if (RootDirectory == null)
            {
                RootDirectory = new Directory
                {
                    Parent = null,
                    Name = PhysicalManager.FileSystemDirectoryName
                };
                DatabaseContext.Entities.Directories.Local.Add(RootDirectory);
            }
            else if (RootDirectory.Name != PhysicalManager.FileSystemDirectoryName)
            {
                RootDirectory.Name = PhysicalManager.FileSystemDirectoryName;
            }

            CurrentDirectory = RootDirectory;
        }

        private static void ReflectPhysicalFileSystemStructureInDatabase()
        {
            var task1 = Task.Run(() => PhysicalManager.GetAllPathsRelativeToDirectory(PhysicalManager.FileSystemDirectoryPath));
            var task2 = Task.Run(() => GetAllPathsRelativeToDirectory(RootDirectory));

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

            //something.SetProgressBarCurrentValue(50);

            //_loadingProgressWindow.ProgressBarVisibility = Visibility.Visible;

            //_loadingProgressWindow.ProgressBarTitle = "Удаление путей из БД.";
            //_loadingProgressWindow.ProgressBarMinimumValue = 0;
            //_loadingProgressWindow.ProgressBarMaximumValue = dbPathsToDelete.Count();
            //_loadingProgressWindow.ProgressBarCurrentValue = 0;
            dbPathsToDelete.ForEach(path =>
            {
                DeletePathRelativeToParentDirectory(RootDirectory, path);
                //_loadingProgressWindow.ProgressBarCurrentValue += 1;
            });

            //_loadingProgressWindow.ProgressBarTitle = "Создание путей в БД.";
            //_loadingProgressWindow.ProgressBarMinimumValue = 0;
            //_loadingProgressWindow.ProgressBarMaximumValue = dbPathsToCreate.Count();
            //_loadingProgressWindow.ProgressBarCurrentValue = 0;
            dbPathsToCreate.ForEach(path =>
            {
                CreatePathRelativeToParentDirectory(RootDirectory, path);
                //_loadingProgressWindow.ProgressBarCurrentValue += 1;
            });
        }

        private static IEnumerable<Path> GetAllPathsRelativeToDirectory(Directory directory)
        {
            var paths = new List<Path>();

            directory.Children.ForEach(@object =>
            {
                var path = GetPathFromDirectoryToFsObject(directory, @object);

                path.Value = path.Value.Replace($"{PhysicalManager.FileSystemDirectoryName}", "");
                paths.Add(path);

                if (@object is Directory childDir)
                {
                    var isEndDirectory = childDir.Children.Any() == false;
                    if (isEndDirectory)
                        return;

                    paths.AddRange(GetAllPathsRelativeToDirectory(childDir));
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
        private static Path GetPathFromDirectoryToFsObject(Directory parentDirectory, IFileSystemObject @object)
        {
            if (parentDirectory == @object)
                throw new ArgumentException("Cannot build path from the one fs object to the same fs object");

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

        private static void DeletePathRelativeToParentDirectory(Directory parentDirectory, Path path)
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
                DeletePathRelativeToParentDirectory(directoryToDelete, path);
        }

        private static void CreatePathRelativeToParentDirectory(Directory parentDirectory, Path path)
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

            CreatePathRelativeToParentDirectory(directory, path);
        }

        #endregion Service methods
    }
}
