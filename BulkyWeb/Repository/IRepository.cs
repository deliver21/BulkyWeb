using System.Linq.Expressions;

namespace BulkyWeb.Repository
{
    //    We're working with generic with generic class T
    public interface IRepository<T> where T : class
    {
        // T -could be any classes , it's just a pattern that specifies that  it coulbe apply to Categoryclass for instance
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeproperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeproperties = null, bool track = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
