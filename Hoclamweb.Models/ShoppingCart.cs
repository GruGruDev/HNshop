using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Suitshop.Models.ViewModels;

namespace Suitshop.Models
{
	public class ShoppingCart
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[Range(1,100)]
		public int Quantity { get; set; }
		public DateTime CreateTime { get; set; } = DateTime.Now;

		[Required]
		public int? ProductDetailId { get; set; }
		[ForeignKey("ProductDetailId")]
		[ValidateNever]
		public ProductDetail ProductDetail { get; set; }

		[Required]
		public string? ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		[ValidateNever]
		public ICollection<Item>? Items { get; set; }
	}
}
