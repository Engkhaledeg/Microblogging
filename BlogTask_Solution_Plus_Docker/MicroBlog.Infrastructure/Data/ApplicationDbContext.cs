using Microsoft.EntityFrameworkCore;
using MicroBlog.Core.Entities;

namespace MicroBlog.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts)
            : base(opts) { }

        public DbSet<Post> Posts { get; set; }
    }
}
