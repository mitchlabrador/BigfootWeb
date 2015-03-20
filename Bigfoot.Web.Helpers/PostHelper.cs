using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Reflection;

namespace Bigfoot.Web.Helpers
{

    /// <summary>
    /// This class helps you deal with the strongly typed collection of values from the request.
    /// It also allows for you to pass in manual values, which is useful during testing. When instantiated
    /// with the empty constructor, it assumes that you are refering to the HttpContext.Current context
    /// </summary>
    public class PostHelper
    {
        private string cacheKey = "PostHelper_RequestValues";
        private IContext Context;
        private IRequest Request { get { return Context.Request; } }
        private IResponse Response { get { return Context.Response; } }
        
        public string Message_ExpectedValueNotFound = "Expected value not found: ";

        /// <summary>
        /// Creates an instance of the class using the current context's HttpRequest
        /// </summary>
        public PostHelper()
        {
            this.Context = new AspNetContext();
        }

        /// <summary>
        /// Creates an instance of the class using the HttpRequest specified
        /// </summary>
        public PostHelper(IContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets a value from the request. First it tries the QueryString and second it tries the form collection
        /// </summary>
        /// <param name="key">The parameter to get</param>
        /// <returns>The value of the parameter</returns>
        public string RequestValue(string key)
        {           
            if (!string.IsNullOrEmpty(Request.QueryString[key]))
                return Request.QueryString[key];

            if (!string.IsNullOrEmpty(Request.Form[key]))
                return Request.Form[key];

            return "";
        }

        /// <summary>
        /// Determines whether a certain parameter is present
        /// </summary>
        /// <param name="key">parameter name</param>
        /// <returns>True if found</returns>
        public bool HasValue(string key)
        {
            return !string.IsNullOrEmpty(RequestValue(key));
        }

        /// <summary>
        /// Clears the RequestValues cache for the current request
        /// </summary>
        public void ClearCache()
        {
            if (Context.Items.Contains(cacheKey))
                Context.Items.Remove(cacheKey);
        }

        /// <summary>
        /// Retreives all the values posted through the form querystring and also the ManualValues section
        /// </summary>
        public NameValueCollection RequestValues
        {
            get
            {                
                // Get the values
                var values = new NameValueCollection();
                
                // Add the querystring values
                values.Add(Request.QueryString);

                // Add the form values
                values.Add(Request.Form);

                // Add the collection to the context
                Context.Items.Add(cacheKey, values);
                
                // Returns the values
                return values;
            }
        }

        /// <summary>
        /// Requires that a certain parameter be present in the request
        /// </summary>
        /// <param name="name">paramater name</param>
        /// <returns>The parameter value</returns>
        public string GetValueOrError(string name)
        {
            return GetValueOrError(name, false);
        }

        /// <summary>
        /// Requires that a certain parameter be present in the request
        /// </summary>
        /// <param name="name">paramater name</param>
        /// <returns>Convers the parameter value to an int</returns>
        public int GetIntOrError(string name)
        {
            RequireValue(name);
            return GetInt(name).Value;
        }

        /// <summary>
        /// Requires that a certain parameter be present in the request
        /// </summary>
        /// <param name="name">paramater name</param>
        /// <returns>Convers the parameter value to a double</returns>
        public double GetDoubleOrError(string name)
        {
            RequireValue(name);
            return GetDouble(name).Value;
        }

        /// <summary>
        /// Requires that a certain parameter be present in the request
        /// </summary>
        /// <param name="name">paramater name</param>
        /// <returns>Convers the parameter value to a double</returns>
        public long GetLongOrError(string name)
        {
            RequireValue(name);
            return GetLong(name).Value;
        }

        /// <summary>
        /// Requires that a certain parameter be present in the request
        /// </summary>
        /// <param name="name">paramater name</param>
        /// <returns>Convers the parameter value to a double</returns>
        public decimal GetDecimalOrError(string name)
        {
            RequireValue(name);
            return GetDecimal(name).Value;
        }

        /// <summary>
        /// Requires that a certain parameter be present in the request
        /// </summary>
        /// <param name="name">paramater name</param>
        /// <returns>Convers the parameter value to a date</returns>
        public DateTime GetDateOrError(string name)
        {
            RequireValue(name);
            return GetDate(name).Value;
        }

        /// <summary>
        /// Requires that a certain parameter be present in the request
        /// </summary>
        /// <param name="name">paramater name</param>
        /// <param name="decode">indicates weather you want to UrlDecode the parameter</param>
        /// <returns>The parameter value</returns>
        public string GetValueOrError(string name, bool decode)
        {
            string value;
            if (string.IsNullOrEmpty(RequestValue(name)))
                throw new ApplicationException(Message_ExpectedValueNotFound + name);
            value = RequestValue(name);
            if (decode) value = HttpUtility.UrlDecode(value);
            return value;
        }

        /// <summary>
        /// Validates that a certain parameter be present is present in the request. Throws an error otherwise
        /// </summary>
        /// <param name="name">name of the parameter</param>
        public void RequireValue(string name)
        {
            if (string.IsNullOrEmpty(RequestValue(name)))
                throw new ApplicationException(Message_ExpectedValueNotFound + name);
        }

        /// <summary>
        /// Returns the value for a certain parameter (QueryString or Form)
        /// </summary>
        /// <param name="key">parameter name</param>
        /// <returns>value</returns>
        public string GetValue(string key)
        {
            return RequestValue(key);
        }

        /// <summary>
        /// Returns the value for a certain parameter (QueryString or Form)
        /// </summary>
        /// <param name="key">parameter name</param>
        /// <param name="urlDecode">Decodes the parameter as if it were coming from the querystring</param>
        /// <returns>value</returns>
        public string GetValue(string key, bool urlDecode)
        {
            var value = RequestValue(key);
            if (urlDecode)
                value = HttpUtility.UrlDecode(value);
            return value;
        }

        /// <summary>
        /// Returns the value for a certain parameter (QueryString or Form) if not supplied then it returns the default value provided
        /// </summary>
        /// <param name="key">parameter name</param>
        /// <returns>value</returns>
        public string GetValueOrDefault(string key, string defaultValue)
        {
            return HasValue(key) ? GetValue(key) : defaultValue;
        }

        /// <summary>
        /// Returns the value for a certain parameter (QueryString or Form) if not supplied then it returns the default value provided
        /// </summary>
        /// <param name="key">parameter name</param>
        /// <returns>value</returns>
        public string GetValueOrDefault(string key, string defaultValue, bool urlDecode)
        {
            return HasValue(key) ? GetValue(key, urlDecode) : defaultValue;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as an integer
        /// </summary>
        public int? GetInt(string key)
        {
            var result = new int?();
            if (RequestValue(key) != "")
                result = int.Parse(GetValue(key));
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as an integer
        /// </summary>
        public int GetIntOrDefault(string key, int defaultValue = 0)
        {
            var result = defaultValue;
            if (RequestValue(key) != "")
                int.TryParse(GetValue(key), out result);
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as a Decimal
        /// </summary>
        public decimal? GetDecimal(string key)
        {
            var result = new decimal?();
            if (RequestValue(key) != "")
                result = decimal.Parse(GetValue(key));
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as a Decimal
        /// </summary>
        public decimal GetDecimalOrDefault(string key)
        {
            var result = new decimal();
            if (RequestValue(key) != "")
                result = decimal.Parse(GetValue(key));
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as an double
        /// </summary>
        public double? GetDouble(string key)
        {
            var result = new double?();
            if (RequestValue(key) != "")
                result = double.Parse(GetValue(key));
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as long
        /// </summary>
        public long? GetLong(string key)
        {
            var result = new long?();
            if (RequestValue(key) != "")
                result = long.Parse(GetValue(key));
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as an date 
        /// </summary>
        public DateTime? GetDate(string key)
        {
            var result = new DateTime?();
            DateTime value;
            if (RequestValue(key) != "" && DateTime.TryParse(GetValue(key), out value))
            {
                result = value;
            }
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as an date 
        /// </summary>
        public DateTime GetDateOrDefault(string key)
        {
            var result = new DateTime();
            DateTime value;
            if (RequestValue(key) != "" && DateTime.TryParse(GetValue(key), out value))
            {
                result = value;
            }
            return result;
        }

        /// <summary>
        /// Returns a value for a certain parameter typed as an date in a specific format
        /// </summary>
        public DateTime? GetDate(string key, DateTimeFormatInfo format)
        {
            var result = new DateTime?();
            if (RequestValue(key) != "")
            {
                result = DateTime.Parse(RequestValue(key), format);
            }
            return result;
        }
        
        /// <summary>
        /// Returns a value for a certain parameter typed as a bool. Interprets: true | yes | on | 1   AS True
        /// </summary>
        public bool GetBool(string key)
        {
            return GetValue(key) != "" &&
                        (GetValue(key).ToLower() == "true" ||
                         GetValue(key).ToLower() == "yes" ||
                         GetValue(key).ToLower() == "on" ||
                         GetValue(key).ToLower() == "1");
        }

        /// <summary>
        /// Returns a list of integers from 
        /// </summary>
        /// <param name="data">The string of ints i.e. "1,2,3,4"</param>
        /// <returns></returns>
        public List<int> GetIntArray(string data)
        {
            var result = new List<int>();
            if (RequestValue(data) != "")
            {
                var value = RequestValue(data);
                // Identify the devider
                var devider = ',';
                if (value.Contains(";")) { devider = ';'; }
                else if (value.Contains("|")) { devider = '|'; }
                // Strip the first and last characters if they are deviders
                if (value.StartsWith(devider.ToString())) value = value.Substring(1);
                if (value.EndsWith(devider.ToString())) value = value.Substring(0, value.Length - 2);

                // Split the string and add to the list
                foreach (var v in value.Split(devider))
                {
                    result.Add(int.Parse(v));
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a list of strings from the value suplied
        /// </summary>
        /// <param name="data">The string of values i.e. "1,2,3,4"</param>
        /// <returns></returns>
        public List<string> GetValueArray(string data)
        {
            return GetValueArray(data, ',');
        }

        /// <summary>
        /// Returns a list of strings from the value suplied
        /// </summary>
        /// <param name="data">The string of values i.e. "1,2,3,4"</param>
        /// <param name="devider">The devider to use to split the array</param>
        /// <returns></returns>
        public List<string> GetValueArray(string data, char devider)
        {
            var result = new List<string>();
            if (RequestValue(data) != "")
            {
                var value = RequestValue(data);
                
                // Split the string and add to the list non empty elements
                foreach (var v in value.Split(devider))
                {
                    if (!string.IsNullOrEmpty(v) && !string.IsNullOrEmpty(v.Trim()))
                    {
                        result.Add(v);    
                    }
                }
            }
            return result;
        }

        private List<HttpPostedFile> _files;
        public List<HttpPostedFile> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = new List<HttpPostedFile>();
                    if (Request.Files.Count > 0)
                    {
                        foreach(string f in Request.Files)
                        {
                            _files.Add(Request.Files[f]);
                        }
                    }
                }
                return _files;
            }
        }

        /// <summary>
        /// Determines wether files were uploaded
        /// </summary>
        public bool HasFiles
        {
            get
            {
                if (Files.Count > 0)
                {
                    for (var i = 0; i < Files.Count; i++)
                    {
                        if (Files[i].InputStream.Length > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Validates that a file size with the max file size. Throws an error otherwise
        /// </summary>
        /// <param name="file">File to be validated</param>
        /// /// <param name="maxFilesize">Max file size in bytes</param>
        public void ValidateFileMaxFileSize(double maxFilesize)
        {
            if (Files.Count > 0)
            {
                for (var i = 0; i < Files.Count; i++)
                {                    
                    if (Files[i].ContentLength > maxFilesize)
                    {
                        throw new ApplicationException(Files[i].FileName + " is larger than the maximum file size allowed. Files cannot be larger than " + maxFilesize);
                    }
                }
            }
        }

        /// <summary>
        /// Validates that a file with an extension specified in this comma delimited list is in the request. Throws an error otherwise
        /// </summary>
        /// <param name="fileExtensions">The list of file extensions separated by a comma that are accepted</param>
        public void RequireFileWithExt(string fileExtensions)
        {
            if (Request.Files.Count == 0)
                throw new ApplicationException("No file was uploaded!");

            var ext = VirtualPathUtility.GetExtension(Request.Files[0].FileName).ToLowerInvariant();

            foreach (var validExt in fileExtensions.Split(','))
            {
                if (validExt.ToLowerInvariant() == ext)
                    return;
            }

            // If the extension does not match a proper extension then throw an error
            throw new ApplicationException("File uploaded was not the corrent file type. File extension must be one of the following: " + fileExtensions);
        }

        /// <summary>
        /// Validates that files uploaded match one of the provided extensions
        /// </summary>
        /// <param name="fileExtensions">The list of file extensions separated by a comma that are accepted</param>
        public void ValidateUploadedFilesHaveThisExtension(string fileExtensions)
        {
            if (Request.Files.Count == 0) return;

            // Invalid file
            var invalid = true;
            var hasFiles = false;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (string.IsNullOrEmpty(Request.Files[i].FileName.Trim()))
                {
                    continue;
                }
                else
                {
                    hasFiles = true;
                }
                var ext = VirtualPathUtility.GetExtension(Request.Files[i].FileName).ToLowerInvariant();

                foreach (var validExt in fileExtensions.Split(','))
                {
                    if (validExt.ToLowerInvariant() == ext)
                    {
                        invalid = false;
                        break;
                    }                   
                }
            }

            // If one of the files does not have the right extension and there are files to upload then throw the error
            if (invalid && hasFiles) throw new ApplicationException("File uploaded was not the correct file type. File extension must be one of the following: " + fileExtensions);
        }

        /// <summary>
        /// Retreives all the posted values with a certain prefix
        /// </summary>
        /// <param name="prefix">The prefix of the posted value key</param>
        public Dictionary<string, string> GetValuesWithPrefix(string prefix)
        {
            var vals = new Dictionary<string, string>();

            var postValues = RequestValues;

            foreach (var key in postValues.AllKeys)
            {
                if (key.StartsWith(prefix))
                {
                    if (!string.IsNullOrEmpty(postValues[key]))
                    {
                        vals.Add(key.Replace(prefix, string.Empty), postValues[key]);
                    }
                }
            }

            return vals;
        }

        #region "FillObject"
       
        /// <summary>
        /// Converts a database value to its object representation. Uses the TypeConverter cache to properly translate complex types
        /// </summary>
        /// <param name="fieldType">The Type of the field to convert to</param>
        /// <param name="value">The database value. e.g. the object from the data reader ordinal</param>
        /// <returns>A properly converted object</returns>
        private object GetFieldValue(Type fieldType, object value)
        {
            var typeName = fieldType.Name;
            object newValue = null;
            Type baseType = fieldType.BaseType;

            // Check if an empty value or an empty string
            if (value == null || value.ToString() == String.Empty)
                return newValue;

            if (fieldType.Equals(value.GetType()))
            {
                newValue = value;
            }
            else if (typeName == "Boolean")
            {
                newValue = (value.ToString() == "1" ||
                            value.ToString().ToLower() == "on" ||
                            value.ToString().ToLower() == "true" ||
                            value.ToString().ToLower() == "yes") ? true : false;
            }
            // Nullable types's name starts with nullable
            else if (typeName.StartsWith("Nullable"))
            {
                var typeFullName = fieldType.FullName;
                if (typeFullName.Contains("DateTime"))
                    newValue = Convert.ToDateTime(value);
                else if (typeFullName.Contains("Boolean"))
                    newValue = Convert.ToBoolean(value);
                else if (typeFullName.Contains("Int16"))
                    newValue = Convert.ToInt16(value);
                else if (typeFullName.Contains("Int32"))
                    newValue = Convert.ToInt32(value);
                else if (typeFullName.Contains("Integer"))
                    newValue = Convert.ToInt32(value);
                else if (fieldType.FullName.Contains("Int64"))
                    newValue = Convert.ToInt64(value);
                else if (fieldType.FullName.Contains("Decimal"))
                    newValue = Convert.ToDecimal(value);
                else if (typeFullName.Contains("Double"))
                    newValue = Convert.ToDouble(value);
                else if (typeFullName.Contains("Single"))
                    newValue = Convert.ToSingle(value);
                else if (typeFullName.Contains("UInt16"))
                    newValue = Convert.ToUInt16(value);
                else if (typeFullName.Contains("UInt32"))
                    newValue = Convert.ToUInt32(value);
                else if (typeFullName.Contains("UInt64"))
                    newValue = Convert.ToUInt64(value);
                else if (typeFullName.Contains("SByte"))
                    newValue = Convert.ToSByte(value);
            }
            else if (fieldType.FullName.Equals("System.Guid"))
            {
                newValue = new Guid(value.ToString());
            }
            else if (baseType != null && fieldType.BaseType == typeof(Enum))
            {
                int intEnum;
                if (int.TryParse(value.ToString(), out intEnum))
                    newValue = intEnum;
                else
                {
                    try
                    {
                        newValue = Enum.Parse(fieldType, value.ToString());
                    }
                    catch (Exception)
                    {
                        newValue = Enum.ToObject(fieldType, value);

                    }
                }
            }
            else if (fieldType.FullName.Equals("System.Guid"))
            {
                newValue = new Guid(value.ToString());
            }
            else
            {
                // Try to do a brute convert and ignore any errors that come up                
                try
                {
                    newValue = Convert.ChangeType(value, fieldType);
                }
                catch (Exception) { }
            }


            return newValue;

        }

        public T FillObject<T>()
        {
            return CreateObject<T>("", "");
        }

        public T FillObject<T>(string prefix)
        {
            return CreateObject<T>(prefix, "");
        }

        public T FillObject<T>(string prefix, string suffix)
        {
            return CreateObject<T>(prefix, suffix);
        }

        public object FillObject(object objectToFill)
        {
            return CreateObject(objectToFill, "", "");
        }

        public object FillObject(object objectToFill, string prefix)
        {
            return CreateObject(objectToFill, prefix, "");
        }

        public object FillObject(object objectToFill, string prefix, string suffix)
        {
            return CreateObject(objectToFill, prefix, suffix);
        }

        private T CreateObject<T>(string prefix, string suffix)
        {
            // Create a new instance of the object
            var objObject = Activator.CreateInstance<T>();
            // fill it
            return (T)CreateObject(objObject, prefix, suffix);
        }

        private object CreateObject(object objectToFill, string prefix, string suffix)
        {
            var values = RequestValues;
            // Make sure there are values to hydrate
            if (values == null || values.HasKeys() == false) return objectToFill;

            // Check weather the object is a value type
            if (objectToFill.GetType().IsValueType)
            {
                var value = GetFieldValue(objectToFill.GetType(), values[0]);
                if (value != null) objectToFill = value;
                return objectToFill;
            }

            // Hydrate a complex type it
            //  Get the fields for the type
            List<object> fields = SimpleCache.GetObjectFields(objectToFill);
            foreach (var field in fields)
            {
                // Get the fieldname
                string fieldName;
                if (field is FieldInfo)
                    fieldName = (field as FieldInfo).Name;
                else
                    fieldName = (field as PropertyInfo).Name;

                // Loop through the values
                foreach (string formKey in values.Keys)
                {
                    if ((prefix + fieldName + suffix).ToUpperInvariant() == formKey.ToUpperInvariant())
                    {
                        // Check weather the member is a property or a field and fill it accordingly
                        if (field is FieldInfo)
                            (field as FieldInfo).SetValue(objectToFill, GetFieldValue(((FieldInfo)field).FieldType, values[formKey]), BindingFlags.Default, null, null);
                        else if (field is PropertyInfo)
                            (field as PropertyInfo).SetValue(objectToFill, GetFieldValue(((PropertyInfo)field).PropertyType, values[formKey]), BindingFlags.Default, null, null, null);

                        // Go to the next item
                        break;
                    }

                }

            }

            // Return the filled object
            return objectToFill;
        }
              
        #endregion

        #region "FillCollection"

        /// <summary>
        /// Cretes a collection of objects from posted data. The posted data must be in this format
        /// [prefix]_[uniqueId]_[fieldname]
        /// 1. [prefix] could be anything. This is just a way to identify all items that belong in this array
        /// 2. [uniqueId] identifies the collection item instance
        ///         for instance if I were filling a list of order items then I would have three fields
        ///         1. orderItems_15_itemCount
        ///         2. orderItems_15_productId
        ///         3. orderItems_15_itemPrice
        /// 3. [fieldName] Field name must be separated from the [uniqueid] by a _ and will matched to 
        ///         the actual field name in the object being created for the array
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <param name="prefix">[prefix] could be anything. This is just a way to identify all items that belong in this array</param>
        /// <param name="ignoreUniqueId">If specified ignores this uniqueId. Useful when in template situations</param>
        /// <returns>A List of the type specified filled from the posted form</returns>
        public List<T> FillCollection<T>(string prefix)
        {
            return FillCollection<T>(prefix, "");
        }

        /// <summary>
        /// Cretes a collection of objects from posted data. The posted data must be in this format
        /// [prefix]_[uniqueId]_[fieldname]
        /// 1. [prefix] could be anything. This is just a way to identify all items that belong in this array
        /// 2. [uniqueId] identifies the collection item instance
        ///         for instance if I were filling a list of order items then I would have three fields
        ///         1. orderItems_15_itemCount
        ///         2. orderItems_15_productId
        ///         3. orderItems_15_itemPrice
        /// 3. [fieldName] Field name must be separated from the [uniqueid] by a _ and will matched to 
        ///         the actual field name in the object being created for the array
        /// </summary>
        /// <typeparam name="T">The array item type</typeparam>
        /// <param name="prefix">[prefix] could be anything. This is just a way to identify all items that belong in this array</param>
        /// <param name="ignoreUniqueId">If specified ignores this uniqueId. Useful when in template situations</param>
        /// <returns>A List of the type specified filled from the posted form</returns>
        public List<T> FillCollection<T>(string prefix, string ignoreUniqueId)
        {
            // Add the _ underscore to the end of the prefix
            if (!prefix.EndsWith("_")) prefix += "_";

            // Create the new list
            var newList = new List<T>();

            // Get the posted values
            var values = RequestValues;

            // Get the existing items
            var itemIds = new List<string>();
            foreach (string key in values.Keys)
            {
                if (key.StartsWith(prefix) && key.Substring(prefix.Length).IndexOf("_") > -1)
                {
                    // [prefix]_[key]_[fieldname]
                    var idAndFieldName = key.Substring(prefix.Length); // [key]_[fieldname]
                    var fieldName = idAndFieldName.Substring(idAndFieldName.IndexOf("_")+1);
                    var itemId = idAndFieldName.Substring(0, idAndFieldName.Length - fieldName.Length - 1);

                    // Ignore unique id if specified
                    if (!string.IsNullOrEmpty(ignoreUniqueId) && itemId == ignoreUniqueId) continue;
                    
                    // Add the series to be processed
                    if (!itemIds.Contains(itemId))
                    {
                        itemIds.Add(itemId);
                    }
                }
            }

            // Process all the items
            foreach (var itemId in itemIds)
            {
                newList.Add(FillObject<T>(prefix + itemId + "_"));
            }
            
            // Return the new created list
            return newList;
        }

        #endregion

    }
}
