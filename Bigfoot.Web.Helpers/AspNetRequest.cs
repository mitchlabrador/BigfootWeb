using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bigfoot.Web.Helpers
{
    public class AspNetRequest : IRequest
    {

        public string ApplicationPath
        {
            get { return HttpContext.Current.Request.ApplicationPath; }
        }

        public NameValueCollection ServerVariables
        {
            get { return HttpContext.Current.Request.ServerVariables; }
        }

        public bool IsSecureConnection
        {
            get { return HttpContext.Current.Request.IsSecureConnection; }
        }

        public NameValueCollection Headers
        {
            get { return HttpContext.Current.Request.Headers; }
        }

        public string RawUrl
        {
            get { return HttpContext.Current.Request.RawUrl; }
        }


        public NameValueCollection QueryString
        {
            get { return HttpContext.Current.Request.QueryString; }
        }

        public NameValueCollection Form
        {
            get { return HttpContext.Current.Request.Form; }
        }

        public HttpFileCollection Files 
        { 
            get { return HttpContext.Current.Request.Files; } 
        }
    }
}
