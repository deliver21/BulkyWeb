using BulkyWeb.BulkyUtilities;
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ManagerController : Controller
    {
        private IUnitOfWork _unit;
        private readonly UserManager<IdentityUser> _userManager;
        private ApplicationDbContext _db;
        public ManagerController(IUnitOfWork unit,UserManager<IdentityUser> userManager,ApplicationDbContext db)
        {
            _unit = unit;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index(string ? role)
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        #region API CALLS
        [HttpGet]
        [Authorize]
        public IActionResult GetAll( string role)
        {
            List<ApplicationUser> users= new List<ApplicationUser>();
            var userTable = _unit.ApplicationUser.GetAll().ToList();
            switch (role)
            {
                case "customer":                    
                  users = new List<ApplicationUser>();
                  foreach(var user in userTable)
                  {
                        if(_userManager.IsInRoleAsync(user,SD.Role_Customer).Result)
                        {      
                            users.Add(user);
                        }
                  }
                    break;
                case "company":
                    users = new List<ApplicationUser>();
                    foreach (var user in userTable)
                    {
                        if (_userManager.IsInRoleAsync(user, SD.Role_Company).Result)
                        {
                            user.Role = SD.Role_Company;
                            users.Add(user);
                        }
                    }
                    break;
                case "employee":
                    users = new List<ApplicationUser>();
                    foreach (var user in userTable)
                    {
                        if (_userManager.IsInRoleAsync(user, SD.Role_Employee).Result)
                        {
                            user.Role = SD.Role_Employee;
                            users.Add(user);
                        }
                    }
                    break;
                case "admin":
                    users = new List<ApplicationUser>();
                    foreach (var user in userTable)
                    {
                        if (_userManager.IsInRoleAsync(user, SD.Role_Admin).Result)
                        {
                            user.Role = SD.Role_Admin;
                            users.Add(user);
                        }
                    }
                    break;
                default:
                    users = new List<ApplicationUser>();
                    foreach (var user in userTable)
                    {
                        if (_userManager.IsInRoleAsync(user, SD.Role_Admin).Result)
                        {
                            user.Role = SD.Role_Admin;
                            users.Add(user);
                        }
                        if (_userManager.IsInRoleAsync(user, SD.Role_Company).Result)
                        {
                            user.Role = SD.Role_Company;
                            users.Add(user);
                        }
                        if (_userManager.IsInRoleAsync(user, SD.Role_Customer).Result)
                        {
                            user.Role = SD.Role_Customer;
                            users.Add(user);
                        }
                    }
                    break;
            }            
            return Json(new { data = users});
        }
        #endregion
    }
}
