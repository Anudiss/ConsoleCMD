using CommandParser.Command;
using CommandParser.Hints;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для HintsBoxComponent.xaml
    /// </summary>
    public partial class HintsBoxComponent : Popup
    {
        public CommandEntity[] Items
        {
            get { return (CommandEntity[])GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(CommandEntity[]), typeof(HintsBoxComponent), new PropertyMetadata(null));
        
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

        public CommandEntity SelectedHint => Items[SelectedItemIndex];

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
            DependencyProperty.Register("Bounds", typeof(Rect), typeof(HintsBoxComponent), new PropertyMetadata(Rect.Empty));

        public new double Width => LB.ActualWidth + SystemParameters.VerticalScrollBarWidth;

        public HintsBoxComponent() =>
            InitializeComponent();
        
        public void Open() =>
            IsOpen = true;

        public void Close() =>
            IsOpen = false;

        private void LB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }
    }

    [ValueConversion(typeof(IEnumerable<CommandEntity>), typeof(IEnumerable<TextBlock>))]
    public class CommandEntityToHintConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<CommandEntity> entities == false)
                return null;

            List<TextBlock> textBlocks = new List<TextBlock>();
            foreach (var entity in entities) {
                Hint hint = new Hint(entity);

                TextBlock textBlock = new TextBlock();

                foreach (var unit in hint.HintUnits)
                {
                    if (unit.HasDependecy)
                    {
                        textBlock.Inlines.Add(new Run($"{unit.Dependency.Value} ")
                        {
                            Foreground = HintColorScheme.ColorScheme[unit.Dependency.HintPart]
                        });
                    }

                    textBlock.Inlines.Add(new Run($"{unit.Value} ")
                    {
                        Foreground = HintColorScheme.ColorScheme[unit.HintPart]
                    });
                }
                textBlocks.Add(textBlock);
            }

            return textBlocks;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
