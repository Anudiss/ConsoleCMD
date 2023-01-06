using System.Windows.Media;

namespace ConsoleCMD
{
    public abstract class FileSystemObject
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        private ImageSource _iconSource;
        public ImageSource IconSource
        {
            get => _iconSource;
            set => _iconSource = value;
        }
    }
}
