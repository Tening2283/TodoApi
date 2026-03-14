using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

// Contexte de base de données exact du tutoriel
public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}
