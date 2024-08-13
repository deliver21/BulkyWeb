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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unit, IWebHostEnvironment webHostEnvironment)
        {
            _unit = unit;
            _webHostEnvironment= webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> obj = _unit.Product.GetAll(includeproperties: "Category").ToList();          
            if(obj!=null)
            {
                IEnumerable<SelectListItem> Categorylist = _unit.Category.GetAll().Select(u => new SelectListItem{
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
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _unit.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //Create

                return View(productVM);
            }
            else
            {
                //Update
                // productVM.Product = _unit.Product.Get(u => u.Id == id); return already filled values in the Upsert view
                productVM.Product = _unit.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj ,IFormFile ? file)
        {
            //Examine the validation
            if (ModelState.IsValid)
            {
                //return the root www path
                string wwwRootpath = _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    //Guid.NewGuid().ToString() is used to randomly name files while saving them in the folder
                    //Path.GetExtension(file.FileName) is to get the same extension that the uploaded image
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath = Path.Combine(wwwRootpath, @"images\Products");
                    // While update the product we need to check if the image field is updated or not so that to not change 
                    // the value of the current image , however if it's updated we need to change the value and delete the old path
                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldpath = Path.Combine(wwwRootpath, obj.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Delete(oldpath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    obj.Product.ImageUrl = @"\images\Products\" + filename;
                                     
                }
                if(obj.Product.Id== 0)
                {
                    //Create
                    _unit.Product.Add(obj.Product);
                    TempData["success"] = "Product " + obj.Product.Title +" is added successfully";
                }
                else
                {
                    //Update
                    _unit.Product.Update(obj.Product);
                    //It's a kind of MessageBox i Window form and the name success is set arbitrary                    
                    TempData["success"] ="Product "+obj.Product.Title+" is updated successfully";
                }
                _unit.Save();
                //return View();
                //Apart return View you can return redirect to another action
                return RedirectToAction("Index");
            }           
            return View();
        }
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    //First approach
        //    //Category categoryfromDb = _db.Categories.Find(id);
        //    //Several to search the id in the -dList
        //    //2nd approach
        //    //Category categoryfromDb = _db.Categories.FirstOrDefault(c => c.Id==id); 
        //    //3rd approach
        //    Product productfromDb = _unit.Product.Get(u => u.Id == id);
        //    if (productfromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(productfromDb);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{

        //    //Examine the validation
        //    if (ModelState.IsValid)
        //    {
        //        _unit.Product.Update(obj);
        //        _unit.Save();
        //        TempData["success"] = "Producyt is updated successfully";
        //        //return View();
        //        //Apart return View you can return redirect to another action
        //        return RedirectToAction("Index");
        //    }
        //    return View(obj);
        //}
        //public IActionResult Delete(int? id)
        //{
        //    if (id == 0 || id == null)
        //    {
        //        return NotFound();
        //    }
        //    Product productFromDb = _unit.Product.Get(c => c.Id == id);
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product product = _unit.Product.Get(u => u.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _unit.Product.Remove(product);
        //    _unit.Save();
        //    TempData["success"] = "Product is deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> obj = _unit.Product.GetAll(includeproperties: "Category").ToList();
            return Json(new { data = obj});
        }
        [HttpDelete]
        public IActionResult Delete(int?id)
        {
            Product productToBeDeleted = _unit.Product.Get(u=>u.Id==id);
            if(productToBeDeleted == null)
            {
                return Json(new {success= false , message="Error while deleting"});
            }
            else
            {
                string wwwRootpath = _webHostEnvironment.WebRootPath;
                var oldpath = Path.Combine(wwwRootpath, productToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }
                _unit.Product.Remove(productToBeDeleted);
                _unit.Save();
            }
            return Json(new { success = true, message = "Delete successfully performed" });
        }
        #endregion

    }

}
