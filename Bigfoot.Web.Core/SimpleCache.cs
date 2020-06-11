using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;

namespace Bigfoot.Web.Core
{
    /// <summary>
    /// This is a global application cache that persists accross requests while the application lives. Please takenote that it is server specific
    /// </summary>
    public static class SimpleCache
    {

        public static MemoryCache _cache = new MemoryCache("SimpleCache");
        
        /// <summary>
        /// Determine weather a certain key is contained in the cache
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if found</returns>
        public static bool Contains(String key) { return _cache.Contains(key); }

        /// <summary>
        /// Returns the value stored in the cache
        /// </summary>
        /// <param name="key">The parameter key to the value</param>
        /// <returns>The cached value</returns>
        public static object GetValue(String key)
        {
            return (Contains(key)) ? _cache[key] : null;
        }

        /// <summary>
        /// Returns the value stored in the cache as an integer
        /// </summary>
        /// <param name="key">The parameter key to the value</param>
        /// <returns>The cached value</returns>
        public static int GetInt(String key)
        {
            var val = GetValue(key);
            return val == null ? 0 : int.Parse(val.ToString());
        }
            
        /// <summary>
        /// Adds a value to the cache. Replaces any excisting value if found
        /// </summary>
        /// <param name="key">The key of the value to add</param>
        /// <param name="data">The data to add to the cache</param>
        public static void Add(String key, object data, CacheItemPolicy policy = null)
        {
            if (Contains(key))
            {
                if (policy == null)
                {
                    _cache[key] = data;
                }
                else
                {
                    _cache.Remove(key);
                    _cache.Add(key, data, policy);
                }
            }
            else
            {
                _cache.Add(key, data, policy);
            }
        }

        /// <summary>
        /// Removes a value from the cache safely. It does not throw an error if the value is not found.
        /// </summary>
        /// <param name="key">The key to the cahced value</param>
        public static void Remove(string key)
        {
            if (Contains(key)) _cache.Remove(key);
        }


        /// <summary>
        /// Get the fields / properties to hydrate for an object. 
        /// Caches the object map in order to maximize performance. 
        /// So reflection is used only first time on n object type
        /// </summary>
        /// <param name="obj">Object to use to hydrate</param>
        /// <returns>A list of Fields and Properties</returns>
        public static List<object> GetObjectFields(object obj)
        {
            var cacheKey = "reflectioncache_" + obj.GetType().FullName;
            List<object> fields;
            if (SimpleCache.Contains(cacheKey))
                fields = SimpleCache.GetValue(cacheKey) as List<object>;
            else
            {
                fields = new List<object>();
                foreach (PropertyInfo p in obj.GetType().GetProperties())
                {
                    if (p.CanWrite) fields.Add(p);
                }
                foreach (FieldInfo f in obj.GetType().GetFields())
                {
                    if (f.IsPublic && !f.IsStatic) fields.Add(f);
                }

                //fields.AddRange(obj.GetType().GetFields());

                SimpleCache.Add(cacheKey, fields);
            }
            return fields;
        }

    }

    
        
}