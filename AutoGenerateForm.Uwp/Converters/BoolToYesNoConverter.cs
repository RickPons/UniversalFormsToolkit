using System;
using Windows.UI.Xaml.Data;

namespace AutoGenerateForm.Uwp.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
           if(value is bool)
            {
                var result = (bool) value;

                return result ? "Si" : "No";
            }
            return "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
