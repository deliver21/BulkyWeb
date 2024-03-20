using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public interface IOrderDetailRepository:IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
