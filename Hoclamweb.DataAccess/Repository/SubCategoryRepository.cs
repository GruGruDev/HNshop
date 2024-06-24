using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.DataAccess.Repository
{
	public class SubCategoryRepository : Repository<SubCategory>,ISubCategoryRepository
	{
		public SubCategoryRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(SubCategory SubCategory)
		{
			_db.Update(SubCategory);
		}
	}
}
