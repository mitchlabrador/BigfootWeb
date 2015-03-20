using System;
using System.Linq;
using System.Web;

namespace BigfootWeb.Helpers
{
    public class ContextHelper
    {

        public static HttpContext Context { get { return HttpContext.Current; } }

        public static bool HasData(string key)
        {
            return Context.Items.Contains(key);
        }

        public static object GetData(string key)
        {
            return Context.Items.Contains(key) ? Context.Items[key] : null;
        }

        public static void SetData(string key, object value)
        {
            if (Context.Items.Contains(key))
                Context.Items[key] = value;
            else
                Context.Items.Add(key, value);
        }

        /// <summary>
        /// Removes a value from the cache safely. It does not throw an error if the value is not found.
        /// </summary>
        /// <param name="key">The key to the cahced value</param>
        public static void RemoveData(string key)
        {
            if (HasData(key)) HasData(key);
        }
                

        /// <summary>
        /// Returns the value stored in the cache as an integer
        /// </summary>
        /// <param name="key">The parameter key to the value</param>
        /// <returns>The cached value</returns>
        public static int GetInt(String key)
        {
            var val = GetData(key);
            return val == null ? 0 : int.Parse(val.ToString());
        }

        /// <summary>
        /// Returns the value stored in the cache as an integer
        /// </summary>
        /// <param name="key">The parameter key to the value</param>
        /// <returns>The cached value</returns>
        public static bool GetBool(String key)
        {
            var val = GetData(key);
            return val == null ? false : bool.Parse(val.ToString());
        }
        
    }
}
