using ConsoleCMD.Database;
using System;
using System.Windows;

namespace ConsoleCMD.FileSystem
{
    public partial class LoadingProgressWindow : Window
    {
        public byte[] SpinnerIcon { get; set; } = DefaultIcons.SpinnerIcon.Data;

        public string ProgressBarTitle
        {
            get { return (string)GetValue(ProgressBarTitleProperty); }
            set { SetValue(ProgressBarTitleProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarTitleProperty =
            DependencyProperty.Register("ProgressBarTitle", typeof(string), typeof(LoadingProgressWindow));

        public double ProgressBarCurrentValue
        {
            get { return (double)GetValue(ProgressBarCurrentValueProperty); }
            set { SetValue(ProgressBarCurrentValueProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarCurrentValueProperty =
            DependencyProperty.Register("ProgressBarCurrentValue", typeof(double), typeof(LoadingProgressWindow), new PropertyMetadata(0.0));

        public double ProgressBarMinimumValue
        {
            get { return (double)GetValue(ProgressBarMinimumValueProperty); }
            set { SetValue(ProgressBarMinimumValueProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarMinimumValueProperty =
            DependencyProperty.Register("ProgressBarMinimumValue", typeof(double), typeof(LoadingProgressWindow), new PropertyMetadata(0.0));

        public double ProgressBarMaximumValue
        {
            get { return (double)GetValue(ProgressBarMaximumValueProperty); }
            set { SetValue(ProgressBarMaximumValueProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarMaximumValueProperty =
            DependencyProperty.Register("ProgressBarmMaximumValue", typeof(double), typeof(LoadingProgressWindow), new PropertyMetadata(0.0));

        public Visibility ProgressBarVisibility
        {
            get { return (Visibility)GetValue(ProgressBarVisibilityProperty); }
            set { SetValue(ProgressBarVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ProgressBarVisibilityProperty =
            DependencyProperty.Register("ProgressBarVisibility", typeof(Visibility), typeof(LoadingProgressWindow), new PropertyMetadata(Visibility.Collapsed));

        public LoadingProgressWindow()
        {
            InitializeComponent();
        }
    }
}
