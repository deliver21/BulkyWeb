using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public interface IShoppingCartRepository:IRepository<ShoppingCart>
    {
        void Update(ShoppingCart obj);
    }
}
