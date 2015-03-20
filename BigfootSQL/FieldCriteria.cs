using System;

namespace BigfootSQL
{
    public class FieldCriteria
    {
        public string FieldName { get; set; }
        private object FieldValue { get; set; }
        private object GreaterThanValue { get; set; }
        private object LessThanValue { get; set; }
        private object GreaterThanOrEqualToValue { get; set; }
        private object LessThanOrEqualToValue { get; set; }
        private string FieldValueLike { get; set; }
        private bool FullTextSearch { get; set; }
        private object FieldValueFrom { get; set; }
        private object FieldValueTo { get; set; }
        private object NotEqualToValue { get; set; }        
        private bool? _IsNull { get; set; }        
        private int[] InValues { get; set; }
        private string StartsWithValue { get; set; }
        private string EndsWithValue { get; set; }

        public FieldCriteria(string fieldName)
        {
            FieldName = fieldName;
            _IsNull = new bool?();
        }

        public FieldCriteria EqualTo(object value)
        {
            FieldValue = value;
            return this;
        }

        public FieldCriteria NotEqualTo(object value)
        {
            NotEqualToValue = value;
            return this;
        }

        public FieldCriteria GreaterThan(object value)
        {
            GreaterThanValue = value;
            return this;
        }

        public FieldCriteria LessThan(object value)
        {
            LessThanValue = value;
            return this;
        }

        public FieldCriteria GreaterThanOrEqualTo(object value)
        {
            GreaterThanOrEqualToValue = value;
            return this;
        }

        public FieldCriteria LessThanOrEqualTo(object value)
        {
            LessThanOrEqualToValue = value;
            return this;
        }


        public FieldCriteria Like(string value, bool fullTextSearch)
        {
            FieldValueLike = value;
            FullTextSearch = fullTextSearch;
            return this;
        }

        public FieldCriteria StartsWith(string value)
        {
            StartsWithValue = value;            
            return this;
        }

        public FieldCriteria EndsWith(string value)
        {
            EndsWithValue = value;            
            return this;
        }

        public FieldCriteria InRange(int from, int to)
        {
            FieldValueFrom = from;
            FieldValueTo = to;
            return this;
        }

        public FieldCriteria InRange(DateTime from, DateTime to)
        {
            FieldValueFrom = from;
            FieldValueTo = to;
            return this;
        }

        public FieldCriteria InRange(decimal from, decimal to)
        {
            FieldValueFrom = from;
            FieldValueTo = to;
            return this;
        }

        public FieldCriteria IsNull(bool value)
        {
            _IsNull = value;
            return this;
        }

        public FieldCriteria IN(int[] values)
        {
            InValues = values;
            return this;
        }

        public bool IsEmpty()
        {
            return (FieldValue == null &&
                    NotEqualToValue == null &&
                    GreaterThanValue == null &&
                    LessThanValue == null &&
                    GreaterThanOrEqualToValue == null &&
                    LessThanOrEqualToValue == null &&
                    string.IsNullOrEmpty(FieldValueLike) &&
                    string.IsNullOrEmpty(StartsWithValue) &&
                    string.IsNullOrEmpty(EndsWithValue) &&
                    FieldValueFrom == null &&
                    FieldValueTo == null &&
                    _IsNull.HasValue == false && 
                    InValues == null);
        }

        public SqlHelper GenerateSql()
        {
            var builder = new SqlHelper();
            return GenerateSql(builder);
        }

        public SqlHelper GenerateSql(SqlHelper builder)
        {
            // Exit if it is empty
            if (IsEmpty()) return builder;

            if (FieldValue != null)
            {
                builder.Add(FieldName, FieldValue);
            }
            if (FieldValueFrom != null && FieldValueTo != null)
            {
                builder.BETWEEN(FieldName, FieldValueFrom, FieldValueTo);
            }
            if (!string.IsNullOrEmpty(FieldValueLike))
            {
                builder.LIKE(FieldName, FieldValueLike, FullTextSearch);
            }
            if (GreaterThanValue != null)
            {
                builder.Add(FieldName, ">", GreaterThanValue);
            }
            if (LessThanValue != null)
            {
                builder.Add(FieldName, "<", LessThanValue);
            }
            if (GreaterThanOrEqualToValue != null)
            {
                builder.Add(FieldName, ">=", GreaterThanOrEqualToValue);
            }
            if (LessThanOrEqualToValue != null)
            {
                builder.Add(FieldName, "<=", LessThanOrEqualToValue);
            }
            if (_IsNull.HasValue)
            {
                builder.Add(FieldName + (_IsNull.Value ? " IS NULL" : " IS NOT NULL"));
            }
            if (NotEqualToValue != null)
            {
                builder.Add(FieldName, "!=", NotEqualToValue);
            }
            if (InValues != null)
            {
                builder.IN(FieldName, InValues);
            }
            if (!string.IsNullOrEmpty(StartsWithValue))
            {
                builder.StartsWith(FieldName, StartsWithValue);
            }
            if (!string.IsNullOrEmpty(EndsWithValue))
            {
                builder.EndsWith(FieldName, EndsWithValue);
            }
            return builder;
        }

    }
}
