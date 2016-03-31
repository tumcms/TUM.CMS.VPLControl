using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace TUM.CMS.VplControl.Utilities
{
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                    ((INotifyPropertyChanged) item).PropertyChanged += ItemPropertyChanged;
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                    ((INotifyPropertyChanged) item).PropertyChanged -= ItemPropertyChanged;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender,
                    IndexOf((T) sender));
            OnCollectionChanged(args);
        }
    }

    public static class CollectionEx
    {
        public static TrulyObservableCollection<T> ToTrulyObservableCollection<T>(this IEnumerable<T> enumerableList)
            where T : INotifyPropertyChanged
        {
            if (enumerableList != null)
            {
                // Create an emtpy observable collection object
                var observableCollection = new TrulyObservableCollection<T>();

                // Loop through all the records and add to observable collection object
                foreach (var item in enumerableList)
                    observableCollection.Add(item);

                // Return the populated observable collection
                return observableCollection;
            }
            return null;
        }
    }

    public static class ClassUtility
    {
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(type => type.Namespace != null && type.Namespace.Contains("Nodes")).Where(type => type.FullName != "Node").ToArray();
        }
    }

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

    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding()
        {
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider)
            : base(formatProvider)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb)
            : base(sb)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
        }

        public StringWriterWithEncoding(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding)
            : base(formatProvider)
        {
            this.encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            this.encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding)
            : base(sb, formatProvider)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding ?? base.Encoding; }
        }
    }

    internal static class TypeExtensions
    {
        // http://stackoverflow.com/questions/2224266/how-to-tell-if-type-a-is-implicitly-convertible-to-type-b

        private static readonly Dictionary<Type, List<Type>> dict = new Dictionary<Type, List<Type>>
        {
            {
                typeof (decimal),
                new List<Type>
                {
                    typeof (object),
                    typeof (sbyte),
                    typeof (byte),
                    typeof (short),
                    typeof (ushort),
                    typeof (int),
                    typeof (uint),
                    typeof (long),
                    typeof (ulong),
                    typeof (char)
                }
            },
            {
                typeof (double),
                new List<Type>
                {
                    typeof (object),
                    typeof (sbyte),
                    typeof (byte),
                    typeof (short),
                    typeof (ushort),
                    typeof (int),
                    typeof (uint),
                    typeof (long),
                    typeof (ulong),
                    typeof (char),
                    typeof (float)
                }
            },
            {
                typeof (float),
                new List<Type>
                {
                    typeof (object),
                    typeof (sbyte),
                    typeof (byte),
                    typeof (short),
                    typeof (ushort),
                    typeof (int),
                    typeof (uint),
                    typeof (long),
                    typeof (ulong),
                    typeof (char),
                    typeof (float)
                }
            },
            {
                typeof (ulong),
                new List<Type> {typeof (object), typeof (byte), typeof (ushort), typeof (uint), typeof (char)}
            },
            {
                typeof (long),
                new List<Type>
                {
                    typeof (object),
                    typeof (sbyte),
                    typeof (byte),
                    typeof (short),
                    typeof (ushort),
                    typeof (int),
                    typeof (uint),
                    typeof (char)
                }
            },
            {typeof (uint), new List<Type> {typeof (object), typeof (byte), typeof (ushort), typeof (char)}},
            {
                typeof (int),
                new List<Type>
                {
                    typeof (object),
                    typeof (double),
                    typeof (sbyte),
                    typeof (byte),
                    typeof (short),
                    typeof (ushort),
                    typeof (char)
                }
            },
            {typeof (ushort), new List<Type> {typeof (object), typeof (byte), typeof (char)}},
            {typeof (short), new List<Type> {typeof (object), typeof (byte)}},
            {typeof (bool), new List<Type> {typeof (object), typeof (int), typeof (string)}},
            {typeof (string), new List<Type> {typeof (object)}},
            {typeof (object), new List<Type> {typeof (object)}},
            {typeof (Type), new List<Type> {typeof (object)}}
        };

        public static bool IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
                return true;
            if (dict.ContainsKey(to) && dict[to].Contains(from))
                return true;
            var castable = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(
                    m => m.ReturnType == to &&
                         (m.Name == "op_Implicit" ||
                          m.Name == "op_Explicit")
                );
            return castable;
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }

    public static class ExtensionMethods
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }



}