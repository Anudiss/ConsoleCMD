using ConsoleCMD.Navigation;
using ConsoleCMD.Resources.Connection.PartialClasses;
using System;
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
        public static NavigationComponent Instance =>
            _instance ?? (_instance = new NavigationComponent());

        public event PropertyChangedEventHandler PropertyChanged;

        private static Directory _currentDirectory;
        public static Directory CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;

                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(CurrentDirectory)));
            }
        }
   
        private NavigationComponent()
        {
            InitializeComponent();
        }
    }
}
