using ConsoleCMD.Resources.Connection;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleCMD.Navigation
{
    public class DirectoryTree : Control
    {
        /// <summary>
        /// Текущаю корневая директория
        /// </summary>
        public static readonly DependencyProperty CurrentDirectoryProperty;

        public Directory CurrentDirectory
        {
            get => (Directory)GetValue(CurrentDirectoryProperty);
            set => SetValue(CurrentDirectoryProperty, value);
        }

        static DirectoryTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryTree), new FrameworkPropertyMetadata(typeof(DirectoryTree)));

            CurrentDirectoryProperty = DependencyProperty.Register(
                name: "CurrentDirectory",
                propertyType: typeof(Directory),
                ownerType: typeof(DirectoryTree),
                typeMetadata: null);
        }
    }
}
