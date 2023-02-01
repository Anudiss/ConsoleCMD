using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ConsoleCMD.Converters
{
    public class StringLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str == false || int.TryParse(parameter as string, out var newLength) == false)
                return null;
            return str[0..Math.Min(newLength, str.Length)];
            //return (value is string str == false ? null : str)?[0..Math.Min()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
