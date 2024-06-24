using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suitshop.DataAccess.Repository.IRepository;
using Suitshop.Models;
using Suitshop.Models.ViewModels;
using Suitshop.Utility;

namespace Suitshop.ViewComponents
{
	public class CategoriesAndSubCategoriesViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;
		public CategoriesAndSubCategoriesViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			CatsAndSubCatsVCVM vm = new();

			vm.ShirtsSubCategories = await _unitOfWork.SubCategory
				.GetRange(x => x.Category.Name == SD.Readytowear)
				.Where(x => x.Name.StartsWith("Áo"))
				.Include(x => x.Category)
				.ToListAsync();
			vm.PantsSubCategories = await _unitOfWork.SubCategory
				.GetRange(x => x.Category.Name == SD.Readytowear)
				.Where(x => x.Name.StartsWith("Quần"))
				.Include(x => x.Category)
				.ToListAsync();
			vm.AccessoriesSubCategories = await _unitOfWork.SubCategory
				.GetRange(x => x.Category.Name == SD.Accessories)
				.Where(x => x.Name.StartsWith("Mũ") || x.Name.StartsWith("Thắt"))
				.Include(x => x.Category)
				.ToListAsync();
			vm.BriefcasesSubCategories = await _unitOfWork.SubCategory
				.GetRange(x => x.Category.Name == SD.Accessories)
				.Where(x => x.Name.StartsWith("Balo") || x.Name.StartsWith("Ví"))
				.Include(x => x.Category)
				.ToListAsync();
			vm.ShoesSubCategories = await _unitOfWork.SubCategory
				.GetToInclude(x => x.Category.Name == SD.Shoes)
				.Include(x => x.Category)
				.FirstAsync();

			if(vm.ShoesSubCategories != null && vm.PantsSubCategories!=null && vm.ShoesSubCategories != null)
			{
				return View(vm);
			}

			return View("Error");
		}
	}
}
