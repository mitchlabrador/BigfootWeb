using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigfoot.Web.Helpers
{
    public interface IContext
    {
        IRequest Request { get; }
        IResponse Response { get; }
        IServer Server { get; }
        IDictionary Items { get; }
    }
}
