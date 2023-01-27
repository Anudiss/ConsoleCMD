using ConsoleCMD.Database;
using ConsoleCMD.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using SysIO = System.IO;

namespace ConsoleCMD
{
    public partial class App : Application
    {
        static App()
        {
            DatabaseContext.InitializeEntities();
            DatabaseContext.LoadEntities();

            FileSystem.DatabaseManager.Initialize();
        }

        private void _load_icons()
        {
            //var iconPaths = new string[] {
            //    @"C:\Users\Ильназ\Downloads\Icons\d.png",
            //    @"C:\Users\Ильназ\Downloads\Icons\f.png",
            //    @"C:\Users\Ильназ\Downloads\Icons\a.png",
            //    @"C:\Users\Ильназ\Downloads\spinner (2) (1).png"
            //};
            //foreach (var iconPath in iconPaths)
            //{
            //    var newIcon = new Icon();
            //    DatabaseContext.Entities.Icons.Add(newIcon);
            //    UsefulTools.ImageUploder.UploadImageToObjectProperty(iconPath, newIcon, "Data");
            //}
            //DatabaseContext.Entities.SaveChanges();
        }
    }
}
