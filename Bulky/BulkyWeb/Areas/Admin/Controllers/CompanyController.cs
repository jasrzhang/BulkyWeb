using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        //Data access object
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }

        public IActionResult Index()
        {
            List<Company> Companys = _unitOfWork.Company.GetAll().ToList();
            return View(Companys);
        }

        public IActionResult Upsert(int? id)
        {
           
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company company = _unitOfWork.Company.Get(u => u.Id == id);
                return View(company);
            }


        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
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
        //        Company Company = _unitOfWork.Company.Get(p=> p.Id == id);
        //        if (Company == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            return View(Company);
        //        }
        //    }

        //}

        //[HttpPost]
        //public IActionResult Edit(Company Company)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(Company);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company updated successfully";
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
        //        Company? Company = _unitOfWork.Company.Get(p => p.Id == id);
        //        if (Company == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            return View(Company);
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
        //        Company? Company = _unitOfWork.Company.Get(p=>p.Id == id);
        //        if (Company == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            _unitOfWork.Company.Remove(Company);
        //            _unitOfWork.Save();
        //            TempData["success"] = "Company updated successfully";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companys = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = Companys });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleteing" });
            }
           

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion


    }
}
