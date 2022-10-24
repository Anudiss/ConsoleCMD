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

namespace ConsoleCMD.Pages
{
    /// <summary>
    /// Логика взаимодействия для DesktopPage.xaml
    /// </summary>
    public partial class DesktopPage : Page
    {
        private Dictionary<CategoryComponent, ApplicationComponent[]> categoriesAndApplications
            = new Dictionary<CategoryComponent, ApplicationComponent[]>()
            {
                {
                    new CategoryComponent("Категория 1", "/Resources/Icons/Categories/placeholder.ico"),
                    new ApplicationComponent[] {
                        new ApplicationComponent("Приложение 1", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 2", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 3", "/Resources/Icons/Applications/placeholder.ico"),
                    }
                },
                {
                    new CategoryComponent("Категория 2", "/Resources/Icons/Categories/placeholder.ico"),
                    new ApplicationComponent[] {
                        new ApplicationComponent("Приложение 1", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 2", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 3", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 4", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 5", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 6", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 7", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 8", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 9", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 10", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 11", "/Resources/Icons/Applications/placeholder.ico"),
                        new ApplicationComponent("Приложение 12", "/Resources/Icons/Applications/placeholder.ico"),
                    }
                },
            };
        public DesktopPage()
        {
            InitializeComponent();

            List<Control> categoriesWithSeparators = new List<Control>();
            categoriesAndApplications.Keys.ToList().ForEach(category => {
                categoriesWithSeparators.Add(category); categoriesWithSeparators.Add(new Separator());
            });
            categoriesWithSeparators.RemoveAt(categoriesWithSeparators.Count-1);
            Categories.ItemsSource = categoriesWithSeparators;

            _selectedCategory = null;
        }

        private CategoryComponent _selectedCategory;

        private void LBCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryComponent category = Categories.SelectedItem as CategoryComponent;
            if (_selectedCategory == category)
                return;

            _selectedCategory = category;
            ApplicationComponent[] applications = categoriesAndApplications[category];

            //Applications.ItemsSource = applications.Select(app =>
            //{
            //    ListBoxItem listBoxItem = new ListBoxItem();
            //    listBoxItem.Content = app;
            //    return listBoxItem;
            //});
            Applications.ItemsSource = applications;
        }
    }
}
