using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Services;

public class AppDbContext : DbContext
{
    public string DbPath { get; }

    public AppDbContext()
    {
        DbPath = "lesson1.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={DbPath}");

    public List<List<string>> RunSqlCommand(string sqlCommand)
    {
        List<List<string>> resultArray = [];

        var command = Database.GetDbConnection().CreateCommand();
        command.CommandText = sqlCommand;
        command.CommandType = CommandType.Text;
        Database.OpenConnection();
        var result = command.ExecuteReader();
        while (result.Read())
        {
            List<string> rowResult = new List<string>();
            for (var i = 0; i < result.FieldCount; i++)
            {
                rowResult.Add(Convert.ToString(result[i]));
            }

            resultArray.Add(rowResult);
        }

        return resultArray;
    }

    public DbSet<User> User { get; set; }
    public DbSet<Session> Session { get; set; }
    public DbSet<Gender> Gender { get; set; }
    public DbSet<Todo> Todo { get; set; }
}

public class User
{
    public String userName { get; set; }
    public int id { get; set; }
    public String password { get; set; }

    [Column("gender_id")] public int GenderId { get; set; }

    public ICollection<Session> Sessions { get; set; }
    public Gender Gender { get; set; }
    public ICollection<Todo> Todos { get; set; }
}

public class Gender
{
    public int id { get; set; }
    public String name { get; set; }
    public ICollection<User> Users { get; set; }
}

public class Session
{
    public int id { get; set; }
    public int userId { get; set; }
    public String token { get; set; }
    public User User { get; set; }
}

public class Todo
{
    public int id { get; set; }
    public int userId { get; set; }
    public string title { get; set; }

    public int? status { get; set; }
    public User User { get; set; }
}