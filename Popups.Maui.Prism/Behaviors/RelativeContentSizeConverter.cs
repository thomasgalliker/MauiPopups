using System.Globalization;

namespace Popups.Maui.Prism.Behaviors
{
    internal class RelativeContentSizeConverter : IValueConverter
    {
        private double relativeSize;

        public double RelativeSize
        {
            get => this.relativeSize;
            set
            {
                if (value == 0)
                {
                    this.relativeSize = 1;
                }
                else if (value > 1)
                {
                    this.relativeSize = value / 100;
                }
                else
                {
                    this.relativeSize = value;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pageSize = double.Parse(value.ToString());
            return this.RelativeSize * pageSize;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConvertBack(value, targetType, parameter, culture);
        }
    }
}