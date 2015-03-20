using System;
using System.Collections.Generic;
using System.Linq;

namespace Bigfoot.Utils.Helpers
{
    [Serializable]
    public class Validator 
    {
        [Serializable]
        public class Error
        {
            public string FieldName { get; set; }
            public string Message { get; set; }
            public string FieldFriendlyName { get; set; }

            public Error(string fieldname, string message)
            {
                FieldName = fieldname;
                FieldFriendlyName = fieldname;
                Message = message;
            }

            public Error(string fieldname, string message, string fieldfriendlyname)
            {
                FieldName = fieldname;
                FieldFriendlyName = fieldfriendlyname;
                Message = message;
            }            
        }

        private List<Error> errors = new List<Error>();
        
        public static Validator Start(List<Error> errors)
        {
            var val = new Validator { errors = errors };
            return val;
        }

        public bool HasErrors { get { return errors.Count > 0; } }

        public List<Error> Errors { get { return errors; } }

        public string ErrorsAsString { get { return GetErrorStringFromList(errors); } }

        public static string GetErrorStringFromList(List<Error> errors)
        {
            var errorstring = "";
            foreach (var error in errors)
            {
                if (errorstring != "") errorstring += " | ";
                errorstring += error.Message;
            }
            return errorstring;
        }

        private void AddError(string message, string fieldname, string friendlyfieldname = "")
        {            
            errors.Add(new Error(fieldname, message, GetFriendlyName(fieldname, friendlyfieldname)));
        }

        private string GetFriendlyName(string fieldname, string friendlyfieldname)
        {
            return string.IsNullOrEmpty(friendlyfieldname) ? fieldname : friendlyfieldname;
        }

        public Validator IsNotEmpty(string value, string fieldname, string friendlyfieldname = "")
        {
            if (string.IsNullOrEmpty(value)) AddError(fieldname + " may not be empty!", fieldname, friendlyfieldname);
            return this;
        }
               

        public Validator IsAValidDateAndIsNotEmpty(string value, string fieldname, string friendlyfieldname = "")
        {
            return IsNotEmpty(value, fieldname).IsAValidDate(value, fieldname);
        }

        public Validator IsNotZero(int value, string fieldname, string friendlyfieldname = "")
        {
            if (value == 0) AddError(fieldname + " may not be 0", fieldname, friendlyfieldname);
            return this;
        }

        public Validator IsGreaterThanZero(int value, string fieldname, string friendlyfieldname = "")
        {
            if (value < 1) AddError(fieldname + " must be greater than zero", fieldname, friendlyfieldname);
            return this;
        }

        public Validator IsNotZero(decimal value, string fieldname, string friendlyfieldname = "")
        {
            if (value == 0) AddError(fieldname + " may not be zero", fieldname, friendlyfieldname);
            return this;
        }

        public Validator IsGreaterThan(int value, int compareto, string fieldname, string friendlyfieldname = "")
        {
            if (value < compareto) AddError(GetFriendlyName(fieldname, friendlyfieldname) + " must be larger than " + compareto, fieldname, friendlyfieldname);
            return this;
        }

        public Validator IsGreaterThan(DateTime value, DateTime compareto, string fieldname, string friendlyfieldname = "")
        {
            if (value < compareto) AddError(GetFriendlyName(fieldname, friendlyfieldname) + " must be greater than " + compareto.ToShortDateString(), fieldname, friendlyfieldname);
            return this;
        }

        public Validator IsLessThan(int value, int compareto, string fieldname, string friendlyfieldname = "")
        {
            if (value > compareto) AddError(GetFriendlyName(fieldname, friendlyfieldname) + " must be less than " + compareto, fieldname, friendlyfieldname);
            return this;
        }

        public Validator IsAValidDate(string value, string fieldname, string friendlyfieldname = "")
        {
            DateTime dvalue;
            if (DateTime.TryParse(value, out dvalue) == false) AddError(GetFriendlyName(fieldname, friendlyfieldname) + " is not a valid date!", fieldname, friendlyfieldname);
            return IsAValidDate(dvalue, fieldname);
        }

        public Validator IsAValidDate(DateTime value, string fieldname, string friendlyfieldname = "")
        {
            if (value == DateTime.MinValue || value == DateTime.MaxValue) AddError(GetFriendlyName(fieldname, friendlyfieldname) + " is not a valid date!", fieldname, friendlyfieldname);
            return this;
        }
        
        public Validator IsTrue(bool positivecondition, string error, string fieldname, string friendlyfieldname = "")
        {
            if (positivecondition == false)
            {
                AddError(error, fieldname, friendlyfieldname);
            }
            return this;
        }

        public Validator IsEqualToOneOfTheseOptions(string value, string fieldname, string friendlyfieldname = "", params string[] options)
        {
            var optionstring = "";
            foreach (var option in options)
            {
                if (value == option) return this;
                optionstring += "\"" + option + "\" ";
            }
            AddError(GetFriendlyName(fieldname, friendlyfieldname) + " must equal one of these values: " + optionstring, fieldname, friendlyfieldname);
            return this;
        }

    }
}