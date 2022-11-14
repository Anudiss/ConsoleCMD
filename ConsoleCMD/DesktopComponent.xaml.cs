using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для DesktopComponent.xaml
    /// </summary>
    public partial class DesktopComponent : UserControl
    {
        public ObservableCollection<FileSystemObject> FileSystemObjects
        {
            get { return (ObservableCollection<FileSystemObject>)GetValue(FileSystemObjectsProperty); }
            private set { SetValue(FileSystemObjectsProperty, value); }
        }
        public static readonly DependencyProperty FileSystemObjectsProperty =
            DependencyProperty.Register("FileSystemObjects", typeof(ObservableCollection<FileSystemObject>), typeof(DesktopComponent), new PropertyMetadata(new ObservableCollection<FileSystemObject>()));

        public DesktopComponent()
        {
            InitializeComponent();

            FileSystemObjectList.SelectionMode = SelectionMode.Single;
        }

        private void FileSystemObjectList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            //var fileSystemObject = LBFileSystemObjects.SelectedItem as FileSystemObject;
            //if (fileSystemObject is Directory)
            //{
            //    var directory = fileSystemObject as Directory;
            //    if (directory.Children.Count > 0)
            //        CategoriesList = directory.Children;
            //}
        }
    }
}
