using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bigfoot.Web.Helpers
{
    public class AspNetServer : IServer
    {
        public string MapPath(string path)
        {  
            return HttpContext.Current.Server.MapPath(path);
        }

        //public string HtmlEncode(string data)
        //{
        //    return HttpContext.Current.Server.HtmlEncode(data);
        //}

        //public string HtmlDecode(string data)
        //{
        //    return HttpContext.Current.Server.HtmlDecode(data);
        //}

        //public string UrlEncode(string data)
        //{
        //    return HttpContext.Current.Server.UrlEncode(data);
        //}

        //public string UrlDecode(string data)
        //{
        //    return HttpContext.Current.Server.UrlDecode(data);
        //}
    }
}
