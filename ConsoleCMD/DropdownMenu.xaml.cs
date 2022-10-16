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

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(string[]), typeof(DropdownMenu), new PropertyMetadata(null));

        public int SelectedItemIndex
        {
            get => LB.SelectedIndex;
            set
            {
                if (Items == null)
                    return;
                if (value > Items.Length - 1)
                    LB.SelectedIndex = value % Items.Length;
                else if (value < 0)
                    LB.SelectedIndex = Items.Length + (value % Items.Length);
                else
                    LB.SelectedIndex = value;
            }
        }

        public Point Position
        {
            get => Bounds.TopLeft;
            set => Bounds = new Rect(value.X, value.Y, 0, 0);
        }

        public Rect Bounds
        {
            get { return (Rect)GetValue(RectangleProperty); }
            set { SetValue(RectangleProperty, value); }
        }

        public static readonly DependencyProperty RectangleProperty =
            DependencyProperty.Register("Bounds", typeof(Rect), typeof(DropdownMenu), new PropertyMetadata(Rect.Empty));

        public new double Width
        {
            get => LB.ActualWidth + SystemParameters.VerticalScrollBarWidth;
        }

        public DropdownMenu()
        {
            InitializeComponent();
        }

        private void LB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }
    }
}
