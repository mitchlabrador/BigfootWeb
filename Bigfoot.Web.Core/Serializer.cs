using System;
using System.Text;
using Newtonsoft.Json;

namespace Bigfoot.Web.Core
{
    public class Serializer
    {
        #region "Serilizers"
        
        public static string ToJson(object data)
        {
            return ToJson(data, false);
        }

        public static string ToJson(object data, bool ConvertToBase64)
        {
            var json = JsonConvert.SerializeObject(data); //ser.Serialize(data);

            if (ConvertToBase64) json = ToBase64(json);
            return json;
        }

        public static T FromJson<T>(string data)
        {
            return FromJson<T>(data, false);
        }

        public static T FromJson<T>(string data, bool ConvertFromBase64)
        {
            if (ConvertFromBase64) data = FromBase64(data);

            return JsonConvert.DeserializeObject<T>(data); 
        }

        public static string ToBase64(string data)
        {
            var enc = new UTF8Encoding();
            return Convert.ToBase64String(enc.GetBytes(data));
        }

        public static string FromBase64(string data)
        {
            var enc = new UTF8Encoding();
            return enc.GetString(Convert.FromBase64String(data));
        }
        
        #endregion
    }
}
