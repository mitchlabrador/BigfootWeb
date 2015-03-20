using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bigfoot.Utils.Extensions
{
    public static class ObjectExtensions
    {

        public static bool IsNull(this object source)
        {
            return source == null;
        }

        private static MemberInfo GetMember(this object source, string propertyName, bool ignoreCase = false)
        {
            MemberInfo member;
            BindingFlags bindingAttrs = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            bindingAttrs = (ignoreCase) ? bindingAttrs | BindingFlags.IgnoreCase : bindingAttrs;
            member = source.GetType().GetProperty(propertyName, bindingAttrs);
            if (member == null) member = source.GetType().GetField(propertyName, bindingAttrs);            
            return member;
        }

        public static void SetMemberValue(this object source, string memberName, object value, bool ignoreCase = false)
        {
            var member = source.GetMember(memberName, ignoreCase);
            if (member is FieldInfo)
            {
               ((FieldInfo)member).SetValue(source, value);
            }
            else if (member is PropertyInfo)
            {
                ((PropertyInfo)member).SetValue(source, value);
            }
            else
            {
                throw new Exception("Member: " + memberName + " not found");
            }
        }

        public static object GetMemberValue(this object source, string memberName, bool ignoreCase = false)
        {
            var member = source.GetMember(memberName, ignoreCase);
            if (member == null) return null;
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).GetValue(source);
            }
            else if (member is PropertyInfo){
                return ((PropertyInfo)member).GetValue(source);
            }
            else {
                return null;
            }            
        }

        public static string GetMemberValueAsString(this object source, string memberName, bool ignoreCase = false)
        {
            var value = source.GetMemberValue(memberName, ignoreCase);
            if (value == null) return string.Empty;
            return value.ToString();
        }

        public static int GetMemberValueAsInt(this object source, string memberName, bool ignoreCase = false)
        {
            var value = source.GetMemberValue(memberName, ignoreCase);

            if (value == null) return 0;

            if (value.GetType() == typeof(int))
            {
                return (int)value;
            }
            else
            {
                int outint;
                return int.TryParse(value.ToString(), out outint) ? outint : 0;
            }
        }

        public static DateTime GetMemberValueAsDate(this object source, string memberName, bool ignoreCase = false)
        {
            var value = source.GetMemberValue(memberName, ignoreCase);

            if (value == null) return DateTime.MinValue;

            if (value.GetType() == typeof(DateTime))
            {
                return (DateTime)value;
            }
            else
            {
                DateTime outvalue;
                return DateTime.TryParse(value.ToString(), out outvalue) ? outvalue : DateTime.MinValue;
            }
        }
        public static decimal GetMemberValueAsDecimal(this object source, string memberName, bool ignoreCase = false)
        {
            var value = source.GetMemberValue(memberName, ignoreCase);

            if (value == null) return 0;

            if (value.GetType() == typeof(decimal))
            {
                return (decimal)value;
            }
            else if (value.GetType() == typeof(double))
            {   
                return Convert.ToDecimal((double)value);
            }            
            else
            {
                decimal outvalue;
                return decimal.TryParse(value.ToString(), out outvalue) ? outvalue : 0;
            }
        }

        public static bool GetMemberValueAsBool(this object source, string memberName, bool ignoreCase = false)
        {
            var value = source.GetMemberValue(memberName, ignoreCase);

            if (value == null) return false;

            if (value.GetType() == typeof(bool))
            {
                return (bool)value;
            }
            else
            {
                bool outvalue;
                
                if (bool.TryParse(value.ToString(), out outvalue))
                {
                    return outvalue;
                }
                else
                {
                    var valuestr = value.ToString().ToLowerInvariant();
                    return (valuestr == "true" || valuestr == "yes" || valuestr == "1" || valuestr == "on") ? true : false;
                }
            }
        }

    }
}
