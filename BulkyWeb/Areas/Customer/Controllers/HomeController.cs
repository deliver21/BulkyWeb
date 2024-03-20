using BulkyWeb.BulkyUtilities;
using BulkyWeb.Models;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Authorization;
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
            IEnumerable<Product> productlist = _unit.Product.GetAll(includeproperties: "Category"); 
            return View(productlist);   
        }
        public IActionResult Details(int Id)
        {
            ShoppingCart cart = new()
            {
                //Product = _unit.Product.Get(u => u.Id == Id, includeproperties: "Category"),
                ProductCount = 1,
                //ProductId= Id
            };
            ShoppingCartTrans.ProductId = cart.ProductId;
            ShoppingCartTrans.ProductCount = cart.ProductCount;
            ShoppingCartTrans.Product = cart.Product;

            //ViewData["cart"]= cart;
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
            shoppingCart.ApplicationUserId = userId;
            // To avoid duplication in Db while saving Data
            var shoppingCartFromDb=_unit.ShoppingCart.Get(u=>u.ApplicationUserId== userId && u.ProductId==shoppingCart.ProductId);
            if(shoppingCartFromDb!=null)
            {
                //shoppingCart esxist
                shoppingCartFromDb.ProductCount += shoppingCart.ProductCount;
                _unit.ShoppingCart.Update(shoppingCartFromDb);
            }
            else
            {
                //add cart record
                _unit.ShoppingCart.Add(shoppingCart);
            }
            _unit.Save();
            TempData["success"] = "Item is successfully added to the cart";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
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