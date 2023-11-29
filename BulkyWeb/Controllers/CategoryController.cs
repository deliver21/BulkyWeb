using BulkyWeb;
using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository db) 
        {
            this._categoryRepo= db;
        }
        public IActionResult Index(string sortOrder,string?Search)
        {
            List<Category> objCategoryList=_categoryRepo.GetAll().ToList();
            objCategoryList=objCategoryList.OrderBy(x =>x.Id).ToList();
            if(Search!=null)
            {
                objCategoryList=objCategoryList.Where(x=>x.Name.Contains(Search) || x.DisplayOrder.ToString().Contains(Search)).ToList();
            }
            if(sortOrder=="asc")
            {
                objCategoryList = objCategoryList.OrderBy(x => x.Name).ToList();
            }
            if(sortOrder=="desc")
            {
                objCategoryList = objCategoryList.OrderByDescending(x => x.Name).ToList();
            }
            if(sortOrder=="pair")
            {
                objCategoryList=objCategoryList.OrderBy(x=>x.DisplayOrder).ToList();
            }
            if (sortOrder == "o")
            {
                objCategoryList = objCategoryList.OrderByDescending(x => x.DisplayOrder).ToList();
            }
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create( Category obj)
        {
            if(obj.Name==obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name","The Category Name doesn't have to be the same with the With Display Order");
            }
            if(obj.Name=="Test")
            {
                ModelState.AddModelError("Name","Test is a invalid value");
            }
            //Examine the validation
            if(ModelState.IsValid)
            {
                _categoryRepo.Add(obj);
                _categoryRepo.Save();
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
            if(id==null|| id==0)
            {
                return NotFound();
            }
            //First approach
            //Category categoryfromDb = _db.Categories.Find(id);
            //Several to search the id in the -dList
            //2nd approach
            //Category categoryfromDb = _db.Categories.FirstOrDefault(c => c.Id==id); 
            //3rd approach
            Category categoryfromDb = _categoryRepo.Get(u=>u.Id==id);
            if (categoryfromDb==null)
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
                _categoryRepo.Update(obj);
                _categoryRepo.Save();
                TempData["success"] = "Category is updated successfully";
                //return View();
                //Apart return View you can return redirect to another action
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult Delete(int?id)
        {
            if(id==0 ||id==null)
            {
                return NotFound();
            }
            Category categoryFromDb = _categoryRepo.Get(c=>c.Id==id);
            if (categoryFromDb==null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id) 
        {
            Category category =_categoryRepo.Get(u=>u.Id==id);
            if(category==null)
            {
                return NotFound();  
            }
            _categoryRepo.Remove(category);
            _categoryRepo.Save();
            TempData["success"] = "Category is deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
