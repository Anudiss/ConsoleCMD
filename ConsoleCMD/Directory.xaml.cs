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
    /// Логика взаимодействия для Directory.xaml
    /// </summary>
    public partial class Directory : FileSystemObject
    {
        private List<FileSystemObject> _children = new List<FileSystemObject>();
        public List<FileSystemObject> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public Directory()
        {
            InitializeComponent();
        }

        public Directory(string title)
        {
            InitializeComponent();
            Title = title;
        }

        public Directory(string title, FileSystemObject[] children)
        {
            InitializeComponent();
            Title = title;
            children.ToList().ForEach(child => _children.Add(child));
        }
    }
}
