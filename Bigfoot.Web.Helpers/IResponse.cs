using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigfoot.Web.Helpers
{
    public interface IResponse
    {
        string ApplyAppPathModifier(string url);
        void Redirect(string url, bool endResponse);
    }
}
