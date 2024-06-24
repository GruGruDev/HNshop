using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
    public class ProductDetailCustomerVM
    {
        public Product Product { get; set; }
		public SubCategory SubCategory { get; set; }
		public List<ProductDetail> ProductDes { get; set; }
		public List<Review> Reviews { get; set; }
		public string? sizeChecked { get; set; }
	}
}
