using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;

namespace Radar_Project.Controls
{
    class Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = (bool)value;      

            if (flag)
                return "connected...";
            else
                return "not connected...";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return null;
        }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Boolean))
                throw new ArgumentException("value must be boolean");

           
             return !(bool)value;
            

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
