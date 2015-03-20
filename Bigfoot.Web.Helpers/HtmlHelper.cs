// DEPENDENCIES:
//   Javascript
//   - jquery
//   - jquery.validate
//   - jquery.form
//   - jquery.metadata

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;

namespace BigfootWeb.Helpers
{

    #region HtmlHelper

    /// <summary>
    /// The HTML helper class aids in the raw ouput of html in your views. It has helpers
    /// to output input controls, and all other kind of select tags. This is done in a 
    /// fluid way sort of like jQuery by using the TagBuilder class as the return value of every mehtod
    /// once your are doing composing your html element you simple call ToString and the correctly ouputed 
    /// tag value is returned for inclusing in the view. It also has generic method for Html and Url encode 
    /// string as well as general purpose function for working with HTML.
    /// 
    /// It also has the smarts for dealing with issues like checkbox values not included the post when not checked 
    /// etc. It also makes it easy to create select elements with its options in a more programtically friendly way
    /// including the selection of the proper options when binding your ViewModel etc.
    /// 
    /// You can also use the TagBuilder class to create any HTML tag including the inclussion of Child tags. The HtmlHelper
    /// class however just creates shortcuts to to jumpstart the creation of common tags using the TagBuilder. They then
    /// return the TagBuilder class so you can continue to compose your html output.
    /// 
    /// The TagBuilder is an amazing class that allows you to build a whole field ouput including its label and 
    /// validation requirements in a single line of code.
    /// 
    /// For Example: <%=HtmlHelper.TextBox("Name").Value(123).Label("Amount").ValRequired()%>
    /// </summary>
    public class HtmlHelper
    {
        
        public HtmlHelper() { }

        public static HtmlHelper Create()
        {
            return new HtmlHelper();
        }

        /// <summary>
        /// Html encode some text
        /// </summary>
        /// <param name="data">The string to encode</param>
        /// <returns>The encoded string</returns>
        public string HtmlEncode(string data)
        {
            return HttpUtility.HtmlEncode(data);
        }

        /// <summary>
        /// Html decode some text
        /// </summary>
        /// <param name="data">The string to decode</param>
        /// <returns>The decoded string</returns>
        public string HtmlDecode(string data)
        {
            return HttpUtility.HtmlDecode(data);
        }

        /// <summary>
        /// Create html output from text by replacing the line feeds etc.
        /// </summary>
        /// <param name="data">The text data to convert</param>
        /// <returns>The Html output</returns>
        public string TextToHtml(string data)
        {
            var n = data;
            n = HtmlEncode(n);
            n = n.Replace("\r\n", "<br />");
            n = n.Replace("\n", "<br />");
            n = n.Replace("\r", "<br />");
            n = n.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            n = n.Replace(Environment.NewLine, "<br />");
            return n;
        }

        /// <summary>
        /// Outputs the start the form element declaration
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BeginForm()
        {
            return new TagBuilder("form", RenderMode.openTagOnly);
        }

        /// <summary>
        /// Outputs the start the form element declaration
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BeginForm(string id)
        {
            return new TagBuilder("form", RenderMode.openTagOnly).ID(id);
        }

        /// <summary>
        /// Outputs the end of the form element declaration
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public string EndForm()
        {
            return "</form>";
        }

        /// <summary>
        /// Builds a Javascript Link tag
        /// </summary>
        /// <param name="filePath">The path to the javascrpt file</param>
        /// <returns>The html for the link element</returns>
        public string JSReference(string filePath)
        {
            return new TagBuilder("script").attr("type", "text/javascript").src(filePath).ToString();
        }

        /// <summary>
        /// Builds a Javascript Link tag. If in debug mode then it renders the debug version path, otherwise the min
        /// </summary>
        /// <param name="debugFilePath">The path to the debug javascrpt file</param>
        /// <param name="minFilePath">The path to the min javascrpt file</param>
        /// <returns>The html for the link element</returns>
        public string JSReference(string debugFilePath, string minFilePath, bool isDebugMode)
        {
            if (isDebugMode)
                return new TagBuilder("script").attr("type", "text/javascript").src(debugFilePath).ToString();
            else
                return new TagBuilder("script").attr("type", "text/javascript").src(minFilePath).ToString();
        }

        /// <summary>
        /// Builds a CSS reference tag
        /// </summary>
        /// <param name="filePath">The path to the CSS file</param>
        /// <returns>The html for the CSS reference element</returns>
        public string CSSReference(string filePath)
        {
            return new TagBuilder("link").href(filePath).attr("rel", "stylesheet").attr("type", "text/css").ToString();
        }

        public string GoogleCDN_JQuery()
        {
            return "https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js";
        }

        public string GoogleCDN_JQueryUI()
        {
            return "https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.7/jquery-ui.min.js";
        }

        public string GoogleCDN_JQueryUI_Theme(string themeName)
        {
            return "https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.7/themes/" + themeName + "/jquery-ui.css";
        }

        /// <summary>
        /// Create a div tag
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder DIV()
        {
            return new TagBuilder("div");
        }

        /// <summary>
        /// Creates a div tag and oupts the content provided as the innner html
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder DIV(string content)
        {
            return new TagBuilder("div").InnerHtml(content);
        }

        /// <summary>
        /// Creates a div tag and oupts the content provided as the innner html
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder DIV(string content, string classes)
        {
            return new TagBuilder("div").InnerHtml(content).AddClass(classes);
        }

        /// <summary>
        /// Create a span element
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder SPAN()
        {
            return new TagBuilder("span");
        }

        /// <summary>
        /// Creates a span element including the contnet to include the inner html
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder SPAN(string content)
        {
            return new TagBuilder("span").InnerHtml(content);
        }

        /// <summary>
        /// Creates a span element including the contnet to include the inner html
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder SPAN(string content, string classes)
        {
            return new TagBuilder("span").InnerHtml(content).AddClass(classes);
        }

        /// <summary>
        /// Creates a textbox input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder TextBox(string name)
        {
            return new TagBuilder("input").IDandName(name).type("text");
        }

        /// <summary>
        /// Creates a textbox input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundTextBox(string name)
        {
            return new TagBuilder("input").IDandName(name).type("text").bindValue(name);
        }

        /// <summary>
        /// Creates a textbox input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name</param>
        /// <param name="id">The id for this element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder TextBox(string name, string id)
        {
            return new TagBuilder("input").ID(id).Name(name).type("text");
        }

        public string DisplayError(string error)
        {
            return DisplayError(error, "");
        }

        public string DisplayError(string error, string classNames)
        {
            return !string.IsNullOrEmpty(error)
                    ? new TagBuilder("span").InnerText(error).AddClass("error " + classNames).ToString()
                    : "";
        }

        /// <summary>
        /// Creates a file input
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder File()
        {
            return new TagBuilder("input").type("file");
        }

        /// <summary>
        /// Creates a file input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder File(string name)
        {
            return new TagBuilder("input").IDandName(name).type("file");
        }

        /// <summary>
        /// Creates a file input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name</param>
        /// <param name="id">The id for this element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder File(string name, string id)
        {
            return new TagBuilder("input").Name(name).ID(id).type("file");
        }

        /// <summary>
        /// Creates a password input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Password(string name)
        {
            return new TagBuilder("input").IDandName(name).type("password");
        }

        /// <summary>
        /// Creates a textarea input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder TextArea(string name)
        {
            return new TagBuilder("textarea").IDandName(name);
        }

        /// <summary>
        /// Creates a textarea input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name</param>
        /// <param name="id">The id to assign to the element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder TextArea(string name, string id)
        {
            return new TagBuilder("textarea").Name(name).ID(id);
        }

        /// <summary>
        /// Creates a hidden input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Hidden(string name)
        {
            return new TagBuilder("input").IDandName(name).type("hidden");
        }

        /// <summary>
        /// Creates a hidden input
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="id">The id to assign</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Hidden(string name, string id)
        {
            return new TagBuilder("input").Name(name).ID(id).type("hidden");
        }

        /// <summary>
        /// Creates a button input
        /// </summary>
        /// <param name="text">The text to include in the button</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Button(string text)
        {
            return new TagBuilder("button").InnerText(text);
        }

        /// <summary>
        /// Creates a label for a certain input. You can also alternatively use the .label method on the tagbuilder to add a label to 
        /// an input control while creating the control. The fieldLabel css class is assigned to the label for ease of targeting when formatting.
        /// </summary>
        /// <param name="forName">The input the label belongs to</param>
        /// <param name="labelText">The text to print for the label</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Label(string forName, string labelText)
        {
            return Label(forName, labelText, true);
        }

        /// <summary>
        /// Creates a label for a certain input. You can also alternatively use the .label method on the tagbuilder to add a label to 
        /// an input control while creating the control. The fieldLabel css class is assigned to the label for ease of targeting when formatting.
        /// </summary>
        /// <param name="forName">The input the label belongs to</param>
        /// <param name="labelText">The text to print for the label</param>
        /// <param name="addFieldLabelClass">Determines wheather to add the fieldLabel css class to the label</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Label(string forName, string labelText, bool addFieldLabelClass)
        {
            return new TagBuilder("label").@for(forName).InnerText(labelText).AddClassIf(addFieldLabelClass, "fieldLabel");
        }

        /// <summary>
        /// Creates a checkbox input. It correctly deals with the issue related to the false value of a checkbox not being sent to the client 
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="value">The value of the checkbox</param>
        /// <returns>A string with the html for the checkbox</returns>
        public string Checkbox(string name, bool value)
        {
            return Checkbox(name, value, string.Empty, string.Empty);
        }

        /// <summary>
        /// Creates a checkbox input. It correctly deals with the issue related to the false value of a checkbox not being sent to the client 
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="cssclass">The CSS class to assign to the checkbox</param>
        /// <returns>A string with the html for the checkbox</returns>
        public string Checkbox(string name, bool value, string cssclass)
        {
            return Checkbox(name, value, cssclass, string.Empty);
        }

        /// <summary>
        /// Creates a checkbox input. It correctly deals with the issue related to the false value of a checkbox not being sent to the client 
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="cssclass">The CSS class to assign to the checkbox</param>
        /// <param name="label">The label to include the checkbox</param>
        /// <returns>A string with the html for the checkbox</returns>
        public string Checkbox(string name, bool value, string cssclass, string label)
        {
            return Checkbox(name, name, value, cssclass, label, "", 0);
        }

        /// <summary>
        /// Creates a checkbox input. It correctly deals with the issue related to the false value of a checkbox not being sent to the client 
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="cssclass">The CSS class to assign to the checkbox</param>
        /// <param name="label">The label to include the checkbox</param>
        /// <param name="labelCssClass">The CSS class to assign to the label</param>
        /// <returns>A string with the html for the checkbox</returns>
        public string Checkbox(string name, bool value, string cssclass, string label, string labelCssClass)
        {
            return Checkbox(name, name, value, cssclass, label, labelCssClass, 0);
        }

        public string Checkbox(string name, bool value, string cssclass, string label, string labelCssClass, int tabIndex)
        {
            return Checkbox(name, name, value, cssclass, label, labelCssClass, tabIndex);
        }

        /// <summary>
        /// Creates a checkbox input. It correctly deals with the issue related to the false value of a checkbox not being sent to the client 
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="id">The ID to assign to the element</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="cssclass">The CSS class to assign to the checkbox</param>
        /// <param name="label">The label to include the checkbox</param>
        /// <returns>A string with the html for the checkbox</returns>
        public string Checkbox(string name, string id, bool value, string cssclass, string label)
        {
            return Checkbox(name, id, value, cssclass, label, "", 0);
        }

        /// <summary>
        /// Creates a checkbox input. It correctly deals with the issue related to the false value of a checkbox not being sent to the client 
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="id">The ID to assign to the element</param>
        /// <param name="value">The value of the checkbox</param>
        /// <param name="cssclass">The CSS class to assign to the checkbox</param>
        /// <param name="label">The label to include the checkbox</param>
        /// <param name="labelCssClass">The CSS class to assign to the label</param>
        /// <param name="tabIndex">The tabIndex if any to assign to the checkbox</param>
        /// <returns>A string with the html for the checkbox</returns>
        public string Checkbox(string name, string id, bool value, string cssclass, string label, string labelCssClass, int tabIndex)
        {
            var chkname = name + "_chk";
            var chkid = id + "_chk";
            var chk = new TagBuilder("input").Name(chkname).ID(chkid).type("checkbox");
            var hdn = new TagBuilder("input").Name(name).ID(id).type("hidden");
            // Figure out the value
            if (value)
            {
                chk.@checked(true);
                hdn.value("true");
            }
            else
            {
                chk.@checked(false);
                hdn.value("false");
            }
            // Add the label
            if (string.IsNullOrEmpty(label) == false)
            {
                if (!string.IsNullOrEmpty(labelCssClass))
                    chk.Label(label, labelCssClass);
                else
                    chk.Label(label);
            }
            // Add the class if asked
            if (string.IsNullOrEmpty(cssclass) == false)
            {
                chk.AddClass(cssclass);
            }
            if (tabIndex != 0)
            {
                chk.tabIndex(tabIndex);
            }
            // Wireup events
            chk.onclick("jQuery('#" + id + "').val(this.checked);");
            // Return the value
            return chk.ToString() + hdn;
        }

