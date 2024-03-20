using BulkyWeb.Data;
using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        { 
            this._db = db;
        }       
        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
