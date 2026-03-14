using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

// Contexte étendu : inclut les utilisateurs pour l'authentification
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; } = null!;
}
