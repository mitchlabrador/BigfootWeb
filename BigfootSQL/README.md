Project Description
-------------------

BigfootSQL is a StringBuilder for SQL. It was built with simplicity in mind. It assumes that you are comfortable writing SQL but dislike the effort required to manually execute your SQL queries against the database and hydrating your objects with the data that comes back.

**It does three things very well:**

1. Does not translate your SQL, what you write is what's outputted
2. Handles all the parameterization for you automatically as you build your statements. (protects you against sql injection attacks etc.)
3. Hydrates your POCO objects with minimum effort.

Samples (A mixture of VB and C#, SQL is just a convenience property that automatically creates a new SQLHelper class for you to use): 

    public SqlHelper SQL 
    { 
        get 
        {  
            new SqlHelper(DatabaseOwner, ObjectQualifier, ModuleQualifier, ConnectionString); 
        } 
    }

**Sample Queries:**

    SQL.SELECT("ItemId, ItemDate, Description, Hours")
       .FROM("Timesheet")
       .WHERE("PortalId", App.PortalID)
       .AND("UserId", App.UserID)
       .AND("ItemDate", ">", Today.AddDays(-28))
       .ORDERBY("ItemDate DESC")
       .ExecuteCollection(Of TimesheetItem)()

    SQL.INSERTINTO("Timesheet", "PortalId, UserId, ItemDate, Description, Hours")
       .VALUES(App.PortalID, App.UserID, ItemDate, Desc, Hours)
       .ExecuteNonquery()

    SQL.DELETEFROM("Timesheet").WHERE("ItemId", ItemId).ExecuteNonquery()

    SQL.UPDATE("Timesheet")
            .SET("ItemDate", ItemDate)
            .SET("Description", Desc)
            .SET("Hours", Hours)
        .WHERE("ItemId", ItemId)
        .ExecuteNonquery()


    SQL.SELECT("M.MediaId, Title, Author, Year, Category, Description")
       .FROM("MediaList M")
       .INNERJOIN("Favorites F").ON("F.MediaId=M.MediaId")
       .WHERE("F.UserID",userId).ORDERBY("Title, Year DESC")
       .ExecuteCollection<FavoriteListItem>();


    SQL.SELECT("SettingValue")
       .FROM("Settings")
       .WHERE("SettingName", "MaxFileSize")
       .ExecuteScalarString();


    SQL.SELECT("C.CourseId, C.Author, C.Year, Number, Name, Description")
       .FROM("Courses C")
       .WHERE()
            .OpenParenthesis()
                .LIKE("Keywords",keyword)
                .OR()
                .LIKE("Name",keyword)
                .OR()
                .LIKE("ShortDescription", keyword)
                .OR()
                .LIKE("PrintDescription", keyword)
                .OR()
                .LIKE("Author", keyword)
                .OR("Number", keyword)
            .CloseParenthesis()
        .ExecuteCollection<CourseListItem>();


    SQL.SELECT("UserName, FullName")
       .FROM("Users")
       .ORDERBY("FullName")
       .ExecuteCollection<UserListItem>();


**Other Features:**

 1. INNER JOINS
 2. Paged results
 3. If you don't find a function to do your specialized sql statement... remember it is just a string builder for sql ... so just jump on the .ADD method and add your sql code as you wish. 
 4. Caches your object graph so it does not incur the expense of rediscovering your objects when hydrating them.
 5. Compact code. It is not an ORM but rather a very productive SQL Builder
 6. It does not make your coffee :)

**DotNetNuke Integration**

DotNetNuke has a peculiar way of naming their tables to include a prefix, this is done so that multiple installations of DotNetNuke can use the same database. This means that your queries must include the correct database owner and DNN installation database prefix in your queries when refering to any SQL object like a table, Sproc, View, etc. BigfootSQL makes this extremely simple by following these two steps:

Construct the SqlHelper object by specifying the database owner, the object qualifier (database objects prefix), and also your module prefix if you are using any, if not leave it empty, and of course the connection string you will be using to execute your queries. This example creates a Property whose job is to return a new SqlHelper object properly instantiated with the DNN and module prefixes.

    public SqlHelper SQL 
    { 
        get 
        {  
            new SqlHelper(DatabaseOwner, ObjectQualifier, ModuleQualifier, ConnectionString); 
        } 
    }

In your queries whenever you are referring to a database object like a table, sproc, etc. simply surround it with {{ }} (double brackets). This will append the correct database owner, database object prefix, and your module prefix if any.

Notice the Timesheet table is surrounded by {{ }}, at runtime this will translate the table name to DnnOwner.DnnPrefixModuleNameTimesheet 

    SQL.SELECT("DateEntered, Description, Hours")
       .FROM("{{Timesheet}}").WHERE("UserId", userId)
       .ExecuteCollection<TimesheetEntry>();

To refer to DotNetNuke system tables, Users for example, you can simply enclose the table name in {{{ }}} (tripple brackets), all this does is not append the module object qualifier

    SQL.SELECT("UserId, UserName")
       .FROM("{{{Users}}}")
       .ExecuteCollection<UserInfo>();

That's it! Pretty simple really, check it out, and if you find it doesn't do what you want... simply extend it to handle your scenario... it is supper easy, and since it uses a chainable style syntax, your method now becomes available to the fluid builder syntax.
