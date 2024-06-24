using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Suitshop.Utility;

namespace Suitshop.Models
{
	public class Product
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }
		[Required]
		public int Gender { get; set; }
		[Required]
		public double Price { get; set; }
		public double Saleoff { get; set; }
		public string? Slug { get; set; }
		public void GenerateSlug()
		{
			Slug = SD.CreateSlug(Name, Id);
		}
		[MaxLength(500)]
		public string? Description { get; set; }
		
		public DateTime CreateTime { get; set; } = DateTime.Now;
		public DateTime UpdateTime { get; set; }

		[Required]
		public int SubCategoryId { get; set; }
		[ForeignKey("SubCategoryId")]
		[ValidateNever]
		public SubCategory SubCategory { get; set; }

		[Required]
		public int ColorId { get; set; }
		[ForeignKey("ColorId")]
		[ValidateNever]
		public Color Color { get; set; }

		[ValidateNever]
		public ICollection<ProductDetail>? ProductDetails { get; set; }
		[ValidateNever]
		public ICollection<Image>? Images { get; set; }
	}
}
