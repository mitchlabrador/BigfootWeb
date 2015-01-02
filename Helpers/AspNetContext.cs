using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BigfootWeb.Helpers
{
    public class AspNetContext : IContext
    {
        private IRequest _request;
        private IResponse _response;
        private IServer _server;

        public AspNetContext()
        {
            _request = new AspNetRequest();
            _response = new AspNetResponse();
            _server = new AspNetServer();
        }

        public IRequest Request { get { return _request; } }

        public IResponse Response { get { return _response; } }

        public IServer Server { get { return _server; } }

        public IDictionary Items { get { return HttpContext.Current.Items; } }

        
    }
}
