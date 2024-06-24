using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using Suitshop.Utility;

namespace Suitshop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class SubCategoryController:Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public SubCategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("subCategory")]
		public IActionResult Index()
		{
			List<SubCategory> catDes = _unitOfWork.SubCategory.GetAll().Include(cd => cd.Category).ToList();
			return View(catDes);
		}

		[Route("subCategory/create")]
		[HttpGet, ActionName("Create")]
		public IActionResult CreateGET()
		{
			SubCategoryVM SubCategoryVM = new SubCategoryVM()
			{
				SubCategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				SubCategory = new SubCategory()
			};
			return View(SubCategoryVM);
		}
		[Route("subCategory/create")]
		[HttpPost, ActionName("Create")]
		public IActionResult CreatePOST(SubCategoryVM SubCategoryVM)
		{
			if (ModelState.IsValid)
			{
				var existingSubCategory = _unitOfWork.SubCategory.Get(c => c.Name == SubCategoryVM.SubCategory.Name);
				if (existingSubCategory != null && existingSubCategory.Id != SubCategoryVM.SubCategory.Id)
				{
					ModelState.AddModelError("SubCategory.Name", "This update name already exists.");
					return View(SubCategoryVM);
				}

				SubCategoryVM.SubCategory.UrlName = SD.ReplaceSpace(SubCategoryVM.SubCategory.UrlName);

				existingSubCategory = _unitOfWork.SubCategory.Get(c => c.UrlName == SubCategoryVM.SubCategory.UrlName);
				if (existingSubCategory != null && existingSubCategory.Id != SubCategoryVM.SubCategory.Id)
				{
					ModelState.AddModelError("SubCategory.UrlName", "This update urlName already exists.");
					return View(SubCategoryVM);
				}

				_unitOfWork.SubCategory.Add(SubCategoryVM.SubCategory);
                _unitOfWork.Save();
				TempData["success"] = "Category Detail created succcesfully!";
				return RedirectToAction("Index");
			}else
			{
				SubCategoryVM.SubCategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				});
				TempData["error"] = "Error!";
				return View(SubCategoryVM);
			}

		}

		[Route("subCategory/edit/{id}")]
		[HttpGet, ActionName("Edit")]
		public IActionResult EditGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			SubCategoryVM SubCategoryVM = new SubCategoryVM()
			{
				SubCategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				SubCategory = _unitOfWork.SubCategory.Get(u => u.Id == id)
			};
			if (SubCategoryVM.SubCategory == null)
			{
				return NotFound();
			}
			return View(SubCategoryVM);
		}
		[Route("subCategory/edit/{id}")]
		[HttpPost, ActionName("Edit")]
		public IActionResult EditPOST(SubCategoryVM SubCategoryVM)
		{
			SubCategoryVM.SubCategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			if (ModelState.IsValid)
			{
				var existingSubCategory = _unitOfWork.SubCategory.Get(c => c.Name == SubCategoryVM.SubCategory.Name);
				if (existingSubCategory != null && existingSubCategory.Id != SubCategoryVM.SubCategory.Id)
				{
					ModelState.AddModelError("SubCategory.Name", "This update name already exists.");
					return View(SubCategoryVM);
				}

				SubCategoryVM.SubCategory.UrlName = SD.ReplaceSpace(SubCategoryVM.SubCategory.UrlName);

				existingSubCategory = _unitOfWork.SubCategory.Get(c => c.UrlName == SubCategoryVM.SubCategory.UrlName);
				if (existingSubCategory != null && existingSubCategory.Id != SubCategoryVM.SubCategory.Id)
				{
					ModelState.AddModelError("SubCategory.UrlName", "This update urlName already exists.");
					return View(SubCategoryVM);
				}

				_unitOfWork.SubCategory.Update(SubCategoryVM.SubCategory);
                _unitOfWork.Save();
				TempData["success"] = "Category Detail updated successfully!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return View(SubCategoryVM);
		}

		[Route("subCategory/delete/{id}")]
		[HttpGet, ActionName("Delete")]
		public IActionResult DeleteGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			SubCategoryVM SubCategoryVM = new SubCategoryVM()
			{
				SubCategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				SubCategory = _unitOfWork.SubCategory.Get(u => u.Id == id)
			};
			if (SubCategoryVM.SubCategory == null)
			{
				return NotFound();
			}
			return View(SubCategoryVM);
		}
		[Route("subCategory/delete/{id}")]
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			SubCategory category = _unitOfWork.SubCategory.Get(u => u.Id == id);
			if (category == null)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				_unitOfWork.SubCategory.Remove(category);
                _unitOfWork.Save();
				TempData["success"] = "Category Detail deleted succcesfully!";
				return RedirectToAction("Index");
			}
			return View();
		}
	}
}
