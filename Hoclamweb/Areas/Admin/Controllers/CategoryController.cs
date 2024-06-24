using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Microsoft.AspNetCore.Mvc;
using Suitshop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Suitshop.Utility;

namespace Suitshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		[Route("category")]
		public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

		[Route("category/create")]
		[HttpGet, ActionName("Create")]
		public IActionResult CreateGET()
        {
			return View();
        }
		[Route("category/create")]
		[HttpPost, ActionName("Create")]
		public IActionResult CreatePOST(Category category)
        {
            if (ModelState.IsValid)
            {
				var existingCategory = _unitOfWork.Category.Get(c => c.Name == category.Name);
				if (existingCategory != null && existingCategory.Id != category.Id)
				{
					ModelState.AddModelError("Name", "This name already exists.");
					return View();
				}

                category.UrlName= SD.ReplaceSpace(category.UrlName);

				existingCategory = _unitOfWork.Category.Get(c => c.UrlName == category.UrlName);
				if (existingCategory != null && existingCategory.Id != category.Id)
				{
					ModelState.AddModelError("UrlName", "This urlName already exists.");
					return View();
				}

				_unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created succcesfully!";
                return RedirectToAction("Index");
            }
			TempData["error"] = "Error!";
			return View();
        }

		[Route("category/edit/{id}")]
		[HttpGet, ActionName("Edit")]
		public IActionResult EditGET(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
			Category categoryfrombd = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryfrombd == null)
            {
                return NotFound();
            }
            return View(categoryfrombd);
        }
		[Route("category/edit/{id}")]
		[HttpPost, ActionName("Edit")]
		public IActionResult EditPOST(Category category)
        {
			if (ModelState.IsValid)
			{
				var existingCategory = _unitOfWork.Category.Get(c => c.Name == category.Name);
				if (existingCategory != null && existingCategory.Id != category.Id)
				{
					ModelState.AddModelError("Name", "This update name already exists.");
					return View(category);
				}

				category.UrlName = SD.ReplaceSpace(category.UrlName);

				existingCategory = _unitOfWork.Category.Get(c => c.UrlName == category.UrlName);
				if (existingCategory != null && existingCategory.Id != category.Id)
				{
					ModelState.AddModelError("UrlName", "This urlName already exists.");
					return View(category);
				}

				_unitOfWork.Category.Update(category);
                _unitOfWork.Save();
				TempData["success"] = "Category updated successfully!";
				return RedirectToAction("Index");
			}
            TempData["error"] = "Error!";
			return View(category);
		}

		[Route("category/delete/{id}")]
		[HttpGet, ActionName("Delete")]
		public IActionResult DeleteGET(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryfrombd = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryfrombd == null)
            {
                return NotFound();
            }
            return View(categoryfrombd);
        }
		[Route("category/delete/{id}")]
		[HttpPost, ActionName("Delete")]
        public IActionResult DeleteDELETE(int? id)
        {
            Category category = _unitOfWork.Category.Get(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Remove(category);
                _unitOfWork.Save();
                TempData["success"] = "Category deleted succcesfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
