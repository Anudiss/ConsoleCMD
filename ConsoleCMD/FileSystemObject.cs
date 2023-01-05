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
    public abstract class FileSystemObject
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private ImageSource _iconSource;
        public ImageSource IconSource
        {
            get { return _iconSource; }
            set { _iconSource = value; }
        }
    }
}
