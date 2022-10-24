using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для ApplicationComponent.xaml
    /// </summary>
    public partial class ApplicationComponent : UserControl
    {
        public new string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public new static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(ApplicationComponent), new PropertyMetadata(""));

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }
        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(ApplicationComponent), new PropertyMetadata(""));

        public ApplicationComponent(string name, string iconPath)
        {
            Name = name;
            IconPath = iconPath;
            InitializeComponent();
        }
    }
}
