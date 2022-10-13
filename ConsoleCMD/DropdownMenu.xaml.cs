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
    /// Логика взаимодействия для DropdownMenu.xaml
    /// </summary>
    public partial class DropdownMenu : UserControl
    {
        public string[] Items { get; set; }

        public DropdownMenu()
        {
            InitializeComponent();
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
