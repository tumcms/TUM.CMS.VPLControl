using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Utilities
{
    public static class Converter
    {
        public static Size MeasureString(TextBlock tb)
        {
            var formattedText = new FormattedText(
                tb.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                tb.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public class WidthConverter : IValueConverter
        {
            private readonly double amount;

            public WidthConverter(double amount)
            {
                this.amount = amount;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double) value + amount;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                //This one is a bit tricky, if anyone feels like implementing this...
                throw new NotSupportedException();
            }
        }

        public class WidthConverterFactor : IValueConverter
        {
            private readonly double factor;

            public WidthConverterFactor(double factor)
            {
                this.factor = factor;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (double) value*factor;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                //This one is a bit tricky, if anyone feels like implementing this...
                throw new NotSupportedException();
            }
        }

        public class PointConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var xValue = (double) values[0];
                var yValue = (double) values[1];
                return new Point(xValue, yValue);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException("Cannot convert back");
            }
        }
    }
}