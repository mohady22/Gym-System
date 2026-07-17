using GymSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymSystem.DAL.Contexts
{
    public class GymDbContext:IdentityDbContext<ApplicationUser>
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.;Database=GymDB;Trusted_Connection=true;TrustServerCertificate=true");
        //}
        public GymDbContext(DbContextOptions<GymDbContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Membership> Memberships { get; set; }
    }
}
