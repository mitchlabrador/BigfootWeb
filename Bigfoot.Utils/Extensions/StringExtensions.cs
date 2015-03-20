using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Bigfoot.Utils.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrWhiteSpace(val);
        }

        /// <summary>
        /// Example: "Welcome {0} (Last login: {1})".FormatString(userName, lastLogin)
        /// </summary>
        /// <param name="args"<>The arguments to replace/param>
        /// <returns>The merged arguments</returns>
        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Example: "Top result for {Name} was {TopResult}".FormatWith(student)
        /// </summary>
        /// <param name="source">The source object containing the data</param>
        /// <returns>The formatted string</returns>
        public static string FormatStringWithObject(this string format, object source)
        {
            return FormatStringWithObject(format, null, source);
        }

        /// <summary>
        /// Example: "Top result for {Name} was {TopResult}".FormatWith(student)
        /// </summary>
        /// <param name="source">The source object containing the data</param>
        /// <returns>The formatted string</returns>
        public static string FormatStringWithObject(this string format, IFormatProvider provider, object source)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            List<object> values = new List<object>();
            string rewrittenFormat = r.Replace(format, delegate(Match m)
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group formatGroup = m.Groups["format"];
                Group endGroup = m.Groups["end"];

                values.Add((propertyGroup.Value == "0")
                  ? source
                  : source.GetType().GetProperty(propertyGroup.Value).GetValue(source, null).ToString()); //DataBinder.Eval(source, propertyGroup.Value));

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                  + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }

        /// <summary>
        /// Example: myData.Match("[0-9]")
        /// </summary>
        /// <param name="pattern">Regular expression to match</param>
        /// <returns>True when the regular expression is matched</returns>
        public static bool Match(this string value, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(value);
        }

        public static string Escape(this string val, string stringtoescape, string scapechar)
        {
            if (val.IsEmpty()) return val;
            val = val.Replace(stringtoescape, scapechar + stringtoescape);
            return val;
        }

        public static int AsInt(this string val)
        {
            if (string.IsNullOrEmpty(val))
                return 0;
            return int.Parse(val);
        }

        public static decimal AsDecimal(this string val)
        {
            if (string.IsNullOrEmpty(val))
                return 0;
            return decimal.Parse(val);
        }

        public static string First500Chars(this string val)
        {
            if (string.IsNullOrEmpty(val))
                return "";
            if (val.Length > 500)
                return val.Substring(0, 500) + "...";
            else
                return val;
        }

        public static List<int> BreakIntoIntList(this string val, char devider = ',')
        {
            var list = new List<int>();
            if (val.IsEmpty()) return list;
            foreach (var item in val.Split(devider))
            {
                int intvalue;
                if (!item.IsEmpty() && int.TryParse(item, out intvalue))
                {
                    list.Add(intvalue);                   
                }
            }
            return list;
        }

        public static bool IsEmpty(this string val)
        {
            return string.IsNullOrEmpty(val) || string.IsNullOrEmpty(val.Trim());
        }

        public static bool IsIn(this string val, bool ignorecase = true, params string[] values)
        {
            if (val.IsEmpty()) return false;
            foreach (var compareto in values)
            {
                if (val.Equals(compareto, StringComparison.InvariantCultureIgnoreCase)) return true;
            }
            return false;
        }

        public static bool IsNotEmpty(this string val)
        {
            return !val.IsEmpty();
        }

        public static string When(this string val, bool condition)
        {
            return condition ? val : string.Empty;
        }          

        public static string FirstXCharsWithEllipsis(this string str, int maxChars)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (str.Length <= maxChars) return str;
            return str.Substring(0, maxChars) + "...";
        }
        
        public static string ReplaceCRWithBR(this string str)        
        {
            if (!String.IsNullOrEmpty(str))
            {
                var regex = new System.Text.RegularExpressions.Regex(@"(\r\n|\r|\n)+");
                var newText = regex.Replace(str, "<br />");
                return newText;
            }
            else
                return str;
        }

        /// <summary>
        /// Get the name of a property from a property access lambda. Here are two examples:
        /// <para><example>EXAMPLE-1: var myPropertyName = this.GetPropertyName(() => someObject.SomeProperty);</example></para>
        /// <para><example>EXAMPLE-2: var myPropertyName = this.GetPropertyName(() => SomeStaticClass.SomeProperty);</example></para>
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="theObject"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static string GetPropertyName<TProperty>(this object theObject, Expression<Func<TProperty>> propertyLambda)
        {
            var memberExpression = propertyLambda.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("You must pass a lambda in the form of: '() => Class.Property' or '() => someObject.Property'");
            }
            return memberExpression.Member.Name;
        }

        public static string SplitByUpperCase(this string givenString, bool bodyToLowerCase = false)
        {
            var builder = new StringBuilder();
            if (bodyToLowerCase)
            {
                for (int i = 0; i < givenString.Length; i++)
                {
                    var character = givenString[i];
                    if (i == 0)
                    {
                        builder.Append(character);
                        continue;
                    }
                    if (Char.IsUpper(character) && builder.Length > 0) builder.Append(' ');
                    
                    builder.Append(Char.ToLower(character));
                }
            }
            else
            {
                for (int i = 0; i < givenString.Length; i++)
                {
                    var character = givenString[i];
                    if (i == 0)
                    {
                        builder.Append(character);
                        continue;
                    }
                    if (Char.IsUpper(character) && builder.Length > 0) builder.Append(' ');
                    builder.Append(character);
                }    
            }
            
            return builder.ToString();
        }
    }

}