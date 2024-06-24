using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models.ViewModels
{
	public class CatsAndSubCatsVCVM
	{
		public List<SubCategory> ShirtsSubCategories { get; set; }
		public List<SubCategory> PantsSubCategories { get; set; }
		public List<SubCategory> AccessoriesSubCategories { get; set; }
		public List<SubCategory> BriefcasesSubCategories { get; set; }
		public SubCategory ShoesSubCategories { get; set; }
	}
}
