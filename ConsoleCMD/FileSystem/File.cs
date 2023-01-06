using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для File.xaml
    /// </summary>
    public partial class File : FileSystemObject
    {
        public File(string title = "", ImageSource iconSource = null)
        {
            Title = title;
            IconSource = iconSource ?? new BitmapImage(new Uri("/Resources/Icons/Files/default.png", UriKind.Relative));
        }
    }
}
