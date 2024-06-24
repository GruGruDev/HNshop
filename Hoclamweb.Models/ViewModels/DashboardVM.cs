using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
	public class DashboardVM
	{
		public int Categories { get; set; }
		public int SubCategories { get; set; }
		public int Products { get; set; }
		public int Orders { get; set; }
		public int Users { get; set; }
	}
}
