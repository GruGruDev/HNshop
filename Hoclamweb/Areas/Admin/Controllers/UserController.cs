using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Microsoft.AspNetCore.Mvc;
using Suitshop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Suitshop.Utility;
using Microsoft.EntityFrameworkCore;

namespace Suitshop.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _db;
		public UserController(ApplicationDbContext db)
		{
			_db = db;
		}

		[Route("user")]
		public IActionResult Index()
		{
			List<ApplicationUser> users = _db.ApplicationUsers.Include(x => x.Reviews).Include(x => x.Orders).ToList();

			var userRoles = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();

			foreach (var user in users)
			{
				var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
			}

			return View(users);
		}


		[HttpPost]
		public IActionResult LockUnlock(string id)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(x => x.Id == id);

			if (user == null)
			{
				TempData["error"] = "Không thể khóa hay mở khóa.";
				return RedirectToAction("Index");
			}
			if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
			{
				//user dang bi khoa => mo khoa
				user.LockoutEnd = DateTime.Now;
			}
			else
			{
				//khoa
				user.LockoutEnd = DateTime.Now.AddDays(7);
			}
			_db.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}
