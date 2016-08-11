using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Utilities
{
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
            {typeof (Type), new List<Type> {typeof (object)}},
            {typeof (Color), new List<Type> {typeof (object)}}
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
}