using Connectify.Db.Model;
using Microsoft.EntityFrameworkCore;

namespace Connectify.Db;

public class DbConnectifyContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<User> Users { get; set; }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<Message> Messages { get; set; }
    
    public DbConnectifyContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        ArgumentNullException.ThrowIfNull(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Login = "admin",
            Password = "admin",
            CreatedAt = new DateTime(2020, 1, 1),
        });

        modelBuilder.Entity<User>()
            .HasOne<Room>(r => r.Room)
            .WithMany(r => r.Users)
            .HasForeignKey(r => r.RoomId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Room)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies().UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        base.OnConfiguring(optionsBuilder);
    }
}