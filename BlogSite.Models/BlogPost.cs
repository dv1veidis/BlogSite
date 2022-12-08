using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        [ValidateNever]
        public int CategoryId { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Category Category { get; set; }
    }
}
