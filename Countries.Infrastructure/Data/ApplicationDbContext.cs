using Countries.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Countries.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }

        // Configured the model using Fluent API, Read The Cinfigurations inside Infrastructure Assemmply Configuration files 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}
