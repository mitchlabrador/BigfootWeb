using System.Web;

namespace BigfootWeb.Helpers
{
    public class ViewHelper
    {

        private IContext Context;
        private IRequest Request { get { return Context.Request; } }
        private IResponse Response { get { return Context.Response; } }

        /// <summary>
        /// This constructor creates a new view helper class using the current HttpContext
        /// </summary>
        public ViewHelper()
        {
            Context = new AspNetContext();
        }

        /// <summary>
        /// This constructor creates a new view helper class using the request response and sever objects
        /// passed into it. This is useful when abstracting or moqing these objects
        /// </summary>
        public ViewHelper(IContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Contains the HTML helpers you can use in the views. It is an alias class that provides easy access to the HTMLHelper class.
        /// </summary>
        public HtmlHelper Html { get { return _html ?? (_html = new HtmlHelper()); } }
        private HtmlHelper _html;

        /// <summary>
        /// This is a general purpose html tag builder class. It is an alias class that provides easy access to the TagBuilder class.
        /// </summary>
        public TagBuilder TB(string tagName) { return new TagBuilder(tagName); }
        
        /// <summary>
        /// A helper builder function that outputs the text only if the specified condition is true
        /// (i.e. <%=RenderIf(user.HasGrups, user.GroupString)%>
        /// </summary>
        /// <param name="condition">The condition that determines weather the text is rendered or not</param>
        /// <param name="text">The text / html to render if the condition is true </param>
        /// <returns>Returns the RenderIfBuilder upon which you can further build with an .else statement other if conditions etc.</returns>
        public RenderIfBuilder RenderIf(bool condition, string text) { return Html.RenderIf(condition, text); }

        /// <summary>
        /// HtmlEncode the string for output
        /// </summary>
        /// <param name="data">The string to be encoded</param>
        /// <returns>HtmlEncoded string</returns>
        public string E(string data)
        {
            return HttpUtility.HtmlEncode(data);
        }

        /// <summary>
        /// HttpUtility the string for output
        /// </summary>
        /// <param name="data">The string to be encoded</param>
        /// <returns>HtmlEncoded string</returns>
        public string UrlE(string data)
        {
            return HttpUtility.UrlEncode(data);
        }

        /// <summary>
        /// Quick access to the String.Format function
        /// </summary>
        /// <param name="value">The string where the values will be inserted i.e. The bird flew {0} miles</param>
        /// <param name="values">The values to merge with the format string</param>
        /// <returns>The formated string</returns>
        public string F(string value, params string[] values)
        {
            return string.Format(value, values);
        }

        /// <summary>
        /// Helps in the creation of a unique id to later be used in collection binding
        /// </summary>
        /// <param name="prefix">The prefix that ties the colleciton together</param>
        /// <param name="id">The unique id of the collection item</param>
        /// <param name="fieldName">The name of the field to bind to for a certain item</param>
        public string ColId(string prefix, string id, string fieldName)
        {
            return prefix + "_" + id + "_" + fieldName;
        }

        /// <summary>
        /// Helps in the creation of a unique id to later be used in collection binding
        /// </summary>
        /// <param name="prefix">The prefix that ties the colleciton together</param>
        /// <param name="id">The unique id of the collection item</param>
        /// <param name="fieldName">The name of the field to bind to for a certain item</param>
        public string ColId(string prefix, int id, string fieldName)
        {
            return prefix + "_" + id + "_" + fieldName;
        }

        /// <summary>
        /// Creates a properly formated url parameter. 
        /// </summary>
        /// <param name="paramName">The name of the url parameter</param>
        /// <param name="value">The value of the url parameter</param>
        /// <returns>The properly formatted parameter i.e. parameter=value</returns>
        public string UrlP(string paramName, string value)
        {
            return paramName + "=" + value;
        }

        /// <summary>
        /// Creates a properly formated url parameter. 
        /// </summary>
        /// <param name="paramName">The name of the url parameter</param>
        /// <param name="value">The integer value of the url parameter</param>
        /// <returns>The properly formatted parameter i.e. parameter=value</returns>
        public string UrlP(string paramName, int value)
        {
            return paramName + "=" + value;
        }

        /// <summary>
        /// Easy access to the UrlBuilder class. Aids in the fluid creation of urls and url parameters
        /// </summary>
        public UrlBuilder QS { get { return new UrlBuilder(); } }

        /// <summary>
        /// Class used to aid in the collection of posted values from the request. First it tries the QueryString and second it tries the form collection
        /// </summary>
        public PostHelper Post
        {
            get
            {
                return _values ?? (_values = new PostHelper(Context));
            }
        }
        private PostHelper _values;


        /// <summary>
        /// Creates a DIV element with the provided content and assigns the provided css classes
        /// </summary>
        /// <param name="content">The content for the DIV</param>
        /// <param name="classes">CSS Classes to add</param>
        /// <returns>A TagBuilder</returns>
        public TagBuilder DIV(string content, string classes)
        {
            return Html.DIV(content, classes);
        }

        /// <summary>
        /// Creates a DIV element y calling string.format on the content and assigns the provided css classes
        /// </summary>
        /// <param name="content">The content for the DIV</param>
        /// <param name="classes">CSS Classes to add</param>
        /// <param name="formatValues">The values to merge with the content</param>
        /// <returns>A TagBuilder</returns>
        public TagBuilder DIV(string content, string classes, params string[] formatValues)
        {
            return Html.DIV(string.Format(content, formatValues), classes);
        }

        /// <summary>
        /// Creates a SPAN element with the provided content and assigns the provided css classes
        /// </summary>
        /// <param name="content">The content for the SPAN</param>
        /// <param name="classes">CSS Classes to add</param>
        /// <returns>A TagBuilder</returns>
        public TagBuilder SPAN(string content, string classes)
        {
            return Html.SPAN(content, classes);
        }

        /// <summary>
        /// Creates a SPAN element with the provided by calling string.format on the content and assigns the provided css classes
        /// </summary>
        /// <param name="content">The content for the SPAN</param>
        /// <param name="classes">CSS Classes to add</param>
        /// <param name="formatValues">The values to merge with the content</param>
        /// <returns>A TagBuilder</returns>
        public TagBuilder SPAN(string content, string classes, params string[] formatValues)
        {
            return Html.SPAN(string.Format(content, formatValues), classes);
        }


        /// <summary>
        /// Convert an object into a JSON string
        /// </summary>
        public string ToJson(object data)
        {
            return Serializer.ToJson(data);
        }

        /// <summary>
        /// Converts a JSON string into an object
        /// </summary>
        /// <typeparam name="T">The type to desirialize the string into</typeparam>
        /// <param name="jsonString">The JSON string to deserialize</param>
        public T FromJson<T>(string jsonString)
        {
            return Serializer.FromJson<T>(jsonString);
        }

        /// <summary>
        /// Creates a javascript reference
        /// </summary>
        public string JSReference(string url)
        {
            return Html.JSReference(url);
        }

        public string JSReference(string url, string minUrl, bool isDebugMode)
        {
            return Html.JSReference(url, minUrl, isDebugMode);
        }

        /// <summary>
        /// Creates a CSS reference
        /// </summary>
        public string CSSReference(string url)
        {
            return Html.CSSReference(url);
        }

        /// <summary>
        /// Creates a new SelectItemList and returns it
        /// </summary>
        public SelectItemList ItemList { get { return new SelectItemList(); } }
    }
}
