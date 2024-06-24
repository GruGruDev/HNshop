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
	public class SizeRepository : Repository<Size>, ISizeRepository
	{
		public SizeRepository(ApplicationDbContext db) : base(db)
		{
		}

		public void Update(Size size)
		{
			_db.Update(size);
		}
	}
}
