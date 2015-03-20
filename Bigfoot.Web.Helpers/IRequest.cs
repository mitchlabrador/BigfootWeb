using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bigfoot.Web.Helpers
{
    public interface IRequest
    {
        string ApplicationPath { get; }
        NameValueCollection ServerVariables { get; }
        bool IsSecureConnection { get; }
        NameValueCollection Headers { get; }
        string RawUrl { get; }
        NameValueCollection QueryString { get; }
        NameValueCollection Form { get; }
        HttpFileCollection Files { get; }
    }
}
