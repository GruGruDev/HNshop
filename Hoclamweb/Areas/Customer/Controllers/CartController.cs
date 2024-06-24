using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using Suitshop.Utility;
using System.Security.Claims;

namespace Suitshop.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		public readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("Cart")]
		public IActionResult Index()
		{
			//lay id user
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new ShoppingCartVM();

			ShoppingCartVM.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll()
				.Include(u => u.ProductDetail.Product.SubCategory)
				.Include(u => u.ProductDetail.Product.Images)
				.Include(u => u.ProductDetail.Size)
				.Include(u => u.ApplicationUser)
				.ToList();

			ShoppingCartVM.Order = new();

			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				ShoppingCartVM.Order.Total += item.Quantity * (item.ProductDetail.Product.Price * (1 - item.ProductDetail.Product.Saleoff));
			}

			return View(ShoppingCartVM);
		}

		[HttpPost]
		public IActionResult ChangeInput(int id, int count)
		{
			if (count > 0)
			{
				ShoppingCart scFromDb = _unitOfWork.ShoppingCart.Find(id);
				if (scFromDb != null)
				{
					if (scFromDb.Quantity > 0)
					{
						scFromDb.Quantity = count;
						_unitOfWork.ShoppingCart.Update(scFromDb);
						_unitOfWork.Save();
						return RedirectToAction("Index");
					}
				}

				TempData["success"] = "Sản phẩm không tìm thấy";
				return RedirectToAction("Index");
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Add(int id)
		{
			ShoppingCart scFromDb = _unitOfWork.ShoppingCart.Find(id);
			if (scFromDb != null)
			{
				if (scFromDb.Quantity > 0)
				{
					scFromDb.Quantity += 1;
					_unitOfWork.ShoppingCart.Update(scFromDb);
					_unitOfWork.Save();
					return RedirectToAction("Index");
				}
			}

			TempData["success"] = "Sản phẩm không tìm thấy";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult Minus(int id)
		{
			ShoppingCart scFromDb = _unitOfWork.ShoppingCart.Find(id);
			if (scFromDb != null)
			{
				if (scFromDb.Quantity > 1)
				{
					scFromDb.Quantity -= 1;
					_unitOfWork.ShoppingCart.Update(scFromDb);
					_unitOfWork.Save();
					return RedirectToAction("Index");
				}

				//Xoa san pham neu count = 0
				TempData["success"] = "Xóa sản phẩm thành công";
				_unitOfWork.ShoppingCart.Remove(scFromDb);
				_unitOfWork.Save();
				return RedirectToAction("Index");
			}

			TempData["success"] = "Sản phẩm không tìm thấy";
			return RedirectToAction("Index");
		}


		[HttpPost]
		public IActionResult Delete(int id)
		{
			ShoppingCart scFromDb = _unitOfWork.ShoppingCart.Find(id);
			if (scFromDb != null)
			{
				_unitOfWork.ShoppingCart.Remove(scFromDb);
				_unitOfWork.Save();
				TempData["success"] = "Xóa sản phẩm thành công";
				return RedirectToAction("Index");
			}

			TempData["success"] = "Sản phẩm không tìm thấy";
			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("/Payment")]
		public IActionResult Payment()
		{
			//lay id user
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new ShoppingCartVM();

			ShoppingCartVM.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll().Include(u => u.ProductDetail.Product.Images).Include(u => u.ProductDetail.Size).Include(u => u.ApplicationUser).ToList();
			ShoppingCartVM.Order = new();
			ShoppingCartVM.Order.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			ShoppingCartVM.Order.Name = ShoppingCartVM.Order.ApplicationUser.Name;
			ShoppingCartVM.Order.StreetAddress = ShoppingCartVM.Order.ApplicationUser.StreetAddress;
			ShoppingCartVM.Order.PhoneNumber = ShoppingCartVM.Order.ApplicationUser.PhoneNumber;
			ShoppingCartVM.Order.City = ShoppingCartVM.Order.ApplicationUser.City;
			ShoppingCartVM.Order.PostalCode = ShoppingCartVM.Order.ApplicationUser.PostalCode;

			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				ShoppingCartVM.Order.Total += item.Quantity * (item.ProductDetail.Product.Price * (1 - item.ProductDetail.Product.Saleoff));
			}

			return View(ShoppingCartVM);
		}

		[HttpPost]
		[Route("/Payment")]
		public IActionResult PaymentPOST()
		{
			//lay id user
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll().Include(u => u.ProductDetail.Product.Images).Include(u => u.ProductDetail.Size).Include(u => u.ApplicationUser).ToList();

			//ShoppingCartVM.Order.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
			ShoppingCartVM.Order.ApplicationUserId = userId;
			ShoppingCartVM.Order.OrderDate = DateTime.Now;
			ShoppingCartVM.Order.OrderStatus = SD.Order_WaitForConfirmation;
			ShoppingCartVM.Order.PaymentStatus = SD.Payment_Paid;

			foreach (var order in ShoppingCartVM.ShoppingCarts)
			{
				ShoppingCartVM.Order.Total += order.Quantity * (order.ProductDetail.Product.Price * (1 - order.ProductDetail.Product.Saleoff));
			}

			_unitOfWork.Order.Add(ShoppingCartVM.Order);
			_unitOfWork.Save();

			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				Item i = new()
				{
					Quantity = item.Quantity,
					Price = item.ProductDetail.Product.Price * (1 - item.ProductDetail.Product.Saleoff),
					CreateTime = DateTime.Now,
					ProductDetailId = (int)item.ProductDetailId,
					OrderId = ShoppingCartVM.Order.Id,
				};
				_unitOfWork.Item.Add(i);
				_unitOfWork.Save();
			}

			/////Stripe payment
			var domain = "https://localhost:5000/";
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"Order-Confirmation?id={ShoppingCartVM.Order.Id}",
				CancelUrl = domain + "cart",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var cart in ShoppingCartVM.ShoppingCarts)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(cart.ProductDetail.Product.Price),
						Currency = "vnd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = cart.ProductDetail.Product.Name,
						}
					},
					Quantity = cart.Quantity
				};
				options.LineItems.Add(sessionLineItem);
			};

			var service = new SessionService();
			Session session = service.Create(options);

			//Lưu paymentIntentId vào Order
			_unitOfWork.Order.UpdateStripePaymentId(ShoppingCartVM.Order.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

			//return RedirectToAction("OrderConfirmation", new { id = ShoppingCartVM.Order.Id });
		}

		[Route("/Order-Confirmation")]
		public IActionResult OrderConfirmation(int id)
		{
			Order order = _unitOfWork.Order.GetToInclude(x => x.Id == id)
			.Include(x => x.ApplicationUser)
			.FirstOrDefault();

			if (order != null)
			{
				var service = new SessionService();
				Session session = service.Get(order.SessionId);
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.Order.UpdateStripePaymentId(order.Id, session.Id, session.PaymentIntentId);
					_unitOfWork.Order.UpdateStatus(order.Id, SD.Order_WaitForConfirmation, SD.Payment_Paid);
					_unitOfWork.Save();
				}

				List<ShoppingCart> carts = _unitOfWork.ShoppingCart
					.GetRange(x => x.ApplicationUserId == order.ApplicationUserId)
					.ToList();

				_unitOfWork.ShoppingCart.RemoveRange(carts);
				_unitOfWork.Save();

				return View(id);
			}
			return View();
		}
	}
}
