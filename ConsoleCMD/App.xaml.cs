using ConsoleCMD.Database;
using ConsoleCMD.Database.Models;
using ConsoleCMD.FileSystem;
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

            DatabaseContext.Entities.Icons.Load();
            DatabaseContext.Entities.Extensions.Load();
            DatabaseContext.Entities.Directories.Load();
            DatabaseContext.Entities.Files.Load();

            //var iconPaths = new string[] {
            //    @"C:\Users\Ильназ\Downloads\Icons\d.png",
            //    @"C:\Users\Ильназ\Downloads\Icons\f.png",
            //    @"C:\Users\Ильназ\Downloads\Icons\a.png"
            //};

            //foreach (var iconPath in iconPaths)
            //{
            //    var newIcon = new Icon();
            //    DatabaseContext.Entities.Icons.Add(newIcon);
            //    UsefulTools.ImageUploder.UploadImageToObjectProperty(iconPath, newIcon, "Data");
            //}
            
            //DatabaseContext.Entities.SaveChanges();

            FileSystemManager.InitializeFileSystem();
        }
    }
}
