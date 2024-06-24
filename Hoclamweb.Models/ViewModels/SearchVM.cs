using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
	public class SearchVM
	{
		public List<Product> SearchedProducts {  get; set; }
		public string Searched { get; set; }
	}
}
