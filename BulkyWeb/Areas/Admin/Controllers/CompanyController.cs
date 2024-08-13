using BulkyWeb.Data;
using BulkyWeb.Repository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using BulkyWeb.BulkyUtilities;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unit;
        public CompanyController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public IActionResult Index()
        {
            List<Company> obj = _unit.Company.GetAll().ToList();          
            if(obj!=null)
            {
                IEnumerable<SelectListItem> Categorylist = _unit.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });                
                return View(obj);
            }
            else
            {
                return View();
            }
        }       
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //Create

                return View(new Company());
            }
            else
            {
                //Update
                // productVM.Product = _unit.Product.Get(u => u.Id == id); return already filled values in the Upsert view
                Company obj = _unit.Company.Get(u => u.Id== id);
                return View(obj);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(Company Companyobj )
        {
          if(ModelState.IsValid)
            {
                if(Companyobj.Id==0) 
                {
                    _unit.Company.Add(Companyobj);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unit.Company.Update(Companyobj);
                    TempData["success"] = "Company updated successfully";
                }
                _unit.Save();
                return RedirectToAction("Index");   
            }
            else
            {

                return View(Companyobj);
            }
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> obj = _unit.Company.GetAll().ToList();
            return Json(new { data = obj});
        }
        [HttpDelete]
        public IActionResult Delete(int?id)
        {
            Company companyToBeDeleted = _unit.Company.Get(u=>u.Id==id);
            if(companyToBeDeleted == null)
            {
                return Json(new {success= false , message="Error while deleting"});
            }
            else
            {
                _unit.Company.Remove(companyToBeDeleted);
                _unit.Save();
            }
            return Json(new { success = true, message = "Delete successfully performed"});
        }
        #endregion

    }

}
