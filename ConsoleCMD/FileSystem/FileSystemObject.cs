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

        private byte[] _icon;
        public byte[] IconSource
        {
            get => _icon;
            set => _icon = value;
        }
    }
}
