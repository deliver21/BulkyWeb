using BulkyWeb.BulkyUtilities;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModels;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string ? status)
        {
            //Remove All user OrderHeader with payement status pending because payement failed
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (User.IsInRole(SD.Role_Customer))
            {
                List<OrderHeader> orderFailed = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId && u.OrderStatus == SD.PaymentStatusPending).ToList();
                _unitOfWork.OrderHeader.RemoveRange(orderFailed);
                _unitOfWork.Save();
            }
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                orderHeader=_unitOfWork.OrderHeader.Get(u=>u.Id==orderId,includeproperties: "ApplicationUser"),
                orderDetail=_unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId==orderId,includeproperties:"Product")

            };
            return View(OrderVM);
        }
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        [HttpPost]
        public IActionResult UpdateOrderDetail()
        {
            //possible because of the bindProperty
            var orderHeaderFromDb = new OrderHeader();
            orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.orderHeader.Id);
            orderHeaderFromDb.Name= OrderVM.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber= OrderVM.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress= OrderVM.orderHeader.StreetAddress;
            orderHeaderFromDb.City= OrderVM.orderHeader.City;
            orderHeaderFromDb.State= OrderVM.orderHeader.State;
            orderHeaderFromDb.PostalCode= OrderVM.orderHeader.PostalCode;
            if(!string.IsNullOrEmpty(OrderVM.orderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier=OrderVM.orderHeader.Carrier;
            }
            if(!string.IsNullOrEmpty(OrderVM.orderHeader.Trackingnumber))
            {
                orderHeaderFromDb.Trackingnumber=OrderVM.orderHeader.Trackingnumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Details is updated successfully";
            return RedirectToAction(nameof(Details), new {orderId= orderHeaderFromDb.Id });
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpPost]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.orderHeader.Id,SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["sucess"] = "The payement of "+OrderVM.orderHeader.Name+" is successfully updated to "+SD.StatusInProcess;
            return RedirectToAction(nameof(Details), new {orderId=OrderVM.orderHeader.Id});
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpPost]
        public IActionResult ShipOrder()
        {
            var orderHeader=_unitOfWork.OrderHeader.Get(u=>u.Id==OrderVM.orderHeader.Id);

            orderHeader.Trackingnumber=OrderVM.orderHeader.Trackingnumber;
            orderHeader.Carrier=OrderVM.orderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            orderHeader.ShippingDate=DateTime.Now;
            if(orderHeader.PaymentStatus==SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate =DateOnly.FromDateTime( DateTime.Now.AddDays(30));
            }
            if (!string.IsNullOrEmpty(OrderVM.orderHeader.Carrier))
            {
                orderHeader.Carrier = OrderVM.orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.orderHeader.Trackingnumber))
            {
                orderHeader.Trackingnumber = OrderVM.orderHeader.Trackingnumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["sucess"] = "The payement of " + OrderVM.orderHeader.Name + " is successfully updated to " + SD.StatusShipped;
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.orderHeader.Id });
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpPost]
        public IActionResult CancelOrder()
        {
            var orderHeader=_unitOfWork.OrderHeader.Get(u=>u.Id==OrderVM.orderHeader.Id);
            if(orderHeader.PaymentStatus==SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund= service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusRefund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["sucess"] = "Order cancelled successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.orderHeader.Id });
        }
        [HttpPost]
        [ActionName(nameof(Details))]
        public IActionResult DetailsPayNow()
        {
            OrderVM.orderHeader=_unitOfWork.OrderHeader.Get(u=>u.Id==OrderVM.orderHeader.Id,includeproperties:"ApplicationUser");
            OrderVM.orderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.orderHeader.Id, includeproperties: "Product");

            var domain = "https://localhost:7107/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PayConfirmation?orderHeaderId={OrderVM.orderHeader.Id}",
                //add cancel url
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.orderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            //session.PaymentIntentId is always null while the payement is not succesful done or not done
            _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            //redirect to stripe website
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.orderHeader.Id });
        }
        public IActionResult PayConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            //Confirm Stripe Payment
            if (User.IsInRole(SD.Role_Company))
            {
                if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
                {
                    // order by company

                    var service = new SessionService();
                    Session session = service.Get(orderHeader.SessionId);
                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        _unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                        _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                        _unitOfWork.Save();
                    }
                    //else
                    //{
                    //    List<OrderHeader> orderFailed = _unitOfWork.OrderHeader.  (u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                    //    _unitOfWork.OrderHeader.RemoveRange(orderFailed);
                    //    _unitOfWork.Save();
                    //}
                }
            }
            return View(orderHeaderId);
        }

        #region API CALLS

        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<OrderHeader> objOrderHeaders ;
            if (User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders= _unitOfWork.OrderHeader.GetAll(includeproperties: "ApplicationUser");
            }
            else
            {
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId==userId,includeproperties: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = objOrderHeaders });
        }

        #endregion
    }
}
