using BulkyWeb.BulkyUtilities;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModels;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // HttpContext.Session.SetInt32(SD.SessionCart,
            //_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            if(User.IsInRole(SD.Role_Customer))
            {
                List<OrderHeader> orders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId && u.PaymentStatus == SD.PaymentStatusPending).ToList();
                _unitOfWork.OrderHeader.RemoveRange(orders);
                _unitOfWork.Save();
            }
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId
                , includeproperties: "Product"),
                OrderHeader= new()
            };
            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price=GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal+= (cart.Price*cart.ProductCount);
            }
            return View(shoppingCartVM);
        }
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.ProductCount<=50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if(shoppingCart.ProductCount <=100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
        public IActionResult Plus(int id)
        {
            var shoppingFromDb=_unitOfWork.ShoppingCart.Get(u=>u.Id==id);
            shoppingFromDb.ProductCount += 1;
            _unitOfWork.ShoppingCart.Update(shoppingFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int id)
        {
            var shoppingFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id);
            if(shoppingFromDb.ProductCount > 1)
            {
                shoppingFromDb.ProductCount -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingFromDb);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(shoppingFromDb);
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                _unitOfWork.Save();
                if (claim != null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                   _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                _unitOfWork.Save();
            }
            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int id)
        {
            var shoppingFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id);
            _unitOfWork.ShoppingCart.Remove(shoppingFromDb);
            _unitOfWork.Save();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
               _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles =SD.Role_Customer+","+SD.Role_Company)]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if(_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count()<=0 || _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count()==null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ShoppingCartVM shoppingCartVM = new()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId
                      , includeproperties: "Product"),
                    OrderHeader = new()
                };
                //Populate Shoppinh cart header from ApplicationUser data
                shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
                //if (shoppingCartVM.OrderHeader.ApplicationUser == null)
                //{
                //    return RedirectToAction(nameof(Index));
                //}
                //else
                //{
                shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
                shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
                shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
                shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
                shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
                shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
                foreach (var cart in shoppingCartVM.ShoppingCartList)
                {
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.ProductCount);
                }
                //}

                return View(shoppingCartVM);
            }
        }
        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId
				, includeproperties: "Product");

			shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			shoppingCartVM.OrderHeader.ApplicationUserId=userId;
            
           		//Populate Shoppinh cart header from ApplicationUser data
				ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

				foreach (var cart in shoppingCartVM.ShoppingCartList)
				{
					cart.Price = GetPriceBasedOnQuantity(cart);
					shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.ProductCount);
				}
				if (applicationUser.CompanyId.GetValueOrDefault() == 0)
				{
					//it is a regular customer account and we need to capture the payement
					shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
					shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
				}
				else
				{
                //it's a company user
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
					shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
				}
            //Check if there'are other order from same user####################

            // ################################
            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();

				foreach (var cart in shoppingCartVM.ShoppingCartList)
				{
					OrderDetail orderDetail = new OrderDetail()
					{
						ProductId = cart.ProductId,
						OrderHeaderId = shoppingCartVM.OrderHeader.Id,
						Price = cart.Price,
						Count = cart.ProductCount,
					};
					_unitOfWork.OrderDetail.Add(orderDetail);
					_unitOfWork.Save();
				}
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer account and we need to capture the payement
                //stripe logic
                var domain = "https://localhost:7107/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain+ $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    //add cancel url
                    CancelUrl= domain+"customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach(var item in shoppingCartVM.ShoppingCartList)
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
                        Quantity = item.ProductCount
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session=service.Create(options);
                //session.PaymentIntentId is always null while the payement is not succesful done or not done
                _unitOfWork.OrderHeader.UpdateStripePaymentID(shoppingCartVM.OrderHeader.Id,session.Id,session.PaymentIntentId);
                _unitOfWork.Save();
                //redirect to stripe website
                Response.Headers.Add("Location",session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id});			
		}
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);
            //Confirm Stripe Payment
            
                if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
                {
                    // order by customer

                    var service = new SessionService();
                    Session session = service.Get(orderHeader.SessionId);
                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                        _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                        _unitOfWork.Save();
                    }
                HttpContext.Session.Clear();
                    //else
                    //{
                    //    List<OrderHeader> orderFailed = _unitOfWork.OrderHeader.  (u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                    //    _unitOfWork.OrderHeader.RemoveRange(orderFailed);
                    //    _unitOfWork.Save();
                    //}
                }
            List<ShoppingCart> shoppingCarts= _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId ==orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }
	}
    
}
