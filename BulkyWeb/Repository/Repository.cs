using BulkyWeb.Data;
using BulkyWeb.Repository;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Repository
{
    //We're working with generic with generic class T
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet { get; set; }
        public Repository(ApplicationDbContext db)
        {
            _db = db;

            //Instead of writing _db.Category and so one as in category controller, we write _dbSet that represent the model
            this.dbSet=_db.Set<T>();
            _db.Products.Include(u=>u.Category);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeproperties = null, bool track=false)
        {
            if(track)
            {
                IQueryable<T> query = dbSet;
                query = query.Where(filter);
                if (!string.IsNullOrEmpty(includeproperties))
                {
                    foreach (var includeProp in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }
            else
            {
                IQueryable<T> query = dbSet.AsNoTracking();
                query = query.Where(filter);
                if (!string.IsNullOrEmpty(includeproperties))
                {
                    foreach (var includeProp in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }
           
        }

        public IEnumerable<T> GetAll( string ? includeproperties=null)
        {
           IQueryable<T> query = dbSet;
            if(!string.IsNullOrEmpty(includeproperties))
            {
                foreach(var includeProp in includeproperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
           dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}