using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace ConsoleCMD
{
    public abstract class FileSystemObject : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FileSystemObject), new PropertyMetadata(""));

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceproperty); }
            set { SetValue(IconSourceproperty, value); }
        }
        public static readonly DependencyProperty IconSourceproperty =
            DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(FileSystemObject));
    }
}
