using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace ConsoleCMD
{
    [ValueConversion(typeof(string[]), typeof(string))]
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            string.Join("\n", value as string[]);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (value as string).Split('\n');
    }
}
