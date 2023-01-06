using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Directory.xaml
    /// </summary>
    public partial class Directory : FileSystemObject
    {
        private FileSystemObject[] _children;
        public FileSystemObject[] Children
        {
            get { return _children; }
            set { _children = value; }
        }
        
        public Directory(string title = "", ImageSource iconSource = null, FileSystemObject[] children = null)
        {
            Title = title;
            IconSource = iconSource ?? new BitmapImage(new Uri("/Resources/Icons/directory.png", UriKind.Relative));
            _children = children ?? new FileSystemObject[0];
        }
    }
}