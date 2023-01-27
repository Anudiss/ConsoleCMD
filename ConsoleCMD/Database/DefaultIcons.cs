using ConsoleCMD.Database;
using ConsoleCMD.Database.Models;
using System.Linq;

namespace ConsoleCMD.Database
{
    public static class DefaultIcons
    {
        public static readonly Icon DirectoryIcon = GetDefaultIcon(IconID.Directory);
        public static readonly Icon FileIcon = GetDefaultIcon(IconID.File);
        public static readonly Icon ApplicationIcon = GetDefaultIcon(IconID.Application);
        public static readonly Icon SpinnerIcon = GetDefaultIcon(IconID.Spinner);

        /// <summary>
        /// Метод получения иконки из базы данных
        /// </summary>
        /// <param name="iconID">Иконка по умолчанию</param>
        /// <returns>Иконка byte[]</returns>
        private static Icon GetDefaultIcon(IconID iconID) =>
            DatabaseContext.Entities.Icons.First(icon => icon.Id == (int)iconID);

        /// <summary>
        /// Перечисление иконок по умолчанию
        /// </summary>
        private enum IconID
        {
            Directory = 1,
            File,
            Application,
            Spinner
        }
    }
}
