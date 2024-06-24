using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Suitshop.Utility;

namespace Suitshop.Models
{
    public class Category
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[DisplayName("Category Name")]
        public string Name { get; set; }
		[Required]
		[DisplayName("Url Name")]
		public string UrlName { get; set; }

		[DisplayName("Category Description")]
		[MaxLength(500)]
		public string? Description { get; set; }
		public DateTime CreateTime { get; set; } = DateTime.Now;
		public DateTime UpdateTime { get; set; }

		[ValidateNever]
		public ICollection<SubCategory>? SubCategories { get; set; }
	}
}
