using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public interface ICategoryRepository:IRepository<Category>
    {
        void Update(Category obj);
        void Save();
    }
}
