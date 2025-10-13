using Microsoft.EntityFrameworkCore;

namespace QuerySheper.Persistence
{
    public class GenericDbContext : DbContext
    {
        public GenericDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);

        }
    }
}
