
using System.Data.Common;
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

    cn.Execute("CREATE TABLE files (title VARCHAR, fileType INTEGER, creationDate DATE, extras STRUCT(v VARCHAR, i VARCHAR));");

    cn.Execute("INSERT INTO files (title, creationDate) VALUES ($title, $date)",
        new { title = file.Title, date = file.CreationDate });

    try
    {
        Console.WriteLine("--- attempting to fetch with dapper ---\n");
        var query = cn.Query<MyFileClass>("SELECT * FROM files");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine("--------------");
        Console.WriteLine(e.InnerException);
        Console.WriteLine("--------------");
        Console.WriteLine(e.StackTrace);
    }

    try
    {
        Console.WriteLine("\n--- attempting to fetch with ADO ---\n");

        using var command = cn.CreateCommand();
        command.CommandText = "SELECT * FROM files;";
        var reader = command.ExecuteReader();
        PrintQueryResults(reader);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine("--------------");
        Console.WriteLine(e.InnerException);
        Console.WriteLine("--------------");
        Console.WriteLine(e.StackTrace);
    }
}

static void PrintQueryResults(DbDataReader queryResult)
{
    for (var index = 0; index < queryResult.FieldCount; index++)
    {
        var column = queryResult.GetName(index);
        Console.Write($"{column} ");
    }

    Console.WriteLine();

    while (queryResult.Read())
    {
        for (int ordinal = 0; ordinal < queryResult.FieldCount; ordinal++)
        {
            if (queryResult.IsDBNull(ordinal))
            {
                Console.WriteLine("NULL");
                continue;
            }
            var val = queryResult.GetValue(ordinal);
            Console.Write(val);
            Console.Write(" ");
        }

        Console.WriteLine();
    }
}

public class MyFileClass
{
    public string? Title { get; set; }
    public MyFileType Type { get; set; }
    public DateTime CreationDate { get; set; }
    public Dictionary<string, string>? Extras { get; set; }
}
public enum MyFileType
{
    Physical,
    Digital
}
