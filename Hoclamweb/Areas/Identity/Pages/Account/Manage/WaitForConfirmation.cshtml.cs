// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Utility;

namespace Suitshop.Areas.Identity.Pages.Account.Manage
{
    public class WaitForConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public WaitForConfirmationModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        //public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        //[BindProperty]
        public List<Order> Orders { get; set; }
        public List<Item> Items { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>

		private async Task LoadAsync(ApplicationUser user)
		{
			Orders = await _unitOfWork.Order.GetRange(x => x.ApplicationUserId == user.Id && x.OrderStatus == SD.Order_WaitForConfirmation)
				.ToListAsync();

			Items = await _unitOfWork.Item.GetRange(x => x.Order.ApplicationUserId == user.Id && x.Order.OrderStatus == SD.Order_WaitForConfirmation)
				.Include(x => x.ProductDetail.Product.Images)
				.Include(x => x.ProductDetail.Size)
				.Include(x => x.ProductDetail.Product.SubCategory)
				.ToListAsync();
		}

		public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (id == 0)
            {
                return NotFound();
            }

            Order order = _unitOfWork.Order.Get(x => x.Id == id);

			//hoàn tiền
			if (order.PaymentStatus == SD.Payment_Paid)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = order.PaymentIntentId,
				};

				var service = new RefundService();
				Refund refund = service.Create(options);
			}

			_unitOfWork.Order.UpdateStatus(order.Id, SD.Order_Canceled, SD.Payment_Refund);
			_unitOfWork.Save();

            TempData["success"] = $"Đã hủy đơn hàng số #{id}";

            return RedirectToPage();
        }
    }
}
