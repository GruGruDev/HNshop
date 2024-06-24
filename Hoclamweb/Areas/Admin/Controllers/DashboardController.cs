using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models.ViewModels;
using Suitshop.Utility;

namespace Suitshop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class DashboardController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public DashboardController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("dashboard")]
		public IActionResult Index()
		{
			DashboardVM vm = new DashboardVM()
			{
				Categories = _unitOfWork.Category.GetAll().Count(),
				SubCategories = _unitOfWork.SubCategory.GetAll().Count(),
				Products = _unitOfWork.Product.GetAll().Count(),
				Orders = _unitOfWork.Order.GetAll().Count(),
				Users = _unitOfWork.ApplicationUser.GetAll().Count(),
			};

			return View(vm);
		}
	}
}
