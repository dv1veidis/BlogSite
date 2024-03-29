﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class BlogVM
    {
        public BlogPost BlogPost { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        public string? ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public string? React { get; set; }

        public int? Id { get; set; }

        [ValidateNever]
        public Reaction Reaction { get; set; }
        [ValidateNever]
        public int? LikeCount { get; set; }
        [ValidateNever]
        public int? DislikeCount { get; set; }
    }
}
