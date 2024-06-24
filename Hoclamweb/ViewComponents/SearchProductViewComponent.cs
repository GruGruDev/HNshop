using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;

namespace Suitshop.ViewComponents
{
	public class SearchProductViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;
		public SearchProductViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<Product> products = _unitOfWork.Product.GetAll().Include(x => x.SubCategory).Include(x => x.Images).Include(x => x.Color).ToList();
			return View(products);
		}
	}
}
