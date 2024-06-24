using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //IEnumerable<T> GetAll();
        T Find(int id);
        IQueryable<T> GetAll();
		T Get(Expression<Func<T, bool>> filter);
		IQueryable<T> GetToInclude(Expression<Func<T, bool>> filter);
		IQueryable<T> GetRange(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
