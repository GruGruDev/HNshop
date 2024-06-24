using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using System.Security.Claims;

namespace Suitshop.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class ProductController : Controller
	{
		public readonly IUnitOfWork _unitOfWork;

		public ProductController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("{subcategory}/product/{slug}")]
		public async Task<IActionResult> ProductDetail(string subcategory, string slug)
		{
			if (string.IsNullOrEmpty(slug))
			{
				return NotFound();
			}

			var product = _unitOfWork.Product.GetRange(u => u.Slug == slug).Include(u => u.Images).Include(u => u.ProductDetails).FirstOrDefault();

			if (product == null)
			{
				return NotFound();
			}

			ProductDetailCustomerVM vm = new ProductDetailCustomerVM()
			{
				Product = product,
				ProductDes = await _unitOfWork.ProductDetail.GetRange(u => u.Product.Slug == slug).Include(u => u.Size).ToListAsync(),
				Reviews = await _unitOfWork.Review.GetRange(x => x.ProductId == product.Id).Include(x => x.ApplicationUser).ToListAsync()
			};

			return View(vm);
		}

		[HttpPost]
		[Authorize]
		[Route("{subcategory}/product/{slug}")]
		[ActionName("ProductDetail")]
		public async Task<IActionResult> ProductDetailPOST(string subcategory, string slug, string size, int count)
		{
			//kiem tra product co ton tai
			var product = _unitOfWork.Product.GetRange(u => u.Slug == slug).Include(u => u.Images).FirstOrDefault();

			if (product == null)
			{
				return NotFound();
			}
			//kiem tra product detail co ton tai
			var productDe = _unitOfWork.ProductDetail.Get(u => u.ProductId == product.Id && u.Size.Name == size);

			if (productDe == null)
			{
				return NotFound();
			}

			ProductDetailCustomerVM vm = new ProductDetailCustomerVM()
			{
				Product = product,
				ProductDes = await _unitOfWork.ProductDetail.GetRange(u => u.Product.Slug == slug).Include(u => u.Size).ToListAsync(),
				Reviews = await _unitOfWork.Review.GetRange(x => x.ProductId == product.Id).Include(x => x.ApplicationUser).ToListAsync()
			};

			//kiem tra so luong
			if (productDe.Quantity < count)
			{
				vm.sizeChecked = size;
				ViewData["quantityError"] = "Số lượng sản phẩm không đủ.";
				return View(vm);
			}

			//lay id user
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCart sc = new()
			{
				ProductDetailId = productDe.Id,
				ApplicationUserId = userId,
				Quantity = count,
			};

			//kiem tra da ton tai san pham trong cart chua
			ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ProductDetailId == productDe.Id && u.ApplicationUserId == userId);

			if (cartFromDb == null)
			{
				_unitOfWork.ShoppingCart.Add(sc);
			}
			else
			{
				cartFromDb.Quantity += count;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
			}

			_unitOfWork.Save();

			TempData["success"] = "Thêm vào giỏ hàng thành công.";

			return View(vm);
		}
	}
}
