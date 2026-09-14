using Microsoft.EntityFrameworkCore;

namespace Amazings_API.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<Amazings_API.Models.Customer> Customers { get; set; }
    }
}
