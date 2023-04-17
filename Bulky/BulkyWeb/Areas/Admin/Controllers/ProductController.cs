﻿using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //Data access object
        private readonly IUnitOfWork _unitOfWork;
        //Access to wwwroot
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            //ViewBag.CategoryList = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() }),
                Product = new Product()
            };

            if(id==null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // delete the old image
                        var oldImgPath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using(var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;

                }

                if(productVM.Product.Id ==0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);  
                }
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
                return View(productVM);
            }
        }

        //public IActionResult Edit(int? id)
        //{
        //    if(id == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        Product product = _unitOfWork.Product.Get(p=> p.Id == id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            return View(product);
        //        }
        //    }

        //}

        //[HttpPost]
        //public IActionResult Edit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}
        
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        Product? product = _unitOfWork.Product.Get(p => p.Id == id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            return View(product);
        //        }
        //    }
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    if(id  == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        Product? product = _unitOfWork.Product.Get(p=>p.Id == id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            _unitOfWork.Product.Remove(product);
        //            _unitOfWork.Save();
        //            TempData["success"] = "Product updated successfully";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = products});
        }

        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }
            var oldImageUrl = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImageUrl))
            {
                System.IO.File.Delete(oldImageUrl);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save(); 
            return Json( new { success = true, message = "Deleted Successfully" });
        }

        #endregion


    }
}
