using BulkyWeb.BulkyUtilities;
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Microsoft.AspNetCore.Authorization.Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unit;
        public CategoryController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public IActionResult Index(string ?sortOrder, string? Search)
        {
            List<Category> objCategoryList = _unit.Category.GetAll().ToList();
            objCategoryList = objCategoryList.OrderBy(x => x.Id).ToList();
            if (Search != null)
            {
                objCategoryList = objCategoryList.Where(x => x.Name.Contains(Search) || x.DisplayOrder.ToString().Contains(Search)).ToList();
            }
            if (sortOrder == "asc")
            {
                objCategoryList = objCategoryList.OrderBy(x => x.Name).ToList();
            }
            if (sortOrder == "desc")
            {
                objCategoryList = objCategoryList.OrderByDescending(x => x.Name).ToList();
            }
            if (sortOrder == "pair")
            {
                objCategoryList = objCategoryList.OrderBy(x => x.DisplayOrder).ToList();
            }
            if (sortOrder == "o")
            {
                objCategoryList = objCategoryList.OrderByDescending(x => x.DisplayOrder).ToList();
            }
            return View(objCategoryList);
        }
        [HttpPost]
        public IActionResult Search(string? query)
        {
            List<Category> categories = _unit.Category.GetAll().ToList();
            if (!string.IsNullOrEmpty(query))
            {
                // If the query is not empty, redirect to the SearchWithQuery action
                return RedirectToAction("SearchWithQuery", new { q = query });
            }
            else
            {
                // If the query is empty, stay on the regular Search action
                return RedirectToAction("Index", categories);
            }

        }
        [HttpGet]
        public ActionResult SearchWithQuery(string q)
        {
            ViewBag.Query = q;
            // Your logic for handling search with a query
            List<Category> categories = _unit.Category.GetAll().ToList();
            categories = categories.Where(x => x.Name.Contains(q)).ToList();
            return View("Index", categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Category Name doesn't have to be the same with the With Display Order");
            }
            if (obj.Name == "Test")
            {
                ModelState.AddModelError("Name", "Test is a invalid value");
            }
            //Examine the validation
            if (ModelState.IsValid)
            {
                _unit.Category.Add(obj);
                _unit.Save();
                //It's a kind of MessageBox i Window form and the name success is set arbitrary
                TempData["success"] = "Category is added successfully";
                //return View();
                //Apart return View you can return redirect to another action
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //First approach
            //Category categoryfromDb = _db.Categories.Find(id);
            //Several to search the id in the -dList
            //2nd approach
            //Category categoryfromDb = _db.Categories.FirstOrDefault(c => c.Id==id); 
            //3rd approach
            Category categoryfromDb = _unit.Category.Get(u => u.Id == id);
            if (categoryfromDb == null)
            {
                return NotFound();
            }

            return View(categoryfromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            //Examine the validation
            if (ModelState.IsValid)
            {
                _unit.Category.Update(obj);
                _unit.Save();
                TempData["success"] = "Category is updated successfully";
                //return View();
                //Apart return View you can return redirect to another action
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Category categoryFromDb = _unit.Category.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category category = _unit.Category.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _unit.Category.Remove(category);
            _unit.Save();
            TempData["success"] = "Category is deleted successfully";
            return RedirectToAction("Index");
        }
        #region
        public IActionResult GetAll()
        {
            List<Category> objCategoryList = _unit.Category.GetAll().ToList();
            return Json(new {data=objCategoryList});
        }
        #endregion
    }
}
