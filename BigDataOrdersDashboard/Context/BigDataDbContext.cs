using BigDataOrdersDashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Context
{
    public class BigDataDbContext : DbContext
    {
        public BigDataDbContext(DbContextOptions<BigDataDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Category> Categories { get; set; }
    }
}
