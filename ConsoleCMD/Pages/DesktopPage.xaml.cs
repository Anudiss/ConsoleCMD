using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConsoleCMD.Pages
{
    /// <summary>
    /// Логика взаимодействия для DesktopPage.xaml
    /// </summary>
    public partial class DesktopPage : Page
    {
        private List<Directory> _categoriesHierarchy = new List<Directory>();
        public List<Directory> CategoriesHierarchy
        {
            get { return _categoriesHierarchy; }
            set { _categoriesHierarchy = value; }
        }

        public DesktopPage()
        {
            InitializeComponent();
                
            CategoriesHierarchy.Add(
                new Directory("Корень", new Directory[]
                {
                    new Directory("Ветка 1", new Directory[]
                    {
                        new Directory("Подветка 1")
                    }),
                    new Directory("Ветка 2", new Directory[]
                    {
                        new Directory("Подветка 2")
                    }),
                    new Directory("Ветка 3", new Directory[]
                    {
                        new Directory("Подветка 3")
                    })
                })
            );

            TWCategories.Focus();

            //LBFileSystemObjects.SelectionMode = SelectionMode.Single;
        }

        //private void LBFileSystemObjects_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key != Key.Enter)
        //        return;

        //    var fileSystemObject = LBFileSystemObjects.SelectedItem as FileSystemObject;
        //    if (fileSystemObject is Directory)
        //    {
        //        var directory = fileSystemObject as Directory;
        //        if (directory.Children.Count > 0)
        //            CategoriesList = directory.Children;
        //    }
        //}

    }
}
