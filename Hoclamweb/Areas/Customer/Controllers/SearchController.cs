using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models.ViewModels;

namespace Suitshop.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class SearchController : Controller
	{
		public readonly IUnitOfWork _unitOfWork;

		public SearchController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("Search")]
		public IActionResult Index()
		{
			SearchVM vm = new SearchVM();
			vm.SearchedProducts = _unitOfWork.Product.GetAll().Include(x => x.SubCategory).Include(x => x.Images).Include(x => x.Color).ToList();

			return View(vm);
		}

		[Route("Search")]
		[ActionName("Index")]
		[HttpPost]
		public IActionResult IndexPOST(string search)
		{
			SearchVM vm = new SearchVM();
			vm.SearchedProducts = _unitOfWork.Product.GetRange(x=>x.Name.Contains(search) || x.Color.Name.Contains(search)).Include(x => x.SubCategory).Include(x => x.Images).Include(x => x.Color).ToList();
			vm.Searched = search;

			return View(vm);
		}
	}
}
