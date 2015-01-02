using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigfootWeb.Helpers
{
    public interface IServer
    {
        string MapPath(string path);
        //string HtmlEncode(string data);
        //string HtmlDecode(string data);
        //string UrlEncode(string data);
        //string UrlDecode(string data);        
    }
}
