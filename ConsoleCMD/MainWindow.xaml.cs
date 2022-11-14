using ConsoleCMD.Applications;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using System.Collections.Generic;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var categoryHierarchyRoot =
               new Directory("Корень", null, new Directory[]
               {
                    new Directory("Ветка 1", null, new Directory[]
                    {
                        new Directory("Подветка 1", null, new FileSystemObject[]
                        {
                            new Application("Приложение 1"),
                            new File("Файл 1"),

                        })
                    }),
                    new Directory("Ветка 2", null, new Directory[]
                    {
                        new Directory("Подветка 2")
                    }),
                    new Directory("Ветка 3", null, new Directory[]
                    {
                        new Directory("Подветка 3")
                    })
               });

            Navigation.DirectoryHierarchy.Add(
                CopyOnlyDirectoryHierarchy(categoryHierarchyRoot)
            );

            Navigation.SelectedEndNode += (directory) =>
            {
                var sourceDirectory = FindDirectoryByTitleInHierarchy(categoryHierarchyRoot, directory.Title);
                if (sourceDirectory == null)
                    return;
                Desktop.FileSystemObjects.Clear();
                sourceDirectory.Children.ForEach(fsobject => Desktop.FileSystemObjects.Add(fsobject));
            };
        }

        private Directory CopyOnlyDirectoryHierarchy(Directory root)
        {
            var rootCopy = new Directory();
            rootCopy.Title = root.Title;
            rootCopy.IconSource = root.IconSource;
            rootCopy.Children = new List<FileSystemObject>();
            root.Children.ForEach(fsobject => {
                if (!(fsobject is Directory directory))
                    return;
                rootCopy.Children.Add(
                    CopyOnlyDirectoryHierarchy(directory)
                );
            });
            return rootCopy;
        }

        private Directory FindDirectoryByTitleInHierarchy(Directory root, string title)
        {
            if (root.Title == title)
                return root;
            foreach (var fsobject in root.Children)
            {
                if (!(fsobject is Directory directory))
                    continue;
                var foundDirectory = FindDirectoryByTitleInHierarchy(directory, title);
                if (foundDirectory == null)
                    continue;
                return foundDirectory;
            }
            return null;
        }
    }
}
