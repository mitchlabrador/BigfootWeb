using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigfoot.Utils.Extensions
{
    public static class NumberExtensions
    {

        public static int? AsNullable(this int val)
        {
            var newval = new int?();
            if (val > 0) newval = val;
            return newval;
        }

        /// <summary>
        /// Returns the correct value for a nullable int
        /// </summary>
        public static int SafeValue(this int? val)
        {
            if (val == null) return 0;
            if (val.HasValue == false) return 0;
            return val.Value;            
        }

        public static bool IsInArray(this int val, IEnumerable<int> inarray)
        {
            return val.IsInArray(inarray, null);
        }

        /// <summary>
        /// Makes a number into its negative representation
        /// </summary>
        public static decimal MakeNegative(this decimal val)
        {
            return val - (val * 2);
        }

        /// <summary>
        /// Makes a number into its negative representation
        /// </summary>
        public static int MakeNegative(this int val)
        {
            return val * -1;
        }

        public static int ValueOrDefault(this int val, int defaultvalue, int emptyvalue = 0)
        {
            return val == emptyvalue ? defaultvalue : val;
        }

        public static bool IsBetween(this int val, int start, int end)
        {
            return val >= start && val <= end;
        }

        public static int RoundUp(this decimal i, int roundmultiple)
        {
            return (int)(Math.Ceiling(i / roundmultiple) * roundmultiple);
        }

        public static decimal CurrencyRound(this decimal i)
        {
            return Round(i, 2);
        }

        public static decimal Round(this decimal i, int decimalPlaces)
        {
            return (decimal)Math.Round(i, decimalPlaces);
        }

        public static decimal ThisOrGreaterValue(this decimal d, decimal newnumber)
        {
            return d < newnumber ? newnumber : d;
        }

        public static decimal GetMax(this decimal d, params decimal[] numbers)
        {
            if (numbers == null) throw new ApplicationException("No other numbers where entered");
            var max = numbers.Max();
            return max > d ? max : d;
        }

        public static bool IsBetween(this decimal val, decimal start, decimal end)
        {
            return val >= start && val <= end;
        }

        public static bool IsInArray(this int val, IEnumerable<int> inarray, IEnumerable<int> notinarray)
        {
            var in_inarray = false;
            var in_notinarray = false;
            foreach (var item in inarray)
            {
                if (val == item) { in_inarray = true; break; }
            }
            if (notinarray == null) return in_inarray;
            foreach (var notin in notinarray)
            {
                if (val == notin) { in_notinarray = true; break; }
            }
            return in_inarray == true && in_notinarray == false;
        }


    }
}
