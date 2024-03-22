using Microsoft.EntityFrameworkCore;
using todoApp.Models;

public class TodoContext : DbContext
{
    // DbSet properties for your entity models
    public DbSet<CPR> Cprs { get; set; }
    public DbSet<TodoList> TodoLists { get; set; }

    // Constructor that takes the DbContextOptions and passes it to the base constructor
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    // Optionally, you can override OnModelCreating to further configure your models
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
