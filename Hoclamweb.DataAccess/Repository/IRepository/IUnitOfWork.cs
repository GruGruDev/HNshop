using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ISubCategoryRepository SubCategory { get; }
        IProductRepository Product { get; }
        IProductDetailRepository ProductDetail { get; }
        IImageRepository Image { get; }
        IColorRepository Color { get; }
        ISizeRepository Size { get; }
		IOrderRepository Order { get; }
		IItemRepository Item { get; }
		IReviewRepository Review { get; }
		IShoppingCartRepository ShoppingCart { get; }
		IApplicationUserRepository ApplicationUser { get; }

		void Save();
    }
}
