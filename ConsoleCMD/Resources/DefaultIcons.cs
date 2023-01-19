using ConsoleCMD.Resources.Connection;
using System.Linq;

namespace ConsoleCMD.Resources
{
    public static class DefaultIcons
    {
        public static readonly byte[] DirectoryIcon   = GetDefaultIcon(IconID.Directory);
        public static readonly byte[] FileIcon        = GetDefaultIcon(IconID.File);
        public static readonly byte[] ApplicationIcon = GetDefaultIcon(IconID.Application);

        /// <summary>
        /// Метод получения иконки из базы данных
        /// </summary>
        /// <param name="iconID">Иконка по умолчанию</param>
        /// <returns>Иконка byte[]</returns>
        private static byte[] GetDefaultIcon(IconID iconID) =>
            DatabaseContext.Entities.Icons.First(icon => icon.Id == (int)iconID).Data;

        /// <summary>
        /// Перечисление иконок по умолчанию
        /// </summary>
        private enum IconID
        {
            Directory = 1,
            File,
            Application
        }
    }
}
