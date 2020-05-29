using System;
using System.Globalization;
using System.Windows.Data;

namespace Bililive_dm
{
    public class PluginStatusConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool) value == true)
            {
                return "已激活";
            }
            else
            {
                return "未激活";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}