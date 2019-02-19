using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RimeControl.Converter
{
    public class ListBoxSchemeSysConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSysScheme = (bool)value;
            if (isSysScheme)
            {
                return "* ";
            }
            return "- ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        //参考 https://www.cnblogs.com/tianma3798/p/5927470.html
    }
}
