using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BigfootSQL.Core
{
	/// <summary>
	/// This is a simple SQL syntax builder helper class. It aids in the creation of a SQL statement
	/// by auto creting parameters, as well as preventing against injection attacks etc. It can also
	/// automatically execute the query and hydrate the Model objects.
	/// 
	/// It was designed with simplicity and speed in mind. It is a great replacement to writing
	/// directly against the ADO.NET providers. It is not meant to be a full ORM but rather a rapid query
	/// execution and object hydration helper.
	/// 
	/// It uses a fluid interface for simplicity of code. You must know how to write SQL as it is a light
	/// SQL code builder while automating the rest. It does not generate SQL for you, rather it makes the 
	/// writing, executing, and results mapping simple.
	/// 
	/// Examples (Assumes there is a DB variable of type SqlHelper):
	///     Select a list of orders:
	///         List &lt; OrderListItem &gt; obj;
	///         obj = DB.SELECT("OrderID, OrderDate, ShipToCity").FROM("Orders").WHERE("ShipToState","FL").ExecuteCollection &lt; OrderListItem &gt;();
	///     
	///     Select a single value typed to the correct type:
	///         datetime d;
	///         d = DB.SELECT("OrderDate").FROM("Orders").WHERE("OrderID",OrderID).ExecuteScalar &lt; datetime &gt;()
	/// 
	/// It has several other Execute methods to retrieve DataReaders, DataSets, and many others. Also has ExecuteNonQuery for executing 
	/// void queries.
	/// </summary>
	public class SqlHelper
	{

		#region "Constructors / Private variables"
		private readonly bool _useAzureServiceTokenProvider;
		StringBuilder _sql = new StringBuilder();
		List<DbParameter> _params = new List<DbParameter>();
		public string ConnectionString;

		/// <summary>
		/// Provider factory to be used 
		/// </summary>
		public DbProviderFactory ProviderFactory = null;
		
		public SqlHelper(DbProviderFactory providerFactory = null)
		{
			if (providerFactory == null) providerFactory = SqlClientFactory.Instance;
			ProviderFactory = providerFactory;	
		}
		public SqlHelper(string connectionString, DbProviderFactory providerFactory = null) : this(providerFactory)
		{
			ConnectionString = connectionString;
		}
		/// <summary>
		/// Constructor with options to user Azure service token provider
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		/// <param name="useAzureServiceTokenProvider">Option to use Azure manage identity</param>
		public SqlHelper(string connectionString, bool useAzureServiceTokenProvider)
		{
			ConnectionString = connectionString;
			_useAzureServiceTokenProvider = useAzureServiceTokenProvider;
		}

		#endregion


		#region "AddParam / AddTempParam"

		/// <summary>
		/// Adds a parameter with the provided value and returns the created parameter name
		/// Used when creating dynamic queries and the parameter is not important outside of the immediate query
		/// Used internally to auto created parameters for the WHERE AND OR and other statements
		/// </summary>
		/// <param name="value">The value of the parameter</param>
		/// <returns>The generated name for the parameter</returns>
		public string AddTempParam(object value)
		{
			//var name = "_tempParam" + _params.Count + "_" + Guid.NewGuid().ToString().Substring(24);
			//AddParam(name, value);
			//return "@" + name;
			return AddTempParam(value, "");
		}

		/// <summary>
		/// Adds a parameter with the provided value and returns the created parameter name
		/// Used when creating dynamic queries and the parameter is not important outside of the inmediate query
		/// Used internally to auto created parameters for the WHERE AND OR and other statements
		/// </summary>
		/// <param name="value">The value of the parameter</param>
		/// <param name="prefix">The name to prefix the parameter with</param>
		/// <returns>The generated name for the parameter</returns>
		public string AddTempParam(object value, string prefix)
		{
			prefix = Regex.Replace(prefix, "[^a-zA-Z0-9]", "");
			var name = "TP" +  + _params.Count + "_" + prefix + "_" + Guid.NewGuid().ToString().Substring(24);
			AddParam(name, value);
			return "@" + name;
		}

		/// <summary>
		/// Merges in another SqlHelper object by merging in the parameters and calling ToString on it and appending that output to current helper
		/// </summary>
		/// <param name="helperToMergeIn">The SqlHelper object to merge in</param>
		public SqlHelper MergeSqlHelper(SqlHelper helperToMergeIn)
		{
			Params.AddRange(helperToMergeIn.Params);
			return Add(helperToMergeIn.ToString());
		}

		/// <summary>
		/// Gets the list of parameters currently in the helper
		/// </summary>
		public List<DbParameter> Params { get { return _params; } }

		/// <summary>
		/// Adds a named parameter to the query
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the paremter</param>
		/// <param name="addcomma">Adds a comma before appending the parameter to the statement</param>
		public SqlHelper AddProcParam(string name, object value, bool addcomma = false)
		{
			if (!name.StartsWith("@")) name = "@" + name;
			AddParam(name, value);
			Add((addcomma ? ", " : "") + name + " = " + name);
			return this;
		}

		/// <summary>
		/// Adds a named parameter to the query
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="value">The value of the paremter</param>
		public SqlHelper AddParam(string name, object value)
		{
			if (name.StartsWith("@")) name = name.Substring(1);
			var param = DbProviderFactory().CreateParameter();
			param.ParameterName = name;

			// Determine if the value is null
			bool isnull = false;
			if (value != null && value is DateTime && (DateTime)value == DateTime.MinValue) isnull = true;
			if (value != null && value is DateTimeOffset && (DateTimeOffset)value == DateTimeOffset.MinValue) isnull = true;
			//if (value != null && value is int && (int)value == 0) isnull = true;
				
			// DateTime.MinValue will be considered a null value
			if (isnull)
			{
				param.Value = DBNull.Value;
			}
			else
			{
				param.Value = value;    
			}
			_params.Add(param);
			return this;
		}

		/// <summary>
		/// Adds an output parameter to the query
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="paramType">The parameter type</param>
		/// <param name="addComma">The value of the paremter</param>
		public SqlHelper AddOutputParam(string name, DbType paramType, bool addComma = false)
		{
			if (!name.StartsWith("@"))
				name = "@" + name;

			var param = DbProviderFactory().CreateParameter();
			param.ParameterName = name.StartsWith("@") ? name.Substring(1) : name;
			param.Direction = ParameterDirection.Output;
			param.DbType = paramType;

			// set varchar size to max
			if (paramType == DbType.String)
				param.Size = -1;

			_params.Add(param);
			Add((addComma ? ", " : "") + name + " = " + name + " OUTPUT");
			return this;
		}

		/// <summary>
		/// Adds an InputOutput parameter to the query
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="paramType">The parameter type</param>
		/// <param name="addComma">The value of the paremter</param>
		public SqlHelper AddInputOutputParam(string name, DbType paramType, object value, bool addComma = false)
		{
			if (!name.StartsWith("@"))
				name = "@" + name;

			var param = DbProviderFactory().CreateParameter();
			param.ParameterName = name.StartsWith("@") ? name.Substring(1) : name;
			param.Direction = ParameterDirection.InputOutput;
			param.DbType = paramType;

			// set varchar size to max
			if (paramType == DbType.String)
				param.Size = -1;

			// Determine if the value is null
			bool isnull = false;
			if (value != null && value is DateTime && (DateTime)value == DateTime.MinValue) isnull = true;

			// DateTime.MinValue will be considered a null value
			if (isnull)
			{
				param.Value = DBNull.Value;
			}
			else
			{
				param.Value = value;
			}

			_params.Add(param);
			Add((addComma ? ", " : "") + name + " = " + name + " OUTPUT");
			return this;
		}

		private bool HasParams
		{
			get { return _params.Count > 0; }
		}

		#endregion
		

		#region "SQL Builder methods"

		public StringBuilder RawBuilder
		{
			get { return _sql; }
		}
		
		/// <summary>
		/// Add literal SQL statement to the query
		/// </summary>
		/// <param name="sql">SQL Fragment to add</param>
		public SqlHelper Add(string sql)
		{
			return Append(sql);
		}

		/// <summary>
		/// Add literal SQL statement to the query
		/// </summary>
		/// <param name="sql">SQL Fragment to add</param>
		public SqlHelper Add(SqlHelper sql)
		{
			Params.AddRange(sql.Params);
			return Append(sql.ToString());
		}

		private SqlHelper Append(string sql)
		{
			if (_sql.Length > 0) sql = " " + sql + " ";
			//if (_sql.Length > 0 && sql.EndsWith(" ") == false) _sql.Append(" ");
			_sql.Append(Translate(sql));
			return this;
		}

		/// <summary>
		/// Add a parameter to a WHERE statement. Will generate ColumnName = 'Value' (quotes only added if it is a string)
		/// </summary>
		/// <param name="wherecolumn">The name of the column to search</param>
		/// <param name="value">The value to search for</param>
		public SqlHelper Add(string wherecolumn, object value)
		{
			return Add(wherecolumn, "=", value, false);
		}

		/// <summary>
		/// Add a parameter to a WHERE statement if the condition is met
		/// </summary>
		/// <param name="condition">The condition to be met before the where statement is added</param>
		/// <param name="wherecolumn">The name of the column to search</param>
		/// <param name="operator">The operator for the where statement</param>
		/// <param name="value">The value to search for</param>
		public SqlHelper AddIf(bool condition, string wherecolumn, string @operator, object value)
		{
			if (condition)
			{
				Add(wherecolumn, @operator, value);
			}
			return this;
		}

		/// <summary>
		/// Add a parameter to a WHERE statement. Will generate ColumnName {Operator} 'Value' (quotes only added if it is a string)
		/// </summary>
		/// <param name="wherecolumn">The of the column to search</param>
		/// <param name="value">The value to search for. If it is a string it is properly escaped etc.</param>
		/// /// <param name="isSet">Identifies this comparison as a set statement. Needed for setting null values</param>
		public SqlHelper Add(string wherecolumn, object value, bool isSet )
		{
			return Add(wherecolumn, "=", value, isSet);
		}

		/// <summary>
		/// Add a parameter to a WHERE statement. Will generate ColumnName {Operator} 'Value' (quotes only added if it is a string)
		/// </summary>
		/// <param name="wherecolumn">The of the column to search</param>
		/// <param name="operator">The operator for the search. e.g. = &lt;= LIKE &lt; &gt;> etc.</param>
		/// <param name="value">The value to search for. If it is a string it is properly escaped etc.</param>
		/// <param name="isSet">Identifies this comparison as a set statement. Needed for setting null values</param>
		public SqlHelper Add(string wherecolumn, string @operator, object value, bool isSet = false)
		{
			if (value == null)
			{
				Add(wherecolumn);
				return isSet ? Add("= NULL") : 
							   Add("IS NULL");
			}
			else
				return Add(wherecolumn).Add(@operator).Add(AddTempParam(value, wherecolumn));
		}

		/// <summary>
		/// Turns on the Identity Insert option On for the table
		/// </summary>
		/// <param name="tablename">The table name for which to turn on the identity insert</param>
		public SqlHelper SET_IDENTITY_INSERT_ON(string tablename)
		{
			return Add("SET IDENTITY_INSERT " + tablename + " ON");
		}

		/// <summary>
		/// Turns on the Identity Insert option On for the table
		/// </summary>
		/// <param name="tablename">The table name for which to turn on the identity insert</param>
		public SqlHelper SET_IDENTITY_INSERT_OFF(string tablename)
		{
			return Add("SET IDENTITY_INSERT " + tablename + " OFF");
		}

		/// <summary>
		/// Appends GO to the statement, used when creating compound statements
		/// </summary>
		public SqlHelper GO()
		{
			return Add("GO");
		}

		public SqlHelper LIKE(string wherecolumn, string value)
		{
			return Add(wherecolumn).Add("LIKE").Add("'%" + EscapeForLike(value,true) + "%'");
		}

		public SqlHelper StartsWith(string wherecolumn, string value)
		{
			return Add(wherecolumn).Add("LIKE").Add("'" + EscapeForLike(value, true) + "%'");
		}

		public SqlHelper EndsWith(string wherecolumn, string value)
		{
			return Add(wherecolumn).Add("LIKE").Add("'%" + EscapeForLike(value, true) + "'");
		}

		public SqlHelper Contains(string wherecolumn, string value)
		{
			return LIKE(wherecolumn, value);
		}

		public SqlHelper LIKE(string wherecolumn, string value, bool fullTextSearch)
		{
			if (!string.IsNullOrEmpty(value) && fullTextSearch)
			{
				var firstWord = true;
				OpenParenthesis();
				foreach (var term in value.Split(' '))
				{
					if (firstWord) firstWord = false; else AND();
					LIKE(wherecolumn, term);
				}
				CloseParenthesis();
				return this;
			}
			else
			{
				return LIKE(wherecolumn, value);
			}
		}

		public SqlHelper SELECT(string sql)
		{
			return Add("SELECT " + sql);
		}

		public SqlHelper CASE()
		{
			return Add("CASE");
		}

		public SqlHelper CASE(string condition, string trueResult, string falseResult)
		{
			return CASE().WHEN(condition, trueResult).ELSE(falseResult).END();
		}

		public SqlHelper AS(string columnName)
		{
			return Add("AS " + columnName);
		}

		public SqlHelper WHEN(string condition, string trueResult)
		{
			return Add(string.Format("WHEN {0} THEN {1}", condition, trueResult));
		}

		public SqlHelper ELSE(string elseResult)
		{
			return Add("ELSE " + elseResult);
		}

		public SqlHelper END()
		{
			return Add("END");
		}

		/// <summary>
		/// Used to returned a paged result set
		/// </summary>
		/// <param name="columns">Columns to include in the paged result. When paging this you can specify columns only once</param>
		/// <param name="pageorderby">This is used to generate the RowNumber field resulting in: 
		/// ROW_NUMBER() OVER(ORDER BY Field1, Field2) AS RowNumber </param>
		/// <param name="pageSize">How many records per page</param>
		/// <returns></returns>
		public SqlHelper SELECTPAGED(string columns, string pageorderby, int pageSize)
		{
			//_pageorderby = pageorderby;
			_pageSize = pageSize;
			return Add("SELECT " + columns + ", ROW_NUMBER() OVER(ORDER BY " + pageorderby + " ) AS RowNumber");
		}
		//private string _pageorderby;
		private int _pageSize;

		/// <summary>
		/// Call this function to identify which page to get. Must be called at the end of the statement.
		/// </summary>
		/// <param name="currentpage">The current page (0 based)</param>
		public SqlHelper PAGE(int currentpage)
		{
			var rowStart = _pageSize * currentpage;
			var rowEnd = (_pageSize * currentpage) + _pageSize + 1;
			_sql.Insert(0, "SELECT * FROM (");
			_sql.AppendFormat(") PagedResult WHERE RowNumber > " + rowStart + " AND RowNumber < " + rowEnd);
			//_sql.AppendFormat(") PagedResult WHERE RowNumber > @StartRow AND RowNumber < @EndRow ORDER BY " + _pageorderby);
			return this;
		}
		
		public SqlHelper SELECT_ALL_FROM(string tablename)
		{
			return Add("SELECT * FROM").Add(Translate(tablename));
		}

		public SqlHelper SELECT_ALL_FROM(string tablename, int topCount)
		{
			var topString = topCount == 0 ? "" : " TOP " + topCount + " ";
			return Add("SELECT" + topString + " * FROM").Add(Translate(tablename));
		}

		public SqlHelper SELECT_TOP(int max, string sql)
		{
			return Add("SELECT TOP " + max + " " + sql);
		}
		
		public SqlHelper SELECT_COUNT_ALL_FROM(string tablename)
		{
			return Add("SELECT COUNT(*) FROM").Add(Translate(tablename));
		}

		public SqlHelper SELECT_COUNT_ALL_FROM(SqlHelper query, string alias)
		{
			return Add("SELECT COUNT(*) FROM (").Add(query).Add(")").Add(alias);
		}
		
		public SqlHelper SELECT(params string[] columns)
		{
			var s = "SELECT ";
			var firstcolumn = true;
			foreach (string c in columns)
			{
				if (firstcolumn) {
					s += columns;
					firstcolumn = false;
				}
				else
				{
					s += ", " + c;
				}
			}
			return Add(s);
		}

		public SqlHelper SELECT_IDENTITY()
		{
			return Add("SELECT @@IDENTITY");
		}

		public SqlHelper INNERJOIN(string sql)
		{
			return Add("INNER JOIN " + sql);
		}

		public SqlHelper INNERJOIN(SqlHelper sql, string alias)
		{
			//add the parameters
			_params.AddRange(sql.Params);
			return Add("INNER JOIN (" + sql + ") " + alias);
		}

		public SqlHelper LEFTJOIN(string sql)
		{
			return Add("LEFT JOIN " + sql);
		}

		public SqlHelper LEFTJOIN(SqlHelper sql, string alias)
		{
			Params.AddRange(sql.Params);
			return Add("LEFT JOIN (" + sql + ") " + alias);
		}

		public SqlHelper OUTERJOIN(string sql)
		{
			return Add("FULL JOIN " + sql);
		}

		public SqlHelper UNIONALL(string sql)
		{
			return Add("UNION ALL " + sql);
		}

		public SqlHelper UNIONALL(SqlHelper sql)
		{
			Params.AddRange(sql.Params);
			return Add("UNION ALL " + sql);
		}

		public SqlHelper OUTERJOIN(SqlHelper sql, string alias)
		{
			Params.AddRange(sql.Params);
			return Add("FULL JOIN (" + sql + ") " + alias);
		}

		public SqlHelper ON(string sql)
		{
			return Add("ON " + sql);
		}

		public SqlHelper ON(string leftcolumn, string rightcolumn)
		{
			return Add("ON " + leftcolumn + " = " + rightcolumn);
		}

		public SqlHelper FROM(string sql)
		{
			return Add("FROM " + sql);
		}

		public SqlHelper FROM(SqlHelper sql, string alias)
		{
			Params.AddRange(sql.Params);
			return Add("FROM (" + sql + ") " + alias);
		}

		public SqlHelper WHERE()
		{
			return Add("WHERE");
		}

		public SqlHelper WHERE(string sql)
		{
			return Add("WHERE " + sql);
		}

		public SqlHelper WHERE(SqlHelper sql)
		{   
			return Add("WHERE ").Add(sql);
		}

		public SqlHelper WHERE(string columnname, object value)
		{
			return Add("WHERE").Add(columnname, value);
		}

		public SqlHelper WHERE(string columnname, string @operator, object value)
		{
			return Add("WHERE").Add(columnname, @operator, value);
		}
		
		public SqlHelper ISNOTNULL(string columnName)
		{
			return Add(columnName + " IS NOT NULL");
		}

		public SqlHelper ANDISNOTNULL(string columnName)
		{
			return AND().ISNOTNULL(columnName);
		}

		public SqlHelper ORISNOTNULL(string columnName)
		{
			return OR().ISNOTNULL(columnName);
		}

		public SqlHelper ISNULL(string columnName)
		{
			return Add(columnName + " IS NULL");
		}

		public SqlHelper ANDISNULL(string columnName)
		{
			return AND().ISNULL(columnName);
		}

		public SqlHelper ORISNULL(string columnName)
		{
			return OR().ISNULL(columnName);
		}

		public SqlHelper IN(string columnname, int value)
		{
			return Add(columnname + " IN (" + value + ")");
		}

		public SqlHelper IN(string columnname, string value)
		{
			return Add(columnname + " IN ('" + EscapeApostrophe(value) + "')");
		}

		public SqlHelper WHEREAND()
		{
			return ToString().Contains("WHERE") ? Add("AND") 
												: Add("WHERE");
		}

		public SqlHelper WHEREOR()
		{
			return ToString().Contains("WHERE") ? Add("OR")
												: Add("WHERE");
		}

		public SqlHelper IN(string columnname, params string[] values)
		{

			return Add(columnname + " IN (" + ArrayToINstring(values) + ")");
		}

		public SqlHelper IN(string columnname, bool searchAsIntArray, params string[] values)
		{

			return Add(columnname + " IN (" + ArrayToINint(values) + ")");
		}

		public SqlHelper IN(string columnname, params int[] values)
		{

			return Add(columnname + " IN (" + ArrayToINint(values) + ")");
		}

		public SqlHelper IN(string columnname, SqlHelper sql)
		{
			return Add(columnname + " IN (").MergeSqlHelper(sql).Add(")");
		}

		public SqlHelper ORDERBY(string sql)
		{
			return Add("ORDER BY " + sql);
		}

		public SqlHelper GROUPBY(string sql)
		{
			return Add("GROUP BY " + sql);
		}

		public SqlHelper HAVING(string sql)
		{
			return Add("HAVING " + sql);
		}


		readonly List<string> _insertColumns = new List<string>();


		public SqlHelper INSERTINTO(string tablename, string columns)
		{
			_insertColumns.AddRange(columns.Split(','));
			return Add("INSERT INTO " + tablename + "(" + columns + ")");
		}

		public SqlHelper VALUES(string sql)
		{
			return Add("VALUES ( " + sql + " ) ");
		}

		public SqlHelper VALUES(params object[] ps)
		{
			Add("VALUES (");
			for (var i = 0; i < ps.Length; i++)
			{
				var prefix = i < _insertColumns.Count ? _insertColumns[i] : "";
				var p = ps[i];
				if (i > 0) Add(",");
				Add(AddTempParam(p, prefix));
			}
			return Add(")");
		}



		public SqlHelper OP()
		{
			return OpenParenthesis();
		}

		public SqlHelper OpenParenthesis()
		{
			return Add("(");
		}

		public SqlHelper OP(string wherecolumn, object value)
		{
			return OpenParenthesis(wherecolumn, value);
		}

		public SqlHelper OpenParenthesis(string wherecolumn, object value)
		{
			return Add("(").Add(wherecolumn,value);
		}

		public SqlHelper CP()
		{
			return CloseParenthesis();
		}

		public SqlHelper CloseParenthesis()
		{
			return Add(")");
		}

		public SqlHelper UPDATE(string sql)
		{
			return Add("UPDATE " + sql);
		}

		public SqlHelper SET()
		{
			return Add("SET");
		}

		public SqlHelper SET(string columnname, object value)
		{
			// Grabs the status as of the last update... this allows for multiple update statements
			var startIndex = _sql.ToString().LastIndexOf(" UPDATE ", StringComparison.InvariantCultureIgnoreCase);
			if (startIndex == -1)
			{
				startIndex = 0;
			}
			var sql = _sql.ToString().Substring(startIndex).TrimEnd();

			// Add the first SET if not found
			if (sql.IndexOf(" SET", StringComparison.InvariantCultureIgnoreCase) == -1)
				Add("SET");
			else if (!sql.TrimEnd().EndsWith(","))
				Add(",");
			
			//// Adds a comma when required
			//if (!sql.TrimEnd().EndsWith(" SET") && !sql.TrimEnd().EndsWith(",")) Add(",");

			//if (!_sql.ToString().TrimEnd().EndsWith(" SET") && 
			//    !_sql.ToString().TrimEnd().EndsWith(","))
			//    Add(",");

			Add(columnname, value, true);

			return this;
		}

		public SqlHelper DELETEFROM(string sql)
		{
			return Add("DELETE FROM " + sql);
		}

		public SqlHelper AND()
		{
			return Add("AND");
		}

		public SqlHelper AND(string sql)
		{
			return Add("AND " + sql);
		}

		public SqlHelper AND(string column, object value)
		{
			return Add("AND").Add(column, value);
		}

		public SqlHelper AND(string column, string @operator, object value)
		{
			return Add("AND").Add(column, @operator, value);
		}

		public SqlHelper AND_BETWEEN(string column, DateTime startValue, DateTime endValue)
		{
			return AND().BETWEEN(column, startValue, endValue);
		}

		public SqlHelper BETWEEN(string column, object startValue, object endValue)
		{
			return Add(column + " BETWEEN " + AddTempParam(startValue, column) + " AND " + AddTempParam(endValue, column));
		}

		public SqlHelper BETWEEN(string value, string startColumn, string endColumn)
		{
			return Add(value + " BETWEEN " + startColumn + " AND " + endColumn);
		}
		
		public SqlHelper OR() 
		{
			return Add("OR"); 
		}

		public SqlHelper OR(string sql)
		{
			return Add("OR " + sql);
		}

		public SqlHelper OR(string column, object value)
		{
			return Add("OR").Add(column, value);
		}

		public SqlHelper OR(string column, string @operator, object value )
		{
			return Add("OR").Add(column, @operator, value);
		}

		public SqlHelper OR_BETWEEN(string column, DateTime startValue, DateTime endValue)
		{
			return OR().BETWEEN(column, startValue, endValue);
		}

		public SqlHelper DECLARE(string varname, string vartype)
		{
			if (varname.StartsWith("@")==false ) varname = "@" + varname;
			return Add("DECLARE " + varname + ((vartype.ToLower() == "table") ? " AS " : " ") + vartype);
		}

		public SqlHelper DECLARE(string varname, string vartype, object value)
		{
			if (varname.StartsWith("@")==false ) varname = "@" + varname;
			return DECLARE(varname, vartype).Add("SET " + varname + " = " + AddTempParam(value, varname.Substring(1)));
		}

		public SqlHelper DECLARE_TABLE(string varname, string columns)
		{
			return DECLARE(varname, "TABLE").Add("( " + columns + " )");
		}

		/// <summary>
		/// Execute a StoredProcedure without parameters
		/// </summary>
		/// <param name="spname">SP Name</param>
		public SqlHelper EXECUTE(string spname)
		{
			return Add("EXECUTE " + spname);
		}

		/// <summary>
		/// Execute a StoredProcedure. By passing the SP name and a list values as parameters to the SP
		/// </summary>
		/// <param name="spname">SP Name</param>
		/// <param name="ps">Parameters for the Stored Procedure in the proper order</param>
		public SqlHelper EXECUTE(string spname, params object[] ps)
		{
			Add("EXECUTE " + spname);
			var first = true;
			foreach (var p in ps)
			{
				if (first)
					first = false;
				else
					Add(",");

				Add(AddTempParam(p));
			}
			return this;
		}

		/// <summary>
		/// Execute a function by passing the function anem of and a list of paremters. 
		/// e.g. SELECT_SCALARFUNCTION("GetOrderName", OrderDate, CustomerID)
		/// </summary>
		/// <param name="fname">Name of the function</param>
		/// <param name="ps">List of parameters values to pass into the function. Must be in the right order</param>
		public SqlHelper SELECT_SCALARFUNCTION(string fname, params object[] ps)
		{
			Add("SELECT " + fname + "(");
			var first = true;
			foreach (var p in ps)
			{
				if (first)
					first = false;
				else
					Add(",");

				Add(AddTempParam(p));
			}
			Add(")");
			return this;
		}

		private void AddIfNotFound(string statement)
		{
			if (_sql.ToString().IndexOf(statement) == -1)
			{
				Add(statement);
			}
		}

		#endregion

		

		#region "ToString / DebugSql / Clear"

		/// <summary>
		/// Clear the current query
		/// </summary>
		public void Clear()
		{
			_sql = new StringBuilder();
			_params=new List<DbParameter>();
		}

		/// <summary>
		/// Auto writes the finished statement as close as possible.
		/// </summary>
		public override string ToString()
		{
			Debug.WriteLine(DebugSql);
			return _sql.ToString();
		}
		
		/// <summary>
		/// Creates an executable SQL statement including declaration of SQL parameters for debugging purposes.
		/// </summary>
		public string DebugSql
		{
			get
			{
				var value = "====NEW QUERY====\n";
				foreach (DbParameter param in _params)
				{
					var dbValue = "";
					var addQuotes = (param.DbType == DbType.String ||
									 param.DbType == DbType.DateTime ||
					                 param.DbType == DbType.DateTimeOffset);
					dbValue = param.Value == null ? "NULL" : EscapeApostrophe(param.Value.ToString());
					if (addQuotes && dbValue != "NULL") dbValue = "'" + dbValue + "'";
					if (param.DbType == DbType.Boolean)
						dbValue = dbValue == "False" ? "0" : "1";
					if (param.DbType == DbType.DateTime && dbValue != "NULL" && (DateTime)param.Value == DateTime.MinValue)
						dbValue = "NULL";
					if (param.DbType == DbType.DateTimeOffset && dbValue != "NULL" && (DateTimeOffset)param.Value == DateTimeOffset.MinValue)
						dbValue = "NULL";

					var dbTypeName = param.DbType.ToString();
					if (param.DbType == DbType.String)
						dbTypeName = "nvarchar(max)";
					else if (param.DbType == DbType.Int32)
						dbTypeName = "int";
					else if (param.DbType == DbType.DateTimeOffset)
						dbTypeName = "DateTimeOffset";
					else if (param.DbType == DbType.DateTime)
						dbTypeName = "DateTime";
					else if (param.DbType == DbType.Boolean)
						dbTypeName = "bit";

					value += "DECLARE @" + param.ParameterName + " " + dbTypeName;
					value += " SET @" + param.ParameterName + " = " + dbValue;
					
					value += "\n";
				}
				value += _sql + "\n";
				
				//Add \r\n before each of these words
				string[] words = { "SELECT", "FROM", "WHERE", "INNER JOIN", "LEFT JOIN", "ORDER BY", "GROUP BY", "DECLARE", 
									 "SET", "VALUES", "INSERT INTO", "DELETE FROM", "UPDATE" };
				foreach (string w in words)
					value = value.Replace(w, "\r\n" + w);
				
				// Return the value
				return value;
			}
		}

		

		#endregion


		#region "DotNetNuke integration: Translate | GetName"

		public String Owner;
		public String Qualifier;
		public String ModuleQualifier;
		
		public SqlHelper(string owner, string qualifier, string moduleQualifier, string connectionString)
		{
			if (!String.IsNullOrEmpty(owner) && !owner.EndsWith("."))
				owner += ".";

			if (!String.IsNullOrEmpty(qualifier) && !qualifier.EndsWith("_"))
				qualifier += "_";

			if (!String.IsNullOrEmpty(moduleQualifier) && !moduleQualifier.EndsWith("_"))
				moduleQualifier += "_";

			Owner = owner;
			Qualifier = qualifier;
			ModuleQualifier = moduleQualifier;
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Creates a new SqlHelper object the same as this except without any sql commands in it 
		/// </summary>
		/// <returns>New sql helper object with the Owner, Qualifier, etc. properly configured</returns>
		public SqlHelper New() { return new SqlHelper(Owner, Qualifier, ModuleQualifier, ConnectionString); }

		/// <summary>
		/// Used to support DotNetNuke queries where database object names
		/// are dynamic based on the prefix of the installation.
		/// By using {{{TableName}}} it will translate into a proper DNN core table name
		/// By using {{TableName}} it will translate into a proper custom DNN Module table name
		/// </summary>
		/// <param name="sql">The sql fragment to translate</param>
		/// <returns>The translated string</returns>
		public string Translate(string sql)
		{
			// Translate databaseOwner and objectQualifier tokens
			sql = sql.Replace("{databaseOwner}", Owner);
			sql = sql.Replace("{objectQualifier}", Qualifier);

			// Get the tokens for Core {{{tablename}}} and Module {{tablename}}
			var tokens = GetTokens(sql,"{{{", "}}}");
			GetTokens(sql,tokens,"{{", "}}");
			
			// Replace the tokens
			if (tokens.Count > 0)
				foreach (var t in tokens.Keys)
				{
					sql = sql.Replace(t, 
									  t.StartsWith("{{{") ? GetName(tokens[t], false) 
														  : GetName(tokens[t], true)
									  );
				}


			// Return the translated sql
			return sql;
		}
		
		private string GetName(string name, bool addModuleQualifier)
		{
			var value = Owner + Qualifier;
			if (!string.IsNullOrEmpty(ModuleQualifier) &&  addModuleQualifier)
				value += ModuleQualifier;
			value += name;
			return value;
		}


		#endregion
		

		#region "Static Methods"
		/// <summary>
		/// Converts a collection of int into a value list to be used in and IN statement
		/// </summary>
		/// <param name="ps">List of int values</param>
		/// <returns>Properly formatted list for IN statement</returns>
		public static string ArrayToINint(params int[] ps)
		{
			var result = "";
			foreach (var p in ps)
			{
				if (result.Length > 0) result += ", ";
				result += p.ToString();
			}
			return result;
		}

		/// <summary>
		/// Converts a collection of string into a value list to be used in and IN statement
		/// </summary>
		/// <param name="ps">List of int values</param>
		/// <returns>Properly formatted list for IN statement</returns>
		public static string ArrayToINint(params string[] ps)
		{
			var result = "";
			foreach (var p in ps)
			{
				if (result.Length > 0) result += ", ";
				result += int.Parse(p).ToString();
			}
			return result;
		}

		/// <summary>
		/// Converts a collection of int into a value list to be used in and IN statement
		/// </summary>
		/// <param name="ps">List of int values</param>
		/// <returns>Properly formatted list for IN statement</returns>
		public static string ArrayToINstring(params string[] ps)
		{
			var result = "";
			foreach (var p in ps)
			{
				if (result.Length > 0) result += ", ";
				result += "'" + EscapeApostrophe(p) + "'";
			}
			return result;
		}

		/// <summary>
		/// Escapes the apostrophe on strings
		/// </summary>
		/// <param name="sql">SQL statement fragment</param>
		/// <returns>The clean SQL fragment</returns>
		public static string EscapeApostrophe(string sql)
		{
			sql = sql.Replace("'", "''");
			return sql;
		}

		/// <summary>
		/// Properly excapes a string to be included in a LIKE statement
		/// </summary>
		/// <param name="value">Value to search for</param>
		/// <param name="escapeApostrophe">Weather to escape the apostrophe. Prevents double escaping of apostrophes</param>
		/// <returns>The transalted value ready to be used in a LIKE statement</returns>
		public static string EscapeForLike(string value, bool escapeApostrophe)
		{
			// Do not process a string that is null or empty
			if (string.IsNullOrEmpty(value)) return value;

			string[] specialChars = {"%", "_", "-", "^"};
			string newChars;

			// Escape the [ braket
			newChars = value.Replace("[", "[[]");

			// Replace the special chars
			foreach (string t in specialChars){
				newChars = newChars.Replace(t, "[" + t + "]");
			}

			// Escape the apostrophe if requested
			if (escapeApostrophe)
				newChars = EscapeApostrophe(newChars);

			return newChars;
		}

		/// <summary>
		/// Retreives a list of tokens included in a string
		/// </summary>
		/// <param name="s">The string object being exteded</param>
		/// <param name="startDelimiter">The start delimiter for the token</param>
		/// <param name="endDelimiter">The end delimiter for the token</param>
		/// <returns>A dictionary of tokens present in the string</returns>
		public static Dictionary<string, string> GetTokens(string s, string startDelimiter, string endDelimiter)
		{
			return GetTokens(s, new Dictionary<string, string>(), startDelimiter, endDelimiter);
		}

		/// <summary>
		/// Retreives a list of tokens included in a string
		/// </summary>
		/// <param name="s">The string object being exteded</param>
		/// <param name="startDelimiter">The start delimiter for the token</param>
		/// <param name="endDelimiter">The end delimiter for the token</param>
		/// <param name="tokens">The dictionary of tokens to add the new finds to</param>
		/// <returns>A dictionary of tokens present in the string</returns>
		public static Dictionary<string, string> GetTokens(string s, Dictionary<string, string> tokens, string startDelimiter, string endDelimiter)
		{
			var marker = 0;
			do
			{
				// Get the start and end of the tokens
				var startIndex = s.IndexOf(startDelimiter, marker);
				int endIndex;

				if (startIndex > -1)
					endIndex = s.IndexOf(endDelimiter, startIndex);
				else
					break;

				// Add the token and increase the marker
				if (endIndex > -1)
				{
					var token = s.Substring(startIndex, (endIndex + endDelimiter.Length) - startIndex);
					var tokenValue = token.Substring(startDelimiter.Length, token.Length - (startDelimiter.Length + endDelimiter.Length));
					tokens.Add(token, tokenValue);
					marker = endIndex + endDelimiter.Length;
				}

			} while (marker < s.Length);

			return tokens;
		}
		#endregion

		#region Attributes

		[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
		private sealed class IgnoreAttribute : Attribute
		{
			public IgnoreAttribute(){}
		}

		[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
		private sealed class DbNameAttribute : Attribute
		{
			public DbNameAttribute(string dbName)
			{
				this.DBName = dbName;
			}

			// This is a named argument
			public string DBName { get; set; }
		}

		#endregion


		#region ExecuteObject

		public SqlHelper INSERTOBJECT(string tableName, object data)
		{
			return INSERTOBJECT(tableName, data, null, null);
		}

		public SqlHelper INSERTOBJECT(string tableName, object data, string includeOnlyTheseColumns, string excludeTheseColumns)
		{
			// Get the properties to use in the statement and their values
			var objectData = GetObjectProperties(data, includeOnlyTheseColumns, excludeTheseColumns);

			// Determine whether there are any properties that passed
			if (objectData.Count == 0) throw new ApplicationException("There are no properties to insert!");

			// Call the insert method
			return INSERTOBJECT(tableName, objectData);
		}

		/// <summary>
		/// Creates an insert statement from a dictionary of column names and corresponding property
		/// </summary>
		/// <param name="tableName">Table to insert into</param>
		/// <param name="insertData">Dictionary of columns and values</param>
		public SqlHelper INSERTDICTIONARY(string tableName, Dictionary<string, object> insertData)
		{
			var insertColumns = "";
			var values = new List<object>();
			foreach (var column in insertData.Keys)
			{
				if (!String.IsNullOrEmpty(insertColumns)) insertColumns += ", ";
				insertColumns += column;
				values.Add(insertData[column]);
			}
			return INSERTINTO(tableName, insertColumns).VALUES(values.ToArray());
		}

		public SqlHelper UPDATEOBJECT(string tableName, object data)
		{
			return UPDATEOBJECT(tableName, data, null, null);
		}

		public SqlHelper UPDATEOBJECT(string tableName, object data, string includeOnlyTheseColumns, string excludeTheseColumns)
		{
			// Get the properties to use in the statement and their values
			var objectData = GetObjectProperties(data, includeOnlyTheseColumns, excludeTheseColumns);

			// Determine whether there are any properties that passed
			if (objectData.Count == 0) throw new ApplicationException("There are no properties to update!");

			// Call the update method
			return UPDATEOBJECT(tableName, objectData);
		}

		/// <summary>
		/// Creates an update statement from a dictionary of column names and corresponding property
		/// </summary>
		/// <param name="tableName">Table to update into</param>
		/// <param name="updateData">Dictionary of columns and values</param>
		public SqlHelper UPDATEDICTIONARY(string tableName, Dictionary<string, object> updateData)
		{
			UPDATE(tableName);
			foreach (var column in updateData.Keys)
			{
				SET(column, updateData[column]);
			}
			return this;
		}

		


		/// <summary>
		/// Gets a list of properties and their values from a particular object using reflection
		/// </summary>
		/// <param name="data">The object to inspect</param>
		/// <param name="includeOnlyTheseProperties">Only columns in this list will be included (comma separated list)</param>
		/// <param name="excludeTheseProperties">Columns in this list will be excluded (comma separated list)</param>
		/// <returns>A dictionary with the property names and their values</returns>
		public Dictionary<string, object> GetObjectProperties(object data, string includeOnlyTheseProperties, string excludeTheseProperties)
		{
			var cacheKey = "getObjectProperties:" + data.GetType().FullName;
			if (Cache.Contains(cacheKey))
			{
				return (Dictionary<string, object>) Cache.GetValue(cacheKey);
			}

			// Stores the propertiers and their values that pass all the exclude / include rules
			var objectData = new Dictionary<string, object>();

			// Get a searchable list of the include only list and the exclude list
			var onlyThese = GetStringArray(includeOnlyTheseProperties);
			var exclude = GetStringArray(excludeTheseProperties);

			// Determine which properties to include
			foreach (var prop in data.GetType().GetProperties())
			{
				// Do not include private properties
				if (!prop.CanRead) continue;

				// Match the property to the include only list (if list is empty then ignore)
				if (onlyThese.Count > 0 && !onlyThese.Contains(prop.Name)) continue;

				// Match the property to the exclude list (if list is empty then ignore)
				if (exclude.Count > 0 && exclude.Contains(prop.Name)) continue;

				// Ingore the property if the ignore attribute is assigned
				var ignoreAttr = prop.GetCustomAttributes(typeof(IgnoreAttribute), false) as IgnoreAttribute[];
				if (ignoreAttr != null && ignoreAttr.Length > 0) continue;

				// Use a different name other than the property name for the sql
				var name = prop.Name;
				var nameAttr = prop.GetCustomAttributes(typeof(DbNameAttribute), false) as DbNameAttribute[];
				if (nameAttr != null && nameAttr.Length > 0) name = nameAttr[0].DBName;

				// If it passes all the previous checks then add the property to the insert
				objectData.Add(name, prop.GetValue(data, null));
			}

			// Add it to cache
			Cache.Add(cacheKey, objectData);

			// Call the insert method
			return objectData;
		}

		/// <summary>
		/// Gets an array of strings from a comma delimeted string (trims white space)
		/// </summary>
		/// <param name="data">String to split</param>
		/// <returns>Array of strings</returns>
		private List<string> GetStringArray(string data)
		{
			var returnValue = new List<string>();
			if (string.IsNullOrEmpty(data)) return returnValue;

			foreach (var s in data.Split(','))
			{
				returnValue.Add(s.Trim());
			}
			return returnValue;
		}

		#endregion


		#region "Execute SQL"

		/// <summary>
		/// Executes the query and returns a Scalar value
		/// </summary>
		/// <returns>Object (null when dbnull value is returned)</returns>
		public object ExecuteScalar()
		{
			var rvalue = DbExecuteScalar(CommandType.Text, ToString(), _params.ToArray());
			
			if (rvalue == DBNull.Value) rvalue = null;

			return rvalue;
		}

		/// <summary>
		/// Executes the query and returns a Scalar value
		/// </summary>
		/// <returns>Object (null when dbnull value is returned)</returns>
		public async Task<object> ExecuteScalarAsync()
		{
			var rvalue = await DbExecuteScalarAsync(CommandType.Text, ToString(), _params.ToArray());

			if (rvalue == DBNull.Value) rvalue = null;

			return rvalue;
		}

		

		/// <summary>
		/// Executes the query and returns a Scalar value for the specific generic value
		/// </summary>
		/// <returns>A typed object of T</returns>
		public T ExecuteScalar<T>()
		{
			var rvalue = ExecuteScalar();
			if (rvalue != null)
			{
				var tc = TypeDescriptor.GetConverter(typeof(T));
				return (T)tc.ConvertFromInvariantString(rvalue.ToString());
			}
			
			return default(T);
		}

		/// <summary>
		/// Executes the query and returns a Scalar value for the specific generic value
		/// </summary>
		/// <returns>A typed object of T</returns>
		public async Task<T> ExecuteScalarAsync<T>()
		{
			var rvalue = await ExecuteScalarAsync();
			if (rvalue != null)
			{
				var tc = TypeDescriptor.GetConverter(typeof(T));
				return (T)tc.ConvertFromInvariantString(rvalue.ToString());
			}

			return default;
		}

		/// <summary>
		/// Appends a SELECT @@IDENTITY statement to the query and then executes
		/// </summary>
		/// <returns>The identity of the just inserted record</returns>
		public int ExecuteScalarIdentity()
		{
			SELECT_IDENTITY();
			return ExecuteScalarInt();
		}

		/// <summary>
		/// Executes the query and returns a sclar value of type int
		/// </summary>
		public int ExecuteScalarInt()
		{
			return ExecuteScalar<int>();
		}

		/// <summary>
		/// Executes the query and returns a sclar value of type int
		/// </summary>
		public async Task<int> ExecuteScalarIntAsync()
		{
			return await ExecuteScalarAsync<int>();
		}

		/// <summary>
		/// Appends a SELECT @@IDENTITY statement to the query and then executes
		/// </summary>
		/// <returns>The identity of the just inserted record</returns>
		public async Task<int> ExecuteScalarIdentityAsync()
		{
			SELECT_IDENTITY();
			return await ExecuteScalarIntAsync();
		}

		/// <summary>
		/// Executes the query and returns a sclar value of type int
		/// </summary>
		public decimal ExecuteScalarDecimal()
		{
			return ExecuteScalar<decimal>();
		}

		/// <summary>
		/// Executes the query and returns a sclar value of type int
		/// </summary>
		public async Task<decimal> ExecuteScalarDecimalAsync()
		{
			return await ExecuteScalarAsync<decimal>();
		}

		/// <summary>
		/// Executes the query and returns a scalar value of type bool
		/// </summary>
		public bool ExecuteScalarBool()
		{
			return ExecuteScalar<bool>();
		}

		/// <summary>
		/// Executes the query and returns a scalar value of type bool
		/// </summary>
		public async Task<bool> ExecuteScalarBoolAsync()
		{
			return await ExecuteScalarAsync<bool>();
		}

		/// <summary>
		/// Executes the query and returns a sclar value of type string
		/// </summary>
		public string ExecuteScalarString()
		{
			return ExecuteScalar<string>();
		}

		/// <summary>
		/// Executes the query and returns a sclar value of type string
		/// </summary>
		public async Task<string> ExecuteScalarStringAsync()
		{
			return await ExecuteScalarAsync<string>();
		}

		/// <summary>
		/// Returns a SqlRecord. Works like Scalar except it can 
		/// return multiple fields values for the first record of the resultset
		/// </summary>
		/// <returns>Dictionary with FieldName, Value</returns>
		public SqlRecord ExecuteSqlRecord()
		{
			return new SqlRecord(ExecuteValues());
		}

		/// <summary>
		/// Returns a SqlRecord. Works like Scalar except it can 
		/// return multiple fields values for the first record of the resultset
		/// </summary>
		/// <returns>Dictionary with FieldName, Value</returns>
		public async Task<SqlRecord> ExecuteSqlRecordAsync()
		{
			return new SqlRecord(await ExecuteValuesAsync());
		}

		/// <summary>
		/// Returns a List of SqlRecord objects. 
		/// </summary>
		/// <returns>SqlRecord with a Dictionary data property with FieldName, Value</returns>
		public List<SqlRecord> ExecuteSqlRecordCollection()
		{
			using (var reader = ExecuteReader())
			{
				var list = new List<SqlRecord>();
				
				while (reader.Read())
				{
					var values = new Dictionary<string, object>();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						try
						{
							var key = reader.GetName(i);
							object value = null;
							if (reader.IsDBNull(i) == false)
							{
								value = reader[i];
							}
							values.Add(key, value);
						}
						catch (IndexOutOfRangeException)
						{
							continue;
						}
					}
					list.Add(new SqlRecord(values));
				}

				return list;               
			}
		}

		/// <summary>
		/// Returns a List of SqlRecord objects. 
		/// </summary>
		/// <returns>SqlRecord with a Dictionary data property with FieldName, Value</returns>
		public async Task<List<SqlRecord>> ExecuteSqlRecordCollectionAsync()
		{
			using (var reader = await ExecuteReaderAsync())
			{
				var list = new List<SqlRecord>();

				while (reader.Read())
				{
					var values = new Dictionary<string, object>();
					for (int i = 0; i < reader.FieldCount; i++)
					{
						try
						{
							var key = reader.GetName(i);
							object value = null;
							if (reader.IsDBNull(i) == false)
							{
								value = reader[i];
							}
							values.Add(key, value);
						}
						catch (IndexOutOfRangeException)
						{
							continue;
						}
					}
					list.Add(new SqlRecord(values));
				}

				return list;
			}
		}

		/// <summary>
		/// Executes a query that does not return a value
		/// </summary>
		public int ExecuteNonquery()
		{
			return DbExecuteNonQuery(CommandType.Text, ToString(), _params.ToArray());
		}

		/// <summary>
		/// Executes a query that does not return a value
		/// </summary>
		public async Task<int> ExecuteNonqueryAsync()
		{
			return await DbExecuteNonQueryAsync(CommandType.Text, ToString(), _params.ToArray());
		}

		/// <summary>
		/// Executes a query and hydrates an object with the result
		/// </summary>
		/// <typeparam name="T">The type of the object to hydrate and return</typeparam>
		/// <returns>I hydrated object of the type specified</returns>
		public T ExecuteObject<T>()
		{
			using (var reader = ExecuteReader())
			{
				return FillObject<T>(reader);    
			}            
		}

		/// <summary>
		/// Executes a query and hydrates an object with the result
		/// </summary>
		/// <typeparam name="T">The type of the object to hydrate and return</typeparam>
		/// <returns>I hydrated object of the type specified</returns>
		public async Task<T> ExecuteObjectAsync<T>()
		{
			using (var reader = await ExecuteReaderAsync())
			{
				return FillObject<T>(reader);
			}
		}

		/// <summary>
		/// Returns a dictionary of FieldName. Works like Scalar except it can 
		/// return multiple fields values for the first record of the resultset
		/// </summary>
		/// <returns>Dictionary with FieldName, Value</returns>
		public Dictionary<string, object> ExecuteValues()
		{
			using (var reader = ExecuteReader())
			{  
				Dictionary<string, object> values = null;
				if (reader.Read() == false) { return values; }
				values = new Dictionary<string, object>();
				for (int i = 0; i < 100; i++)
				{
					try
					{
						var key = reader.GetName(i);
						object value = null;
						if (reader.IsDBNull(i) == false)
						{
							value = reader[i];
						}
						values.Add(key, value);
					}
					catch (IndexOutOfRangeException)
					{
						return values;
					}
				}
				return values;               
			}
		}

		/// <summary>
		/// Executes the query and maps the results to a collection of Dictionary<string, object> 		
		/// </summary>		
		/// <returns>A collection of Dictionary<string, object></returns>
		public async Task<List<Dictionary<string, object>>> ExecuteValuesCollectionAsync()
		{
			var objCollection = new List<Dictionary<string, object>>();
			using (var reader = await ExecuteReaderAsync())
			{				
				try
				{
					while (reader.Read())
					{
						Dictionary<string, object> values = new Dictionary<string, object>();
						for (int i = 0; i < 100; i++)
						{
							try
							{
								var key = reader.GetName(i);
								object value = null;
								if (reader.IsDBNull(i) == false)
								{
									value = reader[i];
								}
								values.Add(key, value);
							}
							catch (IndexOutOfRangeException)
							{
								// ignore
							}
						}
						objCollection.Add(values);						
					}

					return objCollection;
				}
				finally
				{
					DisposeReader(reader);					
				}
			}
		}

		/// <summary>
		/// Returns a dictionary of FieldName. Works like Scalar except it can 
		/// return multiple fields values for the first record of the resultset
		/// </summary>
		/// <returns>Dictionary with FieldName, Value</returns>
		public async Task<Dictionary<string, object>> ExecuteValuesAsync()
		{
			using (var reader = await ExecuteReaderAsync())
			{
				Dictionary<string, object> values = null;
				if (reader.Read() == false) { return values; }
				values = new Dictionary<string, object>();
				for (int i = 0; i < 100; i++)
				{
					try
					{
						var key = reader.GetName(i);
						object value = null;
						if (reader.IsDBNull(i) == false)
						{
							value = reader[i];
						}
						values.Add(key, value);
					}
					catch (IndexOutOfRangeException)
					{
						return values;
					}
				}
				return values;
			}
		}

		/// <summary>
		/// Executes the query and returnes a collection of strings. 
		/// Useful when needing a quick lookup set of values
		/// </summary>
		/// <returns>A string collection</returns>
		public List<string> ExecuteStringCollection()
		{
			using (var reader = ExecuteReader())
			{
				List<string> values = null;
				while (reader.Read())
				{
					if (reader.IsDBNull(0) == false)
					{
						if (values == null) values = new List<string>();
						values.Add(reader[0].ToString());
					}
				}
				return values;
			}
		}

		/// <summary>
		/// Executes the query and returns a collection of strings. 
		/// Useful when needing a quick lookup set of values
		/// </summary>
		/// <returns>A string collection</returns>
		public async Task<List<string>> ExecuteStringCollectionAsync()
		{
			using (var reader = await ExecuteReaderAsync())
			{
				List<string> values = null;
				while (reader.Read())
				{
					if (reader.IsDBNull(0) == false)
					{
						if (values == null) values = new List<string>();
						values.Add(reader[0].ToString());
					}
				}
				return values;
			}
		}

		/// <summary>
		/// Executes the query and maps the results to a collection of objects 
		/// of the type specified through the generic argument
		/// </summary>
		/// <typeparam name="T">The of object for the collection</typeparam>
		/// <returns>A collection of T</returns>
		public List<T> ExecuteCollection<T>()
		{
			using (var reader = ExecuteReader())
			{
				return FillCollection<T>(reader);
			}
		}

		/// <summary>
		/// Executes the query and maps the results to a collection of objects 
		/// of the type specified through the generic argument
		/// </summary>
		/// <typeparam name="T">The of object for the collection</typeparam>
		/// <returns>A collection of T</returns>
		public async Task<List<T>> ExecuteCollectionAsync<T>()
		{
			using (var reader = await ExecuteReaderAsync())
			{
				return FillCollection<T>(reader);
			}
		}

		/// <summary>
		/// Executes the query and returns a DataReader
		/// </summary>
		public DbDataReader ExecuteReader()
		{
			return DbExecuteReader(CommandType.Text, ToString(), _params.ToArray());
		}

		/// <summary>
		/// Executes the query and returns a DataReader
		/// </summary>
		public async Task<DbDataReader> ExecuteReaderAsync()
		{
			return await DbExecuteReaderAsync(CommandType.Text, ToString(), _params.ToArray());
		}

		#endregion


		#region "Database Execution"

		/// <summary>
		/// Returns a DbProviderFactory for the specified ProviderName
		/// </summary>
		public DbProviderFactory DbProviderFactory()
		{
			//return DbProviderFactories.GetFactory(ProviderName);
			return SqlClientFactory.Instance;
		}

		/// <summary>
		/// Creates a db connection using the ProviderFactory and returns it. It can optionally automatically open the connection
		/// </summary>
		public DbConnection CreateDbConnection(bool openConnection)
		{
			if (_useAzureServiceTokenProvider)
			{
				var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("development", StringComparison.InvariantCultureIgnoreCase);
				var conn = DbProviderFactory().CreateConnection() as SqlConnection;
				conn.ConnectionString = ConnectionString;
				var providerConnString = $"{ConnectionString.Trim().TrimEnd(';')}{(isDev ? ";RunAs=Developer;DeveloperTool=VisualStudio" : ";RunAs=App;")}";
				var provider = new AzureServiceTokenProvider(providerConnString);
				var token = provider.GetAccessTokenAsync("https://database.windows.net/").Result;
				conn.AccessToken = token;
				if (openConnection) conn.Open();
				return conn;
			}
			else
			{
				var conn = DbProviderFactory().CreateConnection();
				conn.ConnectionString = ConnectionString;
				if (openConnection) conn.Open();
				return conn;
			}
			
		}
		
		/// <summary>
		/// This method creates a command and assigns parameters to it provided
		/// </summary>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of DbParameters to be associated with the command or 'null' if no parameters are required</param>
		private DbCommand CreateDbCommand(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			var command = DbProviderFactory().CreateCommand();
			
			// Set the command text (stored procedure name or SQL statement)
			command.CommandText = commandText;

			// Set the command type
			command.CommandType = commandType;

			// Attach the command parameters if they are provided
			if (commandParameters != null)
			{
				foreach (var p in commandParameters)
				{
					if (p == null) continue;
					// Check for derived output value with no value assigned
					if ((p.Direction == ParameterDirection.InputOutput ||
							p.Direction == ParameterDirection.Input) &&
						(p.Value == null))
					{
						p.Value = DBNull.Value;
					}
					command.Parameters.Add(p);
				}
			   
			}

			// Associate the connection with the command
			command.Connection = CreateDbConnection(true);

			// Return the new created command
			return command;
		}

		/// <summary>
		/// Properly disposes a command by also disposing the underlying connection
		/// </summary>
		/// <param name="cmd">The command to dispose</param>
		private void DisposeDbCommand(DbCommand cmd)
		{
			if (cmd == null) return;
			if (cmd.Connection != null) cmd.Connection.Dispose();
			cmd.Dispose();
		}
		

		/// <summary>
		/// Execute a DbCommand (that returns no resultset) using the provided parameters.
		/// </summary>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public int DbExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters)
		{
			// Create a command and prepare it for execution
			var cmd = CreateDbCommand(commandType, commandText, commandParameters);

			try
			{
				// Finally, execute the command
				var retval = cmd.ExecuteNonQuery();

				// Detach the DbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();
				
				// Return the value
				return retval;
			}
			finally
			{
				DisposeDbCommand(cmd);
			}
		}

		/// <summary>
		/// Execute a DbCommand (that returns no resultset) using the provided parameters.
		/// </summary>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public async Task<int> DbExecuteNonQueryAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
		{
			// Create a command and prepare it for execution
			var cmd = CreateDbCommand(commandType, commandText, commandParameters);

			try
			{
				// Finally, execute the command
				var retval = await cmd.ExecuteNonQueryAsync();

				// Detach the DbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				// Return the value
				return retval;
			}
			finally
			{
				DisposeDbCommand(cmd);
			}
		}


		/// <summary>
		/// Execute a DbCommand using the provided parameters.
		/// </summary>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public object DbExecuteScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters)
		{
			// Create a command and prepare it for execution
			var cmd = CreateDbCommand(commandType, commandText, commandParameters);
			
			try {

				// Execute the command & return the results
				var retval = cmd.ExecuteScalar();

				// Detach the DbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();
			
				return retval;

			}
			finally
			{
				DisposeDbCommand(cmd);
			}
		}

		/// <summary>
		/// Execute a DbCommand using the provided parameters.
		/// </summary>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public async Task<object> DbExecuteScalarAsync(CommandType commandType, string commandText, params DbParameter[] commandParameters)
		{
			// Create a command and prepare it for execution
			var cmd = CreateDbCommand(commandType, commandText, commandParameters);

			try
			{
				// Execute the command & return the results
				var retval = await cmd.ExecuteScalarAsync();

				// Detach the DbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				return retval;

			}
			finally
			{
				DisposeDbCommand(cmd);
			}
		}

		/// <summary>
		/// Create and prepare a DbCommand, and call ExecuteReader with the appropriate CommandBehavior.
		/// </summary>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of DbParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <returns>SqlDataReader containing the results of the command</returns>
		public DbDataReader DbExecuteReader(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			// Create a command and prepare it for execution
			var cmd = CreateDbCommand(commandType, commandText, commandParameters);

			try
			{
				var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

				// Detach the DbParameters from the command object, so they can be used again.
				// HACK: There is a problem here, the output parameter values are fletched 
				// when the reader is closed, so if the parameters are detached from the command
				// then the SqlReader can't set its values. 
				// When this happen, the parameters can�t be used again in other command.
				var canClear = true;
				foreach (DbParameter commandParameter in cmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input)
						canClear = false;
				}
				if (canClear) cmd.Parameters.Clear();

				return reader;
			}
			catch
			{				
				throw;
			}
			finally
			{
				DisposeDbCommand(cmd);
			}
		}

		/// <summary>
		/// Create and prepare a DbCommand, and call ExecuteReader with the appropriate CommandBehavior.
		/// </summary>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of DbParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <returns>SqlDataReader containing the results of the command</returns>
		public async Task<DbDataReader> DbExecuteReaderAsync(CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			// Create a command and prepare it for execution
			var cmd = CreateDbCommand(commandType, commandText, commandParameters);
			try
			{
				var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

				// Detach the DbParameters from the command object, so they can be used again.
				// HACK: There is a problem here, the output parameter values are fletched 
				// when the reader is closed, so if the parameters are detached from the command
				// then the SqlReader can't set its values. 
				// When this happen, the parameters can´t be used again in other command.
				var canClear = true;
				foreach (DbParameter commandParameter in cmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input)
						canClear = false;
				}
				if (canClear) cmd.Parameters.Clear();

				return reader;
			}
			catch
			{	
				throw;
			}
			finally
			{
				DisposeDbCommand(cmd);				
			}
		}

		#endregion


		#region "Object Hydration"

		#region "Cache"

		/// <summary>
		/// Generic cache class to cache object graphs and type converters
		/// </summary>
		private static class Cache
		{
			static Dictionary<string, object> _cache = new Dictionary<string, object>();


			public static bool Contains(String key) { return _cache.ContainsKey(key); }
			public static object GetValue(String key)
			{
				return (_cache.ContainsKey(key)) ? _cache[key] : null;
			}
			public static void Add(String key, object data)
			{
				if (_cache.ContainsKey(key))
					_cache[key] = data;
				else
					_cache.Add(key, data);
			}
			public static void Remove(string key)
			{
				if (_cache.ContainsKey(key)) _cache.Remove(key);
			}
		}

		#endregion


		#region "GetValue / TypeConverters"

		public interface ITypeConverter { object GetValue(Type fieldType, object value); }

		/// <summary>
		/// Adds a custom type converter. When hydrating an object, certain complex types might have special needs 
		/// when it comes to deserializing from database. You can add a customer converter for the type, in order to 
		/// have the auto hydration use the this type converter instead of the Default type converter
		/// </summary>
		/// <param name="typeName">Full name of the type to use the converter for</param>
		/// <param name="converter">The converter oject to use. It must implement the ITypeConverter interface</param>
		public static void AddTypeConverter(string typeName, ITypeConverter converter)
		{
			typeName = typeName.ToLower();
			var list = GetAllConverters();
			if (list.ContainsKey(typeName))
				list[typeName] = converter;
			else
				list.Add(typeName, converter);
		}

		/// <summary>
		/// Retreives a list of ITypeConverters in the system
		/// </summary>
		private static Dictionary<String, ITypeConverter> GetAllConverters()
		{
			const string key = "TypeConverters";
			if (!Cache.Contains(key))
			{
				var types = new Dictionary<String, ITypeConverter>();
				Cache.Add(key, types);
			}
			return (Dictionary<String, ITypeConverter>)Cache.GetValue(key);
		}

		/// <summary>
		/// Gets the database value converter for a certain Type name
		/// </summary>
		/// <param name="typeName">The full name of the type e.g. System.String</param>
		/// <returns>A ITypeConverter object from the cache</returns>
		private static ITypeConverter GetTypeConverter(String typeName)
		{
			typeName = typeName.ToLower();
			var converters = GetAllConverters();
			return (converters.ContainsKey(typeName)) ? converters[typeName] : null;
		}

		/// <summary>
		/// Converts a database value to its object representation. Uses the TypeConverter cache to properly translate complex types
		/// </summary>
		/// <param name="fieldType">The Type of the field to convert to</param>
		/// <param name="value">The database value. e.g. the object from the data reader ordinal</param>
		/// <returns>A properly converted object</returns>
		public static object GetValue(Type fieldType, object value)
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
				// Try to get a specific type converter
				//      when no type converter is foudn then do a brute convert and ignore any errors that come up                
				var converter = GetTypeConverter(fieldType.Name);
				if (converter != null)
				{
					newValue = converter.GetValue(fieldType, value);
				}
				else
				{
					try
					{
						newValue = Convert.ChangeType(value, fieldType);
					}
					catch (Exception) { }
				}
			}


			return newValue;

		}

		#endregion


		#region "FillObject / FillCollection | DataReader"

		/// <summary>
		/// Fills an object using generics from a data reader object
		/// </summary>
		/// <typeparam name="T">The type of the object to fill</typeparam>
		/// <param name="dr">The data reader object used to hydrate the object</param>
		/// <returns>A hydrated object of type T</returns>
		public static T FillObject<T>(DbDataReader dr)
		{
			return FillObject<T>(dr, true);
		}

		/// <summary>
		/// Fills a collection of objects using generics from a data reader object
		/// </summary>
		/// <typeparam name="T">The Type of the collection item object to fill</typeparam>
		/// <param name="reader">The reader object used to hydrate the collection</param>
		/// <returns>Collection of type of type T</returns>
		public static List<T> FillCollection<T>(DbDataReader reader)
		{
			var objCollection = new List<T>();

			try
			{
				while (reader.Read())
				{
					objCollection.Add(CreateObject<T>(reader));
				}
			}
			finally
			{
				DisposeReader(reader);
			}

			return objCollection;
		}

		private static void DisposeReader(DbDataReader reader)
		{
			if (reader != null)
			{
				if (reader.IsClosed == false) 
					reader.Close();
				reader.Dispose();
			}
		}
		
		/// <summary>
		/// Fill a particular object
		/// </summary>
		/// <typeparam name="T">The type of object to fill</typeparam>
		/// <param name="dr">The reader object used to hydrate the object</param>
		/// <param name="manageDataReader">When set to true, closes the reader when finished</param>
		/// <returns>A hydrated object of type T</returns>
		private static T FillObject<T>(DbDataReader dr, bool manageDataReader)
		{
			try
			{
				var objFillObject = default(T);
				// Make sure the data reader has data
				if (manageDataReader && dr.Read() == false) return objFillObject;

				// Fill the object            
				objFillObject = CreateObject<T>(dr);

				// Return the filled object
				return objFillObject;
			}
			// Close the reader when in charge
			finally
			{
				// Make sure to dispose the data reader                
				if (manageDataReader) DisposeReader(dr);
			}            
		}

		private static T CreateObject<T>(DbDataReader dr)
		{
			// Create the object and determin if the object is a single value
			var singleValue = false; 

			T objObject;

			// String and value types are single objects
			if (typeof(T) == typeof(String))
			{
				objObject = (T)(object)String.Empty;
				singleValue = true;
			}
			else if (typeof(T).IsValueType)
			{
				objObject = default(T);
				singleValue = true;
			}
			else
			{
				objObject = Activator.CreateInstance<T>();
			}

			
			// Check weather the object is a value type
			if (singleValue)
			{
				var value = GetValue(objObject.GetType(), dr.GetValue(0));
				if (value != null) objObject = (T)value;
				return objObject;
			}

			// Hydrate a complex type it
			//  Get the fields for the type
			List<object> fields = GetFields(objObject);

			//  Get the ordinals in the data reader
			int[] arrOrdinals = GetOrdinals(fields, dr);

			for (int i = 0; i < fields.Count; i++)
			{
				var field = fields[i];

				//   Make sure the value is found
				if (arrOrdinals[i] != -1)
				{
					//  Get the value from the reader
					var value = dr.GetValue(arrOrdinals[i]);

					//  Make sure the value is not null before assigning
					if (value == DBNull.Value) continue;

					// Check weather the member is a property or a field and fill it accordingly
					if (field is FieldInfo)
						(field as FieldInfo).SetValue(objObject, GetValue(((FieldInfo)field).FieldType, value), BindingFlags.Default, null, null);
					else if (field is PropertyInfo)
						(field as PropertyInfo).SetValue(objObject, GetValue(((PropertyInfo)field).PropertyType, value), BindingFlags.Default, null, null, null);
				}
			}

			return objObject;
		}

		/// <summary>
		/// Match the object fields against the ordinal of the DataReader. The ordinal of the DataReader is used 
		/// Rather than the string indexer to maximize performance
		/// </summary>
		/// <param name="fields">The list of members to hydrate for the object</param>
		/// <param name="dr">The data reader to map the ordinals from</param>
		/// <returns></returns>
		private static int[] GetOrdinals(List<object> fields, DbDataReader dr)
		{
			var arrOrdinals = new int[fields.Count];
			var columns = new Hashtable();

			// Get the column names
			for (var ci = 0; ci < dr.FieldCount; ci++)
				columns[dr.GetName(ci).ToUpperInvariant()] = "";

			// Match the fields to the column name ordinal
			for (var fi = 0; fi < fields.Count; fi++)
			{
				string fieldName = (fields[fi] is FieldInfo)
											? ((FieldInfo)fields[fi]).Name.ToUpperInvariant()
											: ((PropertyInfo)fields[fi]).Name.ToUpperInvariant();

				if (columns.ContainsKey(fieldName))
					arrOrdinals[fi] = dr.GetOrdinal(fieldName);
				else
					arrOrdinals[fi] = -1;
			}

			return arrOrdinals;

		}

		

		#endregion

		/// <summary>
		/// Get the fields / properties to hydrate for an object. 
		/// Caches the object map in order to maximize performance. 
		/// So reflection is used only first time on n object type
		/// </summary>
		/// <param name="obj">Object to use to hydrate</param>
		/// <returns>A list of Fields and Properties</returns>
		private static List<object> GetFields(object obj)
		{
			var cacheKey = "reflectioncache_" + obj.GetType().FullName;
			List<object> fields;
			if (Cache.Contains(cacheKey))
				fields = Cache.GetValue(cacheKey) as List<object>;
			else
			{
				fields = new List<object>();
				foreach (PropertyInfo p in obj.GetType().GetProperties())
				{
					if (p.CanWrite) fields.Add(p);
				}
				foreach (FieldInfo f in obj.GetType().GetFields())
				{
					if (f.IsPublic && !f.IsStatic) fields.Add(f);
				}

				fields.AddRange(obj.GetType().GetFields());

				Cache.Add(cacheKey, fields);
			}
			return fields;
		}
		#endregion 


		#region ValuesCollection
		[Serializable]
		public class SqlRecord
		{
			private Dictionary<string, object> Data;
			public bool HasValue { get { return Data != null; } }

			public SqlRecord()
			{
				Data = new Dictionary<string, object>();                
			}

			public void AddFieldValue(string fieldname, object value)
			{
				Data.Add(fieldname, value);
			}

			public SqlRecord(Dictionary<string, object> data)
			{
				this.Data = data;
			}

			public string GetString(string key)
			{
				if (!HasValue) return "";
				var value = Data[key];
				if (value == null || value == DBNull.Value)
				{
					return "";
				}
				return value.ToString();
			}

			public int GetInt(string key)
			{
				int value = 0;
				int.TryParse(GetString(key), out value);
				return value;
			}

			public decimal GetDecimal(string key)
			{
				decimal value = 0;
				decimal.TryParse(GetString(key), out value);
				return value;
			}

			public DateTime GetDate(string key)
			{
				DateTime value = DateTime.MinValue;
				DateTime.TryParse(GetString(key), out value);
				return value;
			}

			public bool GetBool(string key)
			{
				bool value = false;
				var stringval = GetString(key).ToLowerInvariant();
				if (stringval == "1" || stringval == "on" || stringval == "true" || stringval == "yes")
				{
					value = true;
				}
				else
				{
					bool.TryParse(stringval, out value);
				}
				return value;
			}
			
		}

		#endregion

	}

	public static class SqlHelperExtensions
	{
		public static SqlHelper EXISTS_IN_TABLE(this SqlHelper sqlHelper, string tableName, Func<SqlHelper, SqlHelper> whereAction)
		{
			sqlHelper.Add("IF EXISTS (SELECT 1 FROM ").Add(tableName);
			whereAction(sqlHelper);
			sqlHelper.CloseParenthesis();
			sqlHelper.Add("SELECT 1 ELSE SELECT 0");
			return sqlHelper;
		}
	}
}