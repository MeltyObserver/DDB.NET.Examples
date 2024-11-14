
using Dapper;
using DuckDB.NET.Data;

var file = new MyFileClass()
{
    Title = "Sql Cookbook",
    Type = MyFileType.Digital,
    CreationDate = DateTime.Now,
    Extras = new Dictionary<string, string>
    {
        {"page count", "572"}
    }
};

var connectionString = "DataSource=:memory:";
using (var cn = new DuckDBConnection(connectionString))
{
    cn.Open();

    Console.WriteLine("DuckDB version: {0}\n", cn.ServerVersion);

    cn.Execute("CREATE TABLE files (fileType INTEGER, creationDate DATE, title VARCHAR, extras STRUCT(v VARCHAR, i VARCHAR));");

    Console.WriteLine("--- attempting to insert a date ---\n");

    cn.Execute("INSERT INTO files (creationDate, title) VALUES ($date, $title)",
        new { date = file.CreationDate, title = file.Title });

    var query = cn.Query<MyFileClass>("SELECT * FROM files");

    foreach (var q in query)
    {
        Console.WriteLine($"{q.CreationDate} ");
    }
}

public class MyFileClass
{
    public MyFileType Type { get; set; }
    public DateTime CreationDate { get; set; }
    public string? Title { get; set; }
    public Dictionary<string, string>? Extras { get; set; }
}
public enum MyFileType
{
    Physical,
    Digital
}
