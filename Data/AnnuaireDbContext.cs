using Microsoft.EntityFrameworkCore;
using AnnuaireCESI.Models;

namespace AnnuaireCESI.Data;

public class AnnuaireDbContext : DbContext
{
    public AnnuaireDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<AccessRequest> AccessRequests { get; set; }
}