using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.DataAccess.Repository
{
	public class ProductDetailRepository : Repository<ProductDetail>, IProductDetailRepository
	{
		public ProductDetailRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(ProductDetail productDetail)
		{
			_db.Update(productDetail);
		}
	}
}
