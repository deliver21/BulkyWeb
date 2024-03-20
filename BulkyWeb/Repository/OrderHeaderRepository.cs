using BulkyWeb.Data;
using BulkyWeb.Models;

namespace BulkyWeb.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            this._db = db;
        }
        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? payementStatus = null)
        {
            var orderFromDb=_db.OrderHeaders.First(x => x.Id == id);
            if(orderFromDb != null) {
                orderFromDb.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(payementStatus) )
                {
                    orderFromDb.PaymentStatus = payementStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.OrderHeaders.First(x => x.Id == id);
            if(!string.IsNullOrEmpty(sessionId)) {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate= DateTime.Now;
            }
        }
    }
}