        /// <summary>
        /// Creates a radio input. It correctly deals with the issue related to the false value of a checkbox not being sent to the client 
        /// </summary>
        /// <param name="name">The value to assign to the input as the name and id</param>
        /// <param name="value">The value of the checkbox</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Radio(string name, string value)
        {
            return new TagBuilder("input").type("radio").Name(name).value(value);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundRadioList(string name, SelectItemList items)
        {
            return BoundRadioList(name, items.ToArray(), -1);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundRadioList(string name, SelectItem[] items)
        {
            return BoundRadioList(name, items, -1);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <param name="tabIndex">The tabIndex to assign to the radio item</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundRadioList(string name, SelectItemList items, int tabIndex)
        {
            return BoundRadioList(name, items.ToArray(), tabIndex);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <param name="tabIndex">The tabIndex to assign to the radio item</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundRadioList(string name, SelectItem[] items, int tabIndex)
        {
            var parent = new TagBuilder("span").ID(name + "Container");

            var i = 0;
            foreach (var item in items)
            {

                // Add radioItem
                var radioItem = parent.AddChild("span").AddClass("radioItem");

                // Add the radio input
                var radio = radioItem.AddChild("input").Name(name).type("radio").value(item.Value).ID(name + i).bindChecked(name);

                // Set the tabIndex
                if (tabIndex > -1) radio.tabIndex(tabIndex);

                // Add the radio label
                radioItem.AddChild("label").@for(name + i).AddClass("radioLabel").InnerText(item.Text);

                // Make the id unique
                i += 1;
            }

            return parent;
        }

        /// <summary>
        /// Create a radio list where the value is meant to be a boolean value.
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="value">True or false. These string value will be translated to true: on, yes, true, 1 as needed to identify as true</param>
        /// <param name="items">Option list</param>
        public TagBuilder RadioList(string name, bool value, SelectItemList items)
        {
            foreach (SelectItem item in items.ToArray())
            {
                var itemBoolValue = false;
                switch (item.Value.ToLower())
                {
                    case "on":
                        itemBoolValue = true;
                        break;
                    case "yes":
                        itemBoolValue = true;
                        break;
                    case "true":
                        itemBoolValue = true;
                        break;
                    case "1":
                        itemBoolValue = true;
                        break;
                }
                item.Selected = (itemBoolValue == value);
            }
            return RadioList(name, items, -1, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Create a radio list where the value is meant to be a string.
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="value">True or false. These string value will be translated to true: on, yes, true, 1 as needed to identify as true</param>
        /// <param name="items">Option list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, string value, SelectItemList items)
        {
            return RadioList(name, value, items.ToArray());
        }

        /// <summary>
        /// Create a radio list where the value is meant to be a string.
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="value">True or false. These string value will be translated to true: on, yes, true, 1 as needed to identify as true</param>
        /// <param name="items">Option list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, string value, SelectItem[] items)
        {
            if (string.IsNullOrEmpty(value) == false)
            {
                foreach (var item in items)
                {
                    if (item.Value.ToLower() == value.ToLower())
                        item.Selected = true;
                }
            }
            return RadioList(name, new SelectItemList(items), -1, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Create a radio list where the value is meant to be a string.
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="value">True or false. These string value will be translated to true: on, yes, true, 1 as needed to identify as true</param>
        /// <param name="items">Option list</param>
        /// <param name="tabIndex">The tab index to assign to the radio item</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, string value, SelectItemList items, int tabIndex)
        {
            return RadioList(name, value, items, tabIndex, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Create a radio list where the value is meant to be a string.
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="value">True or false. These string value will be translated to true: on, yes, true, 1 as needed to identify as true</param>
        /// <param name="items">Option list</param>
        /// <param name="tabIndex">The tab index to assign to the radio item</param>
        /// <param name="addCssClassToInput">Classes to add to the radio input</param>
        /// <param name="addCssClassToLabel">Classes to add to the the label of the input control</param>
        /// <param name="addCssClassToRow">This will add a set of css classes to the the row contianing the the input and label</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, string value, SelectItemList items, int tabIndex, string addCssClassToInput, string addCssClassToLabel, string addCssClassToRow)
        {
            if (addCssClassToInput == null) throw new ArgumentNullException("addCssClassToInput");
            if (string.IsNullOrEmpty(value) == false)
            {
                foreach (var item in items.ToArray())
                {
                    if (item.Value.ToLower() == value.ToLower())
                        item.Selected = true;

                }
            }
            return RadioList(name, items, tabIndex, addCssClassToInput, addCssClassToLabel, addCssClassToRow);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, SelectItemList items)
        {
            return RadioList(name, items, -1, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, SelectItem[] items)
        {
            return RadioList(name, new SelectItemList(items), -1, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <param name="tabIndex">The tabIndex to assign to the radio item</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, SelectItem[] items, int tabIndex)
        {

            return RadioList(name, new SelectItemList(items), tabIndex, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <param name="tabIndex">The tabIndex to assign to the radio item</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, SelectItemList items, int tabIndex)
        {
            return RadioList(name, items, tabIndex, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">Element Name</param>
        /// <param name="items">Option list</param>
        /// <param name="tabIndex">The tabIndex to assign to the radio item</param>
        /// <param name="addCssClassToInput">Add a CSS class to every input created</param>
        /// <param name="addCssClassToLabel">Add a css class to every label for every input</param>
        /// <param name="addCssClassToRow">This will add a set of css classes to the the row contianing the the input and label</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder RadioList(string name, SelectItemList items, int tabIndex, string addCssClassToInput, string addCssClassToLabel, string addCssClassToRow)
        {
            var parent = new TagBuilder("span").ID(name + "Container");
            if (string.IsNullOrEmpty(addCssClassToInput)) addCssClassToInput = "";
            if (string.IsNullOrEmpty(addCssClassToLabel)) addCssClassToLabel = "";
            if (string.IsNullOrEmpty(addCssClassToRow)) addCssClassToRow = "";

            var i = 0;
            foreach (var item in items.ToArray())
            {

                // Add radioItem
                var radioItem = parent.AddChild("span").AddClass("radioItem " + addCssClassToRow);

                // Add the radio input
                var radio = radioItem.AddChild("input").Name(name).type("radio").value(item.Value).ID(name + i).AddClass(addCssClassToInput);
                // Mark as selected
                if (item.Selected)
                    radio.@checked(true);

                // Set the tabIndex
                if (tabIndex > -1) radio.tabIndex(tabIndex);

                // Add the radio label
                radioItem.AddChild("label").@for(name + i).AddClass("radioLabel " + addCssClassToLabel).InnerText(item.Text);

                // Make the id unique
                i += 1;
            }

            return parent;
        }

        /// <summary>

        /// <summary>
        /// Create a radio list
        /// </summary>
        /// <param name="name">The name to assign to the checkbox inputs</param>
        /// <param name="items">The list of items for the list</param>
        /// <param name="selectedValues">The list of values to select</param>
        /// <returns>
        ///         Span with the id of [name]Container that encompases the whole list. 
        ///         A span item for each checkbox with the class checkboxContainer assigned. A [name]_chk_[i] for each checkbox input to be rendered. 
        ///         A hidden input field for each checkbox item, when it is submitted to the server, it will be the list of values separated by commas
        /// </returns>
        public TagBuilder CheckboxList(string name, SelectItemList items, List<string> selectedValues)
        {
            if (selectedValues != null && selectedValues.Count > 0)
            {
                foreach (var val in selectedValues)
                {
                    foreach (var item in items.ToArray())
                    {
                        if (val == item.Value) item.Selected = true;
                    }
                }
            }
            return CheckboxList(name, items, -1);
        }

        /// <summary>
        /// Creates a list of checkbox to allow the user to select from multiple values. Values are posted to the server as 1,2,3 etc.
        /// </summary>
        /// <param name="name">The name to assign to the checkbox inputs</param>
        /// <param name="items">The list of items for the list</param>
        /// <param name="selectedValues">The list of values to select</param>
        /// <returns>
        ///         Span with the id of [name]Container that encompases the whole list. 
        ///         A span item for each checkbox with the class checkboxContainer assigned. A [name]_chk_[i] for each checkbox input to be rendered. 
        ///         A hidden input field for each checkbox item, when it is submitted to the server, it will be the list of values separated by commas
        /// </returns>
        public TagBuilder CheckboxList(string name, SelectItemList items, IEnumerable<int> selectedValues)
        {
            if (selectedValues != null)
            {
                foreach (var val in selectedValues)
                {
                    foreach (var item in items.ToArray())
                    {
                        if (val == int.Parse(item.Value)) item.Selected = true;
                    }
                }
            }
            return CheckboxList(name, items, -1);
        }

        /// <summary>
        /// Creates a list of checkbox to allow the user to select multiple values. Values are posted to the server as 1,2,3 etc.
        /// </summary>
        /// <param name="name">The name to assign to the checkbox inputs</param>
        /// <param name="items">The list of items for the list</param>
        /// <param name="tabIndex">The tab index for the items</param>
        /// <returns>
        ///         Span with the id of [name]Container that encompases the whole list. 
        ///         A span item for each checkbox with the class checkboxContainer assigned. A [name]_chk_[i] for each checkbox input to be rendered. 
        ///         A hidden input field for each checkbox item, when it is submitted to the server, it will be the list of values separated by commas
        /// </returns>
        public TagBuilder CheckboxList(string name, SelectItemList items, int tabIndex)
        {

            var parent = new TagBuilder("span").ID(name + "Container");

            var i = 0;
            foreach (var item in items.ToArray())
            {

                var checkboxId = name + "_chk_" + i;
                var checkboxName = name + "_chk";
                var hiddenId = name + i;
                var hiddenName = name;

                // Add item container
                var checkboxContainer = parent.AddChild("span").AddClass("checkboxContainer");

                // Add the radio input
                checkboxContainer.AddChild("input").type("checkbox")
                                    .Name(checkboxName)
                                    .ID(checkboxId)
                                    .value(item.Value)
                                    .@checked(item.Selected)
                                    .tabIndex(tabIndex, -1)
                                    .onclick("jQuery('#" + hiddenId + "').val((this.checked ? $(this).val() : ''));");

                // Add the hidden field that will hold the value (default the value if the item is selected)
                checkboxContainer.AddChild("input").type("hidden")
                                 .Name(hiddenName)
                                 .ID(hiddenId)
                                 .valueIf(item.Selected, item.Value);

                // Add the radio label
                checkboxContainer.AddChild("label").@for(name + i).AddClass("checkboxLabel").InnerText(item.Text);

                // Make the id unique
                i += 1;
            }

            return parent;
        }

        /// <summary>
        /// Create a bound select list
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundSelect(string name)
        {
            return new TagBuilder("select").IDandName(name).bindValue(name);
        }

        /// <summary>
        /// Create a bound select list
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <param name="options">The javascript array containing the options to bind to the select list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundSelect(string name, string options)
        {
            return new TagBuilder("select").IDandName(name).bindValue(name).bindOptions(options);
        }

        /// <summary>
        /// Create a bound select list. It is assumed that the source will be an Array of SelectItem(s). The optionsText is set to "Text" and the optionsValue is to "Value"
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <param name="options">The javascript array containing the options to bind to the select list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundSelectToSelectItems(string name, string options)
        {
            return new TagBuilder("select").IDandName(name).bindValue(name).bindOptions(options, "Text", "Value");
        }

        /// <summary>
        /// Create a bound select list. It is assumed that the source will be an Array of SelectItem(s). The optionsText is set to "Text" and the optionsValue is to "Value"
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <param name="options">The javascript array containing the options to bind to the select list</param>
        /// <param name="optionsCaption">The caption to display as selected the select box is null</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundSelectToSelectItems(string name, string options, string optionsCaption)
        {
            return new TagBuilder("select").IDandName(name).bindValue(name).bindOptions(options, "Text", "Value", optionsCaption);
        }

        /// <summary>
        /// Create a bound select list
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <param name="options">The javascript array containing the options to bind to the select list</param>
        /// <param name="optionsText">The property in the array to bind the option text to</param>
        /// <param name="optionsValue">The property in the array to bind the option value to</param>
        /// <param name="optionsCaption">The caption to display as selected the select box is null</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundSelect(string name, string options, string optionsText, string optionsValue, string optionsCaption)
        {
            return new TagBuilder("select").IDandName(name).bindValue(name).bindOptions(options, optionsText, optionsValue, optionsCaption);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name)
        {
            return new TagBuilder("select").IDandName(name);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="name">Name of the html element</param>
        /// <param name="id">Element ID</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, string id)
        {
            return new TagBuilder("select").Name(name).ID(id);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <param name="options">The list of options to add to the select list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, SelectItem[] options)
        {
            return new TagBuilder("select").options(options).IDandName(name);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <param name="options">The list of options to add to the select list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, SelectItemList options)
        {
            return new TagBuilder("select").options(options.ToArray()).IDandName(name);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="name">Name of the html element</param>
        /// <param name="id">Element ID</param>
        /// <param name="options">The list of options to add to the select list</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, string id, SelectItem[] options)
        {
            return new TagBuilder("select").options(options).Name(name).ID(id);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="value">The selected value for the list</param>
        /// <param name="options">The list of options to add to the select list</param>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, SelectItem[] options, object value)
        {
            return new TagBuilder("select").options(options, value).IDandName(name);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="value">The selected value for the list</param>
        /// <param name="options">The list of options to add to the select list</param>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, SelectItem[] options, int value)
        {
            return new TagBuilder("select").options(options, value).IDandName(name);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="value">The selected value for the list</param>
        /// <param name="options">The list of options to add to the select list</param>
        /// <param name="name">Name use for the id and name of the html element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, SelectItem[] options, string value)
        {
            return new TagBuilder("select").options(options, value).IDandName(name);
        }

        /// <summary>
        /// Create a select list
        /// </summary>
        /// <param name="id">The ID to assign to the element</param>
        /// <param name="value">The selected value for the list</param>
        /// <param name="options">The list of options to add to the select list</param>
        /// <param name="name">Name use for the html element</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Select(string name, string id, SelectItem[] options, object value)
        {
            return new TagBuilder("select").options(options, value).Name(name).ID(id);
        }

        /// <summary>
        /// Create an anchor tag
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Link()
        {
            return new TagBuilder("a").href("javascript:void(0);");
        }

        /// <summary>
        /// Create an anchor tag
        /// </summary>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder BoundLink(string text, string clickCallback)
        {
            return new TagBuilder("a").href("javascript:void(0);").text(text).bindClick(clickCallback);
        }

        /// <summary>
        /// Create an anchor tag
        /// </summary>
        /// <param name="LinkText">The text to use for the link</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Link(string LinkText)
        {
            return new TagBuilder("a").InnerText(LinkText).href("javascript:void(0);");
        }

        /// <summary>
        /// Create an anchor tag that is absolutely positioned over a certain element in the html and is used as 
        /// box if you will around a certain image, block etc.
        /// </summary>
        /// <param name="Title">This is the caption that comes up when you mouse over the link</param>
        /// <param name="top">Distance from the top of the parent element</param>
        /// <param name="left">Distance from the left of the parent element</param>
        /// <param name="height">The height of the link area to cover in case of an image the height of the image</param>
        /// <param name="width">The width of the link area to cover in case of an image the width of the image</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder LinkOver(string Title, int top, int left, int width, int height)
        {
            return new TagBuilder("a").title(Title).href("javascript:void(0);").absolute(top, left).size(width, height);
        }

        public TagBuilder LinkOverSpriteOnHover(string Title, int top, int left, int width, int height, string hoverSpriteImage, string spritePosition)
        {
            return new TagBuilder("a").title(Title).href("javascript:void(0);").absolute(top, left).size(width, height)
                                      .showSpriteOnMouseOver(hoverSpriteImage, spritePosition);
        }

        public TagBuilder LinkOverSpriteOnHover(string Title, string hoverSpriteImage, string spritePosition)
        {
            return LinkOverSpriteOnHover(Title, hoverSpriteImage, spritePosition, false);
        }

        public TagBuilder LinkOverSpriteOnHover(string Title, string hoverSpriteImage, string spritePosition, bool debug)
        {
            return new TagBuilder("a").title(Title).href("javascript:void(0);").showSpriteOnMouseOver(hoverSpriteImage, spritePosition, debug);
        }

        /// <summary>
        /// Creates an anchor tag. This link does not have text, instead it uses as the background a sprite image
        /// </summary>
        /// <param name="Title">This is the caption that comes up when you mouse over the link</param>
        /// <param name="img">The sprite image</param>
        /// <param name="bgposition">The x y coordinate of the image in the sprite</param>
        /// <param name="width">The width of the sprite image used for this link</param>
        /// <param name="height">The height of the sprite image used for this link</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder LinkSprite(string Title, string img, string bgposition, int width, int height)
        {
            return new TagBuilder("a").title(Title).href("javascript:void(0);").sprite(img, bgposition, width, height).inlineblock();
        }

        /// <summary>
        /// Creates an anchor tag. This link does not have text, instead it uses as the background a sprite image. It also has
        /// a rolloever effect with another section of the sprite image. The rollover image site must be the same size
        /// </summary>
        /// <param name="Title">This is the caption that comes up when you mouse over the link</param>
        /// <param name="img">The sprite image</param>
        /// <param name="bgposition">The x y coordinate of the image in the sprite</param>
        /// <param name="width">The width of the sprite image used for this link</param>
        /// <param name="height">The height of the sprite image used for this link</param>
        /// <param name="rolloverposition">The x y coordinate of the image in the sprite for the rollover.</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder LinkSpriteRollover(string Title, string img, string bgposition, int width, int height, string rolloverposition)
        {
            return new TagBuilder("a").title(Title).href("javascript:void(0);").spriteRollover(img, bgposition, width, height, rolloverposition).inlineblock();
        }

        /// <summary>
        /// Creates an anchor tag. This link does not have text, instead it uses as the background a sprite image. It acts as a toggle using a sprite.
        /// </summary>
        /// <param name="Title">This is the caption that comes up when you mouse over the link</param>
        /// <param name="img">The sprite image</param>
        /// <param name="onPosition">The x y coordinate of the image in the sprite for the on state</param>
        /// <param name="offPosition">The x y coordinate of the image in the sprite for off state</param>
        /// <param name="isOn">Determines whether to show the on state by default</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder LinkToggleSprite(string Title, string img, string onPosition, string offPosition, bool isOn)
        {
            return LinkToggleSprite(Title, img, onPosition, offPosition, isOn, "", "");
        }

        /// <summary>
        /// Creates an anchor tag. This link does not have text, instead it uses as the background a sprite image. It acts as a toggle using a sprite.
        /// </summary>
        /// <param name="Title">This is the caption that comes up when you mouse over the link</param>
        /// <param name="img">The sprite image</param>
        /// <param name="onPosition">The x y coordinate of the image in the sprite for the on state</param>
        /// <param name="offPosition">The x y coordinate of the image in the sprite for off state</param>
        /// <param name="isOn">Determines whether to show the on state by default</param>
        /// <param name="onClickScript">The code to execute to toggle on</param>
        /// <param name="offClickScript">The code to execute to toggle off</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder LinkToggleSprite(string Title, string img, string onPosition, string offPosition, bool isOn, string onClickScript, string offClickScript)
        {
            if (!onClickScript.Trim().EndsWith(";")) onClickScript += ";";
            if (!offClickScript.Trim().EndsWith(";")) offClickScript += ";";
            return new TagBuilder("a").title(Title).href("javascript:void(0);")
                                      .attr("onPosition", onPosition)
                                      .attr("offPosition", offPosition)
                                      .attr("currentState", isOn ? "on" : "off")
                                      .addStyle("background-image: url('" + img + "')")
                                      .addStyle("background-position: " + (isOn ? onPosition : offPosition))
                                      .onmouseover("var $this = jQuery(this); " +
                                                   "var hoverPosition = $this.attr('currentState') == 'on' ? $this.attr('offPosition') : $this.attr('onPosition'); " +
                /*"alert('mouseover: ' + hoverPosition);" +*/
                                                   "$this.css('background-position', hoverPosition);")
                                      .onmouseout("var $this = jQuery(this); " +
                                                   "var hoverPosition = $this.attr('currentState') == 'on' ? $this.attr('onPosition') : $this.attr('offPosition'); " +
                /*"alert('mouseout: ' + hoverPosition);" +*/
                                                   "$this.css('background-position', hoverPosition);")
                                      .onclick("var $this = jQuery(this); " +
                                               "var hoverPosition = $this.attr('currentState') == 'on' ? $this.attr('offPosition') : $this.attr('onPosition'); " +
                                               "$this.css('background-position', hoverPosition); " +
                                               "if ($this.attr('currentState') == 'on') " +
                                               "    { " + offClickScript + " $this.attr('currentState', 'off'); }" +
                                               "else " +
                                               "    { " + onClickScript + " $this.attr('currentState', 'on'); }");
        }

        /// <summary>
        /// Creates an anchor tag that surrounds two divs that can be floated together to form the sliding door technique.
        /// </summary>
        /// <param name="Title">This is the text that will appear inside the link</param>
        /// <param name="leftCSSClass">This is the CSS class assigned to the left part of the sliding door technique</param>
        /// <param name="rightCSSClass">This is the CSS class assigned to the right part of the sliding door technique</param>        
        public TagBuilder LinkWithSlidingDoor(string Title, string leftCSSClass, string rightCSSClass)
        {
            var tb = new TagBuilder("a").href("javascript:void(0);").AddClass("nouline");

            tb.AddChild("div").text(Title).AddClass(leftCSSClass)
                .Parent
              .AddChild("div").AddClass(rightCSSClass);

            return tb;

        }


        /// <summary>
        /// Create an anchor tag
        /// </summary>
        /// <param name="LinkText">The text to use for the link</param>
        /// <param name="href">The url to use for the link</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Link(string LinkText, string href)
        {
            return string.IsNullOrEmpty(href) ? Link(LinkText) : new TagBuilder("a").InnerText(LinkText).href(href);
        }

        /// <summary>
        /// Create an image tag
        /// </summary>
        /// <param name="src">Pass in the url to the image tag</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder Image(string src)
        {
            return new TagBuilder("img").src(src);
        }

        /// <summary>
        /// Creates an image tag using a sprite image as the background. To accomplish this it uses a blank image as the src for the image
        /// and it sets the background of the image element to be the xy coordinate of the right image in the sprite
        /// </summary>
        /// <param name="spriteImageUrl">The sprite image</param>
        /// <param name="bgposition">The x y coordinate of the image in the sprite</param>
        /// <param name="width">The width of the sprite image used for this link</param>
        /// <param name="height">The height of the sprite image used for this link</param>
        /// <param name="blankImageUrl">The blank image to use as the source</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageSprite(string spriteImageUrl, string bgposition, int width, int height, string blankImageUrl)
        {
            return new TagBuilder("img").src(blankImageUrl).sprite(spriteImageUrl, bgposition, width, height);
        }

        /// <summary>
        /// Creates an image tag using a sprite image as the background. To accomplish this it uses a blank image as the src for the image
        /// and it sets the background of the image element to be the xy coordinate of the right image in the sprite. It also adds
        /// a rollover effect to another xy coordinate in the sprite image where the rollover image is. The rolloever image in the sprite
        /// must be the same size as the original image
        /// </summary>
        /// <param name="spriteImageUrl">The sprite image</param>
        /// <param name="bgposition">The x y coordinate of the image in the sprite</param>
        /// <param name="width">The width of the sprite image used for this link</param>
        /// <param name="height">The height of the sprite image used for this link</param>
        /// <param name="rolloverposition">The x y coordinate of the image in the sprite to use for the rollover.</param>
        /// <param name="blankImageUrl">The blank image to use as the source</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageSpriteRollover(string spriteImageUrl, string bgposition, int width, int height, string rolloverposition, string blankImageUrl)
        {
            return new TagBuilder(blankImageUrl).src("blank.png").spriteRollover(spriteImageUrl, bgposition, width, height, rolloverposition);
        }

        /// <summary>
        /// Create an image tag with a rollover image effect
        /// </summary>
        /// <param name="src">The source of the image to use as the default state</param>
        /// <param name="hoversrc">The url of the image to change to when the mouse rolls over the image</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageHover(string src, string hoversrc)
        {
            var setsrcscript = "this.src='{0}'";
            return new TagBuilder("img").src(src).onmouseover(string.Format(setsrcscript, hoversrc)).onmouseout(string.Format(setsrcscript, src));
        }



        /// <summary>
        /// Create an image tag sorrounded by an anchor tag.
        /// </summary>
        /// <param name="src">The source of the image to use</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageLink(string src)
        {
            var lnk = new TagBuilder("a").href("javascript:void(0);").AddClass("pointer");
            lnk.AddChild("img").src(src).AddClass("pointer");
            return lnk;
        }

        /// <summary>
        /// Create an image tag sorrounded by an anchor tag.
        /// </summary>
        /// <param name="src">The source of the image to use</param>
        /// <param name="url">The url to assign to the anchor tag</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public string ImageLink(string src, string url)
        {
            return ImageLink(src, url, "");
        }

        /// <summary>
        /// Create an image tag sorrounded by an anchor tag.
        /// </summary>
        /// <param name="src">The source of the image to use</param>
        /// <param name="url">The url to assign to the anchor tag</param>
        /// <param name="alt">Assign this alt text to the image</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public string ImageLink(string src, string url, string alt)
        {
            return new TagBuilder("a").href(url).InnerHtml(Image(src).Alt(alt).ToString()).ToString();
        }

        /// <summary>
        /// Create an image tag sorrounded by an anchor tag that also displays some text in it
        /// </summary>
        /// <param name="src">The source of the image to use as the default state</param>
        /// <param name="text">The text to display inside the image</param>
        /// <param name="textTop">The top position of the text relative to the image dimensions</param>
        /// <param name="textLeft">The left position of the text relative to the image dimensions</param>
        /// <param name="textClass">The classes to be applied to the span containing the text</param>
        /// <param name="linkClass">Assigns this class to the link class</param>
        /// <param name="imgClass">Assigns this class to the image class</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageLinkWithText(string src, string text, int textTop, int textLeft, string textClass, string linkClass, string imgClass)
        {
            var tag = (new TagBuilder("a"))
                        .href("javascript:void(0);").AddClass(linkClass)
                        .addStyle("position: relative; display: inline-block; text-decoration: none;")
                        .AddChild("span").text(text).absolute(textTop, textLeft).AddClass(textClass).addStyle("cursor: pointer;")
                        .Parent
                        .AddChild("img").src(src).AddClass(imgClass)
                        .Parent;
            return tag;
        }


        // <summary>
        // Create an image tag sorrounded by an anchor tag that also displays some text on right side of image
        // </summary>
        // <param name="src">The source of the image to use as the default state</param>
        // <param name="text">The text to display inside the image</param>
        // <param name="textClass">The classes to be applied to the span containing the text</param>
        // <param name="linkClass">Assigns this class to the link class</param>
        // <param name="imgClass">Assigns this class to the image class</param>
        // <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageLinkWithTextImageLeft(string src, string text, string textClass, string linkClass, string imgClass)
        {
            var tag = (new TagBuilder("a"))
                        .href("javascript:void(0);").AddClass(linkClass)
                        .addStyle("display: inline-block; text-decoration: none;")
                        .AddChild("img").src(src).AddClass(imgClass)
                        .Parent
                        .AddChild("span").text(text).AddClass(textClass).addStyle("cursor: pointer;")
                        .Parent;
            return tag;
        }

        ///// <summary>
        ///// Create an image tag sorrounded by an anchor tag that also displays some text on left side of image
        ///// </summary>
        ///// <param name="src">The source of the image to use as the default state</param>
        ///// <param name="text">The text to display inside the image</param>
        ///// <param name="textClass">The classes to be applied to the span containing the text</param>
        ///// <param name="linkClass">Assigns this class to the link class</param>
        ///// <param name="imgClass">Assigns this class to the image class</param>
        ///// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageLinkWithTextImageRight(string src, string text, string textClass, string linkClass, string imgClass)
        {
            var tag = (new TagBuilder("a"))
                        .href("javascript:void(0);").AddClass(linkClass)
                        .addStyle("display: inline-block; text-decoration: none;")
                        .AddChild("span").text(text).AddClass(textClass).addStyle("cursor: pointer;")
                        .Parent
                        .AddChild("img").src(src).AddClass(imgClass)
                        .Parent;
            return tag;
        }

        /// <summary>
        /// Create an image tag sorrounded by an anchor tag with a rollover effect.
        /// </summary>
        /// <param name="src">The source of the image to use as the default state</param>
        /// <param name="hoversrc">The source of the image to use as the hover state</param>
        /// <param name="url">The url to assign to the anchor tag</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public string ImageLinkHover(string src, string hoversrc, string url)
        {
            return ImageLinkHover(src, hoversrc, url, "");
        }

        /// <summary>
        /// Create an image tag sorrounded by an anchor tag with a rollover effect.
        /// </summary>
        /// <param name="src">The source of the image to use as the default state</param>
        /// <param name="hoversrc">The source of the image to use as the hover state</param>
        /// <param name="url">The url to assign to the anchor tag</param>
        /// <param name="alt">Assign this alt text to the image</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public string ImageLinkHover(string src, string hoversrc, string url, string alt)
        {
            return ImageLinkHover(src, hoversrc, url, alt, false);
        }

        /// <summary>
        /// Create an image tag sorrounded by an anchor tag with a rollover effect.
        /// </summary>
        /// <param name="src">The source of the image to use as the default state</param>
        /// <param name="hoversrc">The source of the image to use as the hover state</param>
        /// <param name="url">The url to assign to the anchor tag</param>
        /// <param name="alt">Assign this alt text to the image</param>
        /// <param name="newWindow">Determine whether to open the link in a new window</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public string ImageLinkHover(string src, string hoversrc, string url, string alt, bool newWindow)
        {
            var tb = new TagBuilder("a").href(url).inlineblock().InnerHtml(ImageHover(src, hoversrc).Alt(alt).ToString());
            if (newWindow)
                tb.Target_Blank();
            return tb.ToString();
        }

        /// <summary>
        /// Create an image tag sorrounded by an anchor tag with a rollover effect that also displays some text in it
        /// </summary>
        /// <param name="src">The source of the image to use as the default state</param>
        /// <param name="hoversrc">The url of the image to change to when the mouse rolls over the image</param>
        /// <param name="text">The text to display inside the image</param>
        /// <param name="textTop">The top position of the text relative to the image dimensions</param>
        /// <param name="textLeft">The left position of the text relative to the image dimensions</param>
        /// <param name="textClass">The classes to be applied to the span containing the text</param>
        ///  <param name="linkClass">Assigns this class to the link class</param>
        /// <param name="imgClass">Assigns this class to the image class</param>
        /// <returns>A TagBuilder class to continue to create the tag by adding attr classes etc.</returns>
        public TagBuilder ImageLinkHoverWithText(string src, string hoversrc, string text, int textTop, int textLeft, string textClass, string linkClass, string imgClass)
        {
            var tag = (new TagBuilder("a"))
                        .href("javascript:void(0);")
                        .addStyle("position: relative; display: inline-block; text-decoration: none;").AddClass(linkClass)
                        .AddChild("span").text(text).absolute(textTop, textLeft).AddClass(textClass)
                            .addStyle("cursor: pointer;")
                            .onmouseover("jQuery(this).next().attr('src', '" + hoversrc + "')")
                            .onmouseout(" jQuery(this).next().attr('src', '" + src + "')")
                        .Parent
                        .AddChild("img").src(src).onmouseover("this.src='" + hoversrc + "'").onmouseout("this.src='" + src + "'").AddClass(imgClass)
                        .Parent;
            return tag;
        }

        /// <summary>
        /// Renders the content only if the speicfied condition is true
        /// </summary>
        /// <param name="condition">The condition that determines weather to render or not. If true it renders</param>
        /// <param name="content">The content to be redered</param>
        /// <returns>A RenderIfBuilder so that you may continue to build your conditional render statement with else etc.</returns>
        public RenderIfBuilder RenderIf(bool condition, string content)
        {
            return new RenderIfBuilder(condition, content);
        }

        /// <summary>
        /// Renders the content only if the speicfied condition is true
        /// </summary>
        /// <param name="condition">The condition that determines weather to render or not. If true it renders</param>
        /// <param name="format">The format to be merged with the supplied value used in the String.Format call</param>
        /// <param name="values">The values to be used in the String.Format call</param>
        /// <returns>A RenderIfBuilder so that you may continue to build your conditional render statement with else etc.</returns>
        public RenderIfBuilder RenderIf(bool condition, string format, params string[] values)
        {
            return new RenderIfBuilder(condition, format, values);
        }

        /// <summary>
        /// Renders only if value given value is not empty
        /// </summary>
        /// <param name="data">IF this data is empty then nothing renders</param>
        /// <returns>A RenderIfBuilder so that you may continue to build your conditional render statement with else etc.</returns>
        public RenderIfBuilder RenderIfNotEmpty(string data)
        {
            return string.IsNullOrEmpty(data) == false && data.Trim().Length > 0
                       ? new RenderIfBuilder(true, data)
                       : new RenderIfBuilder(false, data);
        }

        /// <summary>
        /// Renders only if value given value is not empty
        /// </summary>
        /// <param name="data">IF this data is empty then nothing renders</param>
        /// <param name="content">The content to render if the data is not empty</param>
        /// <returns>A RenderIfBuilder so that you may continue to build your conditional render statement with else etc.</returns>
        public RenderIfBuilder RenderIfNotEmpty(string data, string content)
        {
            return string.IsNullOrEmpty(data) == false && data.Trim().Length > 0
                       ? new RenderIfBuilder(true, content)
                       : new RenderIfBuilder(false, content);
        }

        /// <summary>
        /// Renders only if value given is not empty
        /// </summary>
        /// <param name="data">IF this data is empty then nothing renders</param>
        /// <param name="content">The content to render using String.Format if the data is not empty</param>
        /// <param name="params">The values to assign to the String.Format</param>
        /// <returns>A RenderIfBuilder so that you may continue to build your conditional render statement with else etc.</returns>
        public RenderIfBuilder RenderIfNotEmpty(string data, string content, params string[] @params)
        {
            return string.IsNullOrEmpty(data) == false && data.Trim().Length > 0
                       ? new RenderIfBuilder(true, content, @params)
                       : new RenderIfBuilder(false, content, @params);
        }

        public string SetFocusAfter(string selector, int millisseconds)
        {
            return SetFocusAfter(selector, millisseconds, true);
        }

        public string SetFocusAfter(string selector, int millisseconds, bool addScriptTag)
        {
            var code = string.Format("setTimeout('jQuery(\"{0}\").focus()', {1}); ", selector, millisseconds);
            return addScriptTag ? "<script type='text/javascript'>" + code + "</script>"
                                : code;
        }

        /// <summary>
        /// Generates an HTML table from a given data table
        /// </summary>
        /// <param name="data">The data to ouput as the html table</param>
        /// <returns>HTML String</returns>
        public string GenerateHtmlTable(DataTable data)
        {
            var tb = new StringBuilder();
            tb.AppendLine("<table>");

            // Create the headers
            tb.AppendLine("<thead>");
            tb.AppendLine("<tr>");
            foreach (DataColumn col in data.Columns)
            {
                tb.AppendFormat("<th>{0}</th>{1}", col.ColumnName, "\r\n");
            }
            tb.AppendLine("</tr>");
            tb.AppendLine("</thead>");

            // Create the rows
            tb.AppendLine("<tbody>");
            foreach (DataRow row in data.Rows)
            {
                tb.AppendLine("<tr>");
                foreach (DataColumn col in data.Columns)
                {
                    tb.AppendLine("<td>");
                    if (row.IsNull(col) == false)
                    {
                        tb.Append(HttpUtility.HtmlEncode(row[col].ToString()));
                    }
                    tb.AppendLine("</td>" + "\r\n");
                }
                tb.AppendLine("</tr>");
            }
            tb.AppendLine("</tbody>");
            tb.AppendLine("</table>");

            // Return the data
            return tb.ToString();
        }

        /// <summary>
        /// Creates a Link based date picker. It does this by creating three components
        /// 1. A link element
        /// 2. A hidden element carrying the date value with the format of the server
        /// 3. A script that changes the value of the hidden input and the link when a date is selected
        /// **The hidden element will always carry the USDate unless manually configured to do otherwise
        /// </summary>
        /// <param name="linkText">The text to display when no date is selected</param>
        /// <param name="title">The hover title</param>
        /// <param name="id">The id to assign to the link</param>
        /// <param name="value">The initial value</param>
        /// <param name="datePickerFormat">This is the date format used by the jQuery.DatePicker control</param>
        /// <param name="linkDateFormat">This will be the format of date when displayed as the link text. 
        /// this is used to format the value parameter. After that everytime you select a date, the datePickerFormat is used
        /// to assign it to the link text</param>
        /// <param name="linkClass">The class(es) if any to apply to the link</param>
        /// <param name="calContainerClass">The class(es) if any to apply to the div containing the calendar</param>
        /// <returns>An Html string with the Input, Link, DatePicker Div, and the script</returns>
        public string LinkDatePicker(string linkText, string title, string id, DateTime value,
                                            string datePickerFormat, string linkDateFormat,
                                            string linkClass, string calContainerClass)
        {
            var linkId = id + "_link";
            var calId = id + "_cal";
            var fieldId = id;
            var hiddenFieldValue = "";

            // If a default value was supplied then set the linkText to the value supplied 
            //  as well as the hidden field's value
            if (value != DateTime.MinValue)
            {
                linkText = value.ToString(linkDateFormat);
                hiddenFieldValue = value.ToString(CultureInfo.CreateSpecificCulture("en-US")
                                                             .DateTimeFormat.ShortDatePattern);
            }

            // Add the selecotr link
            var helper = new StringBuilder();
            helper.AppendLine(new TagBuilder("a").href(string.Format("javascript:jQuery('#{0}').show();void(0);", calId))
                                                 .InnerText(linkText).title(title).AddClass(linkClass).ID(linkId).ToString());
            // Add the div to contain the calendar
            helper.AppendLine("<div id='" + calId + "' style='display:none' class='" + calContainerClass + "'></div>");
            helper.AppendLine(Hidden(fieldId).value(hiddenFieldValue).ToString());
            helper.AppendLine("<script type='text/javascript'>");
            helper.AppendLine("     jQuery(function () {");
            helper.AppendLine("         jQuery('#" + calId + "').datepicker({");
            helper.AppendLine("             onSelect: function (dateText, inst) {");
            helper.AppendLine("                 jQuery('#" + fieldId + "').val(dateText);");
            helper.AppendLine("                 jQuery('#" + linkId + "').text(jQuery.datepicker.formatDate('" + datePickerFormat + "', new Date(dateText)));");
            helper.AppendLine("                 jQuery('#" + calId + "').hide();");
            helper.AppendLine("             }");
            helper.AppendLine("         });");
            helper.AppendLine("     });");
            helper.AppendLine("</script>");

            return helper.ToString();
        }



    }

    #endregion


    #region RenderIfBuilder

    /// <summary>
    /// This is a conditional render helper. It supports outputing content based on the conditions specified
    /// </summary>
    public class RenderIfBuilder
    {

        bool _condition;
        string _truecontent;
        string _elsecontent;
        bool _containselsecontent;

        /// <summary>
        /// Render the content if the condition is true
        /// </summary>
        /// <param name="condition">The condition to define weather to render the content</param>
        /// <param name="truecontent">The content to render</param>
        public RenderIfBuilder(bool condition, string truecontent)
        {
            _condition = condition;
            _truecontent = truecontent;
        }

        /// <summary>
        /// Render the content if the condition is true using String.Format
        /// </summary>
        /// <param name="condition">The condition to define weather to render the content</param>
        /// <param name="trueformat">The content to use in the String.Format</param>
        /// <param name="values">The values to use for the String.Format</param>
        public RenderIfBuilder(bool condition, string trueformat, params string[] values)
        {
            _condition = condition;
            _truecontent = string.Format(trueformat, values);
        }

        /// <summary>
        /// Renders this content if the condition is false
        /// </summary>
        /// <param name="elsecontent">The content to render</param>
        /// <returns>Returns the builder to aid in the fluid</returns>
        public RenderIfBuilder Else(string elsecontent)
        {
            _containselsecontent = true;
            _elsecontent = elsecontent;
            return this;
        }

        /// <summary>
        /// Renders this content if the condition is false using String.Format
        /// </summary>
        /// <param name="elseformat">The content to render</param>
        /// <param name="params">The parameters to use in the String.Format</param>
        /// <returns>Returns the builder to aid in the fluid</returns>
        public RenderIfBuilder Else(string elseformat, params string[] @params)
        {
            _containselsecontent = true;
            _elsecontent = string.Format(elseformat, @params);
            return this;
        }

        /// <summary>
        /// Renders the corrent content
        /// </summary>
        public override string ToString()
        {
            if (_condition) { return _truecontent; }
            return _containselsecontent ? _elsecontent : "";
        }
    }

    #endregion


    #region TabBuilder


    /// <summary>
    /// Html tag render mode enumeration
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// Renders using a corresponding </tag>
        /// </summary>
        Normal,
        /// <summary>
        /// It is a selfclosing tag. Like for inputs
        /// </summary>
        SelfClose,
        /// <summary>
        /// Does not ouput any tag closing. Useful when ouputing the beginning tag of a form but you don't want to close it.
        /// </summary>
        openTagOnly
    }

    /// <summary>
    /// The tag builder class is the heart of the html builder. It is used to create any html tag
    /// using a fluid interface. It has a ton of convinience methods to aid in the bulding of tags.
    /// It goes beyond the mere building of tags though and also adds javscript here and there to create
    /// interactivity like rollover effects, rollover effects with sprites etc.
    /// 
    /// It also relies on the metadata method for adding jquery.validation to input elements.
    /// 
    /// The tagbuilder class also understands the concept of input and label combinations to aid
    /// in minimizing the amount of code you have to write to create the very common label input validation 
    /// combination
    /// </summary>
    public class TagBuilder
    {
        private string TagName { get; set; }
        private string ElementId { get; set; }
        private Dictionary<string, string> TagAttributes { get; set; }
        public RenderMode? Mode { get; set; }

        /// <summary>
        /// Returns this instance's parent if it is inside another collection
        /// </summary>
        public TagBuilder Parent { get; set; }

        /// <summary>
        /// Returns the last added child. If you want to access all children use the children collection instead
        /// </summary>
        public TagBuilder Child { get; set; }
        public List<TagBuilder> Children { get; set; }

        private string _innerText = "";
        private string _innerHtml = "";
        private string _label = "";
        private string _labelclass;
        private bool _labelIsHtml = false;
        private string _labelWidth;
        private List<SelectItem> _options = new List<SelectItem>();
        private string _selectedOption = "";

        /// <summary>
        /// Creates a new TagBuilder by specifying the name of the tag you want to create i.e. h1
        /// </summary>
        /// <param name="tagName">The html tag name to use i.e. h1</param>
        public TagBuilder(string tagName)
        {
            TagAttributes = new Dictionary<string, string>();
            Children = new List<TagBuilder>();
            TagName = tagName;
            Mode = new RenderMode?();
        }


        /// <summary>
        /// Creates a new TagBuilder by specifying the name of the tag you want to create i.e. h1
        /// </summary>
        /// <param name="tagName">The html tag name to use i.e. h1</param>
        /// <param name="parent">Create a tag as an embeded child of another tag for hiarchical tag ouput</param>
        public TagBuilder(string tagName, TagBuilder parent)
        {
            TagAttributes = new Dictionary<string, string>();
            Children = new List<TagBuilder>();
            TagName = tagName;
            Parent = parent;
            Mode = new RenderMode?();
        }

        /// <summary>
        /// Creates a new TagBuilder by specifying the name of the tag you want to create i.e. h1
        /// </summary>
        /// <param name="tagName">The html tag name to use i.e. h1</param>
        /// <param name="mode">Specifies the render mode for the. SelfClosing etc.</param>
        public TagBuilder(string tagName, RenderMode mode)
        {
            TagAttributes = new Dictionary<string, string>();
            Children = new List<TagBuilder>();
            TagName = tagName;
            Mode = mode;
        }

        /// <summary>
        /// Creates a new TagBuilder by specifying the name of the tag you want to create, id and name to use and the class name to assign to it
        /// </summary>
        /// <param name="tagName">The html tag name to use i.e. h1</param>
        /// <param name="idAndName">The ID and Name to assign</param>
        /// <param name="className">The className to assign to the task</param>
        public TagBuilder(string tagName, string idAndName, string className)
        {
            TagAttributes = new Dictionary<string, string>();
            Children = new List<TagBuilder>();
            TagName = tagName;
            Mode = new RenderMode?();
            AddAttribute("id", idAndName);
            AddAttribute("name", idAndName);
            AddAttribute("class", className);
        }

        /// <summary>
        /// Adds a child to the children tags collection and returns the newaly added child
        /// </summary>
        /// <param name="tagname">The tag name for the child to add</param>
        /// <returns>The added child. Use the Parent property to get back to parent builder</returns>
        public TagBuilder AddChild(string tagname)
        {
            return AddChild(new TagBuilder(tagname));
        }

        /// <summary>
        /// Adds a child TagBuilder to the children tags collection and returns the newaly added child. 
        /// This is useful when calling render on the parent to make sure that it renders all of it's 
        /// children as well
        /// </summary>
        /// <param name="child">The TagBuilder object to add</param>
        /// <returns>The added child. Use the Parent property to get back to parent builder</returns>
        public TagBuilder AddChild(TagBuilder child)
        {
            child.Parent = this;
            Children.Add(child);
            return child;
        }


        /// <summary>
        /// Retreives a child element by its tag name
        /// </summary>
        /// <param name="tagName">The tag to retreive i.e. img or label etc.</param>
        /// <returns>The Child TagBuilder</returns>
        public TagBuilder GetChildByTagName(string tagName)
        {
            foreach (var child in Children)
            {
                if (child.TagName.ToLowerInvariant() == tagName.ToLowerInvariant())
                    return child;
            }
            return null;
        }

        /// <summary>
        /// Retreives a child element by the element id assigned through the ID function of the tagbuilder class
        /// </summary>
        /// <param name="id">The ID of the element to retreive</param>
        /// <returns>The Child TagBuilder</returns>
        public TagBuilder GetChildById(string id)
        {
            foreach (var child in Children)
            {
                if (child.ElementId.ToLowerInvariant() == id.ToLowerInvariant())
                    return child;
            }
            return null;
        }

        /// <summary>
        /// Adds a class to the element only if a certain condition is met
        /// </summary>
        /// <param name="condition">Condition to use</param>
        /// <param name="classname">Class name to add</param>
        public TagBuilder AddClassIf(bool condition, string classname)
        {
            return AddClassIf(condition, classname, string.Empty);
        }

        /// <summary>
        /// Adds a class to the element only if a certain condition is met. If it adds another class
        /// </summary>
        /// <param name="condition">Condition to use</param>
        /// <param name="classname">Class name to add when condition is true</param>
        /// <param name="otherclassname">Class name to add when condition is false</param>
        public TagBuilder AddClassIf(bool condition, string classname, string otherclassname)
        {
            if (condition)
            {
                AddClass(classname);
            }
            else if (string.IsNullOrEmpty(otherclassname) == false)
            {
                AddClass(otherclassname);
            }
            return this;
        }

        public TagBuilder Add508Style()
        {
            return addStyle("position:absolute; left:-5000px; top:0px; width:1px; height:1px; overflow:hidden;");
        }

        /// <summary>
        /// Adds a class(es) to the tag
        /// </summary>
        /// <param name="classes">List of tags to add</param>
        public TagBuilder AddClass(params string[] classes)
        {
            foreach (var c in classes)
            {
                MergeAttribute("class", c);
            }
            return this;
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the width in pixes
        /// </summary>
        /// <param name="value">Width in pixes</param>
        public TagBuilder width(int value)
        {
            return width(value.ToString(), value.ToString());
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the width in pixes for both the tag and its label
        /// </summary>
        /// <param name="inputWidth">Width in pixels for the input</param>
        /// <param name="labelWidth">Width in pixels for the label</param>
        public TagBuilder width(int inputWidth, int labelWidth)
        {
            return width(inputWidth.ToString(), labelWidth.ToString());
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the width
        /// </summary>
        /// <param name="value">Width value</param>
        public TagBuilder width(string value)
        {
            return width(value, value);
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the width for both the tag and its label
        /// </summary>
        /// <param name="inputWidth">Width for the input</param>
        /// <param name="labelWidth">Width for the label</param>
        public TagBuilder width(string inputWidth, string labelWidth)
        {
            AppendPixels(ref inputWidth);
            AppendPixels(ref labelWidth);
            _labelWidth = labelWidth;
            return MergeAttribute("style", "width: " + inputWidth);
        }

        /// <summary>
        /// Adds a background color to the element
        /// </summary>
        /// <param name="color">The background color to apply (optional)</param>
        public TagBuilder bgcolor(string color)
        {
            return addStyle("background-color: " + color);
        }

        /// <summary>
        /// Adds a background iamge to the element
        /// </summary>
        /// <param name="imageUrl">The url to the background image</param>
        public TagBuilder bgimage(string imageUrl)
        {
            return addStyle("background-image: url('" + imageUrl + "')");
        }

        /// <summary>
        /// Adds the background image to the element
        /// </summary>
        /// <param name="imageUrl">The background image</param>
        /// <param name="repeatHorizontally">True to repeat horizontally</param>
        /// <param name="repeatVertically">True to repeat vertically</param>
        public TagBuilder bgimage(string imageUrl, bool repeatHorizontally, bool repeatVertically)
        {
            return addStyle("background-image: url('" + imageUrl + "')")
                    .bgrepeat(repeatHorizontally, repeatVertically);
        }

        /// <summary>
        /// Adds the background image to the element
        /// </summary>
        /// <param name="imageUrl">The background image</param>
        /// <param name="position">The background position</param>
        /// <param name="repeatHorizontally">True to repeat horizontally</param>
        /// <param name="repeatVertically">True to repeat vertically</param>
        public TagBuilder bgimage(string imageUrl, bool repeatHorizontally, bool repeatVertically, string position)
        {
            return addStyle("background-image: url('" + imageUrl + "')")
                    .bgposition(position)
                    .bgrepeat(repeatHorizontally, repeatVertically);
        }

        /// <summary>
        /// Adds the background-repeat css property to the element
        /// </summary>
        /// <param name="repeatHorizontally">True to repeat horizontally</param>
        /// <param name="repeatVertically">True to repeat vertically</param>
        public TagBuilder bgrepeat(bool repeatHorizontally, bool repeatVertically)
        {
            if (repeatHorizontally && !repeatVertically)
            {
                addStyle("background-repeat: repeat-x");
            }
            else if (!repeatHorizontally && repeatVertically)
            {
                addStyle("background-repeat: repeat-y");
            }
            else if (!repeatHorizontally && !repeatVertically)
            {
                addStyle("background-repeat: no-repeat");
            }

            return this;
        }




        private void AppendPixels(ref string value)
        {
            if (value.Trim().EndsWith("%") == false && value.Trim().EndsWith("px") == false)
            {
                value += "px";
            }
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the width for the label
        /// </summary>
        /// <param name="value">Width value</param>
        public TagBuilder labelwidth(string value)
        {
            if (value.Trim().EndsWith("%") == false && value.Trim().EndsWith("px") == false)
            {
                value += "px";
            }
            _labelWidth = value;
            return this;
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the height for the element
        /// </summary>
        /// <param name="value">Height value</param>
        public TagBuilder height(string value)
        {
            return MergeAttribute("style", "height: " + value);
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the width and height for the tag
        /// </summary>
        /// <param name="width">Width value in pixels</param>
        /// <param name="height">Height value in pixels</param>
        public TagBuilder size(int width, int height)
        {
            return addStyle(string.Format("width: {0}px; height: {1}px", width, height));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the font size for the tag
        /// </summary>
        /// <param name="pixels">Font-size in pixels</param>
        public TagBuilder fontsize(int pixels)
        {
            return MergeAttribute("style", "font-size: " + pixels + "px");
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the font size for the tag
        /// </summary>
        /// <param name="value">Font-size value</param>
        public TagBuilder fontsize(string value)
        {
            return MergeAttribute("style", "font-size: " + value);
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the font-weight for the tag as bold
        /// </summary>
        public TagBuilder bold()
        {
            return MergeAttribute("style", "font-weight: bold");
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the padding-top for the tag
        /// </summary>
        /// <param name="pixels">Padding-top in pixels</param>
        public TagBuilder padTop(int pixels)
        {
            return addStyle(string.Format("padding-top: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the padding-left for the tag
        /// </summary>
        /// <param name="pixels">Padding-left in pixels</param>
        public TagBuilder padLeft(int pixels)
        {
            return addStyle(string.Format("padding-left: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the padding-right for the tag
        /// </summary>
        /// <param name="pixels">Padding-right in pixels</param>
        public TagBuilder padRight(int pixels)
        {
            return addStyle(string.Format("padding-right: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the padding-bottom for the tag
        /// </summary>
        /// <param name="pixels">Padding-bottom in pixels</param>
        public TagBuilder padBottom(int pixels)
        {
            return addStyle(string.Format("padding-bottom: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the padding for the tag in pixes
        /// </summary>
        public TagBuilder pad(int allSidePx)
        {
            return addStyle(string.Format("padding: {0}px", allSidePx));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the padding for the tag in pixes
        /// </summary>
        public TagBuilder pad(int topPx, int rightPx, int bottomPx, int leftPix)
        {
            return addStyle(string.Format("padding: {0}px {1}px {2}px {3}px", topPx, rightPx, bottomPx, leftPix));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the margin-top for the tag
        /// </summary>
        /// <param name="pixels">Margin-top in pixels</param>
        public TagBuilder marginTop(int pixels)
        {
            return addStyle(string.Format("margin-top: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the margin-left for the tag
        /// </summary>
        /// <param name="pixels">Margin-left in pixels</param>
        public TagBuilder marginLeft(int pixels)
        {
            return addStyle(string.Format("margin-left: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the margin-right for the tag
        /// </summary>
        /// <param name="pixels">Margin-right in pixels</param>
        public TagBuilder marginRight(int pixels)
        {
            return addStyle(string.Format("margin-right: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the margin-bottom for the tag
        /// </summary>
        /// <param name="pixels">Margin-bottom in pixels</param>
        public TagBuilder marginBottom(int pixels)
        {
            return addStyle(string.Format("margin-bottom: {0}px", pixels));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the margin for the tag in pixels
        /// </summary>
        public TagBuilder margin(int topPx, int rightPx, int bottomPx, int leftPix)
        {
            return addStyle(string.Format("margin: {0}px {1}px {2}px {3}px", topPx, rightPx, bottomPx, leftPix));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the display as inline-block
        /// </summary>
        public TagBuilder inlineblock()
        {
            return addStyle("display: inline-block");
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the text-decoration as none
        /// </summary>
        public TagBuilder noDecoration()
        {
            return MergeAttribute("style", "text-decoration: none");
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the color
        /// </summary>
        public TagBuilder color(string value)
        {
            return MergeAttribute("style", "color: " + value);
        }

        public TagBuilder bgposition(string value)
        {
            return addStyle("background-position: " + value);
        }

        /// <summary>
        /// Adds the method attribte to the tag. Used in the form tag
        /// </summary>
        /// <param name="value">post, get etc.</param>
        public TagBuilder method(string value)
        {
            return AddAttribute("method", value);
        }

        /// <summary>
        /// Adds the multipart attribute to a form element
        /// </summary>
        public TagBuilder multipart()
        {
            return AddAttribute("enctype", "multipart/form-data");
        }

        /// <summary>
        /// Sets the action on a form element
        /// </summary>
        /// <param name="url">The url to submit the form to</param>
        public TagBuilder action(string url)
        {
            return AddAttribute("action", url);
        }

        /// <summary>
        /// Sets the alt tag on an image element
        /// </summary>
        /// <param name="value">The alt text</param>
        public TagBuilder Alt(string value)
        {
            return AddAttribute("alt", value);
        }

        /// <summary>
        /// Sets the InnerText value of the element. I.E. TextArea inner text
        /// </summary>
        public TagBuilder text(string value)
        {
            return InnerText(value);
        }

        /// <summary>
        /// Sets the InnerText value of the element. I.E. TextArea inner text
        /// </summary>
        public TagBuilder InnerText(string value)
        {
            _innerText = value;
            return this;
        }

        /// <summary>
        /// Sets the InnerHtml value of the element
        /// </summary>
        public TagBuilder html(string value)
        {
            return InnerHtml(value);
        }

        /// <summary>
        /// Sets the InnerHtml value of the element
        /// </summary>
        public TagBuilder InnerHtml(string value)
        {
            _innerHtml = value;
            return this;
        }

        /// <summary>
        /// Sets the id attribute of the element
        /// </summary>
        public TagBuilder ID(string value)
        {
            ElementId = value;
            return AddAttribute("id", value);
        }

        /// <summary>
        /// Sets the name attribute of the element
        /// </summary>
        public TagBuilder Name(string value)
        {
            return AddAttribute("name", value);
        }

        /// <summary>
        /// Sets the id and name attribute of the element
        /// </summary>
        public TagBuilder IDandName(string value)
        {
            ID(value);
            Name(value);
            return this;
        }

        /// <summary>
        /// Sets the for attribute of the label element
        /// </summary>
        public TagBuilder @for(string name)
        {
            return AddAttribute("for", name);
        }

        /// <summary>
        /// Sets the href attribute for an anchor tag
        /// </summary>
        public TagBuilder href(string value)
        {
            return AddAttribute("href", value);
        }

        /// <summary>
        /// Sets the type attribute of input element
        /// </summary>
        public TagBuilder type(string inputtype)
        {
            return AddAttribute("type", inputtype);
        }

        /// <summary>
        /// Sets the value attribute of an input element
        /// </summary>
        public TagBuilder value(object val)
        {
            return TagName.ToLower() == "textarea" ? InnerText(GetValue(val)) : AddAttribute("value", GetValue(val));
        }

        /// <summary>
        /// Sets the value attribute of an input element
        /// </summary>
        public TagBuilder valueIf(bool condition, object val)
        {
            if (!condition) return this;
            return TagName.ToLower() == "textarea" ? InnerText(GetValue(val)) : AddAttribute("value", GetValue(val));
        }

        #region Data-Bind

        /// <summary>
        /// Adds a binding attribute to the element
        /// </summary>
        public TagBuilder dataBind(string binding)
        {
            return MergeAttribute("data-bind", binding);
        }

        /// <summary>
        /// Adds a text binding attribute to the element
        /// </summary>
        public TagBuilder bindText(string value)
        {
            return bindProp("text", value);
        }

        /// <summary>
        /// Adds a visible binding attribute to the element
        /// </summary>
        public TagBuilder bindVisible(string value)
        {
            return bindProp("visible", value);
        }

        /// <summary>
        /// Adds a Html binding attribute to the element
        /// </summary>
        public TagBuilder bindHtml(string value)
        {
            return bindProp("html", value);
        }

        /// <summary>
        /// Adds a css binding attribute to the element
        /// </summary>
        public TagBuilder bindCss(string value)
        {
            return bindProp("css", value);
        }

        /// <summary>
        /// Adds a style binding attribute to the element
        /// </summary>
        public TagBuilder bindStyle(string value)
        {
            return bindProp("style", value);
        }

        /// <summary>
        /// Adds a attribute binding attribute to the element
        /// </summary>
        public TagBuilder bindAttr(string value)
        {
            return bindProp("attr", value);
        }

        /// <summary>
        /// Adds a click event binding attribute to the element
        /// </summary>
        public TagBuilder bindClick(string value)
        {
            return bindProp("click", value);
        }

        /// <summary>
        /// Adds a event event binding attribute to the element
        /// </summary>
        public TagBuilder bindEvent(string value)
        {
            return bindProp("event", value);
        }

        /// <summary>
        /// Adds a submit event binding attribute to the element
        /// </summary>
        public TagBuilder bindSubmit(string value)
        {
            return bindProp("submit", value);
        }

        /// <summary>
        /// Adds a enable binding attribute to the element
        /// </summary>
        public TagBuilder bindEnable(string value)
        {
            return bindProp("enable", value);
        }

        /// <summary>
        /// Adds a disable binding attribute to the element
        /// </summary>
        public TagBuilder bindDisable(string value)
        {
            return bindProp("disable", value);
        }

        /// <summary>
        /// Adds a value binding attribute to the element
        /// </summary>
        public TagBuilder bindValue(string value)
        {
            return bindProp("value", value);
        }

        /// <summary>
        /// Adds a checked binding attribute to the element
        /// </summary>
        public TagBuilder bindChecked(string value)
        {
            return bindProp("checked", value);
        }

        /// <summary>
        /// Adds a options  binding attribute to the element
        /// </summary>
        public TagBuilder bindOptions(string list)
        {
            return bindProp("options", list);
        }

        /// <summary>
        /// Adds a options binding attribute to the element
        /// </summary>
        public TagBuilder bindOptions(string list, string optionsText, string optionsValue)
        {
            return bindOptions(list).bindOptionsText(optionsText).bindOptionsValue(optionsValue);
        }

        /// <summary>
        /// Adds a options binding attribute to the element
        /// </summary>
        public TagBuilder bindOptions(string list, string optionsText, string optionsValue, string optionsCaption)
        {
            return bindOptions(list)
                    .bindOptionsText(optionsText)
                    .bindOptionsValue(optionsValue)
                    .bindOptionsCaption(optionsCaption);
        }

        /// <summary>
        /// Adds a options binding attribute to the element
        /// </summary>
        public TagBuilder bindOptions(string list, string optionsText, string optionsValue, string optionsCaption, string value)
        {
            return bindOptions(list)
                    .bindOptionsText(optionsText)
                    .bindOptionsValue(optionsValue)
                    .bindOptionsCaption(optionsCaption)
                    .bindValue(value);
        }

        /// <summary>
        /// Adds a optionsText binding attribute to the element
        /// </summary>
        public TagBuilder bindOptionsText(string value)
        {
            return bindProp("optionsText", JSBuilder.GetString(value));
        }

        /// <summary>
        /// Adds a optionsValue binding attribute to the element
        /// </summary>
        public TagBuilder bindOptionsValue(string value)
        {
            return bindProp("optionsValue", JSBuilder.GetString(value));
        }

        /// <summary>
        /// Adds a optionsCaption binding attribute to the element
        /// </summary>
        public TagBuilder bindOptionsCaption(string value)
        {
            return bindProp("optionsCaption", JSBuilder.GetString(value));
        }

        /// <summary>
        /// Adds a selectedOptions binding attribute to the element. 
        /// The selectedOptions binding controls which elements in a multi-select list are currently selected. This is intended to be used in conjunction with a <select> element and the options binding
        /// </summary>
        public TagBuilder bindSelectedOptions(string value)
        {
            return bindProp("selectedOptions", value);
        }

        /// <summary>
        /// Adds a template binding attribute to the element
        /// </summary>
        public TagBuilder bindTemplate(string templateId)
        {
            return bindTemplate(templateId, null, null, null, null, null);
        }

        /// <summary>
        /// Adds a template binding attribute to the element with a foreach attribute for iteration
        /// </summary>
        public TagBuilder bindTemplate(string templateId, string forEach)
        {
            return bindTemplate(templateId, null, forEach, null, null, null);

        }

        /// <summary>
        /// Adds a template binding attribute to the element with a foreach attribute for iteration. pass in an empty value to ignore that setting
        /// </summary>
        public TagBuilder bindTemplate(string templateId, string data, string forEach, string afterAdd, string beforeAdd, string templateOptions)
        {
            var templ = new JSObject();
            templ.Add("name", JSBuilder.GetString(templateId))
                 .AddIfNotEmpty("data", data)
                 .AddIfNotEmpty("foreach", forEach)
                 .AddIfNotEmpty("afterAdd", afterAdd)
                 .AddIfNotEmpty("beforeAdd", beforeAdd)
                 .AddIfNotEmpty("templateOptions", templateOptions);
            return bindTemplate("template", templ.ToString());
        }

        /// <summary>
        /// Binds a certain property (visible, text, etc.) to a certain source value
        /// </summary>
        public TagBuilder bindProp(string propName, string value)
        {
            return MergeAttribute("data-bind", propName + ": " + value);
        }

        #endregion

        /// <summary>
        /// Cleans up the value to add to the html
        /// </summary>
        private string GetValue(object val)
        {
            if (val == null)
            {
                return "";
            }
            if (ReferenceEquals(val.GetType(), typeof(int)) && (int)val == 0)
            {
                return "";
            }
            if (ReferenceEquals(val.GetType(), typeof(decimal)) && (decimal)val == 0)
            {
                return "";
            }
            if (ReferenceEquals(val.GetType(), typeof(double)) && (double)val == 0)
            {
                return "";
            }
            if (ReferenceEquals(val.GetType(), typeof(string)) && string.IsNullOrEmpty(Convert.ToString(val)))
            {
                return "";
            }
            if (ReferenceEquals(val.GetType(), typeof(int?)) && ((int?)val).HasValue == false)
            {
                return "";
            }
            if (ReferenceEquals(val.GetType(), typeof(decimal?)) && ((decimal?)val).HasValue == false)
            {
                return "";
            }
            if (ReferenceEquals(val.GetType(), typeof(double?)) && ((double?)val).HasValue == false)
            {
                return "";
            }

            // if a match was not found then shove the tostring in there
            return val.ToString();
        }

        /// <summary>
        /// Add an onclick event to the tag
        /// </summary>
        /// <param name="value">The javascript script either inline or function call</param>
        public TagBuilder onclick(string value)
        {
            return AddAttribute("onclick", value);
        }

        /// <summary>
        /// Add an onclick event to the tag
        /// </summary>
        /// <param name="value">The javascript script either inline or function call</param>
        public TagBuilder onclick(JSBuilder value)
        {
            return AddAttribute("onclick", value.ToString());
        }

        /// <summary>
        /// Add an onclick event to the tag. Uses string.format
        /// </summary>
        /// <param name="value">The javascript script either inline or function call</param>
        /// <param name="formatValues">Values used in the string.format call</param>
        public TagBuilder onclick(string value, params object[] formatValues)
        {
            return AddAttribute("onclick", string.Format(value, formatValues));
        }

        public TagBuilder onclick(string value, params string[] formatValues)
        {
            return AddAttribute("onclick", string.Format(value, formatValues));
        }

        public TagBuilder onclick(string value, params int[] formatValues)
        {
            return AddAttribute("onclick", string.Format(value, formatValues));
        }

        /// <summary>
        /// Add an onclick event to the tag only if a certain codition is met
        /// </summary>
        /// <param name="condition">only adds the value if the condition is true</param>
        /// <param name="value">The javascript script either inline or function call</param>
        public TagBuilder onclickif(bool condition, string value)
        {
            if (condition) AddAttribute("onclick", value);
            return this;
        }

        /// <summary>
        /// Add an onclick event to the tag only if a certain codition is met. Uses string.format
        /// </summary>
        /// <param name="condition">only adds the value if the condition is true</param>
        /// <param name="value">The javascript script either inline or function call</param>
        /// <param name="formatValues">Values used in the string.format call</param>
        public TagBuilder onclickif(bool condition, string value, params string[] formatValues)
        {
            if (condition)
                onclick(value, formatValues);
            return this;
        }

        /// <summary>
        /// Add an onclick event to the tag only if a certain codition is met. Uses string.format
        /// </summary>
        /// <param name="condition">only adds the value if the condition is true</param>
        /// <param name="value">The javascript script either inline or function call</param>
        /// <param name="formatValues">Values used in the string.format call</param>
        public TagBuilder onclickif(bool condition, string value, params int[] formatValues)
        {
            if (condition)
                onclick(value, formatValues);
            return this;
        }

        /// <summary>
        /// Adds javascript to the tag to cancel the enter element
        /// </summary>
        public TagBuilder cancelenter()
        {
            return AddAttribute("onkeypress", "if (event.keyCode == 13) { event.cancelBubble = true; return false; }");
        }

        /// <summary>
        /// Adds javascript to the tag to cancel the enter element
        /// </summary>
        public TagBuilder onEnterKey(string script)
        {
            if (!script.Trim().EndsWith(";")) script += ";";
            return AddAttribute("onkeypress", "if (event.keyCode == 13) { " + script + " return false; }");
        }

        /// <summary>
        /// Add an onchange event to the tag
        /// </summary>
        /// <param name="value">The javascript script either inline or function call</param>
        public TagBuilder onchange(string value)
        {
            return AddAttribute("onchange", value);
        }

        /// <summary>
        /// Add an onmouseover event to the tag
        /// </summary>
        /// <param name="value">The javascript script either inline or function call</param>
        public TagBuilder onmouseover(string value)
        {
            return AddAttribute("onmouseover", value);
        }

        /// <summary>
        /// Add an onmouseover event to the add a style to the tag
        /// </summary>
        public TagBuilder onmouseover_setstyle(string style, string value)
        {
            return onmouseover("jQuery(this).css('" + style + "', '" + value + "');");
        }

        /// <summary>
        /// Add an onmouseout event to add a style to the tag
        /// </summary>
        public TagBuilder onmouseout_setstyle(string style, string value)
        {
            return onmouseover("jQuery(this).css('" + style + "', '" + value + "');");
        }

        /// <summary>
        /// Add an onmouseout event to the tag
        /// </summary>
        /// <param name="value">The javascript script either inline or function call</param>
        public TagBuilder onmouseout(string value)
        {
            return AddAttribute("onmouseout", value);
        }

        /// <summary>
        /// Add the column count to a textarea tag
        /// </summary>
        /// <param name="value">Cols count</param>
        public TagBuilder cols(int value)
        {
            return AddAttribute("cols", value.ToString());
        }

        /// <summary>
        /// Add the rows count to a textarea tag
        /// </summary>
        /// <param name="value">Rows count</param>
        public TagBuilder rows(int value)
        {
            return AddAttribute("rows", value.ToString());
        }

        /// <summary>
        /// Sets the style attribute for the lement. Replaces any other styles previously added. 
        /// To progressively add styles you can use the addStyle method instead
        /// </summary>
        public TagBuilder style(string value)
        {
            return AddAttribute("style", value);
        }

        /// <summary>
        /// Adds the style attribute to the class if not present and appends the current style to it following the correct syntax
        /// </summary>
        /// <param name="value">The style to add i.e. color: white</param>
        public TagBuilder addStyle(string value)
        {
            return MergeAttribute("style", value);
        }

        /// <summary>
        /// Adds the title attribute the the class. Used for links
        /// </summary>
        public TagBuilder title(string value)
        {
            return AddAttribute("title", value);
        }

        /// <summary>
        /// Adds a label to the current tag. Useful when adding an input and you want to also add the label for it
        /// </summary>
        /// <param name="value">The text for the label</param>
        public TagBuilder Label(string value)
        {
            _label = value;
            return this;
        }

        /// <summary>
        /// Adds a label to the current tag. Useful when adding an input and you want to also add the label for it
        /// </summary>
        /// <param name="value">The text for the label</param>
        /// <param name="cssclass">Adds this class to the label</param>
        public TagBuilder Label(string value, string cssclass)
        {
            _label = value;
            _labelclass = cssclass;
            return this;
        }

        /// <summary>
        /// Adds a label to the current tag. Useful when adding an input and you want to also add the label for it
        /// </summary>
        /// <param name="value">The text for the label</param>
        /// <param name="cssclass">Adds this class to the label</param>
        /// <param name="labelIsHtml">Will label the label text as html so it won't be escaped</param>
        public TagBuilder Label(string value, string cssclass, bool labelIsHtml)
        {
            _label = value;
            _labelclass = cssclass;
            _labelIsHtml = labelIsHtml;
            return this;
        }

        /// <summary>
        /// When creating a select list or a radio list etc. this adds a collection of ListItems as the InnerHtml of the 
        /// tag as option elements
        /// </summary>
        public TagBuilder options(SelectItem[] items)
        {
            _options.AddRange(items);
            return this;
            //return InnerHtml(GetOptionsString(items, ""));
        }

        /// <summary>
        /// When creating a select list or a radio list etc. this adds a collection of ListItems as the InnerHtml of the 
        /// tag as option elements. You also specify the value of the selected item in the list.
        /// </summary>
        public TagBuilder options(SelectItem[] items, object value)
        {
            this.value(value);
            _options.AddRange(items);
            _selectedOption = (value == null) ? "" : value.ToString();
            return this;
            //this.value(value);
            //var val = (value == null) ? "" : value.ToString();
            //return InnerHtml(GetOptionsString(items, val));
        }

        /// <summary>
        /// When creating a select list or a radio list etc. this adds a collection of ListItems as the InnerHtml of the 
        /// tag as option elements. You also specify the value of the selected item in the list.
        /// </summary>
        public TagBuilder options(SelectItem[] items, int value)
        {
            this.value(value);
            _options.AddRange(items);
            _selectedOption = (value == 0) ? "" : value.ToString();
            return this;
            //this.value(value);
            //var val = (value == 0) ? "" : value.ToString();
            //return InnerHtml(GetOptionsString(items, val));
        }

        /// <summary>
        /// When creating a select list or a radio list etc. This adds a caption item at the top of the list and selects to specify that no item has been selected
        /// </summary>
        public TagBuilder caption(string caption)
        {
            return this.caption(caption, "-1", true);

        }

        /// <summary>
        /// When creating a select list or a radio list etc. This adds a caption item at the top of the list and selects to specify that no item has been selected
        /// </summary>
        public TagBuilder caption(string caption, string value, bool selected)
        {
            if (selected) this.value(value);
            _options.Insert(0, new SelectItem(caption, value, selected));
            return this;
        }

        /// <summary>
        /// When creating a select list or a radio list etc. This adds a caption item at the top of the list and selects to specify that no item has been selected
        /// </summary>
        public TagBuilder captionIf(bool condition, string caption)
        {
            if (!condition) return this;
            return this.caption(caption, "-1", true);
        }

        /// <summary>
        /// Adds the checked attribute to the element. Used with checkbox elements.
        /// </summary>
        public TagBuilder @checked(bool value)
        {
            return value ? AddAttribute("checked", "checked") : RemoveAttribute("checked");
        }

        /// <summary>
        /// Adds the checked attribute to the element. Used with input elements.
        /// </summary>
        public TagBuilder @readonly()
        {
            return AddAttribute("ReadOnly", "readonly").AddClass("readonlyField");
        }

        /// <summary>
        /// Adds the src attribute to the element. Used with image elements.
        /// </summary>
        public TagBuilder src(string url)
        {
            return AddAttribute("src", url);
        }

        /// <summary>
        /// Adds the target attribute to the element and sets to blank. Used with anchor elements.
        /// </summary>
        public TagBuilder Target_Blank()
        {
            return AddAttribute("target", "_blank");
        }

        /// <summary>
        /// Adds tabIndex to the tag
        /// </summary>
        public TagBuilder tabIndex(int value)
        {
            return AddAttribute("tabindex", value);
        }

        /// <summary>
        /// Adds tabIndex to the tag
        /// </summary>
        public TagBuilder tabIndex(int value, int ignoreValue)
        {
            return AddAttributeIf(ignoreValue != value, "tabindex", value.ToString());
        }

        /// <summary>
        /// Adds tabIndex to the tag
        /// </summary>
        public TagBuilder tabIndex(string value)
        {
            return AddAttribute("tabindex", value);
        }

        public TagBuilder accessKey(int value)
        {
            return AddAttribute("accesskey", value);
        }

        public TagBuilder accessKey(string value)
        {
            return AddAttribute("accesskey", value);
        }

        public TagBuilder accessKey(int value, bool condition)
        {
            if (condition) AddAttribute("accesskey", value);
            return this;
        }

        /// <summary>
        /// This background to the element using a sprite image
        /// </summary>
        /// <param name="img">The sprite image</param>
        /// <param name="bgposition">The x y position of the image to be used in the sprite</param>
        /// <param name="width">The width of the image inside the sprite. sets the width of the element to this</param>
        /// <param name="height">The height of the image inside the sprite. sets the height of the element to this</param>
        public TagBuilder sprite(string img, string bgposition, int width, int height)
        {
            return spriteRollover(img, bgposition, width, height, string.Empty);
        }

        /// <summary>
        /// This adds a rollever effect to the element using a sprite image
        /// </summary>
        /// <param name="img">The sprite image</param>
        /// <param name="bgposition">The x y position of the image to be used in the sprite</param>
        /// <param name="width">The width of the image inside the sprite. sets the width of the element to this</param>
        /// <param name="height">The height of the image inside the sprite. sets the height of the element to this</param>
        /// <param name="rolloverposition">The x y position of the image to be used in the sprite for the rollover. Must be the same width and height as the initial image</param>
        public TagBuilder spriteRollover(string img, string bgposition, int width, int height, string rolloverposition)
        {
            addStyle(string.Format("background-image:url({0}); background-position: {1}; width: {2}px; height: {3}px;", img, bgposition, width, height));
            if (string.IsNullOrEmpty(rolloverposition) == false)
            {
                onmouseover("jQuery(this).css('background-position','" + rolloverposition + "');");
                onmouseout("jQuery(this).css('background-position','" + bgposition + "');");
            }
            return this;
        }

        public TagBuilder spriteRollover(string img, string bgposition, int width, int height, string rolloverposition, string condition)
        {
            addStyle(string.Format("background-image:url({0}); background-position: {1}; width: {2}px; height: {3}px;", img, bgposition, width, height));
            if (string.IsNullOrEmpty(rolloverposition) == false)
            {
                onmouseover("if(" + condition + ") jQuery(this).css('background-position','" + rolloverposition + "');");
                onmouseout("if(" + condition + ") jQuery(this).css('background-position','" + bgposition + "');");
            }
            return this;
        }

        public TagBuilder spriteRollover(string normalClass, string overClass)
        {
            return spriteRollover(normalClass, overClass, false);
        }

        public TagBuilder spriteRollover(string normalClass, string overClass, bool selected)
        {
            AddClass(selected ? overClass : normalClass);
            onmouseover("jQuery(this).removeClass('" + normalClass + "').addClass('" + overClass + "');");
            onmouseout("jQuery(this).removeClass('" + overClass + "').addClass('" + normalClass + "');");
            return this;
        }

        public TagBuilder CollapseFontAndLineHeight()
        {
            return addStyle("font-size:0; line-height:0");
        }

        public TagBuilder showSpriteOnMouseOver(string img, string bgposition)
        {
            return showSpriteOnMouseOver(img, bgposition, false);
        }

        public TagBuilder showSpriteOnMouseOver(string img, string bgposition, bool debug)
        {
            var onscript = string.Format("jQuery(this).css('background-image','url({0})').css('background-position','{1}');", img, bgposition);
            var offscript = "jQuery(this).css('background-image','').css('background-position','');";

            if (debug)
            {
                style("background-position: " + bgposition + "; background-image:url('" + img + "'); ");
                onmouseout(onscript);
                onmouseover(offscript);
            }
            else
            {
                onmouseover(onscript);
                onmouseout(offscript);
            }

            return this;
        }

        public TagBuilder addClassOnMouseOver(string className)
        {
            onmouseover("jQuery(this).addClass('" + className + "');");
            onmouseout("jQuery(this).removeClass('" + className + "');");
            return this;
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the position as absolute top left using pixels
        /// </summary>
        public TagBuilder absolute(int top, int left)
        {
            return addStyle(string.Format("position: absolute; top: {0}px; left: {1}px", top, left));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the position as absolute top right using pixels
        /// </summary>
        public TagBuilder absoluteTR(int top, int right)
        {
            return addStyle(string.Format("position: absolute; top: {0}px; right: {1}px", top, right));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the position as absolute bottom right using pixels
        /// </summary>
        public TagBuilder absoluteBR(int bottom, int right)
        {
            return addStyle(string.Format("position: absolute; bottom: {0}px; right: {1}px", bottom, right));
        }

        /// <summary>
        /// Adds a style attribute if not present to the tag and specifies the position as absolute bottom left using pixels
        /// </summary>
        public TagBuilder absoluteBL(int bottom, int left)
        {
            return addStyle(string.Format("position: absolute; bottom: {0}px; left: {1}px", bottom, left));
        }

        /// <summary>
        /// Adds a class to the element named causesValidation. This is used by the JSHelper to idenfity when to fire the validation routines.
        /// Usually added to links, buttons and other actionable elements to kick start validation when clicked
        /// </summary>
        public TagBuilder causesValidation()
        {
            return AddClass("causesValidation");
        }

        /// <summary>
        /// Adds a placeholder attribute to the elment. Used by jquery.placeholder for watermarking fields
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TagBuilder Placeholder(string value)
        {
            return attr("placeholder", value);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input required
        /// </summary>
        public TagBuilder ValRequired()
        {
            return AddClass("required");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input required
        /// </summary>
        public TagBuilder ValRequired(string errorMsg)
        {
            return AddClass("required").ValMessage("required", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input validate agains a remote url. The remore url must return true when valid
        /// </summary>
        public TagBuilder ValRemote(string remoteUrl)
        {
            return AddAttribute("remote", remoteUrl);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input validate agains a remote url. The remore url must return true when valid
        /// </summary>
        public TagBuilder ValRemote(string remoteUrl, string errorMsg)
        {
            return AddAttribute("remote", remoteUrl).ValMessage("remote", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value have a min length
        /// </summary>
        public TagBuilder ValMinLength(int minLength)
        {
            return AddAttribute("minlength", minLength.ToString());
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value have a min length
        /// </summary>
        public TagBuilder ValMinLength(int minLength, string errorMsg)
        {
            return AddAttribute("minlength", minLength.ToString()).ValMessage("minlength", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value have a max length
        /// </summary>
        public TagBuilder ValMaxLength(int maxLength)
        {
            return AddAttribute("maxlength", maxLength.ToString());
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value have a max length
        /// </summary>
        public TagBuilder ValMaxLength(int maxLength, string errorMsg)
        {
            return AddAttribute("maxlength", maxLength.ToString()).ValMessage("maxlength", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value length be between two numbers
        /// </summary>
        public TagBuilder ValRangeLength(int RangeStart, int RangeEnd)
        {
            return AddAttribute("rangelength", string.Format("[{0},{1}]", RangeStart, RangeEnd));
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value length be between two numbers
        /// </summary>
        public TagBuilder ValRangeLength(int RangeStart, int RangeEnd, string errorMsg)
        {
            return AddAttribute("rangelength", string.Format("[{0},{1}]", RangeStart, RangeEnd)).ValMessage("rangelength", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value me no less than than the min specified
        /// </summary>
        public TagBuilder ValMin(int min)
        {
            return AddAttribute("min", min.ToString());
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value me no less than than the min specified
        /// </summary>
        public TagBuilder ValMin(int min, string errorMsg)
        {
            return AddAttribute("min", min.ToString()).ValMessage("min", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value not be greater than the max specified
        /// </summary>
        public TagBuilder ValMax(int max)
        {
            return AddAttribute("max", max.ToString());
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value not be greater than the max specified
        /// </summary>
        public TagBuilder ValMax(int max, string errorMsg)
        {
            return AddAttribute("max", max.ToString()).ValMessage("max", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value be between two numbers
        /// </summary>
        public TagBuilder ValRange(int start, int end)
        {
            return AddAttribute("range", string.Format("[{0},{1}]", start, end));
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value be between two numbers
        /// </summary>
        public TagBuilder ValRange(int start, int end, string errorMsg)
        {
            return AddAttribute("range", string.Format("[{0},{1}]", start, end)).ValMessage("range", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid email address
        /// </summary>
        public TagBuilder ValEmail()
        {
            return AddClass("email");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid email address
        /// </summary>
        public TagBuilder ValEmail(string errorMsg)
        {
            return AddClass("email").ValMessage("email", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid url address
        /// </summary>
        public TagBuilder ValUrl()
        {
            return AddClass("url");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid url address
        /// </summary>
        public TagBuilder ValUrl(string errorMsg)
        {
            return AddClass("url").ValMessage("url", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid date
        /// </summary>
        public TagBuilder ValDate()
        {
            return AddClass("date");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid date
        /// </summary>
        public TagBuilder ValDate(string errorMsg)
        {
            return AddClass("date").ValMessage("date", errorMsg);
        }

        /// <summary>
        /// Adds date validation to an input control. Makes the input be a valid date based on a certain culture
        /// </summary>
        public TagBuilder ValDate_ForCulture(string culture)
        {
            if (culture.ToLowerInvariant() == "gb")
                return AddClass("dateGB");
            return AddClass("date");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid number
        /// </summary>
        public TagBuilder ValNumber()
        {
            return AddClass("number");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid number
        /// </summary>
        public TagBuilder ValNumber(string errorMsg)
        {
            return AddClass("number").ValMessage("date", errorMsg);
        }

        public TagBuilder ValInt()
        {
            return ValRegExp("^([0-9]+)$", "Must be a whole number without a decimal point");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid credit card
        /// </summary>
        public TagBuilder ValCreditCard()
        {
            return AddClass("creditcard");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid credit card
        /// </summary>
        public TagBuilder ValCreditCard(string errorMsg)
        {
            return AddClass("creditcard").ValMessage("creditcard", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid digits only
        /// </summary>
        public TagBuilder ValDigits()
        {
            return AddClass("digits");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid digits only
        /// </summary>
        public TagBuilder ValDigits(string errorMsg)
        {
            return AddClass("digits").ValMessage("digits", errorMsg);
        }

        /// <summary>
        /// Adds validation to a file input control. Only allows certain extensions for files to upload
        /// </summary>
        public TagBuilder ValAcceptExt(params string[] extensions)
        {
            var extString = "";
            foreach (var e in extensions)
            {
                if (extString.Length > 0)
                    extString += "|";
                extString += e;
            }
            return AddAttribute("accept", extString);
        }

        /// <summary>
        /// Adds validation to a file input control. Only allows certain extensions for files to upload
        /// </summary>
        public TagBuilder ValAcceptExt(string errorMsg, params string[] extensions)
        {
            var extString = "";
            foreach (var e in extensions)
            {
                if (extString.Length > 0)
                    extString += "|";
                extString += e;
            }
            return AddAttribute("accept", extString).ValMessage("accept", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value be the same as the value of another input element
        /// </summary>
        public TagBuilder ValEqualTo(string fieldname)
        {
            if (fieldname.StartsWith("#") == false)
            {
                fieldname = "#" + fieldname;
            }
            return AddAttribute("equalTo", fieldname);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value be the same as the value of another input element
        /// </summary>
        public TagBuilder ValEqualTo(string fieldname, string errorMsg)
        {
            if (fieldname.StartsWith("#") == false)
            {
                fieldname = "#" + fieldname;
            }
            return AddAttribute("equalTo", fieldname).ValMessage("equalTo", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Validates that the value is not equal to the value provided
        /// </summary>
        public TagBuilder ValNot(string value)
        {
            return AddAttribute("not", value);
        }

        /// <summary>
        /// Adds validation to an input control. Validates that the value is not equal to the value provided
        /// </summary>
        public TagBuilder ValNot(string value, string errorMsg)
        {
            return AddAttribute("not", value).ValMessage("not", errorMsg);
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input be a valid password with a minimum length of 7 and incudes at least 1 non-alpha numeric character
        /// </summary>
        public TagBuilder ValComplexPassword()
        {
            return ValRegExp("^.*(?=.{7,})((?=.*\\d)(?=.*[a-z])|(?=.*\\d)(?=.*[a-z])(?=.*[^a-zA-Z0-9])|(?=.*\\d)(?=.*[^a-zA-Z0-9])|(?=.*[a-z])(?=.*[^a-zA-Z0-9])).*$", "Password must be at least 7 characters, must contain a letter and a number, and must also contain one or more non-alphanumeric character.");
        }

        /// <summary>
        /// Adds validation to an input control. Makes the input value match the specified regular expression
        /// </summary>
        public TagBuilder ValRegExp(string expression, string message)
        {
            AddAttribute("regExp", expression);
            return ValMessage("regExp", message);
        }

        public TagBuilder ValPassword()
        {
            return ValPassword("Password must be at least 8 characters, must contain a letter and a number, and one or more special characters.");
        }

        public TagBuilder ValPassword(string message)
        {
            return ValRegExp("^.*(?=.{8,})((?=.*\\d)(?=.*[a-z])|(?=.*\\d)(?=.*[a-z])(?=.*[^a-zA-Z0-9])|(?=.*\\d)(?=.*[^a-zA-Z0-9])|(?=.*[a-z])(?=.*[^a-zA-Z0-9])).*$", message);
        }

        private readonly Dictionary<string, string> _valMessages = new Dictionary<string, string>();
        /// <summary>
        /// Adds validation messages to be used for validators assiged to this element
        /// </summary>
        /// <param name="validator">This is the validator i.e. required</param>
        /// <param name="message">This is the error message to display</param>
        public TagBuilder ValMessage(string validator, string message)
        {
            _valMessages.Add(validator, message);
            return this;
        }

        /// <summary>
        /// Adds an attribute to the element
        /// </summary>
        /// <param name="key">the name of the attribute i.e. href</param>
        /// <param name="value">the value to assign to the attribute i.e. http://google.com</param>
        public TagBuilder attr(string key, int value)
        {
            return AddAttribute(key, value.ToString());
        }

        /// <summary>
        /// Adds an attribute to the element
        /// </summary>
        /// <param name="key">the name of the attribute i.e. href</param>
        /// <param name="value">the value to assign to the attribute i.e. http://google.com</param>
        public TagBuilder attr(string key, string value)
        {
            return AddAttribute(key, value);
        }

        public TagBuilder attrIf(bool condition, string key, string value)
        {
            return AddAttributeIf(condition, key, value);
        }

        /// <summary>
        /// Adds an attribute to the element
        /// </summary>
        /// <param name="key">the name of the attribute i.e. href</param>
        /// <param name="value">the value to assign to the attribute i.e. http://google.com</param>
        public TagBuilder AddAttribute(string key, int value)
        {
            return AddAttribute(key, value.ToString());
        }

        /// <summary>
        /// Adds an attribute to the element
        /// </summary>
        /// <param name="key">the name of the attribute i.e. href</param>
        /// <param name="value">the value to assign to the attribute i.e. http://google.com</param>
        public TagBuilder AddAttribute(string key, string value)
        {
            if (TagAttributes.ContainsKey(key))
            {
                TagAttributes[key] = value;
            }
            else
            {
                TagAttributes.Add(key, value);
            }
            return this;
        }

        public TagBuilder AddAttributes(List<KeyValuePair<string, string>> attributes)
        {
            if (attributes != null && attributes.Count > 0)
            {
                foreach (var attr in attributes)
                {
                    AddAttribute(attr.Key, attr.Value);
                }
            }
            return this;
        }

        public TagBuilder AddAttributeIf(bool condition, string key, string value)
        {
            if (condition == false)
                return this;
            if (TagAttributes.ContainsKey(key))
            {
                TagAttributes[key] = value;
            }
            else
            {
                TagAttributes.Add(key, value);
            }
            return this;
        }

        /// <summary>
        /// Removes an attribute from the element
        /// </summary>
        /// <param name="key">the name of the attribute to remove i.e. href</param>
        public TagBuilder RemoveAttribute(string key)
        {
            if (TagAttributes.ContainsKey(key))
            {
                TagAttributes.Remove(key);
            }
            return this;
        }

        /// <summary>
        /// Adds an attribute to the element if the attribute has not already been added. If it has then it merges the contents 
        /// of the attribute by appending this value. In the case of the style attribute it knows to to properly merge
        /// styles using the right css syntax
        /// </summary>
        /// <param name="key">the name of the attribute style</param>
        /// <param name="value">the value to assign to the attribute color=white</param>
        public TagBuilder MergeAttribute(string key, string value)
        {

            if (TagAttributes.ContainsKey(key))
            {
                // Merge style attributes with a ;
                if (key == "style")
                {
                    var currentStyle = GetAttribute("style").Trim();

                    if (currentStyle.Length > 0)
                    {
                        if (currentStyle.EndsWith(";") == false)
                            currentStyle += "; ";
                        currentStyle += value;
                    }
                    else
                    {
                        currentStyle = value;
                    }

                    TagAttributes[key] = currentStyle;

                    // add a space for all other merging operations
                }
                else if (key == "data-bind")
                {
                    var currentBinding = GetAttribute("data-bind").Trim();

                    if (currentBinding.Length > 0)
                    {
                        if (currentBinding.EndsWith(",") == false && !value.StartsWith(","))
                            currentBinding += ", ";
                        currentBinding += value;
                    }
                    else
                    {
                        currentBinding = value;
                    }

                    TagAttributes[key] = currentBinding;
                }
                else
                {
                    TagAttributes[key] += " " + value;
                }
            }
            else
            {
                TagAttributes.Add(key, value);
            }
            return this;
        }

        /// <summary>
        /// Returns the value of the specified attribute
        /// </summary>
        /// <param name="attrName">The name of the attribute to retreive</param>
        /// <returns>The value of the attribute</returns>
        public string GetAttribute(string attrName)
        {
            return TagAttributes.ContainsKey(attrName) == false ? string.Empty : TagAttributes[attrName];
        }

        /// <summary>
        /// Format all the added attributes into a properly formatted string
        /// </summary>
        private string GetAttributeString()
        {
            const string attributeFormat = " {0}=\"{1}\"";
            var value = "";
            foreach (var key in TagAttributes.Keys)
            {
                value += string.Format(attributeFormat, key, TagAttributes[key]);
            }
            return value;
        }

        /// <summary>
        /// Generates the label string (only when specified) to use for the element
        /// </summary>
        /// <returns></returns>
        private string GetLabelString()
        {
            if (string.IsNullOrEmpty(_label)) return "";

            var lbl = new TagBuilder("label");
            if (string.IsNullOrEmpty(GetAttribute("id")) == false)
            {
                lbl.@for(GetAttribute("id"));
            }
            lbl.AddClass(string.IsNullOrEmpty(_labelclass) == false ? _labelclass : "fieldlabel");
            if (string.IsNullOrEmpty(_labelWidth) == false)
            {
                lbl.width(_labelWidth);
            }
            if (_labelIsHtml)
            {
                lbl.InnerText(_label);
            }
            else
            {
                lbl.InnerHtml(_label);
            }
            return "\r\n" + lbl + "\r\n";
        }

        /// <summary>
        /// Generates the html for all the options added to the element to be included as innerHtml
        /// </summary>
        private string GetOptionsString(IEnumerable<SelectItem> items, string val)
        {
            const string optionFormat = " <option value=\"{0}\" {2}>{1}</option>";
            const string selectedString = " selected=\"selected\"";
            var sb = new StringBuilder();
            var valueFound = false;
            foreach (var item in items)
            {
                var selected = false;
                if (item.Selected)
                {
                    selected = true;
                    valueFound = true;
                }
                else if (string.IsNullOrEmpty(val) == false && item.Value.ToUpper() == val.ToUpper())
                {
                    selected = true;
                    valueFound = true;
                }
                sb.AppendLine(string.Format(optionFormat, item.Value, item.Text, (selected ? selectedString : "")));
            }

            // If the value was not found in the options then add it.
            if (string.IsNullOrEmpty(val) == false && valueFound == false)
            {
                sb.AppendLine(string.Format(optionFormat, val, val, selectedString));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the element properly formated as html code. It also recursively renders all child elements
        /// </summary>
        public override string ToString()
        {
            var renderMode = Mode.HasValue ? Mode.Value : RenderMode.Normal;
            return ToString(renderMode);
        }

        /// <summary>
        /// Returns the element properly formated as html code. It also recursively renders all child elements.
        /// Here you spcify in the maner you want the tag to render (Self closing, no closing tag, or normal)
        /// </summary>
        public string ToString(RenderMode mode)
        {
            const string normalTagFormat = "<{0}{1}>{2}</{0}>";
            const string selfCloseTagFormat = "<{0}{1} />";
            const string openTagFormat = "<{0}{1}>{2}";

            // Create the validation messages string
            var valData = "";
            foreach (var m in _valMessages.Keys)
            {
                if (valData.Length > 0)
                    valData += ", ";
                valData += m + ": '" + _valMessages[m] + "'";
            }
            if (valData.Length > 0)
                AddClass("{messages: {" + valData + "}}");

            // Create the options string
            if (_options.Count > 0) InnerHtml(GetOptionsString(_options, _selectedOption));

            // Create the inner value
            var innerValue = (string.IsNullOrEmpty(_innerHtml) ? HttpUtility.HtmlEncode(_innerText) : _innerHtml);

            // Render the children into the inner html of the element
            foreach (var c in Children)
            {
                innerValue += c.ToString();
            }

            switch (TagName.ToLower())
            {
                case "input":
                    mode = RenderMode.SelfClose;
                    break;
                case "img":
                    mode = RenderMode.SelfClose;
                    break;
            }

            // Create the tag string
            string tagstring;
            if (mode == RenderMode.SelfClose && string.IsNullOrEmpty(innerValue))
            {
                tagstring = string.Format(selfCloseTagFormat, TagName, GetAttributeString());
            }
            else if (mode == RenderMode.openTagOnly)
            {
                tagstring = string.Format(openTagFormat, TagName, GetAttributeString(), innerValue);
            }
            else
            {
                tagstring = string.Format(normalTagFormat, TagName, GetAttributeString(), innerValue);
            }

            // Return the label (if found) and tag string
            return GetLabelString() + tagstring;
        }

    }

    #endregion


    #region SelectItemList

    /// <summary>
    /// This is SelectItem list builder. It aids in the fluid creation of a collection of SelectItems
    /// </summary>
    /// <summary>
    /// This is a list item used when creating RadioList and Select elements
    /// </summary>   
    public class SelectItem
    {

        /// <summary>
        /// Determines if this item is selected in the options list
        /// </summary>
        public bool Selected;

        public SelectItem()
        {
        }

        /// <summary>
        /// Specify the text when creating. Will assign the text to the value and text element
        /// </summary>
        /// <param name="text">The text to use for the option element. Assigned to the value by default as well</param>
        public SelectItem(string text)
        {
            Value = text;
            this.Text = text;
        }

        /// <summary>
        /// Specify the text and value for the option tag
        /// </summary>
        /// <param name="Text">The text to use for the option element.</param>
        /// <param name="Value">The value to use for the option element.</param>
        public SelectItem(string Text, string Value)
        {
            this.Value = Value;
            this.Text = Text;
        }

        /// <summary>
        /// Specify the text and value for the option tag as well as wether it is selected
        /// </summary>
        /// <param name="Text">The text to use for the option element.</param>
        /// <param name="Value">The value to use for the option element.</param>
        /// <param name="Selected">Indictes weather the current item is selected or not</param>
        public SelectItem(string Text, string Value, bool Selected)
        {
            this.Value = Value;
            this.Text = Text;
            this.Selected = Selected;
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (string.IsNullOrEmpty(_value))
                {
                    _value = _text;
                }
            }
        }

    }

    /// <summary>
    /// This is SelectItem list builder. It aids in the fluid creation of a collection of SelectItems
    /// </summary>
    public class SelectItemList
    {

        private List<SelectItem> _items = new List<SelectItem>();

        public SelectItemList() { }

        public SelectItemList(IEnumerable<SelectItem> items)
        {
            _items.AddRange(items);
        }

        /// <summary>
        /// Insers a certain value at the top of the list
        /// </summary>
        public SelectItemList InsertTop(string value, string text)
        {
            _items.Insert(0, new SelectItem(text, value));
            return this;
        }

        /// <summary>
        /// Removes an item list with a particular value
        /// </summary>
        public SelectItemList RemoveValue(string value)
        {
            SelectItem item = null;
            foreach (var i in _items)
            {
                if (i.Value == value) item = i;
            }
            if (item != null) _items.Remove(item);
            return this;
        }

        /// <summary>
        /// Removes an item list with a particular text
        /// </summary>
        public SelectItemList RemoveText(string text)
        {
            SelectItem item = null;
            foreach (var i in _items)
            {
                if (i.Text == text) item = i;
            }
            if (item != null) _items.Remove(item);
            return this;
        }

        /// <summary>
        /// Insers a certain value at the top of the list
        /// </summary>
        public SelectItemList InsertCaption(string text)
        {
            return InsertTop("-1", text);
        }

        /// <summary>
        /// Adds a list item where the text and value is the same
        /// </summary>
        /// <param name="text">Text to use for the list item. Will be used for the value as well</param>
        /// <returns>Retruns the select item list to aid in the fluid interface</returns>
        public SelectItemList Add(string text)
        {
            _items.Add(new SelectItem(text));
            return this;
        }

        /// <summary>
        /// Adds a list item where the text and value might be different
        /// </summary>
        /// <param name="value">The value to assign to the list item</param>
        /// <param name="text">Text to use for the list item.</param>
        /// <returns>Retruns the select item list to aid in the fluid interface</returns>
        public SelectItemList Add(string value, string text)
        {
            _items.Add(new SelectItem(text, value));
            return this;
        }

        /// <summary>
        /// Adds a list item where the text and value might be different
        /// </summary>
        /// <param name="value">The value to assign to the list item</param>
        /// <param name="text">Text to use for the list item.</param>
        /// <returns>Retruns the select item list to aid in the fluid interface</returns>
        public SelectItemList Add(int value, string text)
        {
            _items.Add(new SelectItem(text, value.ToString()));
            return this;
        }

        /// <summary>
        /// Adds a list item where the text and value might be different and you can specify wether the item is selected
        /// </summary>
        /// <param name="value">The value to assign to the list item</param>
        /// <param name="text">Text to use for the list item.</param>
        /// <param name="selected">Determines if the item is selected</param>
        /// <returns>Retruns the select item list to aid in the fluid interface</returns>
        public SelectItemList Add(string value, string text, bool selected)
        {
            _items.Add(new SelectItem(text, value, selected));
            return this;
        }

        /// <summary>
        /// Adds a list of ListItems where the text and value is the same using an array of texts
        /// </summary>
        /// <param name="itemtexts">Array of texts to use when creating the List items. Will be used for the value as well</param>
        /// <returns>Retruns the select item list to aid in the fluid interface</returns>
        public SelectItemList Add(params string[] itemtexts)
        {
            foreach (var t in itemtexts)
            {
                _items.Add(new SelectItem(t));
            }
            return this;
        }

        /// <summary>
        /// Adds a list of SelectItems
        /// </summary>
        /// <param name="items">Array of SelectItem to add</param>
        /// <returns>Returns the select item list to aid in the fluid interface</returns>
        public SelectItemList Add(IEnumerable<SelectItem> items)
        {
            _items.AddRange(items);
            return this;
        }

        /// <summary>
        /// Adds a SelectItem
        /// </summary>
        /// <param name="item">SelectItem</param>
        /// <returns>Returns the select item list to aid in the fluid interface</returns>
        public SelectItemList Add(SelectItem item)
        {
            _items.Add(item);
            return this;
        }

        /// <summary>
        /// Returns the created list of items
        /// </summary>
        public List<SelectItem> ToList()
        {
            return _items;
        }

        /// <summary>
        /// Returns an array of SelectItem
        /// </summary>
        public SelectItem[] ToArray()
        {
            return _items.ToArray();
        }
    
        /// <summary>
        /// Returns the item that matches the provide value
        /// </summary>
        public SelectItem GetItemByValue(string value)
        {
            foreach (var item in _items)
            {
                if (item.Value == value) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns the item that matches the provide text
        /// </summary>
        public SelectItem GetItemByText(string text)
        {
            foreach (var item in _items)
            {
                if (item.Text == text) return item;
            }
            return null;
        }

        /// <summary>
        /// Returns the text of the item that matches the provide value
        /// </summary>
        public string GetItemText(string value)
        {
            var item = GetItemByValue(value);
            if (item != null)
                return item.Text;
            else
                return "";
        }

        /// <summary>
        /// Returns the value of the item that matches the provide text
        /// </summary>
        public string GetItemValue(string text)
        {
            var item = GetItemByText(text);
            if (item != null)
                return item.Value;
            else
                return "";
        }

    }

    #endregion


}