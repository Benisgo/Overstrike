using Overstrike.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Overstrike.Converters {
    public class ModularTypeToBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is ModEntry.ModType type)
                return ModEntry.IsTypeFamilyModular(type);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}