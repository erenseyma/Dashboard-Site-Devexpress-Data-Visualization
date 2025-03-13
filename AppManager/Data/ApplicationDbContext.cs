using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppManager.Models; // DatabaseModel ve diğer modellerin olduğu namespace

namespace AppManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<DatabaseConnectionModel> DatabaseConnections { get; set; }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<DatabaseModel> Databases { get; set; }
        public DbSet<DashboardModel> Dashboards { get; set; }
        public DbSet<ChartModel> Charts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ChartModel ve DashboardModel arasındaki ilişki
            modelBuilder.Entity<ChartModel>()
                .HasOne(c => c.Dashboard)
                .WithMany(d => d.Charts)
                .HasForeignKey(c => c.DashboardId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
