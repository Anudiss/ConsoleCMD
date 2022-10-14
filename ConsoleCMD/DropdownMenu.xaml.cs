using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class DropdownMenu : Popup
    {
        public string[] Items
        {
            get { return (string[])GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(string[]), typeof(DropdownMenu), new PropertyMetadata(null));



        public Thickness CustomMargin
        {
            get { return (Thickness)GetValue(CustomMarginProperty); }
            set { SetValue(CustomMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomMarginProperty =
            DependencyProperty.Register("CustomMargin", typeof(Thickness), typeof(DropdownMenu), new PropertyMetadata(default));


        public SelectionChangedEventHandler SelectionChanged;

        public DropdownMenu()
        {
            InitializeComponent();

            PlacementRectangle = new Rect();

            SelectionChanged += (sender, args) => SelectionChanged?.Invoke(sender, args);
        }
    }
}
