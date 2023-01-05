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

            var rootCategory = new Directory("Корень", null, new Directory[] {
                new Directory("Ветвь 1", null, new Directory[]
                {
                    new Directory("Подветвь 1", null, new Directory[]
                    {
                        new Directory("Подподветвь 1", null, new FileSystemObject[]
                        {
                            new Application("Приложение 1"),
                            new File("Файл 1")
                        }),
                        new Directory("Подподветвь 2", null, new Directory[]
                        {

                        }),
                        new Directory("Подподветвь 3", null, new Directory[]
                        {

                        }),
                    }),
                    new Directory("Подветвь 2", null, new FileSystemObject[]
                    {
                        new Application("Приложение 1"),
                        new File("Файл 1")
                    }),
                    new Directory("Подветвь 3", null, new Directory[]
                    {

                    }),
                }),
                new Directory("Ветвь 2", null, new Directory[]
                {
                }),
                new Directory("Ветвь 3", null, new Directory[]
                {
                }),
            });

            Navigation.RootNode = NodeFromDirectory(rootCategory);

            Navigation.SelectedEndNode += selectedNode =>
            {
                var category = FindAmongChildren(rootCategory, selectedNode.Title);
                
                Desktop.FileSystemObjects.Clear();
                foreach (var fsobject in category.Children)
                    Desktop.FileSystemObjects.Add(fsobject);
            };
        }

        private Node NodeFromDirectory(Directory dir)
        {
            var node = new Node
            {
                Title = dir.Title,
                IconSource = dir.IconSource
            };

            var nodeChildren = new List<Node>(dir.Children.Length);
            
            foreach(var fsobject in dir.Children)
                if (fsobject is Directory childDir)
                    nodeChildren.Add(NodeFromDirectory(childDir));
            
            node.Children = nodeChildren.ToArray();
            
            return node;
        }

        private Directory FindAmongChildren(Directory root, string title)
        {
            if (root.Title == title)
                return root;
            foreach (var fsobject in root.Children)
            {
                if (!(fsobject is Directory directory))
                    continue;
                var foundDirectory = FindAmongChildren(directory, title);
                if (foundDirectory == null)
                    continue;
                return foundDirectory;
            }
            return null;
        }
    }
}
