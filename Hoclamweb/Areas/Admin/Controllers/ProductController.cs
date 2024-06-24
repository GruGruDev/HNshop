using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Microsoft.EntityFrameworkCore;
using Suitshop.Models.ViewModels;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using Image = Suitshop.Models.Image;
using Microsoft.AspNetCore.Authorization;
using Suitshop.Utility;

namespace Suitshop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHost;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
		{
			_unitOfWork = unitOfWork;
			_webHost = webHost;
		}

		[Route("product")]
		public IActionResult Index()
		{
			ProductVM productVM = new ProductVM();
			productVM.Products = _unitOfWork.Product.GetAll().Include(p => p.SubCategory).Include(p => p.Images).Include(p => p.Color).ToList();
			return View(productVM);
		}

		[Route("product/create")]
		[HttpGet, ActionName("Create")]
		public IActionResult CreateGET()
		{
			ProductVM productVM = new ProductVM()
			{
				ProductList = _unitOfWork.SubCategory.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				ColorList = _unitOfWork.Color.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				Product = new Product(),
			};
			return View(productVM);
		}
		[Route("product/create")]
		[HttpPost, ActionName("Create")]
		public IActionResult CreatePOST(ProductVM productVM, List<IFormFile>? files)
		{
			productVM.ProductList = _unitOfWork.SubCategory.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			productVM.ColorList = _unitOfWork.Color.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			if (ModelState.IsValid)
			{
				var existingProduct = _unitOfWork.Product.Get(c => c.Name == productVM.Product.Name);
				if (existingProduct != null && existingProduct.Id != productVM.Product.Id)
				{
					ModelState.AddModelError("Product.Name", "This name already exists.");
					return View(productVM);
				}

                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();

				if (productVM.Product.Id > 0)
				{
					productVM.Product.GenerateSlug();
					_unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
				}

				if (files != null && files.Count > 0)
				{
					//root path
					string wwwRootPath = _webHost.WebRootPath;
					string productPath = Path.Combine(wwwRootPath, @"image\product");

					var proId = _unitOfWork.Product.Get(u => u.Name == productVM.Product.Name);

					//add image to db, root
					foreach (var file in files)
					{
						if (file != null && file.Length > 0)
						{
							string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
							using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
							{
								file.CopyTo(fileStream);
							}

							Image image = new Image();
							image.ImageUrl = @"\image\product\" + fileName;

							if (proId != null)
							{
								image.ProductId = proId.Id;
                                _unitOfWork.Image.Add(image);
							}
							else
							{
								TempData["error"] = "Error!";
								return View(productVM);
							}
						}
					}
                    _unitOfWork.Save();
				}
				TempData["success"] = "Product created succcesfully!";
				return RedirectToAction("Index");
			}
			TempData["error"] = "Error!";
			return View(productVM);
		}

		[Route("product/edit/{id}")]
		[HttpGet, ActionName("Edit")]
		public IActionResult EditGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var images = _unitOfWork.Image.GetRange(u => u.ProductId == id);
			ProductVM productVM = new ProductVM()
			{
				ProductList = _unitOfWork.SubCategory.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				ColorList = _unitOfWork.Color.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				Product = _unitOfWork.Product.Get(u => u.Id == id),
				Images= _unitOfWork.Image.GetRange(u=>u.ProductId == id).ToList(),
			};
			if (productVM.Product == null)
			{
				return NotFound();
			}
			return View(productVM);
		}
		[Route("product/edit/{id}")]
		[HttpPost, ActionName("Edit")]
		public IActionResult EditPOST(ProductVM productVM, List<IFormFile>? files)
		{
			productVM.ProductList = _unitOfWork.SubCategory.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			productVM.ColorList = _unitOfWork.Color.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString(),
			});
			if (ModelState.IsValid)
			{
				var existingProduct = _unitOfWork.Product.Get(c => c.Name == productVM.Product.Name);
				if (existingProduct != null && existingProduct.Id != productVM.Product.Id)
				{
					ModelState.AddModelError("Product.Name", "This update name already exists.");
					return View(productVM);
				}
				if (files != null && files.Count>0)
				{
					string wwwRootPath = _webHost.WebRootPath;
					string productPath = Path.Combine(wwwRootPath, @"image\product");

					//Xóa ảnh cũ trong root
					var deleteImages = _unitOfWork.Image.GetRange(u => u.ProductId == productVM.Product.Id);

					foreach (var image in deleteImages)
					{
						string filePath = Path.Combine(wwwRootPath, image.ImageUrl.TrimStart('\\'));
						if (System.IO.File.Exists(filePath))
						{
							System.IO.File.Delete(filePath);
						}
					}
                    //Xóa ảnh cũ trong db
                    _unitOfWork.Image.RemoveRange(deleteImages);

					//Thêm ảnh mới vào root, db
					foreach (var file in files)
					{
						if (file != null && file.Length > 0)
						{
							string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

							using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
							{
								file.CopyTo(fileStream);
							}
							Image image = new Image();
							image.ImageUrl = @"\image\product\" + fileName;
							image.ProductId = productVM.Product.Id;
							_unitOfWork.Image.Add(image);
						}
					}
				}
				productVM.Product.GenerateSlug();
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();

				productVM.Product = _unitOfWork.Product.Get(u => u.Id == productVM.Product.Id);
				productVM.Images = _unitOfWork.Image.GetRange(u => u.ProductId == productVM.Product.Id).ToList();

				TempData["success"] = "Product updated succcesfully!";
				return View(productVM);
			}
			TempData["error"] = "Error!";
			return View(productVM);
		}

		[Route("product/delete/{id}")]
		[HttpGet, ActionName("Delete")]
		public IActionResult DeleteGET(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var images = _unitOfWork.Image.GetRange(u => u.ProductId == id);
			ProductVM productVM = new ProductVM()
			{
				ProductList = _unitOfWork.SubCategory.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				ColorList = _unitOfWork.Color.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				Product = _unitOfWork.Product.Get(u => u.Id == id),
				Images = _unitOfWork.Image.GetRange(u => u.ProductId == id).ToList(),
			};
			if (productVM.Product == null)
			{
				TempData["error"] = "This product not exists!";
				return RedirectToAction("Index");
			}
			return View(productVM);
		}

		[Route("product/delete/{id}")]
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			Product product = _unitOfWork.Product.Get(u => u.Id == id);
			if (product == null)
			{
				TempData["success"] = "This product not exists!";
				return RedirectToAction("Index");
			}
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHost.WebRootPath;

				//Xóa ảnh cũ trong root
				var deleteImages = _unitOfWork.Image.GetRange(u => u.ProductId == id);

				foreach (var image in deleteImages)
				{
					string filePath = Path.Combine(wwwRootPath, image.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(filePath))
					{
						System.IO.File.Delete(filePath);
					}
				}
                _unitOfWork.Product.Remove(product);
                _unitOfWork.Save();
				TempData["success"] = "Product deleted succcesfully!";
				return RedirectToAction("Index");
			}
			return View();
		}
	}
}
