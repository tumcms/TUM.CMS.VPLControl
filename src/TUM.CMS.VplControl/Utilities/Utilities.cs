using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace TUM.CMS.VplControl.Utilities
{
    public static class Utilities
    {
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
                assembly.GetTypes()
                    .Where(type => type.Namespace != null && type.Namespace.Contains("Nodes"))
                    .Where(type => type.FullName != "Node")
                    .ToArray();
        }


        public static string DataToString(object data)
        {
            if (data == null) return "null";

            Type t;
            string returnString;

            var type = data as Type;

            if (type != null)
            {
                t = type;

                if (t.IsEnum)
                {
                    returnString = "Enum " + t.Name + " {";

                    var counter = 0;
                    foreach (var o in Enum.GetValues(t))
                    {
                        returnString += o.ToString();

                        if (counter < Enum.GetValues(t).Length - 1)
                            returnString += ",";

                        counter++;
                    }
                    returnString += "}";
                }
                else
                    returnString = t.Name + " : Type";
            }
            else
            {
                t = data.GetType();

                if (t.IsGenericType)
                {
                    var collection = data as ICollection;
                    if (collection == null) return "null";
                    var obj = collection;

                    returnString = CollectionToString(obj, 1);
                }
                else
                    returnString = data + " : " + t.Name;
            }

            return returnString;

        }

        private static string CollectionToString(ICollection coll, int depth)
        {
            var tempLine = "";

            for (var i = 0; i < depth - 1; i++)
                tempLine += "  ";

            tempLine = "List" + Environment.NewLine;
            var counter = 0;

            foreach (var item in coll)
            {
                for (var i = 0; i < depth; i++)
                    tempLine += "  ";

                tempLine += "[" + counter + "] ";

                if (item == null)
                {
                    tempLine += "null";

                    if (depth != 1 || counter != coll.Count - 1)
                        tempLine += Environment.NewLine;
                }
                else
                {
                    if (item.GetType().IsGenericType)
                    {
                        var collection = item as ICollection;
                        if (collection == null) return "";
                        var obj = collection;

                        tempLine += CollectionToString(obj, depth + 1);
                    }
                    else
                    {
                        tempLine += item + " : " + item.GetType().Name;

                        if (depth != 1 || counter != coll.Count - 1)
                            tempLine += Environment.NewLine;
                    }
                }

                counter++;
            }
            return tempLine;
        }

    }
}