﻿using System;
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
    /// Логика взаимодействия для File.xaml
    /// </summary>
    public partial class File : FileSystemObject
    {
        public File(string title = "", ImageSource iconSource = null)
        {
            Title = title;
            IconSource = iconSource ?? new BitmapImage(new Uri("/Resources/Icons/Files/default.png", UriKind.Relative));
        }
    }
}
