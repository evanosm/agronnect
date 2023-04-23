using Microsoft.EntityFrameworkCore;
using AnnuaireCESI.Models;

namespace AnnuaireCESI.Data;

public class AnnuaireDbContext : DbContext
{
    public AnnuaireDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<AccessRequest> AccessRequests { get; set; }
}