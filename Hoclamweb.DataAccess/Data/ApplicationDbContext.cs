using Suitshop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Suitshop.DataAccess.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<SubCategory> SubCategories { get; set; }
		public DbSet<Size> Sizes { get; set; }
		public DbSet<Color> Colors { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductDetail> ProductDetails { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

            foreach (var e in modelBuilder.Model.GetEntityTypes())
            {
				var tableName = e.GetTableName();
				if (tableName.StartsWith("AspNet"))
				{
					e.SetTableName(tableName.Substring(6));
				}
            }
        }

	}
}
