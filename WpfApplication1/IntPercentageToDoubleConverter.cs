using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfApplication1
{
    public class IntPercentageToDoubleConverter : IValueConverter
    {
     
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            double res = 0.0d;
            if (value is int)
            {
                int intVal = (int)value;
                res = intVal / 100.0d;
                if (res < 0.0d)
                    res = 0.0d;
                else if (res > 100.0d)
                    res = 100.0d;
            }
            return res;
        }
        
        public object ConvertBack(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            return null;
        }
    }


}
