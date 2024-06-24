using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ApplicationDbContext _db;
		public ICategoryRepository Category { get; private set; }
		public ISubCategoryRepository SubCategory { get; private set; }
		public IProductRepository Product { get; private set; }
		public IProductDetailRepository ProductDetail { get; private set; }
		public IImageRepository Image { get; private set; }
		public IColorRepository Color { get; private set; }
		public ISizeRepository Size { get; private set; }
		public IOrderRepository Order { get; private set; }
		public IItemRepository Item { get; private set; }
		public IReviewRepository Review { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }


		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			Category = new CategoryRepository(_db);
			SubCategory = new SubCategoryRepository(_db);
			Product = new ProductRepository(_db);
			ProductDetail = new ProductDetailRepository(_db);
			Image = new ImageRepository(_db);
			Color = new ColorRepository(_db);
			Size = new SizeRepository(_db);
			Order = new OrderRepository(_db);
			Item = new ItemRepository(_db);
			Review = new ReviewRepository(_db);
			ShoppingCart = new ShoppingCartRepository(_db);
			ApplicationUser = new ApplicationUserRepository(_db);
		}

		public void Save()
		{
			_db.SaveChanges();
		}

	}
}
