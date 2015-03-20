using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bigfoot.Utils.Extensions;

namespace Bigfoot.Web.Helpers
{

    /// <summary>
    /// Logs entries to a log file which exists in the same directory as the current assembly executable
    /// </summary>
    public class LogHelper
    {

        public static string DefaultFilePath = HttpContext.Current.Request.MapPath("~/App_Data/Log.txt");

        /// <summary>
        /// Appends an entry to the log file. A header will be added with the Date/Time so no need to include it in your data entry.
        /// </summary>
        /// <param name="data">The data to add / Do not include date / time as a header as it is automatically added</param>
        public static void Append(string data, string filePath= "")
        {
            if (filePath.IsEmpty()) filePath = DefaultFilePath;
            var entry = "***LOG ENTRY: " + DateTime.Now.ToString("G") + " ***" + Environment.NewLine +
                        data + Environment.NewLine +
                        Environment.NewLine;
            File.AppendAllText(filePath, entry);
        }

    }
}
