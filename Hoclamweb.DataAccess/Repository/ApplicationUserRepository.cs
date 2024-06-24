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
	public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
	{
		public ApplicationUserRepository(ApplicationDbContext db) : base(db)
		{
		}

        public void Update(ApplicationUser ApplicationUser)
        {
            _db.Update(ApplicationUser);
        }
    }
}
