using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bigfoot.Web.Helpers
{

    #region JSHelper

    /// <summary>
    /// This class helps us to create javascript code using server side code for commonly performed 
    /// operation. Although I do not like inline javascript, there are some times in the interested
    /// of productivity when it helps to make these items accessbile and repeatable. The funcitons  
    /// in this helper are particularly useful when using in conjunction with the HtmlHelper and
    /// tag builder classes.
    /// 
    /// It abstracts repetive tasks when making ajax calls, validating input, and adding functionality
    /// not currently present in asp.net in an easily repeatable manner.
    /// </summary>
    public class JSHelper
    {

        /// <summary>
        /// Factory method
        /// </summary>
        public static JSHelper Create()
        {
            return new JSHelper();
        }

        /// <summary>
        /// Returns a string ready to be used in a literal expression in javascript
        /// </summary>
        public string CleanString(string value)
        {
            return JSBuilder.CleanString(value);
        }

        /// <summary>
        /// Outputs a jquery statement to show an element
        /// </summary>
        /// <param name="elementId">The elment id with or without the # selector</param>
        public JQHelper ShowElement(string elementId)
        {
            return JQHelper.Create().SelectID(elementId).show();
        }

        /// <summary>
        /// Outputs a jquery statement to show an element and hide another
        /// </summary>
        /// <param name="showelement">The elment id to show with or without the # selector</param>
        /// <param name="hideelement">The element id to show with or without the # selector</param>
        public JQHelper ShowAndHideElement(string showelement, string hideelement)
        {
            return JQHelper.Create().SelectID(showelement).show().SelectID(hideelement).hide();
        }

        /// <summary>
        /// Outputs a jquery statement to hide an element
        /// </summary>
        /// <param name="elementId">The elment id with or without the # selector</param>
        public JQHelper HideElement(string elementId)
        {
            return JQHelper.Create().SelectID(elementId).hide();
        }

        /// <summary>
        /// Outputs a jquery statement to clear the innerHtml of an element
        /// </summary>
        /// <param name="elementId">The elment id with or without the # selector</param>
        public JQHelper ClearElement(string elementId)
        {
            return JQHelper.Create().SelectID(elementId).clear();
        }

        /// <summary>
        /// Outputs a jquery statement to show an element if it is hidden, or hide it if it is showing
        /// </summary>
        /// <param name="elementId">The elment id with or without the # selector</param>
        public JQHelper ToggleElement(string elementId)
        {
            return JQHelper.Create().SelectID(elementId).toggle();
        }


        #region "SubmitForm"

        /// <summary>
        /// Submits form, and optionally does validation.
        /// </summary>
        public JSBuilder SubmitForm(string formId, bool Validate = true)
        {
            var js = JSBuilder.Create();
            js.Add("bf.submitForm({0}, {1})", formId, JSObject.Create()
                                                              .Add("validate", true)
                                                              .ToString());
            return js;
        }


        /// <summary>
        /// Submits the form and does validation on the inputs contained within the containerId
        /// </summary>
        public JSBuilder SubmitForm_ValidateContainer(string formId, string containerId)
        {
            var js = JSBuilder.Create();
            js.Add("bf.submitForm({0}, {1})", formId, JSObject.Create()
                                                              .Add("validate", true)
                                                              .AddString("validatePartialFormId", containerId)
                                                              .ToString());
            return js;
        }

        #endregion


        #region "SubmitParentForm"

        /// <summary>
        /// Submits the parent form of the the elment that invokes this script. 
        /// Optionally does validation
        /// </summary>
        public JSBuilder SubmitParentForm(bool validate = true)
        {
            var js = JSBuilder.Create();
            js.Add("bf.submitParentForm(this, {0})", JSObject.Create()
                                                             .Add("validate", true)
                                                             .ToString());
            return js;
        }

        /// <summary>
        /// Submits the parent form of the the elment that invokes this script. Validates the input elements within the specified container
        /// </summary>
        public JSBuilder SubmitParentForm_ValidateContainer(string containerId)
        {   
            var js = JSBuilder.Create();
            js.Add("bf.submitParentForm(this, {0})", JSObject.Create()
                                                             .Add("validate", true)
                                                             .AddString("validatePartialFormId", containerId)
                                                             .ToString());
            return js;
        }

        #endregion


        #region "SubmitParentFormViaAjax"

        /// <summary>
        /// Submits the parent form of this element via ajax to the url specified, and updates the supplied updatePanel with the value
        /// returned from the server. Optionally does validation
        /// </summary>
        public JSBuilder SubmitParentFormViaAjax(string postUrl, string updatePanel, bool validate = true, JSObject ajaxFormSubmitOptions = null)
        {
            var script = new JSBuilder();
            script.AddLine("bf.submitParentForm(this,{0})", JSObject.Create()
                                                                    .Add("viaAjax", true)
                                                                    .Add("validate", validate)
                                                                    .AddString("ajaxPostUrl", postUrl)
                                                                    .AddString("updatePanel", JQHelper.GetElementID(updatePanel))
                                                                    .Add("ajaxOptions", ajaxFormSubmitOptions)
                                                                    .ToString());
            return script;
        }

        /// <summary>
        /// Submits the parent form of this element via ajax to the url specified, and updates the supplied updatePanel with the value
        /// returned from the server. Does validation for the inputs contained within a the specified containerId.
        /// </summary>
        public JSBuilder SubmitParentFormViaAjax_ValidateContainer(string postUrl, string updatePanel, string containerId, JSObject ajaxFormSubmitOptions = null)
        {
            var script = new JSBuilder();
            script.AddLine("bf.submitParentForm(this,{0})", JSObject.Create()
                                                                    .Add("viaAjax", true)
                                                                    .Add("validate", true)
                                                                    .AddString("validatePartialFormId", containerId)
                                                                    .AddString("ajaxPostUrl", postUrl)
                                                                    .AddString("updatePanel", JQHelper.GetElementID(updatePanel))
                                                                    .Add("ajaxOptions", ajaxFormSubmitOptions)
                                                                    .ToString());
            return script;

        }
        
        #endregion


        #region "SubmitFormViaAjax"
        
        /// <summary>
        /// Submits specified form via ajax to the url specified, and updates the supplied updatePanel with the value
        /// returned from the server. Can optionally perform validation
        /// </summary>
        public JSBuilder SubmitFormViaAjax(string formId, string postUrl, string updatePanel, bool validate = true, JSObject ajaxFormSubmitOptions = null)
        {
            var script = new JSBuilder();
            script.AddLine("bf.submitForm('{0}',{1})", formId, JSObject.Create()
                                                                        .Add("viaAjax", true)
                                                                        .Add("validate", validate)
                                                                        .AddString("ajaxPostUrl", postUrl)
                                                                        .AddString("updatePanel", updatePanel)
                                                                        .Add("ajaxOptions", ajaxFormSubmitOptions)
                                                                        .ToString());
            return script;
        }

        /// <summary>
        /// Submits specified form via ajax to the url specified, and updates the supplied updatePanel with the value
        /// returned from the server. It validates the inputs within the specified container.
        /// </summary>
        public JSBuilder SubmitFormViaAjax_ValidateContainer(string formId, string postUrl, string updatePanel, string containerId, JSObject ajaxFormSubmitOptions = null)
        {
            var script = new JSBuilder();
            script.AddLine("bf.submitForm('{0}',{1})", formId, JSObject.Create()
                                                                        .Add("viaAjax", true)
                                                                        .Add("validate", true)
                                                                        .AddString("validatePartialFormId", containerId)
                                                                        .AddString("ajaxPostUrl", postUrl)
                                                                        .AddString("updatePanel", updatePanel)
                                                                        .Add("ajaxOptions", ajaxFormSubmitOptions)
                                                                        .ToString());
            return script;
        }
        
        #endregion


        /// <summary>
        /// Sets the focus to the element specified
        /// </summary>
        public string SetFocus(string elementId)
        {
            return JQHelper.Create().SelectID(elementId).AddCommand("focus()").ToScriptBlock();
        }

        /// <summary>
        /// Performs an ajax request and loads the content of the supplied url into the elementid specified.
        /// You can also optionally toggle the contents, this means that if the content is showing the
        /// ajax call is not performed and the elementid's content is cleared and hidden. When the element
        /// is hidden then the ajax request happens and the the fills the element and shows it
        /// </summary>
        /// <param name="url">Url to request from</param>
        /// <param name="elementid">The element that will hold the response</param>
        /// <param name="toggle">When true retreives the content and fills the element only when it is not showing</param>
        public JSBuilder Load(string url, string elementid, bool toggle)
        {
            var js = new JSBuilder();
            js.AddLine("bf.loadUrl('{0}','{1}', {2})", JSBuilder.CleanString(url), 
                                                        JSBuilder.CleanString(elementid), 
                                                        toggle.ToString().ToLowerInvariant());
            return js;
        }


        /// <summary>
        /// Creates an html element into a virtual form, or a partial form. It outputs the correct scripts to wrap validation
        /// and form submission around the inputs inside the supplied virtual form element. By default all elements inside the virtual form
        /// with a class assigned named causesValidation trigger a validation call whenever clicked
        /// i.e. to setup a partial form you call CreatePartialForm("addUser") where addUser is the name of a div
        ///      to then submit the form you simply call SubmitPartialForm("addUser", "/users/createuser") when this happens only the inputs 
        ///      within the div.addUser are validated and submitted via ajax to url specified
        /// </summary>
        /// <param name="formId">The element id that wraps the group of inputs you want to make into a form</param>
        /// <param name="addScriptTag">Determines whether to add the script tag</param>
        /// <returns>Returns a function named ValidateGroup_{formId} which is called by the SubmitPartialForm method to validate before submitting a form</returns>
        public string CreatePartialForm(string formId, bool addScriptTag = true)
        {
            var js = JSBuilder.Create();
            js.AddLine("bf.createPartialForm('{0}')", formId);
            return js.ToString(addScriptTag); 
        }
        
        /// <summary>
        /// Submits a previously setup partial form through CreatePartialForm method to the url specified
        /// </summary>
        /// <param name="formId">The element id containing the partial form</param>
        /// <param name="url">The url you want to submit the partial form to</param>
        /// <param name="validate">Determines weather to validate before submission or not. By default all elements within
        /// the parial form that have the class causesValidation trigger a validation call when clicked</param>
        public AjaxRequest SubmitPartialForm(string formId, string url, bool validate = true)
        {
            var ajax = AjaxRequest.Create(url);
            ajax.Post().PartialForm(formId);
            if (validate)
                ajax.Validate();
            return ajax;
        }

        public string CreateDatePicker(string inputName)
        {
            return JQID(inputName).AddCommand("datepicker()").ToScriptBlock();
        }

        public string CreateDatePicker(string inputName, string dateFormat)
        {
            return JQID(inputName).AddCommand("datepicker({dateFormat: '" + dateFormat + "'})").ToScriptBlock();
        }

        /// <summary>
        /// Makes a jquery UI button with the provided selector and uiClass
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        public string MakeJQButton(string selector)
        {
            return MakeJQButton(selector, "", true);
        }

        /// <summary>
        /// Makes a jquery UI button with the provided with a 
        /// .jqButton selector and ui-icon-circle-triangle-e as the icon
        /// </summary>
        public string MakeJQButton()
        {
            return MakeJQButton(".jqButton", "ui-icon-circle-triangle-e", true);
        }

        /// <summary>
        /// Makes a jquery UI button with the provided selector and uiClass
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        /// <param name="uiClass">The UI Class if any to assign to the button</param>
        /// <returns></returns>
        public string MakeJQButton(string selector, string uiClass)
        {
            return MakeJQButton(selector, uiClass, true);
        }

        /// <summary>
        /// Makes a jquery UI button with the provided selector and uiClass
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        /// <param name="uiClass">The UI Class if any to assign to the button</param>
        /// <param name="includeScriptTag">Determines whether to include the script tag</param>
        /// <returns></returns>
        public string MakeJQButton(string selector, string uiClass, bool includeScriptTag)
        {
            var builder = (string.IsNullOrEmpty(uiClass)) ? JQ.Select(selector).AddCommand("button()")
                                                          : JQ.Select(selector).AddCommand("button({ icons: {primary: '" + uiClass + "'} })");
            return includeScriptTag ? builder.ToScriptBlock()
                                    : builder.ToString();
        }

        /// <summary>
        /// Configures a jquery UI Tabs with the provided selector
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        public string MakeJQTabs(string selector)
        {
            return MakeJQTabs(selector, string.Empty, null, true);
        }

        /// <summary>
        /// Configures a jquery UI Tabs with the provided selector
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        /// <param name="selectFunction">The function to be called when a tab is selected. Receives two parameters (event, ui)</param>
        public string MakeJQTabs(string selector, string selectFunction)
        {
            return MakeJQTabs(selector, selectFunction, null, true);
        }

        /// <summary>
        /// Configures a jquery UI Tabs with the provided selector
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        /// <param name="selectFunction">The function to be called when a tab is selected. Receives two parameters (event, ui)</param>
        /// <param name="selectedTabIndex">The tab to select by default</param>
        public string MakeJQTabs(string selector, string selectFunction, int selectedTabIndex)
        {
            return MakeJQTabs(selector, selectFunction, new JSObject().Add("selected", selectedTabIndex), true);
        }

        /// <summary>
        /// Configures a jquery UI Tabs with the provided selector
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        /// <param name="includeScriptTag">Determines whether to include the script tag</param>
        /// <param name="selectFunction">The function to be called when a tab is selected. Receives two parameters (event, ui)</param>
        public string MakeJQTabs(string selector, string selectFunction, bool includeScriptTag)
        {
            return MakeJQTabs(selector, selectFunction, null, includeScriptTag);
        }

        /// <summary>
        /// Configures a jquery UI Tabs with the provided selector
        /// </summary>
        /// <param name="selector">The jQuery selector</param>
        /// <param name="otherOptions">Add options to the tabs</param>
        /// <param name="includeScriptTag">Determines whether to include the script tag</param>
        /// <param name="selectFunction">The function to be called when a tab is selected. Receives two parameters (event, ui)</param>
        public string MakeJQTabs(string selector, string selectFunction, JSObject otherOptions, bool includeScriptTag)
        {
            var options = new JSObject();
            if (!string.IsNullOrEmpty(selectFunction)) options.Add("select", selectFunction);
            if (otherOptions != null) options.Merge(otherOptions);

            var builder = options.IsEmpty() ? JQ.Select(selector).AddCommand("tabs()") :
                                              JQ.Select(selector).AddCommand("tabs(" + options + ")");
            return includeScriptTag ? builder.ToScriptBlock()
                                    : builder.ToString();
        }
        
        private JQHelper JQ { get { return new JQHelper(); } }
        private JQHelper JQID(string id) { return JQ.SelectID(id); }
    }

    #endregion

    #region JSBuilder
    /// <summary>
    /// This class helps us to create javascript code using server side code for commonly performed 
    /// operation. Although I do not like inline javascript, there are some times in the interested
    /// of productivity when it helps to make these items accessbile and repeatable. The funcitons  
    /// in this helper are particularly useful when using in conjunction with the HtmlHelper and
    /// tag builder classes.
    /// 
    /// It abstracts repetive tasks when making ajax calls, validating input, and adding functionality
    /// not currently present in asp.net in an easily repeatable manner.
    /// 
    /// Like most of the helpers this helper has a fluid interface as well. You can compose 
    /// functionality by calling methods on it before rendering the script.
    /// </summary>
    public class JSBuilder
    {

        private StringBuilder _data = new StringBuilder();

        /// <summary>
        /// This is a static function to easily create a new JSHelper function inline
        /// </summary>
        public static JSBuilder Create()
        {
            return new JSBuilder();
        }

        /// <summary>
        /// Returns a properly formatted string to be include in Javascript, it also appends the quotes to the string
        /// this function also calls the CleanString to make sure any characters not allowed in javascript are safely scaped
        /// </summary>
        /// <param name="value">The string to clean without the beggining and ending literal quotes</param>
        public static string GetString(string value, bool includequotes = false)
        {
            var newvalue = CleanString(value);
            return (includequotes) ? string.Format("'{0}'", newvalue) : newvalue;            
        }

        /// <summary>
        /// Escapes any characters not allowed in javascript
        /// </summary>
        public static string CleanString(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var sb = new StringBuilder();
            foreach (var c in value)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\'':
                        sb.Append("\\\'");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        var i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds raw string to the JS builder. Be aware of line termination etc. As this does not do any of this for you
        /// it simple calls stringbuilder.append on the internal object. User other methods like AddLine to add a complete oneline statment to the script
        /// </summary>
        /// <param name="value">Javascript to add to the builder. i.e. jQuery("#test").addClass("newclass"); </param>
        public JSBuilder Add(string value)
        {
            _data.Append(value);
            return this;
        }

        /// <summary>
        /// Adds raw string to the JS builder using string.format. Be aware of line termination etc. As this does not do any of this for you
        /// it simple calls stringbuilder.append on the internal object. User other methods like AddLine to add a complete oneline statment to the script
        /// </summary>
        /// <param name="value">Javascript to add to the builder. i.e. jQuery("#test").addClass("{0}"); </param>
        /// <param name="params">List of values to use for the string.format</param>
        public JSBuilder Add(string value, params string[] @params)
        {
            _data.AppendFormat(value, @params);
            return this;
        }

        /// <summary>
        /// Adds a complete line of javascript including properly terminating the line with the semicollon
        /// </summary>
        /// <param name="line">Javascript line i.e. jQuery("test").addClass("one")</param>
        public JSBuilder AddLine(string line)
        {
            return Add(" " + line).EndLine();
        }

        /// <summary>
        /// Adds a complete line of javascript including properly terminating the line with the semicollon using string.format
        /// </summary>
        /// <param name="line">Javascript line i.e. jQuery("test").addClass("{0}")</param>
        /// <param name="params">The values to merge using string.format</param>
        public JSBuilder AddLine(string line, params string[] @params)
        {
            return Add(" " + line, @params).EndLine();
        }

        /// <summary>
        /// Adds a semicollon to the builder
        /// </summary>
        public JSBuilder EndLine()
        {
            if (!_data.ToString().Trim().EndsWith(";"))
                Add("; ");
            return this;
        }

        /// <summary>
        /// Determines if the script builder is empty, meaning no commands have been added ot it.
        /// </summary>
        public bool IsEmpty()
        {
            return _data.Length == 0;
        }

        /// <summary>
        /// Wraps the current script with the code specified
        /// </summary>
        /// <param name="format">The format to use. {0} is reserved for the current contents of the script.  
        /// i.e. if(value == {1}) {{ {0} }}; Use double quotes to escape { and }</param>
        /// <param name="params">Values to merge with your script.</param>
        /// <returns></returns>
        public JSBuilder Wrap(string format, params string[] @params)
        {
            // Create the parameter object {0} is reserved for the current string
            var newparams = new List<string> { _data.ToString() };
            if (@params.Length != 0)
            {
                newparams.AddRange(@params);
            }

            // Format the value
            var newvalue = string.Format(format, newparams.ToArray());

            // Replace existing value
            _data = new StringBuilder(newvalue);
            return this;
        }

        /// <summary>
        /// Returns the result of the built script
        /// </summary>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Returns the result of the built script
        /// </summary>
        public string ToString(bool addScriptTag)
        {
            if (addScriptTag)
            {
                var returnVal = new StringBuilder();
                returnVal.AppendLine("<script type='text/javascript'>");
                returnVal.AppendLine();
                returnVal.AppendLine(_data.ToString());
                returnVal.AppendLine();
                returnVal.AppendLine("</script>");
                return returnVal.ToString();
            }
            else
            {
                return _data.ToString();
            }
            
        }

        /// <summary>
        /// Returns the result of the built script in a way that is suitable for inline script. 
        /// i.e. javascript:yourscript;
        /// </summary>
        public string ToInline()
        {
            var script = ToString().TrimEnd();
            if (!script.EndsWith(";")) script += "; ";
            return "javascript:" + script + "void(0);";
        }

        /// <summary>
        /// Shortcut function that creates a JQ object
        /// </summary>
        public JQHelper JQ { get { return JQHelper.Create(); } }

    }
    #endregion

    #region JSObject
    /// <summary>
    /// This class is a helper class to build a JSON string. It is very useful when generating script server side.
    /// </summary>
    public class JSObject
    {

        /// <summary>
        /// Specifies the type of value you are adding to the JSObject. When other the value is not escaped or quoted as you 
        /// would have to do for a string.Y ou can nest JSObjects by adding a property to it, and passing as the value the 
        /// ToString method call of another JSObject
        /// </summary>
        public enum ValueTypeEnum
        {
            String,
            Other
        }

        /// <summary>
        /// Factory method to create a JSObject
        /// </summary>
        public static JSObject Create()
        {
            return new JSObject();
        }


        private Dictionary<string, string> _data = new Dictionary<string, string>();

        /// <summary>
        /// Adds a property to the json object. It asumes that it is not a string so it ouputs the value directly. 
        /// if the value is empty then it oupts null as the value for the property. You can nest JSObjects by
        /// adding a property to it, and passing as the value the ToString method call of another JSObject
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value</param>
        public JSObject Add(string name, string value)
        {
            if (value == string.Empty)
            {
                value = "null";
            }
            return Add(name, value, ValueTypeEnum.Other);
        }

        /// <summary>
        /// Adds a property to the json object. It asumes that it is not a string so it ouputs the value directly. 
        /// if the value is empty then it does not add it. You can nest JSObjects by
        /// adding a property to it, and passing as the value the ToString method call of another JSObject
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value</param>
        public JSObject AddIfNotEmpty(string name, string value)
        {
            return value == string.Empty ? this
                                         : Add(name, value, ValueTypeEnum.Other);
        }

        /// <summary>
        /// Adds a property to the json object. It asumes that it is not a string so it ouputs the value directly. 
        /// if the value is empty then it oupts null as the value for the property. You can nest JSObjects by
        /// adding a property to it, and passing as the value the ToString method call of another JSObject
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value</param>
        public JSObject Add(string name, int value)
        {
            return Add(name, value.ToString(), ValueTypeEnum.Other);
        }

        /// <summary>
        /// Adds a property to the json object. It asumes that it is not a string so it ouputs the value directly. 
        /// if the value is empty then it oupts null as the value for the property. You can nest JSObjects by
        /// adding a property to it, and passing as the value the ToString method call of another JSObject
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value</param>
        public JSObject Add(string name, bool value)
        {
            return Add(name, value.ToString().ToLowerInvariant(), ValueTypeEnum.Other);
        }

        /// <summary>
        /// Adds a property to the json object. It asumes that it is not a string so it ouputs the value directly. 
        /// if the value is empty then it oupts null as the value for the property. You can nest JSObjects by
        /// adding a property to it, and passing as the value the ToString method call of another JSObject
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value</param>
        public JSObject Add(string name, JSObject value)
        {
            if (value == null) return this;
            return Add(name, value.ToString(), ValueTypeEnum.Other);
        }

        /// <summary>
        /// Adds a property to the json object as a string so it properly escapes it and quotes it 
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value string will be escaped and quoted</param>
        public JSObject AddString(string name, string value)
        {
            return Add(name, value, ValueTypeEnum.String);
        }

        /// <summary>
        /// Merges a string into the json object. It properly escapes it and quotes it 
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value string will be escaped and quoted</param>
        public JSObject MergeString(string name, string value)
        {
            return Merge(name, value, ValueTypeEnum.String);
        }

        /// <summary>
        /// Adds a property to the json object. If the valuetype is a string it is escaped and quoted. If it is not it ouputs the value directly. 
        /// if the value is empty then it oupts null as the value for the property
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value. string will be escaped and quoted</param>
        /// <param name="ValueType">The type of value being added. If a string, it will be escaped and quoted</param>
        public JSObject Add(string name, string value, ValueTypeEnum ValueType)
        {
            if (ValueType == ValueTypeEnum.String) value = JSBuilder.GetString(value, true);
            Data.Add(name, value);
            return this;
        }

        /// <summary>
        /// Merges a property into to the json object, if the property exists it overrides it otherwise it adds it. 
        /// If the valuetype is a string it is escaped and quoted. If it is not it ouputs the value directly. 
        /// if the value is empty then it oupts null as the value for the property
        /// </summary>
        /// <param name="name">Property name {name}: {value}</param>
        /// <param name="value">Property value. string will be escaped and quoted</param>
        /// <param name="ValueType">The type of value being added. If a string, it will be escaped and quoted</param>
        public JSObject Merge(string name, string value, ValueTypeEnum ValueType)
        {
            if (ValueType == ValueTypeEnum.String) value = JSBuilder.GetString(value);
            if (HasMember(name))
                Data[name] = value;
            else
                Data.Add(name, value);
            return this;
        }

        /// <summary>
        /// This is the list of properties that will output
        /// </summary>
        public Dictionary<string, string> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Merges another JSObject into this one
        /// </summary>
        public JSObject Merge(JSObject obj)
        {
            if (obj == null) return this;
            foreach (var key in obj.Data.Keys)
            {
                _data.Add(key, obj.Data[key]);
            }
            return this;
        }

        /// <summary>
        /// Counts how many properties have been added to this json object
        /// </summary>
        public int Count()
        {
            return _data.Count;
        }

        public bool HasMember(string memberName)
        {
            return _data.ContainsKey(memberName);
        }

        public string GetValue(string memberName)
        {
            return Data.ContainsKey(memberName) ? Data[memberName] : "";
        }

        /// <summary>
        /// Determines if no data has been added to the object
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return Count() == 0;
        }

        /// <summary>
        /// Properly oupts the JSON sorrounded by { } braces
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(" { ");
            var first = true;
            foreach (var key in _data.Keys)
            {
                if (first == false)
                {
                    sb.Append(", ");
                }
                first = false;
                sb.AppendFormat("{0}: {1}", key, _data[key]);
            }
            sb.Append(" } ");
            return sb.ToString();
        }
    }
    #endregion

    #region JQHelper
    /// <summary>
    /// This is a simple class to aid us in the creation of jQuery scripts
    /// </summary>
    public class JQHelper
    {

        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Determines if the script is empty, meaning no commands have been added to it
        /// </summary>
        public bool IsEmpty()
        {
            return builder.ToString() == string.Empty;
        }

        /// <summary>
        /// Factory method to create a new JQHelper
        /// </summary>
        public static JQHelper Create()
        {
            return new JQHelper();
        }

        /// <summary>
        /// Creates a jQuery helper with the specified element id selector
        /// </summary>
        public static JQHelper ID(string elementId)
        {
            var id = elementId.StartsWith("#") ? elementId : "#" + elementId;
            return Create().Select(id);
        }

        /// <summary>
        /// Selects a set of elements using a jquery selector. If the JQHelper has other 
        /// commands and the script is not ended with a ; it adds it before adding this statement
        /// Appends: [;]jQuery('selector')
        /// </summary>
        /// <param name="selector">Any jQuery selector</param>
        public JQHelper Select(string selector)
        {
            ClosePreviousStatement();
            builder.AppendFormat("jQuery('{0}')", selector);
            return this;
        }

        /// <summary>
        /// Adds a ; if needed to the previous statement
        /// </summary>
        private void ClosePreviousStatement()
        {
            if (IsEmpty() == false && builder.ToString().TrimEnd().EndsWith(";") == false)
            {
                builder.Append(";");
            }
        }

        /// <summary>
        /// Selects a set of elements using a jquery element id selector. If the JQHelper has other 
        /// commands and the script is not ended with a ; it adds it before adding this statement
        /// Appends: [;]jQuery('#selector')
        /// </summary>
        /// <param name="selector">And element id selector with or without the #</param>
        public JQHelper SelectID(string selector)
        {
            return Select(GetElementID(selector));
        }

        /// <summary>
        /// Selects all input elements within the contianer except file inputs. If the JQHelper has other 
        /// commands and the script is not ended with a ; it adds it before adding this statement
        /// Appends: [;]jQuery('#container :input, #container select, #container textarea')
        /// </summary>
        /// <param name="selector">The selector containing the fields. This could any jquery selector</param>
        public JQHelper SelectFields(string selector)
        {
            return Select(string.Format("{0} :input, {0} select, {0} textarea", selector));
        }


        /// <summary>
        /// Selects the parent form of the executing element. If the JQHelper has other 
        /// commands and the script is not ended with a ; it adds it before adding this statement
        /// Appends: [;]jQuery(this).parents('form:eq[0]')
        /// </summary>
        public JQHelper SelectParentForm()
        {
            ClosePreviousStatement();
            builder.Append("jQuery(this).parents('form:eq(0)')");
            return this;
        }



        /// <summary>
        /// Seriliazes the fields within the selection
        /// Appends: [.]fieldSerialize()
        /// </summary>
        public JQHelper fieldSerialize()
        {
            return AddCommand("fieldSerialize()");
        }

        /// <summary>
        /// Adds a class to the select element(s). 
        /// Appends: [.]addClass('className')
        /// </summary>
        /// <param name="className">Class name to add</param>
        public JQHelper addClass(string className)
        {
            return AddCommand("addClass({0})", GetString(className));
        }

        /// <summary>
        /// Removes a class from the select element(s). 
        /// Appends: [.]removeClass('className')
        /// </summary>
        /// <param name="className">Class name to add</param>
        public JQHelper removeClass(string className)
        {
            return AddCommand("removeClass({0})", GetString(className));
        }

        /// <summary>
        /// Removes the matched elements. 
        /// Appends: [.]remove()
        /// </summary>
        public JQHelper remove()
        {
            return AddCommand("remove()");
        }

        /// <summary>
        /// Clears the fields matched by the specified selector. 
        /// Appends: [.]clearFields()
        /// </summary>
        public JQHelper clearFields()
        {
            return AddCommand("clearFields()");
        }

        /// <summary>
        /// Shows the selected element(s)
        /// Appends: [.]show()
        /// </summary>
        public JQHelper show()
        {
            return AddCommand("show()");
        }

        /// <summary>
        /// Hides the selected element(s)
        /// Appends: [.]hide()
        /// </summary>
        public JQHelper hide()
        {
            return AddCommand("hide()");
        }

        /// <summary>
        /// Toggles the visiblity of the selected element(s)
        /// Appends: [.]toggle()
        /// </summary>
        public JQHelper toggle()
        {
            return AddCommand("toggle()");
        }

        /// <summary>
        /// Adds or removes a class when not found from the selected element(s)
        /// Appends: [.]toggleClass('className')
        /// </summary>
        public JQHelper toggleClass(string className)
        {
            return AddCommand("toggleClass(" + JSBuilder.GetString(className) + ")");
        }

        /// <summary>
        /// Removes the html conents and hides the selected element(s)
        /// Appends: [.]html('').hide()
        /// </summary>
        public JQHelper clear()
        {
            AddCommand("html('')");
            AddCommand("hide()");
            return this;
        }

        /// <summary>
        /// Removes the value of the field
        /// Appends: [.]val('')
        /// </summary>
        public JQHelper clearVal()
        {
            return AddCommand("val('')");
        }

        /// <summary>
        /// Removes the value of the field
        /// Appends: [.]val('')
        /// </summary>
        public JQHelper val()
        {
            return AddCommand("val()");
        }

        /// <summary>
        /// Calls the trigger function and triggers a certain event
        /// Appends: [.]trigger('trigger')
        /// </summary>
        public JQHelper trigger(string trigger)
        {
            AddCommand("trigger('" + trigger + "')");
            return this;
        }

        /// <summary>
        /// Calls the ajaxSubmit with the specified url and updates the specified element with the returned value
        /// url passed in will be properly escaped and quoted, so you don't have to clean it up
        /// Appends: [.]ajaxSubmit({ url: '[url]', target: '#[updateElement]' })
        /// </summary>
        public JQHelper ajaxSubmit(string url, string updateElement)
        {
            var options = new JSObject();
            options.AddString("url", url).AddString("target", GetElementID(updateElement));
            return ajaxSubmit(options);
        }

        /// <summary>
        /// Calls the ajaxSubmit with the specified url and updates the specified element with the returned value. 
        /// It also appends the other options if specified.
        /// url passed in will be properly escaped and quoted, so you don't have to clean it up
        /// Appends: [.]ajaxSubmit({ url: '[url]', target: '#[updateElement]' [,[otherOptions]]})
        /// </summary>
        public JQHelper ajaxSubmit(string url, string updateElement, JSObject otherOptions)
        {
            var options = new JSObject();
            options.AddString("url", url).AddString("target", GetElementID(updateElement));
            if (otherOptions != null)
                options.Merge(otherOptions);

            return ajaxSubmit(options);
        }

        /// <summary>
        /// Calls the ajaxSubmit with the specified set of options
        /// JSBuilder.GetString will be called on the url passed in, so you don't have to clean it up
        /// Appends: [.]ajaxSubmit({ url: '[url]', target: '#[updateElement]' [,[otherOptions]]})
        /// </summary>
        public JQHelper ajaxSubmit(JSObject options)
        {
            return AddCommand("ajaxSubmit(" + options + ")");
        }

        //JQ.SelectID(formId).AddCommand("ajaxSubmit({ target: '#" + updatePanel + "', url: " + postUrl + "})").ToString()

        /// <summary>
        /// Calls the html function and sets it to the expression
        /// Appends: [.]html(value)
        /// </summary>
        public JQHelper html(string expression)
        {
            AddCommand("html(" + expression + ")");
            return this;
        }

        /// <summary>
        /// Loads the content of the url
        /// Appends: [.]load('url')
        /// </summary>
        /// <param name="url">The url to load unescaped</param>
        public JQHelper load(string url)
        {
            AddCommand("load(" + GetString(url) + ")");
            return this;
        }

        /// <summary>
        /// Loads the content of the url
        /// Appends: [.]load('url', callback)
        /// </summary>
        /// <returns></returns>
        public JQHelper load(string url, string callback)
        {
            AddCommand("load(" + GetString(url) + ", " + callback + ")");
            return this;
        }

        /// <summary>
        /// Set the focus for the selected element
        /// Appends: [.]focus()
        /// </summary>
        public JQHelper focus()
        {
            return AddCommand("focus()");
        }

        /// <summary>
        /// Runs the provided javascript for each matched element
        /// Appends: [.]each(function(i, item) { [Your Javascript] });
        /// </summary>
        public JQHelper each(string javascript)
        {
            return AddCommand("each(function(i, item){ " + javascript + " })");
        }

        /// <summary>
        /// Set the focus for the specified element
        /// Appends: [.]focus()
        /// </summary>
        public JQHelper focus(string elementId)
        {
            return SelectID(elementId).focus();
        }

        /// <summary>
        /// Serializes the form into an array to be used for posting
        /// Appends: [.]formToArray()
        /// </summary>
        /// <returns></returns>
        public JQHelper formToArray()
        {
            return AddCommand("formToArray()");
        }

        /// <summary>
        /// Adds a command to the selected element(s). It uses the String.Format to merge command and possible parameters
        /// Appends: [.]command('with possible values')
        /// i.e. AddCommand("addClass('{0}')", "class1 class2") appends [.]addClass('class1 class2')
        /// </summary>
        /// <param name="statement">The command name with possible String.Format {} braces to merge with the params</param>
        /// <param name="params">Values to merge with the command. When left empty then nothing is merged</param>
        public JQHelper AddCommand(string statement, params string[] @params)
        {
            if (builder.ToString().EndsWith(".") == false)
                builder.Append(".");
            if (@params != null && @params.Length > 0)
            {
                builder.AppendFormat(statement, @params);
            }
            else
            {
                builder.Append(statement);
            }
            return this;
        }

        /// <summary>
        /// Returns a scaped and properly quoted string for inclussion in javascript
        /// </summary>
        /// <param name="Data">The string to escape and quote</param>
        private static string GetString(string Data)
        {
            return JSBuilder.GetString(Data);
        }

        /// <summary>
        /// Retrives an element id jquery selector. It adds the # at the beggining of the element id if not there
        /// </summary>
        public static string GetElementID(string elementid)
        {
            elementid = elementid.Trim();
            if (elementid.StartsWith("#") == false)
            {
                elementid = "#" + elementid;
            }
            return elementid;
        }

        /// <summary>
        /// Outpus the script ready for inclussion in a page. Does not output the script tag or the jquery on ready by default.
        /// it also does not add a semicolon at the end by default
        /// </summary>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Outpus the script ready for inclussion in a page. Does not output the script tag or the jquery on ready by default.
        /// it also adds a semicolon at the end if requted and it isn't there
        /// </summary>
        public string ToString(bool AddSemicolonAtTheEnd)
        {
            if (AddSemicolonAtTheEnd && builder.ToString().Trim().EndsWith(";") == false)
            {
                builder.Append(";");
            }
            return builder.ToString();
        }

        public string ToScriptBlock()
        {
            var script = new StringBuilder();
            script.Append(" <script type='text/javascript'> ");
            script.Append("jQuery(document).ready(function(){ ");
            script.Append("   " + ToString(true));
            script.Append(" }); ");
            script.Append(" </script>");
            return script.ToString();
        }

    }
    #endregion

    #region AjaxRequest

    /// <summary>
    /// This helper class aids you in the creation of a script to execute an AJAX request.
    /// It goes beyond just the execution of the request, it also adds a host of things you can do
    /// once the request excutes successfully, like filling an element with the returned data or 
    /// removing alement from the dom. This class uses the same fluid interface as the other helpers
    /// so you can build on functionality like this .clearElement("elementid").updatePanel("elementid").focus("elementid")
    /// </summary>
    public class AjaxRequest
    {

        /// <summary>
        /// Enumeration used when using jquery with ajax
        /// </summary>
        public enum DataTypeEnum
        {
            html,
            json,
            xml,
            script,
            jsonp,
            text
        }

        /// <summary>
        /// List of methods used on forms and jquery ajax calls
        /// </summary>
        public enum MethodEnum
        {
            @get,
            post
        }

        /// <summary>
        /// Factory method to creat an ajax request that will act against the supplied url
        /// </summary>
        /// <param name="url">Server url the ajax call uses</param>
        public static AjaxRequest Create(string url)
        {
            return Create(url, null);
        }

        /// <summary>
        /// Factory method to creat an ajax request that will act against the supplied url. It uses the String.Format method
        /// with the url to aid in the parametization of the url
        /// </summary>
        /// <param name="url">Server url the ajax call uses. String.Format will be called on this string</param>
        /// <param name="params">The values to merge with the url</param>
        /// <returns>A new AjaxRequest object</returns>
        public static AjaxRequest Create(string url, params string[] @params)
        {
            var obj = new AjaxRequest
            {
                _url = @params != null ? string.Format(url, @params) : url
            };
            return obj;
        }

        // These are the corresponding private methods of the class
        private string _url;
        private string _updatepanel;
        private string _removeElement;
        private string _confirmMessage;
        //private string _successMessage;
        private string _callback;
        private bool _validate;
        private JSObject _postData = new JSObject();
        private MethodEnum _method = MethodEnum.@get;
        private DataTypeEnum _dataType = DataTypeEnum.html;
        private string _redirectToUrl;
        private string _loadUrl;
        private string _loadElement;
        private string _hideElement;
        private string _showElement;
        private string _removeClassElement;
        private string _removeClassNames;
        private string _focus;
        private bool _clearForm;
        private string _formName;
        private bool _isPartialForm;
        private string _successMessage;
        private string _clearElement;


        /// <summary>
        /// Sumits the inputs of the partial form specified
        /// </summary>
        /// <param name="containerId">The partial form element id</param>
        public AjaxRequest PartialForm(string containerId)
        {
            _formName = JQHelper.GetElementID(containerId);
            _isPartialForm = true;
            return this;
        }

        /// <summary>
        /// Sets the innerHtml value of the element specified with the data returned from the ajax request
        /// </summary>
        /// <param name="name">Element id that will hold the contents</param>
        public AjaxRequest UpdatePanel(string name)
        {
            _updatepanel = JQHelper.GetElementID(name);
            return this;
        }

        /// <summary>
        /// Removes this element once the ajax request completes
        /// </summary>
        /// <param name="name">Element id</param>
        public AjaxRequest RemoveElement(string name)
        {
            _removeElement = JQHelper.GetElementID(name);
            return this;
        }

        /// <summary>
        /// Clears the element specified
        /// </summary>
        /// <param name="name">Element id</param>
        public AjaxRequest ClearElement(string name)
        {
            _clearElement = JQHelper.GetElementID(name);
            return this;
        }

        /// <summary>
        /// Redirects the user to the url specified once the ajax request completes
        /// </summary>
        /// <param name="url">Url to redirect the user to client side</param>
        public AjaxRequest RedirectToUrl(string url)
        {
            _redirectToUrl = url;
            return this;
        }

        /// <summary>
        /// Sets the focus to the element specified once the ajax request completes
        /// </summary>
        /// <param name="elementId">Element id</param>
        public AjaxRequest focus(string elementId)
        {
            _focus = JQHelper.GetElementID(elementId);
            return this;
        }

        /// <summary>
        /// Adds a confirmation message before the ajax request happens.
        /// </summary>
        /// <param name="message">Text to display to the user in the confirmaiton message</param>
        public AjaxRequest ConfirmMessage(string message)
        {
            _confirmMessage = message;
            return this;
        }

        /// <summary>
        /// Calls the supplied function or executes the supplied script once the ajax request completes
        /// </summary>
        /// <param name="function">This could be any valid javscript, either ExecuteOnSuccess() or some actual script like function(data){ alert(data); } </param>
        public AjaxRequest Callback(string function)
        {
            //if (!function.Trim().EndsWith(")"))
            //    function += "()";
            //_callback = function;
            _callback = "function (data) { " + function + "; }";
            return this;
        }

        /// <summary>
        /// Executes the supplied javascript once the ajax request completes. 
        /// It will be wrapped like this: function(responseText, statusText, xhr, $form){ [your code] }
        /// </summary>
        /// <param name="javascript">This could be any valid javscript</param>
        public AjaxRequest CallbackJavascript(string javascript)
        {
            if (!javascript.Trim().EndsWith(";")) javascript += ";";
            _callback = "(function(responseText, statusText, xhr, $form) { " + javascript + "})";
            return this;
        }

        /// <summary>
        /// Shows the alert on success
        /// </summary>
        /// <param name="msg">The message to display in the alert box</param>
        public AjaxRequest SuccessMessage(string msg)
        {
            _successMessage = JSBuilder.CleanString(msg);
            return this;
        }

        /// <summary>
        /// Posts the specified JSObject literal values
        /// </summary>
        /// <param name="obj">The JSObject to post</param>
        public AjaxRequest PostData(JSObject obj)
        {
            _postData.Merge(obj);
            return this;
        }

        /// <summary>
        /// Validates the request vefore sumission
        /// </summary>
        public AjaxRequest Validate()
        {
            _validate = true;
            return this;
        }

        /// <summary>
        /// Adds some literal data to the post data
        /// </summary>
        /// <param name="param">The parameter name for the data</param>
        /// <param name="expression">The expression to assign to the param to be posted. Does not add string quotes.</param>
        public AjaxRequest AddData(string param, string expression)
        {
            _postData.Add(param, expression);
            return this;
        }

        /// <summary>
        /// Adds some literal data to the post data
        /// </summary>
        /// <param name="param">The parameter name for the data</param>
        /// <param name="expression">The expression to assign to the param to be posted. Does not add string quotes.</param>
        public AjaxRequest AddData(string param, int expression)
        {
            _postData.Add(param, expression.ToString());
            return this;
        }

        /// <summary>
        /// Adds some literal data to the post data. While also properly escaping and adding quotes around the string value
        /// </summary>
        /// <param name="param">The parameter name for the data</param>
        /// <param name="value">The value to post. This value will be properly escaped and quoted to be included in the script</param>
        public AjaxRequest AddDataString(string param, string value)
        {
            _postData.AddString(param, value);
            return this;
        }

        /// <summary>
        /// Adds the value of an input field to the post data
        /// </summary>
        /// <param name="inputIds">The ID of the input fields to add</param>
        public AjaxRequest AddDataInput(params string[] inputIds)
        {
            foreach (var inputId in inputIds)
            {
                AddData(inputId, JQHelper.ID(inputId).val().ToString());
            }
            return this;
        }

        /// <summary>
        /// Determins the method of the request, post / get
        /// </summary>
        public AjaxRequest Method(MethodEnum ajaxMethod)
        {
            _method = ajaxMethod;
            return this;
        }

        /// <summary>
        /// Determines the datatype to be expected as the ajax response
        /// </summary>
        public AjaxRequest DataType(DataTypeEnum type)
        {
            _dataType = type;
            return this;
        }

        /// <summary>
        /// Signals that the request will be doing using the GET http method
        /// </summary>
        public AjaxRequest Get()
        {
            return Method(MethodEnum.@get);
        }

        /// <summary>
        /// Signals that the request will be doing using the POST http method
        /// </summary>
        public AjaxRequest Post()
        {
            return Method(MethodEnum.post);
        }

        /// <summary>
        /// Will do a POST request with the specified form
        /// </summary>
        public AjaxRequest Post(string formId)
        {
            Method(MethodEnum.post);
            _formName = JQHelper.GetElementID(formId);
            _isPartialForm = false;
            return this;
        }
        
        /// <summary>
        /// Once the ajax request completes. It will execute another ajax request to load the contents of the url into the element
        /// </summary>
        /// <param name="url">Element to request the contents to be loaded</param>
        /// <param name="elementid">Element id to load with the contents</param>
        public AjaxRequest Load(string url, string elementid)
        {
            _loadUrl = url;
            _loadElement = JQHelper.GetElementID(elementid);
            return this;
        }

        /// <summary>
        /// Hides the element on ajax complete
        /// </summary>
        public AjaxRequest HideElement(string ElementID)
        {
            _hideElement = JQHelper.GetElementID(ElementID);
            return this;
        }

        /// <summary>
        /// Shows the elmenet on ajax complete
        /// </summary>
        public AjaxRequest ShowElement(string ElementID)
        {
            _showElement = JQHelper.GetElementID(ElementID);
            return this;
        }

        /// <summary>
        /// Hides one element and show another on ajax complete
        /// </summary>
        public AjaxRequest HideAndShowElement(string HideElementID, string ShowElementID)
        {
            _hideElement = JQHelper.GetElementID(HideElementID);
            _showElement = JQHelper.GetElementID(ShowElementID);
            return this;
        }

        /// <summary>
        /// Removes a class from an elmenet on ajax complete
        /// </summary>
        public AjaxRequest RemoveClass(string ElementID, string ClassNames)
        {
            _removeClassElement = JQHelper.GetElementID(ElementID);
            _removeClassNames = ClassNames;
            return this;
        }

        /// <summary>
        /// Clears the content of the form on ajax complete
        /// </summary>
        public AjaxRequest ClearForm()
        {
            _clearForm = true;
            return this;
        }
        
        /// <summary>
        /// Creates the script to be outputed to the page
        /// </summary>
        public override string ToString()
        {
            var options = JSObject.Create();
            options.AddString("url", _url);
            options.AddString("method", _method.ToString());
            options.AddString("dataType", _dataType.ToString());

            // Form posting
            if (!string.IsNullOrEmpty(_formName))
            {
                options.AddString("formId", _formName);
                options.Add("isPartialForm", _isPartialForm.ToString().ToLower());
                options.Add("clearForm", _clearForm.ToString().ToLower());
                options.Add("validate", _validate.ToString().ToLower());
            }

            // Add any data that has been selected to post
            if (!_postData.IsEmpty())
            {
                 options.Add("postData", _postData.ToString());
            }
            
            // Wrap the ajax call with confirm
            if (string.IsNullOrEmpty(_confirmMessage) == false)
            {
                options.AddString("confirmMessage", _confirmMessage);
            }

            // Update Panel
            if (string.IsNullOrEmpty(_updatepanel) == false)
            {
                options.AddString("updatePanel", _updatepanel);
            }
            // Remove Element
            if (string.IsNullOrEmpty(_removeElement) == false)
            {
                options.AddString("removeElement", _removeElement);
            }
            // Clear Element
            if (string.IsNullOrEmpty(_clearElement) == false)
            {
                options.AddString("clearElement", _clearElement);
            }
            // Hide Element
            if (string.IsNullOrEmpty(_hideElement) == false)
            {
                options.AddString("hideElement", _hideElement);
            }
            // Show Element
            if (string.IsNullOrEmpty(_showElement) == false)
            {
                options.AddString("showElement", _showElement);
            }
            // Remove classes
            if (string.IsNullOrEmpty(_removeClassElement) == false && string.IsNullOrEmpty(_removeClassNames) == false)
            {
                options.AddString("removeClassFromElement", _removeClassElement);
                options.AddString("removeClassFromElementClassNames", _removeClassNames);
            }
            // Callback function
            if (string.IsNullOrEmpty(_callback) == false)
            {
                options.Add("callback", _callback);
            }
            
            // Set the focus
            if (string.IsNullOrEmpty(_focus) == false)
            {
                options.AddString("focus", _focus);
            }
            // Redirect to url
            if (string.IsNullOrEmpty(_redirectToUrl) == false)
            {
                options.AddString("redirectToUrl", _redirectToUrl);                
            }
            // Load data into element
            if (string.IsNullOrEmpty(_loadUrl) == false && string.IsNullOrEmpty(_loadElement) == false)
            {
                options.AddString("loadUrl", _loadUrl);
                options.AddString("loadElement", _loadElement);
            }

            // Success message
            if (string.IsNullOrEmpty(_successMessage) == false)
            {
                options.AddString("successMessage", _successMessage);
            }

            // Function call
            return string.Format("bf.ajax('{0}', {1})", JSBuilder.GetString(_url), options.ToString());
        }

    }

    #endregion

    #region AjaxForm
    /// <summary>
    /// Creates the script necessary to Ajaxify an html form. It wraps the jquery.form
    /// </summary>
    public class AjaxForm
    {

        private string _formName = "";
        private string _url = "";
        private bool _validate = true;
        private string _validateFunction = "";
        private string _type = "POST";
        // xml || json || script || null (default)
        private string _dataType = "";
        // element to be udpated
        private string _target = "";
        // Determine weather to replace the contents of the target or the whole target element
        private bool _replaceTarget;
        private string _beforeSerialize = "";
        private string _beforeSubmit = "";
        private string _success = "";
        // default = null
        private bool _resetForm;
        // default = null
        private bool _clearForm;
        private bool _wrapOnReady;
        private JSObject _otherOptions = new JSObject();

        private bool _wrapScriptTag;
        public AjaxForm()
        {
            _clearForm = false;
        }

        /// <summary>
        /// Factory method to ajaxify the form
        /// </summary>
        /// <param name="formName">The name of the form</param>
        public static AjaxForm Create(string formName)
        {
            return new AjaxForm
            {
                _formName = formName
            };
        }

        /// <summary>
        /// Factory method to ajaxify the form
        /// </summary>
        /// <param name="formName">The name of the form</param>
        /// <param name="url">The url the form will be posted to</param>
        public static AjaxForm Create(string formName, string url)
        {
            return new AjaxForm
            {
                _formName = formName,
                _url = url
            };
        }

        /// <summary>
        /// Cancels any validation from happening when submitting the form
        /// </summary>
        public AjaxForm DontValidate()
        {
            _validate = false;
            return this;
        }

        /// <summary>
        /// Specifies the function to call to determine whether the form is valid or not.
        /// </summary>
        /// <param name="functionName">The name of the function without the () at the end</param>
        /// <returns></returns>
        public AjaxForm ValidateFunction(string functionName)
        {
            functionName = functionName.Trim().EndsWith("()") ? functionName : functionName + "()";
            _validateFunction = functionName;
            return this;
        }

        public AjaxForm POST()
        {
            return RequestType("POST");
        }

        public AjaxForm POST(string postUrl)
        {
            RequestType("POST");
            _url = postUrl;
            return this;
        }

        public AjaxForm GET()
        {
            return RequestType("GET");
        }

        public AjaxForm GET(string getUrl)
        {
            RequestType("GET");
            _url = getUrl;
            return this;
        }

        /// <summary>
        /// Sets the ajax request to be a GET or POST
        /// </summary>
        /// <param name="value">GET or POST</param>
        public AjaxForm RequestType(string value)
        {
            _type = value;
            return this;
        }

        /// <summary>
        /// Specifies the dataType being returned from the ajax request
        /// </summary>
        /// <param name="value">xml || json || script || null (which is the default signals it that is html)</param>
        public AjaxForm DataType(string value)
        {
            _dataType = value;
            return this;
        }

        /// <summary>
        /// Specifies the datatype being returned from the ajax request as bing json
        /// </summary>
        public AjaxForm json()
        {
            return DataType("json");
        }

        /// <summary>
        /// Replaces the content of this element id with the data returned from the request
        /// </summary>
        public AjaxForm Target(string elementId)
        {
            _target = elementId;
            return this;
        }

        /// <summary>
        /// Replaces the content of this element id with the data returned from the request. 
        /// When replace target = false, then it appends the data instead of replacing it which is the default behaviour
        /// </summary>
        public AjaxForm Target(string elementId, bool replaceTarget)
        {
            _target = elementId;
            _replaceTarget = replaceTarget;
            return this;
        }

        /// <summary>
        /// Calls this function or javascript before the form is serialized. Allows you to do some preprocessing on the form
        /// </summary>
        /// <param name="script">function() or actual javascript to run like function(){ statements; }</param>
        public AjaxForm BeforeSerialize(string script)
        {
            _beforeSerialize = script;
            return this;
        }

        /// <summary>
        /// Calls this function or javascript before the form is submitted. Allows you to do some preprocessing on the form before it is submitted
        /// </summary>
        /// <param name="script">function() or actual javascript to run like function(){ statements; }</param>
        public AjaxForm BeforeSubmit(string script)
        {
            _beforeSubmit = script;
            return this;
        }

        /// <summary>
        /// Calls this function after the ajax request completes successfully
        /// </summary>
        /// <param name="script">function() or actual javascript to run like function(){ statements; }</param>
        public AjaxForm Success(string script)
        {
            _success = script;
            return this;
        }

        /// <summary>
        /// Calls this function fter the ajax request completes with a status code other than 200
        /// </summary>
        /// <param name="script">Function name or anonymous function(){ statements; }</param>
        public AjaxForm Error(string script)
        {
            _otherOptions.Add("error", script);
            return this;
        }

        /// <summary>
        /// Redirects to the specified url after the ajax request completes successfully
        /// </summary>
        /// <param name="url">The url you wish to redirect to</param>
        public AjaxForm Success_Redirect(string url)
        {
            _success = "function() { window.location = '" + url + "'; }";
            return this;
        }

        /// <summary>
        /// Clears the form on ajax complete
        /// </summary>
        public AjaxForm ClearForm()
        {
            _clearForm = true;
            return this;
        }

        /// <summary>
        /// Resets the form on ajax complete
        /// </summary>
        public AjaxForm ResetForm()
        {
            _resetForm = true;
            return this;
        }

        /// <summary>
        /// Wraps the script created with this interface in a jQuery onready event to be outputed to the page
        /// </summary>
        public AjaxForm OnReady()
        {
            return OnReady(false);
        }

        /// <summary>
        /// Wraps the script created with this interface in a jQuery onready event to be outputed to the page, and it determines weather to
        /// add the script tag to the output once you call ToString
        /// </summary>
        public AjaxForm OnReady(bool addScriptTag)
        {
            _wrapOnReady = true;
            _wrapScriptTag = addScriptTag;
            return this;
        }

        /// <summary>
        /// Wraps in a script tag as well as in a jQuery on ready event
        /// </summary>
        public AjaxForm AddScriptTag()
        {
            return AddScriptTag(true);
        }

        /// <summary>
        /// Wraps in a script tag. If addJQueryOnReady is true then it also wraps it in a jQuery on ready event
        /// </summary>
        public AjaxForm AddScriptTag(bool addJQueryOnReady)
        {
            _wrapScriptTag = true;
            return OnReady(addJQueryOnReady);
        }

        /// <summary>
        /// Grabs all the values and options specified and creates the proper script to ouput to the page. If you called ton OnReady it outputs the script
        /// within a jQuery on document ready function, if you flagged it to AddScriptTag it sorrounds the whole script with the propery <script></script> tag
        /// </summary>
        /// <returns>Properly formatted javascript to include in the page</returns>
        public string ToString(bool addScriptTag)
        {
            return AddScriptTag().ToString();
        }

        /// <summary>
        /// Grabs all the values and options specified and creates the proper script to ouput to the page. If you called ton OnReady it outputs the script
        /// within a jQuery on document ready function, if you flagged it to AddScriptTag it sorrounds the whole script with the propery <script></script> tag
        /// </summary>
        /// <returns>Properly formatted javascript to include in the page</returns>
        public override string ToString()
        {
            var options = new JSObject();
            options.AddString("url", _url);
            options.AddString("type", _type);
            options.Merge(_otherOptions);
            if (!string.IsNullOrEmpty(_dataType))
            {
                options.AddString("dataType", _dataType);
            }
            if (!string.IsNullOrEmpty(_target))
            {
                options.AddString("target", _target);
                if (_replaceTarget)
                {
                    options.Add("replaceTarget", "true");
                }
            }

            // If the user does not specify a validation function, then assign a default
            if (string.IsNullOrEmpty(_validateFunction))
                _validateFunction = "jQuery('#" + _formName + "').validate().form()";

            if (!string.IsNullOrEmpty(_beforeSerialize) & _validate == false)
            {
                options.Add("beforeSerialize", _beforeSerialize);
            }
            else if (!string.IsNullOrEmpty(_beforeSerialize) & _validate)
            {
                options.Add("beforeSerialize", "function(){ " + _beforeSerialize + "(); if (!" + _validateFunction + ") return false; }");
            }
            else if (_validate)
            {
                options.Add("beforeSerialize", "function() { if (!" + _validateFunction + ") return false; }");
            }
            if (!string.IsNullOrEmpty(_beforeSubmit))
            {
                options.Add("beforeSubmit", _beforeSubmit);
            }
            if (!string.IsNullOrEmpty(_success) || options.HasMember("error"))
            {
                // Need to check that the response is actually a success before executing it
                var ssb = new StringBuilder();
                ssb.Append("function(responseText, statusText, xhr, $form) { ");

                // Sometimes the response is 0 or undefined but there is not an error in the response 
                //   (this happens sometimes when uploading a form with a file input)
                ssb.Append("    var undefinedWithError = false; ");
                ssb.Append("    try {");
                ssb.Append("         if ((xhr.status == undefined || xhr.status == 0) && responseText.indexOf('<span><h1>Server Error in') != -1) { ");
                ssb.Append("              undefinedWithError = true;");
                ssb.Append("         }");
                ssb.Append("    } catch(e){log('###ERROR: ' + e);}");

                //ssb.Append("    alert('statusIs0OrUndefined: ' + (xhr.status == undefined || xhr.status == 0));")
                //ssb.Append("    alert('statusText: ' + statusText + ' \n status: ' + xhr.status + '\n undefinedWithError: ' + undefinedWithError + '\n\n' + responseText);")
                //ssb.Append("if (undefinedWithError) alert('***ERROR***: StatusText: ' + statusText + ' status: ' + xhr.status);")

                // Trigger an error handler if any
                if (options.HasMember("error"))
                    ssb.Append("if (undefinedWithError)(" + options.GetValue("error") + ")(responseText, statusText, xhr, $form);");

                // Call success only after checking that there were no undefined with errors
                if (!string.IsNullOrEmpty(_success))
                    ssb.Append("if (!undefinedWithError) (" + _success + ")(responseText, statusText, xhr, $form);");

                ssb.Append("}");
                options.Add("success", ssb.ToString());
            }

            if (_resetForm)
            {
                options.Add("resetForm", "true");
            }
            if (_clearForm)
            {
                options.Add("clearForm", "true");
            }

            var script = "";

            // Add the script tag if requested
            if (_wrapScriptTag)
                script += "<script type='text/javascript'>";
            // Add the jquery on ready tag
            if (_wrapOnReady)
                script += "jQuery(function(){";
            script += "jQuery('#" + _formName + "').ajaxForm(" + options + ");";
            // Close the jquery on ready tag
            if (_wrapOnReady)
                script += "});";
            // Close the script onready tag
            if (_wrapScriptTag)
                script += "</script>";

            return script;
        }

    }
    #endregion
    
}