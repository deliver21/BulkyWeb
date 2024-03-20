using BulkyWeb.Data;
using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            this._db = db;
        }
        public void Update(Product obj)
        {
            var objfromDb = _db.Products.FirstOrDefault(u => u.Id==obj.Id);
            if(objfromDb != null)
            {
                objfromDb.Category= obj.Category;
                objfromDb.Title= obj.Title;
                objfromDb.Description= obj.Description;
                objfromDb.ISBN= obj.ISBN;
                objfromDb.Author= obj.Author;
                objfromDb.ListPrice= obj.ListPrice;
                objfromDb.Price= obj.Price;
                objfromDb.Price50= obj.Price50;
                objfromDb.Price100= obj.Price100;
                objfromDb.CategoryId= obj.CategoryId;
                if(!String.IsNullOrEmpty(obj.ImageUrl))
                {
                    objfromDb.ImageUrl = obj.ImageUrl;
                }
                _db.Products.Update(objfromDb);
            }
        }
    }
}
