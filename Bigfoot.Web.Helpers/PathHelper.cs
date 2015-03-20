using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Bigfoot.Web.Helpers
{

    public class PathHelper
    {

        public IContext Context { get; private set; }

        public IRequest Request { get { return Context.Request; } }

        public IResponse Response { get { return Context.Response; } }

        public IServer Server { get { return Context.Server; } }

        public PathHelper()
        {
            Context = new AspNetContext();
        }

        public PathHelper(IContext context)
        {
            Context = context;
        }

        public string AppPath
        {
            get
            {
                return "~/";
            }
        }
        
        public string CssPath
        {
            get
            {
                return AppPath + "Content/";
            }
        }
        
        public string ScriptsPath
        {
            get
            {
                return AppPath + "Scripts/";
            }
        }
        
        public string ImagesPath
        {
            get
            {
                return AppPath + "Content/images/";
            }
        }
        
        /// <summary>
        /// Gets a browser usable path to a content file within the system. It uses the extention of the file to determine the location of the file
        /// within the content folder.
        /// i.e. "image.jpg" will return "/desktopmodules/modulename/content/images/image.jpg"
        /// i.e. "menu/left.jpg" will return "/desktopmodules/modulename/content/images/menu/left.jpg"
        /// i.e. "test.js" will return "/desktopmodules/modulename/content/js/test.js"
        /// </summary>
        /// <param name="fileName">The name of the file relative to the content folder i.e. left.jpg or menu/left.jpg</param>
        /// <returns>A url that may be used from the browser to access the static resource</returns>
        public string StaticUrl(string fileName)
        {
            return StaticUrl(fileName, false);
        }
        
        /// <summary>
        /// Gets a browser usable path to a content file within the system. It uses the extention of the file to determine the location of the static file
        /// within the content folder.
        /// i.e. "image.jpg" will return "/desktopmodules/modulename/content/images/image.jpg"
        /// i.e. "menu/left.jpg" will return "/desktopmodules/modulename/content/images/menu/left.jpg"
        /// i.e. "test.js" will return "/desktopmodules/modulename/content/js/test.js"
        /// i.e. "~/Test/test.js" will return /desktopmodules/modulename/Test/test.js"
        /// </summary>
        /// <param name="fileName">The name of the file relative to the content folder i.e. left.jpg or menu/left.jpg</param>
        /// <param name="includeDomain">Determines weather to include the domain name in the url. (i.e. http://domain.com/desktopmodules/modulename/content/image.jpg)</param>
        /// <returns>A url that may be used from the browser</returns>
        public string StaticUrl(string fileName, bool includeDomain)
        {
            var relUrl = GetRelativeUrl(fileName);
            
            if (fileName.StartsWith("~/"))
                relUrl = AppPath + fileName.Substring(2);
            else if (fileName.StartsWith("/~/")) // This notation is used to signify the DNN application root
                relUrl = fileName.Substring(1);
            
            return GetAbsoluteUrl(relUrl, includeDomain);
        }
        
        /// <summary>
        /// Adds the authentication token to a url. Provided that you are doing cookieless authentication
        /// </summary>
        /// <param name="url">The url to use</param>
        /// <returns>Returns the modified url</returns>
        public string AddAuthTokenToUrl(string url, bool removeDomainBeforeApplyingToken = false)
        {
            // Just add the token when not removing the domain
            if (removeDomainBeforeApplyingToken == false || !url.StartsWith("http"))
            {                
                return Response.ApplyAppPathModifier(url);
            }
            
            // remove the http before attempting to add the token
            var secureDomainRemoved = false;
            var domainRemoved = false;
            var apppath = GetFullHostName(false, true);
            var secureapppath = GetFullHostName(true, false);
            // remove trailing dash
            apppath = apppath.Substring(0, apppath.Length - 1);
            secureapppath = secureapppath.Substring(0, secureapppath.Length - 1);
            
            // Remove http
            if (url.StartsWith(apppath))
            {
                url = url.Substring(apppath.Length);
                domainRemoved = true;
            }
            // Remove https
            if (url.StartsWith(secureapppath))
            {
                url = url.Substring(secureapppath.Length);
                secureDomainRemoved = true;
            }
            
            // Apply the token
            url = Response.ApplyAppPathModifier(url);
            
            // Add the domain back to the url and return it
            if (secureDomainRemoved)
            {
                url = secureapppath + url;
            }
            else if (domainRemoved)
            {
                url = apppath + url;
            }
            
            // Return the url
            return url;
        }
        
        /// <summary>
        /// Retreives the relative url of a file based on its extention
        /// </summary>
        /// <param name="filename">filename: test.jpg returns ~/content/images/test.jpg</param>
        /// <returns>The server side relative url to the file</returns>
        public string GetRelativeUrl(string filename)
        {
            // Don't process files that already include the root element "~/" or the /~/
            if (filename.StartsWith("~/") || filename.StartsWith("/~/"))
                return filename;
            
            filename = filename.ToLower();
            if (filename.EndsWith("js"))
                return ScriptsPath + filename;
            else if (filename.EndsWith("css"))
                return CssPath + filename;
            else if (filename.EndsWith("jpg") || filename.EndsWith("gif") || filename.EndsWith("png"))
                return ImagesPath + filename;            
            else
                return filename;
        }
        
        /// <summary>
        /// Returns the server root application path. /AppLocation/ or / when at the root of the domain. This is the path to the application after the domain name
        /// </summary>
        /// <returns>Returns / or /appname/ or /apps/appname/ etc. What is after the domain name</returns>
        public string GetApplicationPath()
        {
            return GetApplicationPath(false);
        }
        
        /// <summary>
        /// Root application path. /app/ or / when at the root of the domain. 
        /// </summary>
        /// <param name="includeDomain">Determines weather the domain name is included</param>
        /// <returns>Returns the full application path root including the domain name if asked.</returns>
        public string GetApplicationPath(bool includeDomain, bool https = false, bool forcehttp = false)
        {
            var apppath = Request.ApplicationPath;
            if (apppath.EndsWith("/") == false)
                apppath += "/";
            
            if (includeDomain || https)
            {
                var host = GetFullHostName(https, forcehttp);
                // Remove the host last "/"
                host = host.Substring(0, host.Length - 1);
                // Combine the application path and the host
                apppath = host + apppath;
            }
            return apppath;
        }
        
        /// <summary>
        /// Gets the application host name of the current request. Returns something like this http://applicationhost.com:4454 if it is port 80 then
        /// the port number is not used
        /// </summary>
        public string GetApplicationHost
        {
            get
            {
                return GetFullHostName();
            }
        }
        
        /// <summary>
        /// Gets the application host name of the current request. Returns something like this http://applicationhost.com:4454 if it is port 80 then
        /// the port number is not used
        /// </summary>
        public string GetFullHostName(bool https = false, bool forcehttp = false)
        {
            
            var port = Request.ServerVariables["SERVER_PORT"];
            if (String.IsNullOrEmpty(port) || port == "80" || port == "443")
                port = "";
            else
                port = ":" + port;
            
            var protocol = Request.ServerVariables["SERVER_PORT_SECURE"];
            
            if (forcehttp)
            {
                protocol = "http://";
            }
            else if (https)
            {
                protocol = "https://";
            }
            else if (!string.IsNullOrEmpty(protocol) && protocol != "0")
            {
                protocol = "https://";
            }
            else
            {
                protocol = "http://";
            }
            
            var domain = Request.ServerVariables["SERVER_NAME"];
            
            return protocol + domain + port + "/";
        }
        
        /// <summary>
        /// Returns the physical path to a relative url. 
        /// For example url: ~/content/media/xyz.ppt returns c:\inetpub\wwwroot\appdomain.com\internal\content\media\xyz.ppt
        /// </summary>
        /// <param name="url">The relative application path i.e. ~/content/media/xyz.ppt</param>
        /// <returns>Full file system path i.e. c:\inetpub\wwwroot\appdomain.com\internal\content\media\xyz.ppt</returns>
        public string GetPhysicalPath(string url)
        {
            return Context.Server.MapPath(url);
        }
        
        /// <summary>
        /// Returns the absolute path to a url in the appliation. 
        /// For example url: ~/content/media/xyz.ppt returns /applicationpath/content/media/xyz.ppt
        /// </summary>
        /// <param name="url">The relative application path i.e. ~/content/media/xyz.ppt</param>
        /// <returns>Returns abosolute path to the url without the domain name. i.e. /applicationpath/content/media/xyz.ppt</returns>
        public string GetAbsoluteUrl(string url)
        {
            return GetAbsoluteUrl(url, false, false);
        }
        
        /// <summary>
        /// Returns the absolute path to a url in the appliation. 
        /// For example url: ~/content/media/xyz.ppt returns /applicationpath/content/media/xyz.ppt or http://applicationhsot/applicationpath/content/media/xyz.ppt
        /// </summary>
        /// <param name="url">The relative application path i.e. ~/content/media/xyz.ppt</param>
        /// <param name="includeFullPath">Determines weather to include the domain name</param>
        /// <returns>Returns abosolute path to the url without the domain name. i.e. /applicationpath/content/media/xyz.ppt
        /// or http://applicationhsot/applicationpath/content/media/xyz.ppt</returns>
        public string GetAbsoluteUrl(string url, bool includeFullPath)
        {
            return GetAbsoluteUrl(url, includeFullPath, false);
        }
        
        private string JoinUrlPath(string elem1, string elem2)
        {
            // Nothing to do if they are both empty
            if (string.IsNullOrEmpty(elem1) || string.IsNullOrEmpty(elem2))
            {
                return elem1 + elem2;
            }
            // The leading element contains the trailing dash
            if (!elem1.EndsWith("/"))
                elem1 = elem1 + "/";
            
            // Remove the starting dash from the second element
            if (elem2.StartsWith("/"))
            {
                if (elem2.Length > 1)
                    elem2 = elem2.Substring(1);
                else
                    elem2 = "";
            }
            // join them and return them
            return elem1 + elem2;
        }
        
        /// <summary>
        /// Returns the absolute path to a url in the appliation.  Optionally adds the url authentication token to the url
        /// For example url: ~/content/media/xyz.ppt returns /applicationpath/content/media/xyz.ppt or http://applicationhsot/applicationpath/content/media/xyz.ppt
        /// </summary>
        /// <param name="url">The relative application path i.e. ~/content/media/xyz.ppt</param>
        /// <param name="includeFullPath">Determines weather to include the domain name</param>
        /// <param name="addUrlAuthToken">Determines whether to add the url authentication token to the path</param>
        /// <returns>Returns abosolute path to the url without the domain name. i.e. /applicationpath/content/media/xyz.ppt
        /// or http://applicationhsot/applicationpath/content/media/xyz.ppt</returns>
        public string GetAbsoluteUrl(string url, bool includeFullPath, bool addUrlAuthToken, bool https = false)
        {
            var host = GetFullHostName(https);
            var apppath = GetApplicationPath(includeFullPath);
            
            // Remove the leading slash
            if (url.StartsWith("~/"))
            {
                url = apppath + url.Substring(2);
                //if (url.StartsWith("http") == false ) url = url.Substring(1);
            }
            
            // When requiring https and the url already has the host name, make sure the protocol is https
            if (https && url.StartsWith("https://") == false)
            {
                if (url.StartsWith("http://"))
                {
                    url = Regex.Replace(url, "^http://", "https://");
                }
                else
                {
                    url = JoinUrlPath(GetFullHostName(true), url);
                }
            }
            
            // Only add the apppath for urls that do not already include the host
            if (includeFullPath && url.StartsWith("http") == false)
            {
                url = JoinUrlPath(host, url);
            }
            
            // Add the authentication token
            if (addUrlAuthToken)
            {
                url = AddAuthTokenToUrl(url, true);
            }
            else
            {
                url = RemoveAuthenticationToken(url);
            }
            
            // Make sure the initial 
            
            return url;
        }
        
        public string RemoveAuthenticationToken(string url)
        {
            // The token uses the following format: /(F(*******)/
            const string pat = @"/\((f|F)\(.*\)\)";
            url = Regex.Replace(url, pat, "");
            return url;
        }
        
        /// <summary>
        /// Creates a unique file name for the path in the file. This prevents from duplicate file names (overriding). 
        /// Also creates the file path if it does not exist
        /// </summary>
        /// <param name="filePath">The full phisical path to the file</param>
        /// <returns>A full physicial path to a unique file name that may be used in the specified path</returns>
        public string GetUniqueFileName(string filePath)
        {
            // Make sure the file name is not empty
            if (string.IsNullOrEmpty(filePath))
                throw new ApplicationException("File Name is invalid!");
            
            // Ensure the path exists
            var folderPath = Path.GetDirectoryName(filePath) + '\\';
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            
            // This will hold the final file name (without the directory after the unique algorithm is run through)
            var newFileName = Path.GetFileName(filePath);
            
            // Ensure the filename is unique in the directory
            if (File.Exists(folderPath + newFileName))
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(newFileName);
                var ext = Path.GetExtension(newFileName);
                var count = 1;
                
                while (File.Exists(folderPath + fileNameWithoutExt + count + ext))
                {
                    count++;
                }
                
                newFileName = fileNameWithoutExt + count + ext;
            }
            
            return folderPath + newFileName;
        }
        
        public bool IsUsingSsl
        {
            get
            {
                if (Request.IsSecureConnection ||
                    // This second option is for ssl load balancers
                    string.Equals(Request.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                
                return false;
            }
        }
        
        public void RequireSsl(bool condition = true)
        {
            // Require the connection to be secure
            if (!IsUsingSsl && condition)
            {
                // Get the rawurl
                var rawurl = Request.RawUrl;
                
                // Build the url
                var redirecturl = "";
                if (string.IsNullOrEmpty(rawurl) == false && rawurl.StartsWith("/"))
                {
                    rawurl = rawurl.Length > 1 ? rawurl.Substring(1) : "";
                }
                redirecturl = GetFullHostName(true) + rawurl;
                
                // Redirect                
                Response.Redirect(redirecturl, true);
            }
        }
        
        public string AddParameterToUrl(string url, string param, string value)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            if (url.Contains("?"))
            {
                url += string.Format("&{0}={1}", param, value);
            }
            else
            {
                url += string.Format("?{0}={1}", param, value);
            }
            return url;
        }
        
        /// <summary>
        /// returns /(F(AUTHTOKEN-IF-AUTHENTICATED))/applicationpath/view/ 
        /// For content files (that do not require athentication) returns /applicationpath/content/images/urlpath
        /// </summary>
        public string ClientUrl(string urlpath)
        {
            return ClientUrl(urlpath, false);
        }
        
        /// <summary>
        /// returns /(F(AUTHTOKEN-IF-AUTHENTICATED))/applicationpath/view/ 
        /// For content files (that do not require athentication) returns /applicationpath/content/images/urlpath
        /// </summary>
        public string ClientUrl(string urlpath, bool includeDomain)
        {
            return ClientUrl(urlpath, includeDomain, true);
        }
        
        /// <summary>
        /// returns /(F(AUTHTOKEN-IF-AUTHENTICATED))/applicationpath/view/ 
        /// For content files (that do not require athentication) returns /applicationpath/content/images/urlpath
        /// </summary>
        public string ClientUrl(string urlpath, bool includeDomain, bool includeToken, bool https = false)
        {
            // Get the relative url
            var url = GetRelativeUrl(urlpath);

            // Remove the relative path from the front and add the application path
            if (url.StartsWith("~/"))
            {
                url = GetApplicationPath(includeDomain, https) + url.Substring(2);
            }

            // Add https if requested
            if (https)
            {
                var securehostname = GetFullHostName(true);
                // When it does not include the domain name then simply append the secure host
                if (url.ToLowerInvariant().StartsWith("/"))
                {
                    // Do not do substring on root paths "/"
                    var urlwithoutleadingslash = (url == "/") ? "" : url.Substring(1);
                    url = securehostname + urlwithoutleadingslash;
                }
                // replace the protocol if not secure when requested
                else if (url.ToLowerInvariant().StartsWith("http://"))
                {
                    url = Regex.Replace(url, "^http://", "https://");
                }
            }

            // Apply the authentication token
            if (includeToken &&
                    !urlpath.EndsWith("jpg") &&
                    !urlpath.EndsWith("png") &&
                    !urlpath.EndsWith("gif") &&
                    !urlpath.EndsWith("flv") &&
                    !urlpath.EndsWith("swf") &&
                    !urlpath.EndsWith("js") &&
                    !urlpath.EndsWith("css"))
                url = AddAuthTokenToUrl(url, true);

            // Remove the token if found
            if (!includeToken) url = RemoveAuthenticationToken(url);

            // Return the client url
            return url;
        }
        
    }
    
}
