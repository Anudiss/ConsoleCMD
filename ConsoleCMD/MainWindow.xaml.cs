using ConsoleCMD.Applications;
using System.Windows;

namespace ConsoleCMD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Command command = new Command(
                    names: new[] { "set_bg", "bg", "set_background_color" },
                    description: "Выдаёт инфорофыовфцвд",
                    executor: (args) => (Command.ReturnCode.Success, ""),
                    arguments: new[] { new Argument(ArgumentType.Color, "Color", "Просто да", true),
                                       new Argument(ArgumentType.Bool, "Replace", "", true)},
                    flags: null
                );
            Console.WriteLine($"{command.Pattern}");
            Console.Focus();
        }
    }
}
