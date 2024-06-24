using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Suitshop.Utility;

namespace Suitshop.Models
{
	public class SubCategory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Required]
		[MaxLength(100)]
		[DisplayName("Category Detail Name")]
		public string Name { get; set; }
		[Required]
		[DisplayName("Url Name")]
		public string UrlName { get; set; }

		[DisplayName("Category Detail Description")]
		[MaxLength(500)]
		public string? Description { get; set; }
		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }

		[Required]
		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
		[ValidateNever]
		public Category Category { get; set; }
		[ValidateNever]
		public ICollection<Product>? Products { get; set; }
	}
}
