using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Utility;

namespace Suitshop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ColorController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public ColorController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("color")]
		public IActionResult Index()
		{
			List<Color> colors = _unitOfWork.Color.GetAll().ToList();
			return View(colors);
		}

		[Route("color/create")]
		[HttpGet, ActionName("Create")]
		public IActionResult CreateGET()
		{
			return View();
		}

		[Route("color/create")]
		[HttpPost, ActionName("Create")]
		public IActionResult CreatePOST(Color color)
		{
			if (ModelState.IsValid)
			{
				var existingSize = _unitOfWork.Color.Get(c => c.Name == color.Name);
				if (existingSize != null && existingSize.Id != color.Id)
				{
					ModelState.AddModelError("Name", "This name already exists.");
					return View();
				}
				_unitOfWork.Color.Add(color);
                _unitOfWork.Save();
				TempData["success"] = "Color created succcesfully!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return View();
		}

		[Route("color/edit/{id}")]
		[HttpGet, ActionName("Edit")]
		public IActionResult EditGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Color color = _unitOfWork.Color.Get(u => u.Id == id);
			if (color == null)
			{
				return NotFound();
			}
			return View(color);
		}

		[Route("color/edit/{id}")]
		[HttpPost, ActionName("Edit")]
		public IActionResult EditPOST(Color color)
		{
			if (ModelState.IsValid)
			{
				var existingColor = _unitOfWork.Color.Get(c => c.Name == color.Name);
				if (existingColor != null && existingColor.Id != color.Id)
				{
					ModelState.AddModelError("Name", "This update name already exists.");
					return View(color);
				}
				_unitOfWork.Color.Update(color);
                _unitOfWork.Save();
				TempData["success"] = "Color updated successfully!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return View(color);
		}

		[Route("color/delete/{id}")]
		[HttpGet, ActionName("Delete")]
		public IActionResult DeleteGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Color color = _unitOfWork.Color.Get(u => u.Id == id);
			if (color == null)
			{
				return NotFound();
			}
			return View(color);
		}

		[Route("color/delete/{id}")]
		[HttpPost, ActionName("Delete")]
		public IActionResult DeleteDELETE(int? id)
		{
			Color color = _unitOfWork.Color.Get(u => u.Id == id);
			if (color == null)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				_unitOfWork.Color.Remove(color);
				_unitOfWork.Save();
				TempData["success"] = "Color deleted succcesfully!";
				return RedirectToAction("Index");
			}
			return View();
		}
	}
}
