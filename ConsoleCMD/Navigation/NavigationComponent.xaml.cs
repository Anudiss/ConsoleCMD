using ConsoleCMD.Database.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace ConsoleCMD
{
    /// <summary>
    /// Панель навигации на основе иерархии директорий
    /// </summary>
    public partial class NavigationComponent : UserControl, INotifyPropertyChanged
    {
        private static NavigationComponent _instance;
        public static NavigationComponent Instance => _instance ??= new NavigationComponent();

        public event PropertyChangedEventHandler PropertyChanged;

        private Directory _currentDirectory;
        public Directory CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;

                PropertyChanged?.Invoke(this, new (nameof(CurrentDirectory)));
            }
        }

        private NavigationComponent()
        {
            InitializeComponent();
        }
    }
}
