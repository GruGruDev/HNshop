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
	public class ProductDetailController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public ProductDetailController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("productDetail")]
		public IActionResult Index()
		{
			List<ProductDetail> pd = _unitOfWork.ProductDetail.GetAll().Include(pd => pd.Product.Color).Include(pd => pd.Size).ToList();
			return View(pd);
		}

		[Route("productDetail/create")]
		[HttpGet, ActionName("Create")]
		public IActionResult CreateGET()
		{
			ProductDetailVM vm = new ProductDetailVM()
			{
				ProductList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				
				SizeList = _unitOfWork.Size.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
			};
			return View(vm);
		}

		[Route("productDetail/create")]
		[HttpPost, ActionName("Create")]
		public IActionResult CreatePOST(ProductDetailVM vm)
		{
			vm.ProductList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			vm.SizeList = _unitOfWork.Size.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			if (ModelState.IsValid)
			{
				var existingProDe = _unitOfWork.ProductDetail.Get(c => c.ProductId == vm.ProductDetail.ProductId && c.SizeId == vm.ProductDetail.SizeId);
				if (existingProDe != null && existingProDe.Id != vm.ProductDetail.Id)
				{
					ModelState.AddModelError("ProductDetail.ProductId", "This Product Detail with size already exists.");
					ModelState.AddModelError("ProductDetail.SizeId", "This Product Detail with size already exists.");
					return View(vm);
				}

                _unitOfWork.ProductDetail.Add(vm.ProductDetail);
                _unitOfWork.Save();
				TempData["success"] = "Product Detail created succcesfully!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return View(vm);
		}

		[Route("productDetail/edit/{id}")]
		[HttpGet, ActionName("Edit")]
		public IActionResult EditGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			ProductDetailVM vm = new ProductDetailVM()
			{
				ProductDetail = _unitOfWork.ProductDetail.Get(p => p.Id == id),
				ProductList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				SizeList = _unitOfWork.Size.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
			};
			if (vm.ProductDetail == null)
			{
				return NotFound();
			}
			return View(vm);
		}

		[Route("productDetail/edit/{id}")]
		[HttpPost, ActionName("Edit")]
		public IActionResult EditPOST(ProductDetailVM vm)
		{
			vm.ProductList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			vm.SizeList = _unitOfWork.Size.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			if (ModelState.IsValid)
			{
				var existingProDe = _unitOfWork.ProductDetail.Get(c => c.ProductId == vm.ProductDetail.ProductId && c.SizeId == vm.ProductDetail.SizeId);
				if (existingProDe != null && existingProDe.Id != vm.ProductDetail.Id)
				{
					ModelState.AddModelError("ProductDetail.ProductId", "This Product Detail with size already exists.");
					ModelState.AddModelError("ProductDetail.SizeId", "This Product Detail with size already exists.");
					return View(vm);
				}

                _unitOfWork.ProductDetail.Update(vm.ProductDetail);
                _unitOfWork.Save();
				TempData["success"] = "Product Detail updated succcesfully!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return View(vm);
		}

		[Route("productDetail/delete/{id}")]
		[HttpGet, ActionName("Delete")]
		public IActionResult DeleteGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			ProductDetailVM vm = new ProductDetailVM()
			{
				ProductDetail = _unitOfWork.ProductDetail.Get(p => p.Id == id),
				ProductList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				SizeList = _unitOfWork.Size.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
			};
			if (vm.ProductDetail == null)
			{
				return NotFound();
			}
			return View(vm);
		}

		[Route("productDetail/delete/{id}")]
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			ProductDetail proDe = _unitOfWork.ProductDetail.Get(u => u.Id == id);
			if (proDe == null)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
                _unitOfWork.ProductDetail.Remove(proDe);
                _unitOfWork.Save();
				TempData["success"] = "Product Detail deleted succcesfully!";
				return RedirectToAction("Index");
			}
			return View();
		}
	}
}
