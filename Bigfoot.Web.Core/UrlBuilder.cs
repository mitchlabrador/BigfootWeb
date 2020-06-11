using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Bigfoot.Web.Core
{

    /// <summary>
    /// This simple class aids you in the fluid creation of urls and url parameters
    /// </summary>

    public class UrlBuilder
    {
        private readonly string _url = "";
        private readonly NameValueCollection _params = new NameValueCollection();

        public UrlBuilder() { }

        public UrlBuilder(string url)
        {
            _url = url;
        }

        /// <summary>
        /// Adds a Url parameter to the querystring
        /// </summary>
        /// <param name="param">parameter name</param>
        /// <param name="value">value</param>
        /// <returns>The UrlBulder object to continue to add paramters etc. Call ToString to finish</returns>
        public UrlBuilder Add(string param, string value)
        {
            _params.Add(param, value);
            return this;
        }


        /// <summary>
        /// Adds a Url parameter to the querystring
        /// </summary>
        /// <param name="param">parameter name</param>
        /// <param name="value">adds an integer parameter</param>
        /// <returns>The UrlBulder object to continue to add paramters etc. Call ToString to finish</returns>
        public UrlBuilder Add(string param, int value)
        {
            _params.Add(param, value.ToString());
            return this;
        }

        /// <summary>
        /// Builds your url with the parameters etc. By default it does not urlencode your parameters
        /// </summary>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Builds your url with the parameters etc. You can optionally urlencode your url parametes
        /// </summary>
        /// <param name="encode">UrlEncode or not your parameters</param>
        public string ToString(bool encode)
        {
            var qs = new StringBuilder();
            foreach (var key in _params.AllKeys)
            {
                var value = _params[key];
                // Encode if requested
                if (encode) value = HttpUtility.UrlEncode(value);
                if (qs.Length > 0) qs.Append("&");
                qs.AppendFormat("{0}={1}", key, value);
            }

            // When building a full url determine weather there are already parameters in it, or this is the first set of parameters
            var returnUrl = _url;
            if (returnUrl.Length > 0) returnUrl += returnUrl.Contains("?") ? "&" : "?";

            return returnUrl + qs;
        }

        /// <summary>
        /// UrlEncodes a certain value
        /// </summary>
        public static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        /// <summary>
        /// UrlDecodes a certain value
        /// </summary>
        public static string UrlDecode(string value)
        {
            return HttpUtility.UrlDecode(value);
        }


        /// <summary>
        /// Creates a url parameter
        /// </summary>
        public static string UrlP(string parameterName, string parameterValue)
        {
            return UrlP(parameterName, parameterValue, false);
        }

        /// <summary>
        /// Creates a url parameter
        /// </summary>
        public static string UrlP(string parameterName, string parameterValue, bool encode)
        {
            if (encode) parameterValue = UrlEncode(parameterValue);
            return parameterName + "=" + parameterValue;
        }

    }
    
}