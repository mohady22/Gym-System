using A_MVC01.Configurations;
using A_MVC01.Models;
using Microsoft.EntityFrameworkCore;

namespace A_MVC01.Contexts
{
    public class GymDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=GymDB;Trusted_Connection=true;TrustServerCertificate=true");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfigurations());
        }
        public DbSet<Plan> Plans { get; set; } 
    }
}
