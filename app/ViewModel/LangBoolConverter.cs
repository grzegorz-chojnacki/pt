using System;
using System.Globalization;
using System.Windows.Data;

namespace app.ViewModel {
    public class LangBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((string)value == (string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((bool)value) ? (string)parameter : null;
        }
    }
}
