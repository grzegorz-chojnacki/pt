using System;
using System.Globalization;
using System.Windows.Data;

namespace app.ViewModel {
    public class SortBoolConverter<T> : IValueConverter where T : Enum {
        protected T ConvertInternal(object o) => (T)(object)int.Parse((string)o);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.Equals(ConvertInternal(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((bool)value) ? (object)ConvertInternal(parameter) : null;
        }
    }
    public class SortByBoolConverter : SortBoolConverter<SortBy> { }
    public class SortDirectionBoolConverter : SortBoolConverter<SortDirection> { }
}
