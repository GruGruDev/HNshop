using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using Suitshop.Utility;

namespace Suitshop.Areas.Customer.Controllers
{
	[Area("Customer")]

	public class CategoryController : Controller
	{
		public readonly IUnitOfWork _unitOfWork;
		public int PageSize = SD.PageSize;

		public CategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("/{urlName}")]
		public async Task<IActionResult> Index(string urlName, string? subCat, string? price, string? color, string? sort, int? pageNumber)
		{
			if (string.IsNullOrEmpty(urlName))
			{
				return NotFound();
			}

			ViewData["urlName"] = urlName != null ? urlName : "";
			ViewData["subCat"] = subCat != null ? subCat : "";
			ViewData["price"] = price != null ? price : "";
			ViewData["color"] = color != null ? color : "";
			ViewData["sort"] = sort != null ? sort : "";

			CustomerCategoryVM vm = new CustomerCategoryVM();

			vm.Category = _unitOfWork.Category.Get(x => x.UrlName == urlName);

			if (vm.Category == null)
			{
				return NotFound();
			}

			return View(await LoadAsync(vm, urlName, subCat, price, color, sort, pageNumber));
		}

		[Route("/{urlName}")]
		[HttpPost, ActionName("Index")]
		public async Task<IActionResult> IndexPOST(string urlName, string? subCat, string? price, string? color, string? sort, int? pageNumber)
		{
			if (string.IsNullOrEmpty(urlName))
			{
				return NotFound();
			}

			ViewData["urlName"] = urlName != null ? urlName : "";
			ViewData["subCat"] = subCat != null ? subCat : "";
			ViewData["price"] = price != null ? price : "";
			ViewData["color"] = color != null ? color : "";
			ViewData["sort"] = sort != null ? sort : "";

			CustomerCategoryVM vm = new CustomerCategoryVM();

			vm.Category = _unitOfWork.Category.Get(x => x.UrlName == urlName);

			if(vm.Category == null)
			{
				return NotFound();
			}

			return View(await LoadAsync(vm, urlName, subCat, price, color, sort, pageNumber));
		}

		public async Task<CustomerCategoryVM> LoadAsync(CustomerCategoryVM vm, string urlName, string? subCat, string? price, string? color, string? sort, int? pageNumber)
		{

			var productFilter = _unitOfWork.Product.GetRange(u => u.SubCategory.Category.UrlName == urlName);
			if (subCat != null)
			{
				productFilter = productFilter.Where(u => u.SubCategory.Name == subCat);
				vm.SelectedSubCat = subCat;
			}

			if (price != null)
			{
				switch (price)
				{
					case "price1":
						productFilter = productFilter.Where(u => u.Price >= 0 && u.Price <= 1000000);
						vm.SelectedPrice = "price1";
						break;
					case "price2":
						productFilter = productFilter.Where(u => u.Price > 1000000 && u.Price <= 5000000);
						vm.SelectedPrice = "price2";
						break;
					case "price3":
						productFilter = productFilter.Where(u => u.Price > 5000000);
						vm.SelectedPrice = "price3";
						break;
				}
			}

			if (color != null)
			{
				productFilter = productFilter.Where(u => u.Color.Name == color);
				vm.SelectedColor = color;
			}

			if (sort != null)
			{
				switch (sort)
				{
					case "asc":
						productFilter = productFilter.OrderBy(u => u.Price);
						vm.SelectedSort = sort;
						break;
					case "desc":
						productFilter = productFilter.OrderByDescending(u => u.Price);
						vm.SelectedSort = sort;
						break;
					case "az":
						productFilter = productFilter.OrderBy(u => u.Name);
						vm.SelectedSort = sort;
						break;
					case "za":
						productFilter = productFilter.OrderByDescending(u => u.Name);
						vm.SelectedSort = sort;
						break;
				}
			}

			var filteredProducts = productFilter
					.Include(u => u.SubCategory)
					.Include(u => u.Images)
					.Include(u => u.Color)
					.Include(u => u.ProductDetails);

			var paginatedProduct = await PaginatedList<Product>.CreateAsync(filteredProducts, pageNumber ?? 1, PageSize);

			vm.Products = paginatedProduct;

			vm.SubCategories = _unitOfWork.SubCategory.GetRange(u => u.Category.UrlName == urlName).Include(u => u.Category).ToList();
			vm.ProductColors = _unitOfWork.Product.GetRange(u => u.SubCategory.Category.UrlName == urlName)
			.Include(u => u.Color)
			.ToList();
			

			return vm;
		}
	}
}
