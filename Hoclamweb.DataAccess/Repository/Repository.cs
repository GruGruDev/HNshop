using Suitshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Suitshop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        public DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Find(int id)
        {
            return dbSet.Find(id);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet.Where(filter).AsNoTracking();
            return query.FirstOrDefault();
        }

		public IQueryable<T> GetToInclude(Expression<Func<T, bool>> filter)
		{
			IQueryable<T> query = dbSet.Where(filter).AsNoTracking();
			return query;
		}

		public IQueryable<T> GetRange(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet.Where(filter).AsNoTracking();
            return query;
        }

        public IQueryable<T> GetAll()
        {
            return dbSet;
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
