using ConsoleCMD.Database.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для DesktopComponent.xaml
    /// </summary>
    public partial class DesktopComponent : UserControl
    {
        public ObservableCollection<File> FileSystemObjects
        {
            get { return (ObservableCollection<File>)GetValue(FileSystemObjectsProperty); }
            private set { SetValue(FileSystemObjectsProperty, value); }
        }
        public static readonly DependencyProperty FileSystemObjectsProperty =
            DependencyProperty.Register("FileSystemObjects", typeof(ObservableCollection<File>), typeof(DesktopComponent), new PropertyMetadata(new ObservableCollection<File>()));

        public DesktopComponent()
        {
            InitializeComponent();

            FileSystemObjectList.SelectionMode = SelectionMode.Single;
        }

        private void FileSystemObjectList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
        }
    }
}
