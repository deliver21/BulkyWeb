using BulkyWeb.BulkyUtilities;
using BulkyWeb.Models;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unit;
        public HomeController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public IActionResult Index()
        {
            //List<OrderDetail> orderDetail = new();
            //orderDetail = _unit.OrderDetail.GetAll().ToList();
            //List<OrderHeader> order = new();
            //order = _unit.OrderHeader.GetAll().ToList();
            //_unit.OrderDetail.RemoveRange(orderDetail);
            //_unit.OrderHeader.RemoveRange(order);
            //_unit.Save();
            var claimsIdentity=(ClaimsIdentity)User.Identity;
            var claim=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
               _unit.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
            }
            else
            {
                HttpContext.Session.SetInt32(SD.SessionCart,0);
            }
            IEnumerable<Product> productlist = _unit.Product.GetAll(includeproperties: "Category"); 
            return View(productlist);   
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unit.Product.Get(u => u.Id == productId, includeproperties: "Category"),
                ProductCount = 1,
                ProductId = productId
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]           
        //[Authorize] is to get the user id of the user that posts the product
        //regardless to their role
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity to get the user Id of the logged in user
            //(ClaimsIdentity)User.Identity is to convert it in ClaimsIdentity
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;           
                HttpContext.Session.SetInt32(SD.SessionCart,
               _unit.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            
            shoppingCart.ApplicationUserId = userId;
            // To avoid duplication in Db while saving Data
            var shoppingCartFromDb=_unit.ShoppingCart.Get(u=>u.ApplicationUserId== userId && u.ProductId==shoppingCart.ProductId);
            if(shoppingCartFromDb!=null)
            {
                //shoppingCart exist
                shoppingCartFromDb.ProductCount += shoppingCart.ProductCount;
                _unit.ShoppingCart.Update(shoppingCartFromDb);
                _unit.Save();
            }
            else
            {
                //add cart record
                _unit.ShoppingCart.Add(shoppingCart);
                _unit.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unit.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            
            TempData["success"] = "Item is successfully added to the cart";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult test()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}