using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
	public class ProductVM
	{
		public Product Product { get; set; }
		public List<Product>? Products { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem>? ProductList { get; set; }
		public List<Image>? Images { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem>? ColorList { get; set; }
	}
}
