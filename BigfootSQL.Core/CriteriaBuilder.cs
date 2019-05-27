using System;
using System.Collections.Generic;

namespace BigfootSQL.Core
{
    public class CriteriaBuilder
    {
        public class OpenGroup {}
        public class CloseGroup {}
        public class And { }
        public class Or { }
        public class Literal
        {
            public Literal(string sql) { SQL = sql; }
            public Literal(string sql, params string[] formatValues) { SQL = string.Format(sql, formatValues); }
            public string SQL { get; set; }
        }
        
        public List<object> CriteriaStack;
        public string SortBy { get; set; }
        public int MaxRecords = int.MaxValue;

        public bool HasCriteria { get { return CriteriaStack != null && CriteriaStack.Count > 0; } }

        public CriteriaBuilder()
        {
            CriteriaStack = new List<object>();
        }

        public static CriteriaBuilder Create()
        {
            return new CriteriaBuilder();
        }

        private CriteriaBuilder Add(object stackItem)
        {
            CriteriaStack.Add(stackItem);
            return this;
        }

        public CriteriaBuilder BeginGroup()
        {
            return Add(new OpenGroup());
        }

        public CriteriaBuilder EndGroup()
        {
            return Add(new CloseGroup());
        }

        public CriteriaBuilder EqualTo(string fieldName, object value)
        {
            AddField(NewField(fieldName).EqualTo(value));
            return this;
        }

        public CriteriaBuilder NotEqualTo(string fieldName, object value)
        {
            AddField(NewField(fieldName).NotEqualTo(value));
            return this;
        }

        public CriteriaBuilder GreaterThan(string fieldName, object value)
        {
            AddField(NewField(fieldName).GreaterThan(value));
            return this;
        }

        public CriteriaBuilder LessThan(string fieldName, object value)
        {
            AddField(NewField(fieldName).LessThan(value));
            return this;
        }

        public CriteriaBuilder GreaterThanOrEqualTo(string fieldName, object value)
        {
            AddField(NewField(fieldName).GreaterThanOrEqualTo(value));
            return this;
        }

        public CriteriaBuilder LessThanOrEqualTo(string fieldName, object value)
        {
            AddField(NewField(fieldName).LessThanOrEqualTo(value));
            return this;
        }

        public CriteriaBuilder StartsWith(string fieldName, string value)
        {
            AddField(NewField(fieldName).StartsWith(value));
            return this;
        }

        public CriteriaBuilder EndsWith(string fieldName, string value)
        {
            AddField(NewField(fieldName).EndsWith(value));
            return this;
        }

        public CriteriaBuilder Like(string fieldName, string value, bool fullTextSearch)
        {
            AddField(NewField(fieldName).Like(value, fullTextSearch));
            return this;
        }

        public CriteriaBuilder IN(string fieldName, params int[] values)
        {

            AddField(NewField(fieldName).IN(values));
            return this;
        }

        public CriteriaBuilder InRange(string fieldName, int from, int to)
        {
            AddField(NewField(fieldName).InRange(from, to));
            return this;
        }

        public CriteriaBuilder InRange(string fieldName, DateTime from, DateTime to)
        {
            AddField(NewField(fieldName).InRange(from, to));
            return this;
        }

        public CriteriaBuilder InRange(string fieldName, decimal from, decimal to)
        {
            AddField(NewField(fieldName).InRange(from, to));
            return this;
        }

        public CriteriaBuilder AddLiteral(string sql)
        {
            CriteriaStack.Add(new Literal(sql));
            return this;
        }

        public CriteriaBuilder AddLiteral(string sql, params string[] formatValues )
        {
            CriteriaStack.Add(new Literal(sql, formatValues));
            return this;
        }

        public CriteriaBuilder AddField(FieldCriteria fieldCritera)
        {
            return Add(fieldCritera);
        }

        public CriteriaBuilder AddFieldsMatchAll(params FieldCriteria[] fields)
        {
            var first = true;
            foreach (var f in fields)
            {
                if (f.IsEmpty()) continue;
                if (!first) AND();
                AddField(f);
                first = false;
            }
            return this;
        }

        public CriteriaBuilder AddFieldsMatchOne(params FieldCriteria[] fields)
        {
            var first = true;
            foreach (var f in fields)
            {
                if (f.IsEmpty()) continue;
                if (!first) OR();
                AddField(f);
                first = false;
            }
            return this;
        }

        public CriteriaBuilder AND()
        {
            return Add(new And());
        }

        public CriteriaBuilder OR()
        {
            return Add(new Or());
        }

        public CriteriaBuilder AND(FieldCriteria fieldCritera)
        {
            return fieldCritera.IsEmpty() ? this 
                                          : AND().AddField(fieldCritera);
        }

        public CriteriaBuilder OR(FieldCriteria fieldCritera)
        {
            return fieldCritera.IsEmpty() ? this
                                          : OR().AddField(fieldCritera);
        }

        public CriteriaBuilder IsNull(string fieldName, bool value)
        {
            return AddField(NewField(fieldName).IsNull(value));
        }

        public FieldCriteria NewField(string fieldName)
        {
            return new FieldCriteria(fieldName);
        }

        public CriteriaBuilder AddToSortBy(string sort)
        {
            if (!string.IsNullOrEmpty(SortBy)) SortBy += ", ";
            SortBy += sort;
            return this;
        }

        public SqlHelper GenerateSql()
        {
            var sql = new SqlHelper();
            return GenerateSql(sql);
        }
        

        public SqlHelper GenerateSql(SqlHelper sql)
        {
            foreach (var c in CriteriaStack)
            {
                if (c.GetType() == typeof(OpenGroup))
                {
                    sql.OpenParenthesis();
                }
                if (c.GetType() == typeof(CloseGroup))
                {
                    sql.CloseParenthesis();
                }
                if (c.GetType() == typeof(And))
                {
                    sql.AND();
                }
                if (c.GetType() == typeof(Or))
                {
                    sql.OR();
                }
                if (c.GetType() == typeof(FieldCriteria))
                {
                    ((FieldCriteria) c).GenerateSql(sql);
                }
                if (c.GetType() == typeof(Literal))
                {
                    sql.Add(((Literal)c).SQL);
                }
            }
            return sql;
        }
        
    }
}
