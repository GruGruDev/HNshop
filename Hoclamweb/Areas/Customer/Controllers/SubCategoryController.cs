using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using Suitshop.Utility;

namespace Suitshop.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class SubCategoryController : Controller
	{
        public readonly IUnitOfWork _unitOfWork;
		public int PageSize = SD.PageSize;

		public SubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		[Route("{category}/{urlName}")]
		public async Task<IActionResult> Index(string category, string urlName, string? price, string? color, string? sort, int? pageNumber)
		{
			if(category == null)
			{
				return NotFound();
			}

			ViewData["urlName"] = urlName;
			ViewData["category"] = category;
			ViewData["price"] = price;
			ViewData["color"] = color;
			ViewData["sort"] = sort;

			CustomerSubCategoryVM vm = new CustomerSubCategoryVM();

			vm.SubCategory = _unitOfWork.SubCategory.GetToInclude(u => u.UrlName == urlName).Include(x => x.Category).FirstOrDefault();

			if (vm.SubCategory == null)
			{
				return NotFound();
			}

			return View(await LoadAsync(vm, urlName, price, color, sort, pageNumber));
		}

		[Route("{category}/{urlName}")]
		[HttpPost, ActionName("Index")]
		public async Task<IActionResult> IndexPOST(string category, string urlName, string? price, string? color, string? sort, int? pageNumber)
		{
			if (category == null)
			{
				return NotFound();
			}

			ViewData["urlName"] = urlName;
			ViewData["category"] = category;
			ViewData["price"] = price;
			ViewData["color"] = color;
			ViewData["sort"] = sort;

			CustomerSubCategoryVM vm = new CustomerSubCategoryVM();

			vm.SubCategory = _unitOfWork.SubCategory.GetToInclude(u => u.UrlName == urlName).Include(x => x.Category).FirstOrDefault();

			if (vm.SubCategory == null)
			{
				return NotFound();
			}

			return View(await LoadAsync(vm, urlName, price, color, sort, pageNumber));
		}

		public async Task<CustomerSubCategoryVM> LoadAsync(CustomerSubCategoryVM vm, string urlName, string? price, string? color, string? sort, int? pageNumber)
		{
			vm.ProductColors = await _unitOfWork.Product.GetRange(u => u.SubCategory.UrlName == urlName)
				.Include(u => u.Color)
				.ToListAsync();

			IQueryable<Product> productFilter = _unitOfWork.Product.GetRange(u => u.SubCategory.UrlName == urlName);

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

			return vm;
		}
	}
}
