using ConsoleCMD.FileSystem;
using ConsoleCMD.Resources.Connection;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

            Directory rootCategory = FileSystemManager.Root;

            Navigation.RootNode = NodeFromDirectory(rootCategory);

            Navigation.SelectionChanged += selectedNode =>
            {
                var category = FindAmongChildren(rootCategory, selectedNode.Title);
                
                Desktop.FileSystemObjects.Clear();
                foreach (var fsobject in category.Children)
                {
                    if (fsobject is File)
                        Desktop.FileSystemObjects.Add(fsobject);
                }
            };
        }

        private Node NodeFromDirectory(Directory dir)
        {
            var node = new Node(dir.Name, dir.IconOrDefault);

            var nodeChildren = new List<Node>(dir.SubDirectories.Count());
            
            foreach(var childDir in dir.SubDirectories)
                nodeChildren.Add(NodeFromDirectory(childDir));
            
            node.Children = nodeChildren.ToArray();
            
            return node;
        }

        private Directory FindAmongChildren(Directory root, string name)
        {
            if (root.Name == name)
                return root;
            foreach (var fsobject in root.SubDirectories)
            {
                if (!(fsobject is Directory directory))
                    continue;
                var foundDirectory = FindAmongChildren(directory, name);
                if (foundDirectory == null)
                    continue;
                return foundDirectory;
            }
            return null;
        }

        private void Console_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
