using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
		}
		public void Update(Product product)
		{
			_db.Update(product);
		}
	}
}
