using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigfoot.Utils.Extensions
{
    public static class BooleanExtentions
    {
        public static string ToLower(this bool val)
        {
            return val.ToString().ToLower();
        }

        public static string ToJs(this bool val)
        {
            return val.ToString().ToLower();
        }
    }
}
