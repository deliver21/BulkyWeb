using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
        //Update order status or payement status
        void UpdateStatus(int id ,string orderStatus, string? payementStatus =null);
        //Update payement status
        void UpdateStripePaymentID(int id ,string sessionId ,string paymentIntentId);
    }
}
