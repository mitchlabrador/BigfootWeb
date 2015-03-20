# Bigfoot

The Bigfoot project is made of of several individual efforts:

- **BigfootSQL**: A MicroORM

- **BigfootCSS**: BigfootCSS is a set of helper CSS classes

- **Bigfoot.Web.Client**: A collection of commonly used Javascript accross most web applications

- **Bigfoot.Web.Helpers**: A collection of helpers used for most web applications

- **Bigfoot.Utils**: A collection of common extensions and utilities that may be used accross all .net projects

Below is a detailed description of each. **They are all available as individual Nuget Packages using their corresponding names.**


BigfootSQL
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





BigfootCSS
-------------------

BigfootCSS is a set of helper CSS classes created to aid in the rapid creation of prototype user interfaces. It was built with simplicity and minimum footprint in mind.



**It does three things well:**

1. Everything is defined as classes. It will not interfere with other styles unless explicitly applied.
2. Classes are single purpose, for example "xl" defines the font size to be Extra Large and nothing else. There are some compound classes, but they are minimum.
3. Does its best to apply regardless of target. When a class is assigned it does its best to ensure it applies whether you assign it to an Input, an Anchor tag, paragraph, or a table cell.

**Here is a partial list of what's included:**

As I said before these classes try to fix the problem of being overridden by element direct styling typical in a, input, td, select, and textarea tags. When you assign one of these classes to any element, it insures that it takes precedence.

**Font Size:**

**s** = small (13px)

**xs** = extra small (10px)

**xxs** = extra extra small (9px)

**m** = medium (16px)

**l** = large (19px)

**xl** =extra large (24px)

**xxl** = extra extra large (32px)

**Floats:**

To float elements left or right simply apply these classes to the element. Remember to wrap them with a container that has the clearfix class applied as in the example above.

**fl** = float left

**fr** = float right

**Widths, Heights, Padding and Margins:**

I know that some of you will balk at the idea of classes with preset sizes, but before you decide to start hurling stones, remember what I said before, every class that is there, is there because I use it often and find it to simplify and speed up my development. Like I’ve said this is a good middle ground before custom css rules for everything, and inline styles (which are evil).

**w{number}** = Sets the width of the element to the {number} of pixels specified. for example w25, sets the width of the element to 25px.  Numbers from 1-10 are in 1 pixel increments, 10-200 are in 5 pixel increments and 200-600 are in 25 pixel increments and 600-980 are in 10 pixel increments. With the exception of the standard ad sizes as found here

**wp{number}** = Sets the width of the element in percentages of it’s parent’s size. 1-10 and 95-100 are in 1% increments, the reset are in 5% point increments.
h{number} = Sets the height of the element in pixels. Numbers from 1-31 are in 1 pixel increments, from 30-250 are in 5 pixel increments, 250-600 are in 10 pixel increments

**m{number}** = Sets the margin for an element to the specified number. Numbers from 1-10 are in 1 pixel increments, 10-50 are in 5 pixel increments.

**mt{number} , mr{number}, mb{number}, ml{number}** = Sets the margin-[top, right, bottom, left] correspondingly. Numbers from 1-10 are in 1 pixel increments, 10-50 are in 5 pixel increments.

**p{number}** = Sets the padding for an element to the specified number. Numbers from 1-10 are in 1 pixel increments, 10-50 are in 5 pixel increments.

**pt{number} , pr{number}, pb{number}, pl{number}** = Sets the padding-[top, right, bottom, left] correspondingly. Numbers from 1-10 are in 1 pixel increments, 10-50 are in 5 pixel increments.


**Colors and Backgrounds:**

These are particularly useful in application development, as you often want to change the color or background to a preset set of colors, but it is not imperative that you be extremely precise with the color as you would with the palette of a hand crafted site. In these cases having some preset colors greatly improves the consistency and speed with which you can achieve element relevance through design in your applications.

**bg{color}** = stands for background-color and then the color specified. colors included are: white, black, light, dark, hl (which means highlight, sets the background to a light yellow), the colors are as described by their name.

**black, white, red, xdard, dark, light, light[1-3], xlight** = Sets the color css attribute of elements (including overriding the anchor tag color if applied to an anchor) to the appropriate color.

**Display and Position:**
 
**hide** = sets the display property of the element to none

**block** = sets the display property of the element to block

**inline** = sets the display property of the element to inline

**inlineb** = sets the display property of the element to inline-block

**prel** = sets the position to relative

**pabs** = sets the position to absolute

**pfixed** = sets the position to fixed

**pstatic** = sets the position to static

**Text alignment:**
 
**tr** = text-align: right

**tl** = text-align: left

**tc** = text-align: center

**tj** = text-align: justify

**tvt** = vertical-align: top

**tvc** = vertical-align: middle

**tvb** = vertical-align: bottom

**Miscellaneous Classes:**
 
**pointer** = sets the cursor to be the pointer when the mouse is over this element

**cnomal** = sets the mouse cursor to the normal sate

**b** = set the font-weight to bold

**i** = sets the contained text to italic

**nowrap** =adds text-whitespace: nowrap

**strike** = adds text-decoration: line-through

**hlist** = makes an unordered list’s \<ul\> elements display inline and horizotally, useful when creating menu and other horizontal use of \<ul\>

**tablemin** = turns a properly formatted table (one that uses the \<thead\> \<th\> and \<tbody\> elements) into a nicely displayed one by adding formatting to it in a very simplistic style

**noscroll** = sets the overflow to hidden in order to hide the scroll bar.

**scroll** = sets the showing of scroll bar to auto, (overflow: auto)

**hscroll** = shows the horizontal scroll bar

**vscroll** = shows the vertical scroll bar
