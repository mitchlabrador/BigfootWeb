using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bigfoot.Utils.Extensions
{
    public static class EnumHelpers    
    {

        /// <summary>
        /// Gets a list of enum and value
        /// </summary>
        public static IEnumerable<KeyValuePair<string, string>> GetItems(this Type enumType)
        {
            if (!typeof(Enum).IsAssignableFrom(enumType)){
                throw new ArgumentException("Type must be an enum");
            }
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType).Cast<int>();
            var items = names.Zip(values, (name, value) => new KeyValuePair<string, string>(name, value.ToString()));
            return items;
        }

        /// <summary>
        /// Parses a string into it's enum representation in a case insensitive way
        /// </summary>
        /// <typeparam name="T">The type to parse the enum into</typeparam>
        /// <param name="strType">The string value</param>
        /// <param name="result">The parsed value</param>
        /// <returns>True when the parsing was successful</returns>
        public static bool TryParse<T>(this Enum theEnum, string strType, out T result)
        {
            string strTypeFixed = strType.Replace(' ', '_');
            if (Enum.IsDefined(typeof(T), strTypeFixed))
            {
                result = (T)Enum.Parse(typeof(T), strTypeFixed, true);
                return true;
            }
            else
            {
                foreach (string value in Enum.GetNames(typeof(T)))
                {
                    if (value.Equals(strTypeFixed,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        result = (T)Enum.Parse(typeof(T), value);
                        return true;
                    }
                }
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Converts an enum to its value implementation. Example: int a = MyEnum.Item1.Convert<int>();
        /// </summary>
        /// <typeparam name="ConvertType">The type to convert them enum to</typeparam>
        /// <returns>The Generic type</returns>
        public static ConvertType Convert<ConvertType>(this Enum e)
        {
            object o = null;
            Type type = typeof(ConvertType);

            if (type == typeof(int))
            {
                o = System.Convert.ToInt32(e);
            }
            else if (type == typeof(long))
            {
                o = System.Convert.ToInt64(e);
            }
            else if (type == typeof(short))
            {
                o = System.Convert.ToInt16(e);
            }
            else
            {
                o = System.Convert.ToString(e);
            }

            return (ConvertType)o;
        }
    }
}