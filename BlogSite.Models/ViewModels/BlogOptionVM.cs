using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSite.Models.ViewModels
{
    public class BlogOptionVM
    {
        public IEnumerable<BlogPost> BlogPost { get; set; }
        [ValidateNever]
        public int? CategoryListId { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        public string? ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public string? UserName { get; set; }
        [ValidateNever]
        public string? Action { get; set; }
    }
}
