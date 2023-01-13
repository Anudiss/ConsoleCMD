using ConsoleCMD.Resources.Connection;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для Application.xaml
    /// </summary>
    public partial class Application : FileSystemObject
    {
        public Application(string title = "", byte[] iconSource = null)
        {
            Title = title;
            IconSource = iconSource ?? Icons.ApplicationDefaultIcon;
        }
    }
}
