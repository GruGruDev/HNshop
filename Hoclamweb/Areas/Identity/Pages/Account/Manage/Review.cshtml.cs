using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;

namespace Suitshop.Areas.Identity.Pages.Account.Manage
{
	public class ReviewModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUnitOfWork _unitOfWork;

		public ReviewModel(
			UserManager<ApplicationUser> userManager,
			IUnitOfWork unitOfWork)
		{
			_userManager = userManager;
			_unitOfWork = unitOfWork;
		}

		[TempData]
		public string? StatusMessage { get; set; }
		public int ItemId { get; set; }
		public int ProductId { get; set; }

		public void OnGet(int itemId, int productId)
		{
			if (itemId != 0)
			{
				ItemId = itemId;
			}
			if (productId != 0)
			{
				ProductId = productId;
			}
		}

		public async Task<IActionResult> OnPost(int itemId, int rating, string title, string des, int productId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var item = _unitOfWork.Item.Find(itemId);

			if (item == null)
			{
				return NotFound($"Không tìm thấy item với ID '{itemId}'.");
			}

			item.isReview = true;
			_unitOfWork.Item.Update(item);

			Review rv = new Review();
			rv.Title = title;
			rv.Rating = rating;
			rv.Description = des;
			rv.ProductId = productId;
			rv.ApplicationUserId = user.Id;

			_unitOfWork.Review.Add(rv);
			_unitOfWork.Save();

			TempData["success"] = "Bạn đã đánh giá thành công.";

			return RedirectToPage("/Account/Manage/Shipped");
		}
	}
}
