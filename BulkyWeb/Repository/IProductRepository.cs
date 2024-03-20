using BulkyWeb.Models;
namespace BulkyWeb.Repository
{
    public interface IProductRepository:IRepository<Product>
    {
        void Update(Product obj);
    }
}