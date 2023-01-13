using ConsoleCMD.Resources.Connection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для Directory.xaml
    /// </summary>
    public partial class Directory : FileSystemObject
    {
        private IEnumerable<FileSystemObject> _children;
        public IEnumerable<FileSystemObject> Children
        {
            get { return _children; }
            set { _children = value; }
        }
        
        public Directory(string title = "", byte[] iconSource = null, IEnumerable<FileSystemObject> children = null)
        {
            Title = title;
            IconSource = iconSource ?? Icons.DirectoryDefaultIcon;
            _children = children ?? Array.Empty<FileSystemObject>();
        }

        public static Directory FromDatabaseObject(Resources.Connection.Directory directory) =>
            new Directory(directory.Name, directory.Icon?.Data, directory.Directory1.Select(d => (FileSystemObject)FromDatabaseObject(d))
                                                               .Concat(directory.File.Select(f => File.FromDatabaseObject(f))));

        public static implicit operator Directory(Resources.Connection.Directory directory) =>
            FromDatabaseObject(directory);
    }
}