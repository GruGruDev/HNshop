using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitshop.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]

        public string? Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
		public DateTime UpdateTime { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }

        [NotMapped]
        public string? Role { get; set; }

	}
}
