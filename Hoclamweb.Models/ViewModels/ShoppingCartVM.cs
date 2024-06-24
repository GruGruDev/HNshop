using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public List<ShoppingCart> ShoppingCarts { get; set; }
		public Order Order { get; set; }
	}
}
