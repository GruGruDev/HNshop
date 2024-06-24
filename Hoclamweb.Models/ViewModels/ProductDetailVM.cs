using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
	public class ProductDetailVM
	{
		public ProductDetail ProductDetail { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem>? ProductList { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem>? SizeList { get; set; }
		
	}
}
