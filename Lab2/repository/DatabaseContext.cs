using Lab2.model;
using Microsoft.EntityFrameworkCore;

namespace Lab2.repository;

class DatabaseContext : DbContext
{
    public DbSet<Attempt> Attempts => Set<Attempt>();

    public DatabaseContext() => Database.EnsureCreated(); 
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5435;Username=postgres;Password=qwerty;Database=postgres");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
    }
}