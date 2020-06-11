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
    public interface IContextHelper
    {
        /// <summary>
        /// These are the items stored in the context cache
        /// </summary>
        IDictionary<object, object> Items { get; }
        /// <summary>
        /// Retrieves the application root if not at the root of the domain.
        /// http://localhost:3000/foo/bar return String.Empty because the application is running at the root, and /foo/bar is part of the Path within the application
        /// Doc: https://stackoverflow.com/questions/58614864/whats-the-difference-between-httprequest-path-and-httprequest-pathbase-in-asp-n
        /// </summary>
        string ApplicationPathBase { get; }
        /// <summary>
        /// Determines if the current connection is secure
        /// </summary>
        bool IsSecureConnection { get; }
        /// <summary>
        /// Gets the headers from the request
        /// </summary>
        IHeaderDictionary Headers { get; }
        /// <summary>
        /// Gets back the complete RAW url (not decoded)
        /// </summary>
        string RawUrl { get; }
        /// <summary>
        /// Returns the Query string
        /// </summary>
        IQueryCollection QueryString { get; }
        /// <summary>
        /// Gets the form in the case a form was uploaded
        /// </summary>
        IFormCollection Form { get; }
        /// <summary>
        /// Gets all files uploaded
        /// </summary>
        IFormFileCollection Files { get; }
        /// <summary>
        /// Current request host e.g. https://www.microsoft.com:5360/subpage/another = www.microsoft.com
        /// </summary>
        string Host { get; }
        /// <summary>
        /// Gets the port being used if specified in the url 
        ///     e.g. https://www.microsoft.com:5360/subpage/another = 5360
        ///          https://www.google.com = NULL
        /// </summary>
        int? Port { get; }

        /// <summary>
        /// End the response and redirect the user to the provided url
        /// </summary>
        /// <param name="url">Url to send the user to</param>
        /// <param name="endResponse">Stop executing on the response</param>
        void Redirect(string url, bool endResponse);

        /// <summary>
        /// This is the application Path: "~/"
        /// </summary>
        string AppPath { get; }
        /// <summary>
        /// Default Path to CSS: AppPath + "Content/"
        /// </summary>
        string CssPath { get; }
        /// <summary>
        /// Default Scripts Path: AppPath + "Scripts/"
        /// </summary>
        string ScriptsPath { get; }
        /// <summary>
        /// Default Images Path: AppPath + "Content/images/"
        /// </summary>
        string ImagesPath { get; }

        /// <summary>
        /// This helper helps you to deal with the strongly typed collection of values from the request.
        /// It also allows for you to pass in manual values, which is useful during testing. When instantiated
        /// with the empty constructor, it assumes that you are refering to the HttpContext.Current context
        /// </summary>
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

        /// <summary>
        /// Determines if the current request was done over SSL
        /// </summary>
        bool IsUsingSsl { get; }

        /// <summary>
        /// Determines if the user is using SSL, if not forces a redirect
        /// </summary>
        /// <param name="condition">Only forces the redirect if the condition is true</param>
        void RequireSsl(bool condition = true);

        /// <summary>
        /// Adds a parameter to a url and returns the updated url. If the url already has parameters it adds to it, otherwise it appends to existing parameters
        /// </summary>
        /// <param name="url">Url, if empty nothing happens</param>
        /// <param name="param">Query parameter to add</param>
        /// <param name="value">Query value. Does NOT encode</param>
        /// <returns>The updated Url</returns>
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

        /// <summary>
        /// Determines of the context has the provided key
        /// </summary>
        /// <param name="key">Key to check for</param>
        /// <returns>True if it is found</returns>
        bool HasData(string key);

        /// <summary>
        /// Gets the provided Key from the Context or NULL (does not error out)
        /// </summary>
        /// <param name="key">The Context Key to get</param>
        /// <returns>The value found in the key or NULL if not found</returns>
        object GetData(string key);

        /// <summary>
        /// Add or replace this value in the Context
        /// </summary>
        /// <param name="key">Value Key</param>
        /// <param name="value">Value</param>
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
