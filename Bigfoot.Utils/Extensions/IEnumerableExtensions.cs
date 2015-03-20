using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigfoot.Utils.Extensions
{
    public static class IEnumerableExtensions
    {

        public static string ToCommaSeparatedString<T>(this IEnumerable<T> enumerable)
        {
            var value = "";
            foreach (var current in enumerable)
            {
                if (value.Length > 0) value += ",";
                value += current.ToString();
            }
            return value;
        }

    }
}
