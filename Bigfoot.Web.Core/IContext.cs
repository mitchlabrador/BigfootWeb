using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bigfoot.Web.Core
{
    public interface IContext
    {

        IDictionary<object, object> Items { get; }

        string ApplicationPath { get; }
        bool IsSecureConnection { get; }
        IHeaderDictionary Headers { get; }
        string RawUrl { get; }
        IQueryCollection QueryString { get; }
        IFormCollection Form { get; }
        IFormFileCollection Files { get; }
        string Host { get; }
        int? Port { get; }

        void Redirect(string url, bool endResponse);

        string AppPath { get; }

        string CssPath { get; }

        string ScriptsPath { get; }

        string ImagesPath { get; }

        PostHelper Post { get; }

        /// <summary>
        /// Gets a browser usable path to a content file within the system. It uses the extention of the file to determine the location of the file
        /// within the content folder.
        /// i.e. "image.jpg" will return "/desktopmodules/modulename/content/images/image.jpg"
        /// i.e. "menu/left.jpg" will return "/desktopmodules/modulename/content/images/menu/left.jpg"
        /// i.e. "test.js" will return "/desktopmodules/modulename/content/js/test.js"
        /// </summary>
        /// <param name="fileName">The name of the file relative to the content folder i.e. left.jpg or menu/left.jpg</param>
        /// <returns>A url that may be used from the browser to access the static resource</returns>
        string StaticUrl(string fileName);

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
        string StaticUrl(string fileName, bool includeDomain);

        /// <summary>
        /// Retreives the relative url of a file based on its extention
        /// </summary>
        /// <param name="filename">filename: test.jpg returns ~/content/images/test.jpg</param>
        /// <returns>The server side relative url to the file</returns>
        string GetRelativeUrl(string filename);

        /// <summary>
        /// Returns the server root application path. /AppLocation/ or / when at the root of the domain. This is the path to the application after the domain name
        /// </summary>
        /// <returns>Returns / or /appname/ or /apps/appname/ etc. What is after the domain name</returns>
        string GetApplicationPath();

        /// <summary>
        /// Root application path. /app/ or / when at the root of the domain. 
        /// </summary>
        /// <param name="includeDomain">Determines weather the domain name is included</param>
        /// <returns>Returns the full application path root including the domain name if asked.</returns>
        string GetApplicationPath(bool includeDomain, bool https = false, bool forcehttp = false);

        /// <summary>
        /// Gets the application host name of the current request. Returns something like this http://applicationhost.com:4454 if it is port 80 then
        /// the port number is not used
        /// </summary>
        string GetApplicationHost { get; }

        /// <summary>
        /// Gets the application host name of the current request. Returns something like this http://applicationhost.com:4454 if it is port 80 then
        /// the port number is not used
        /// </summary>
        string GetFullHostName(bool https = false, bool forcehttp = false);

        /// <summary>
        /// Returns the absolute path to a url in the appliation. 
        /// For example url: ~/content/media/xyz.ppt returns /applicationpath/content/media/xyz.ppt
        /// </summary>
        /// <param name="url">The relative application path i.e. ~/content/media/xyz.ppt</param>
        /// <returns>Returns abosolute path to the url without the domain name. i.e. /applicationpath/content/media/xyz.ppt</returns>
        string GetAbsoluteUrl(string url);

        /// <summary>
        /// Returns the absolute path to a url in the appliation. 
        /// For example url: ~/content/media/xyz.ppt returns /applicationpath/content/media/xyz.ppt or http://applicationhsot/applicationpath/content/media/xyz.ppt
        /// </summary>
        /// <param name="url">The relative application path i.e. ~/content/media/xyz.ppt</param>
        /// <param name="includeFullPath">Determines weather to include the domain name</param>
        /// <returns>Returns abosolute path to the url without the domain name. i.e. /applicationpath/content/media/xyz.ppt
        /// or http://applicationhsot/applicationpath/content/media/xyz.ppt</returns>
        string GetAbsoluteUrl(string url, bool includeFullPath);

        //string JoinUrlPath(string elem1, string elem2);

        /// <summary>
        /// Returns the absolute path to a url in the appliation.  Optionally adds the url authentication token to the url
        /// For example url: ~/content/media/xyz.ppt returns /applicationpath/content/media/xyz.ppt or http://applicationhsot/applicationpath/content/media/xyz.ppt
        /// </summary>
        /// <param name="url">The relative application path i.e. ~/content/media/xyz.ppt</param>
        /// <param name="includeFullPath">Determines weather to include the domain name</param>
        /// <param name="addUrlAuthToken">Determines whether to add the url authentication token to the path</param>
        /// <returns>Returns abosolute path to the url without the domain name. i.e. /applicationpath/content/media/xyz.ppt
        /// or http://applicationhsot/applicationpath/content/media/xyz.ppt</returns>
        string GetAbsoluteUrl(string url, bool includeFullPath, bool https = false);

        /// <summary>
        /// Creates a unique file name for the path in the file. This prevents from duplicate file names (overriding). 
        /// Also creates the file path if it does not exist
        /// </summary>
        /// <param name="filePath">The full phisical path to the file</param>
        /// <returns>A full physicial path to a unique file name that may be used in the specified path</returns>
        string GetUniqueFileName(string filePath);

        bool IsUsingSsl { get; }

        void RequireSsl(bool condition = true);

        string AddParameterToUrl(string url, string param, string value);

        /// <summary>
        /// returns /applicationpath/view/ 
        /// For content files (that do not require athentication) returns /applicationpath/content/images/urlpath
        /// </summary>
        string ClientUrl(string urlpath);

        /// <summary>
        /// returns /applicationpath/view/ 
        /// For content files (that do not require athentication) returns /applicationpath/content/images/urlpath
        /// </summary>
        string ClientUrl(string urlpath, bool includeDomain);

        /// <summary>
        /// returns /applicationpath/view/ 
        /// For content files (that do not require athentication) returns /applicationpath/content/images/urlpath
        /// </summary>
        string ClientUrl(string urlpath, bool includeDomain, bool https = false);

        bool HasData(string key);

        object GetData(string key);

        void SetData(string key, object value);

        /// <summary>
        /// Removes a value from the cache safely. It does not throw an error if the value is not found.
        /// </summary>
        /// <param name="key">The key to the cahced value</param>
        void RemoveData(string key);

        /// <summary>
        /// Returns the value stored in the cache as an integer
        /// </summary>
        /// <param name="key">The parameter key to the value</param>
        /// <returns>The cached value</returns>
        int GetInt(String key);

        /// <summary>
        /// Returns the value stored in the cache as an integer
        /// </summary>
        /// <param name="key">The parameter key to the value</param>
        /// <returns>The cached value</returns>
        bool GetBool(String key);
    }
}
