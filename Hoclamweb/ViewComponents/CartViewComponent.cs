using Microsoft.AspNetCore.Mvc;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Utility;
using System.Security.Claims;

namespace Suitshop.ViewComponents
{
	public class CartViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;
		public CartViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			int count = 0;

			if (claim != null)
			{
				//if (HttpContext.Session.GetInt32(SD.Session_Cart) != null)
				//{
				//	return View(HttpContext.Session.GetInt32(SD.Session_Cart));
				//}
				//HttpContext.Session.SetInt32(SD.Session_Cart, _unitOfWork.ShoppingCart.GetRange(x => x.ApplicationUserId == claim.Value).Count());
				//return View(HttpContext.Session.GetInt32(SD.Session_Cart));

				count = _unitOfWork.ShoppingCart.GetRange(x => x.ApplicationUserId == claim.Value).Count();
				return View(count);
			}
			else
			{
				//HttpContext.Session.Clear();
				return View(count);
			}
		}
	}

}
