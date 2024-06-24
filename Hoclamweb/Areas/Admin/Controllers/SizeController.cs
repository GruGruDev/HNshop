using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using Suitshop.Utility;

namespace Suitshop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class SizeController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public SizeController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("size")]
		public IActionResult Index()
		{
			SizeVM vm= new SizeVM();
			vm.Sizes = _unitOfWork.Size.GetAll().ToList();

			return View(vm);
		}

		//[Route("size/create")]
		//[HttpGet, ActionName("Create")]
		//public IActionResult CreateGET()
		//{
		//	return View();
		//}

		[HttpPost]
		public IActionResult Create(Size size)
		{
			if (ModelState.IsValid)
			{
				var existingSize = _unitOfWork.Size.Get(c => c.Name == size.Name);
				if (existingSize != null && existingSize.Id != size.Id)
				{
					ModelState.AddModelError("Name", "This name already exists.");
					return View();
				}
				_unitOfWork.Size.Add(size);
                _unitOfWork.Save();
				TempData["success"] = "Tạo size mới thành công!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return RedirectToAction("Index");
		}

		//[Route("size/edit/{id}")]
		//[HttpGet, ActionName("Edit")]
		//public IActionResult EditGET(int? id)
		//{
		//	if (id == null || id == 0)
		//	{
		//		return NotFound();
		//	}
		//	Size size = _unitOfWork.Size.Get(u => u.Id == id);
		//	if (size == null)
		//	{
		//		return NotFound();
		//	}
		//	return View(size);
		//}

		[HttpPost]
		public IActionResult Update(Size size)
		{
			if (ModelState.IsValid)
			{
				var existingSize = _unitOfWork.Size.Get(c => c.Name == size.Name);
				if (existingSize != null && existingSize.Id != size.Id)
				{
					ModelState.AddModelError("Name", "This update name already exists.");
					return View(size);
				}
				_unitOfWork.Size.Update(size);
                _unitOfWork.Save();
				TempData["success"] = "Cập nhật size thành công!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return RedirectToAction("Index");
		}

		//[Route("size/delete/{id}")]
		//[HttpGet, ActionName("Delete")]
		//public IActionResult DeleteGET(int? id)
		//{
		//	if (id == null || id == 0)
		//	{
		//		return NotFound();
		//	}
		//	Size size = _unitOfWork.Size.Get(u => u.Id == id);
		//	if (size == null)
		//	{
		//		return NotFound();
		//	}
		//	return View(size);
		//}

		[HttpPost]
		public IActionResult Delete(int? id)
		{
			Size size = _unitOfWork.Size.Get(u => u.Id == id);
			if (size == null)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				_unitOfWork.Size.Remove(size);
                _unitOfWork.Save();
				TempData["success"] = "Xoá size thành công!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return RedirectToAction("Index");
		}
	}
}
