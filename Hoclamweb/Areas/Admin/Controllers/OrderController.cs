using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Microsoft.AspNetCore.Mvc;
using Suitshop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Suitshop.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using Suitshop.DataAccess.Repository;

namespace Suitshop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[Route("order")]
		public IActionResult Index()
		{
			OrderVM vm = new OrderVM();
			vm.Orders = _unitOfWork.Order.GetAll().ToList();
			vm.RadioSelected = "all";
			return View(vm);
		}

		[Route("order")]
		[HttpPost, ActionName("Index")]
		public IActionResult IndexPOST(string orderStatus)
		{
			OrderVM vm = new OrderVM();

			if (orderStatus == "all")
			{
				vm.Orders = _unitOfWork.Order.GetAll().ToList();
				vm.RadioSelected = "all";
				return View(vm);
			}

			vm.Orders = _unitOfWork.Order.GetRange(u => u.OrderStatus == orderStatus).ToList();
			vm.RadioSelected = orderStatus;
			return View(vm);
		}

		[Route("order/edit/{id}")]
		public IActionResult Edit(int id)
		{
			OrderVM vm = new OrderVM();
			vm.Order = _unitOfWork.Order.GetToInclude(x => x.Id == id).Include(x => x.Items).FirstOrDefault();
			vm.Items = _unitOfWork.Item.GetToInclude(x => x.OrderId == id)
				.Include(x => x.ProductDetail.Product.Images)
				.Include(x => x.ProductDetail.Size)
				.ToList();
			return View(vm);
		}

		public IActionResult Confirm()
		{
			var order = _unitOfWork.Order.Get(x => x.Id == OrderVM.Order.Id);

			if (order == null)
			{
				return NotFound();
			}

			order.OrderStatus = SD.Order_WaitForShip;
			_unitOfWork.Order.Update(order);
			_unitOfWork.Save();

			TempData["success"] = $"Xác nhận đơn hàng #{order.Id} thành công !";

			return RedirectToAction("Edit", new { id = order.Id });
		}

		public IActionResult Ship()
		{
			var order = _unitOfWork.Order.Get(x => x.Id == OrderVM.Order.Id);

			if (order == null)
			{
				return NotFound();
			}
			_unitOfWork.Order.UpdateStatus(order.Id, SD.Order_Shipped);
			_unitOfWork.Save();

			TempData["success"] = $"Đơn hàng #{order.Id} đã giao hàng!";

			return RedirectToAction("Edit", new { id = order.Id });
		}

		public IActionResult Cancel()
		{
			var order = _unitOfWork.Order.Get(x => x.Id == OrderVM.Order.Id);

			if (order == null)
			{
				return NotFound();
			}

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

			TempData["success"] = $"Đơn hàng #{order.Id} đã hủy và hoàn tiền!";

			return RedirectToAction("Edit", new { id = order.Id });
		}

		public IActionResult UpdateInfo()
		{
			var existingOrder = _unitOfWork.Order.Get(x => x.Id == OrderVM.Order.Id);

			existingOrder.Name = OrderVM.Order.Name;
			existingOrder.PhoneNumber = OrderVM.Order.PhoneNumber;
			existingOrder.StreetAddress = OrderVM.Order.StreetAddress;
			existingOrder.City = OrderVM.Order.City;
			existingOrder.PostalCode = OrderVM.Order.PostalCode;

			if (!string.IsNullOrEmpty(OrderVM.Order.TrackingNumber))
			{
				existingOrder.TrackingNumber = OrderVM.Order.TrackingNumber;
			}
			if (!string.IsNullOrEmpty(OrderVM.Order.Carrier))
			{
				existingOrder.Carrier = OrderVM.Order.Carrier;
			}

			_unitOfWork.Order.Update(existingOrder);
			_unitOfWork.Save();

			TempData["success"] = "Order updated successfully!";
			return RedirectToAction("Edit", new { id = existingOrder.Id });
		}
	}
}
