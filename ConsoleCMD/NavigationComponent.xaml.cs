using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для NavigationComponent.xaml
    /// </summary>
    public partial class NavigationComponent : UserControl
    {
        public ObservableCollection<Directory> DirectoryHierarchy
        {
            get { return (ObservableCollection<Directory>)GetValue(DirectoryHierarchyProperty); }
            private set { SetValue(DirectoryHierarchyProperty, value); }
        }
        public static readonly DependencyProperty DirectoryHierarchyProperty =
            DependencyProperty.Register("DirectoryHierarchy", typeof(ObservableCollection<Directory>), typeof(NavigationComponent), new PropertyMetadata(new ObservableCollection<Directory>()));

        public Action<Directory> SelectedEndNode;

        public NavigationComponent()
        {
            InitializeComponent();
        }

        public new void Focus() => TW.Focus();

        private void TW_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedNode = TW.SelectedItem as Directory;
            if (selectedNode.Children.Count == 0)
                SelectedEndNode?.Invoke(selectedNode);
        }
    }
}
