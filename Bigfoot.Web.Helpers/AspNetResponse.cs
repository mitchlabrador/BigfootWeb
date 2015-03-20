using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bigfoot.Web.Helpers
{
    public class AspNetResponse : IResponse
    {
        public string ApplyAppPathModifier(string url)
        {
            return HttpContext.Current.Response.ApplyAppPathModifier(url);
        }
        public void Redirect(string url, bool endResponse)
        {
            HttpContext.Current.Response.Redirect(url, endResponse);
        }
    }
}
