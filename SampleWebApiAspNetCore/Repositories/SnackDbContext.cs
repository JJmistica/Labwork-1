using Microsoft.EntityFrameworkCore;
using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.Repositories
{
    public class SnackDbContext : DbContext
    {
        public SnackDbContext(DbContextOptions<SnackDbContext> options)
            : base(options)
        {
        }

        public DbSet<SnackEntity> SnackItems { get; set; } = null!;
    }
}
