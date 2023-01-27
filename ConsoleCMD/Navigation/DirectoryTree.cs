using ConsoleCMD.Database.Models;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleCMD.Navigation
{
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(ListBox))]
    public class DirectoryTree : Control
    {
        /// <summary>
        /// Текущаю корневая директория
        /// </summary>
        public static readonly DependencyProperty CurrentDirectoryProperty;

        private ListBox _itemsHost;

        public Directory CurrentDirectory
        {
            get => (Directory)GetValue(CurrentDirectoryProperty);
            set => SetValue(CurrentDirectoryProperty, value);
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => _itemsHost.SelectionChanged += value;
            remove => _itemsHost.SelectionChanged -= value;
        }

        static DirectoryTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DirectoryTree),
                new FrameworkPropertyMetadata(typeof(DirectoryTree)));

            CurrentDirectoryProperty = DependencyProperty.Register(
                name: "CurrentDirectory",
                propertyType: typeof(Directory),
                ownerType: typeof(DirectoryTree),
                typeMetadata: null);
        }

        public override void OnApplyTemplate() => AttachItemsHost();

        private void AttachItemsHost() =>
            _itemsHost = (ListBox)GetTemplateChild("PART_ItemsHost");
    }
}
