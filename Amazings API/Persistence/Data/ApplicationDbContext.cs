using Amazings_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Amazings_API.Persistence.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Customer> Customers { get; set; }

		public DbSet<Transaction> Transactions { get; set; }

		public DbSet<Account> Accounts { get; set; }

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Account>()
				.Property(a => a.Balance)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Transaction>()
				.Property(t => t.Amount)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Customer>(entity =>
			{
				entity.Property(c => c.FirstName).HasMaxLength(50).IsRequired();
				entity.Property(c => c.LastName).HasMaxLength(50).IsRequired();
				entity.Property(c => c.Email).HasMaxLength(256).IsRequired();
				entity.Property(c => c.Phone).HasMaxLength(30).IsRequired();
				entity.HasIndex(c => c.Email).IsUnique();
			});

			modelBuilder.Entity<User>(entity =>
			{
				entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
				entity.Property(u => u.PasswordHash).IsRequired();
				entity.Property(u => u.Role)
					.HasMaxLength(20)
					.IsRequired()
					.HasDefaultValue("Customer");
				entity.HasIndex(u => u.Username).IsUnique();
			});
		}
	}
}
