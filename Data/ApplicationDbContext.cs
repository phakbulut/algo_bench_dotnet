using algo.Data;
using Microsoft.EntityFrameworkCore;

namespace AlgorithmProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AlgorithmLog> AlgorithmLogs { get; set; }  // AlgorithmLog tablosu

        // DbSet'lerinizi burada tanımlayın
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
} 