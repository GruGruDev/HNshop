using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Suitshop.Models
{
	public class Item
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Required]
		public int Quantity { get; set; }
		[Required]
		public double Price { get; set; }
		public bool isReview { get; set; } = false;
		public DateTime CreateTime { get; set; } = DateTime.Now;

		[Required]
		public int ProductDetailId { get; set; }
		[ForeignKey("ProductDetailId")]
		[ValidateNever]
		public ProductDetail ProductDetail { get; set; }

		[Required]
		public int OrderId { get; set; }
		[ForeignKey("OrderId")]
		[ValidateNever]
		public Order Order { get; set; }
	}
}
