using Suitshop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
    public class CustomerCategoryVM
    {
        public PaginatedList<Product> Products { get; set; }
        public List<ProductDetail> ProductDes { get; set; }
        public List<SubCategory> SubCategories { get; set; }
		public List<Product> ProductColors { get; set; }
		public Category Category { get; set; }

		public string? SelectedSubCat { get; set; }
		public string? SelectedPrice { get; set; }
		public string? SelectedColor { get; set; }
		public string? SelectedSort { get; set; }
	}
}
