namespace SirCab.UI.Converters
{
    public class BooleanToCornerRadiusConverter : IValueConverter
    {
        public static readonly BooleanToCornerRadiusConverter Instance = new();

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isUpdating)
                return isUpdating ? new CornerRadius(20, 20, 10, 10) : new CornerRadius(20);

            return new CornerRadius(20);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}