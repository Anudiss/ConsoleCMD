using ConsoleCMD.Resources.Connection;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для File.xaml
    /// </summary>
    public partial class File : FileSystemObject
    {
        private File(string title = "", byte[] iconSource = null)
        {
            Title = title;
            IconSource = iconSource ?? Icons.FileDefaultIcon;
        }

        public static File FromDatabaseObject(Resources.Connection.File file) =>
            new File(file.Name, file.Icon.Data);

        public static implicit operator File(Resources.Connection.File file) =>
            File.FromDatabaseObject(file);
    }
}
