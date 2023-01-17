using System.Windows;

namespace ConsoleCMD
{
    public partial class App : Application
    {
        static App()
        {
            FileSystemManager.InitializeFileSystem();
        }
    }
}
