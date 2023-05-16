using BlogSite.DataAccess.Repository.IRepository;
using BlogSite.Models;
using BlogSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using BlogSite.Utility;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;

namespace BlogSite.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = SD.Role_User_Indi)]
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public BlogController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Blogs()
        {
            BlogOptionVM blogOptionVM = new()
            {
                BlogPost = _unitOfWork.BlogPost.GetAll(includeProperties: "Category,ApplicationUser"),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };
            return View(blogOptionVM);
        }

        public IActionResult UserBlogs()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return Redirect("Index");
            }
            BlogOptionVM blogOptionVM = new()
            {
                BlogPost = _unitOfWork.BlogPost.GetAllFromId(u => u.ApplicationUserId == claim.Value, includeProperties: "Category,ApplicationUser"),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };
            return View(blogOptionVM);
        }

        public IActionResult SearchedBlogs(int? CategoryListId, string? UserName)
        {

            if (CategoryListId != null && CategoryListId != 0 && UserName != null)
            {
                BlogOptionVM blogOptionVMCategory = new()
                {
                    BlogPost = _unitOfWork.BlogPost.GetAllFromId(u => u.CategoryId == CategoryListId, includeProperties: "Category,ApplicationUser"),
                    CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                };
                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.UserName == UserName);
                BlogOptionVM blogOptionVMUser = new()
                {
                    BlogPost = blogOptionVMCategory.BlogPost.Where(u => u.ApplicationUserId == applicationUser.Id),
                    CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                };
                return View(blogOptionVMUser);
            }
            else if (UserName != null)
            {
                ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.UserName == UserName);
                BlogOptionVM blogOptionVMUser = new()
                {
                    BlogPost = _unitOfWork.BlogPost.GetAllFromId(u => u.ApplicationUserId == applicationUser.Id, includeProperties: "Category,ApplicationUser"),
                    CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                };
                return View(blogOptionVMUser);
            }
            else if (CategoryListId != null && CategoryListId != 0)
            {
                BlogOptionVM blogOptionVMCategory = new()
                {
                    BlogPost = _unitOfWork.BlogPost.GetAllFromId(u => u.CategoryId == CategoryListId, includeProperties: "Category,ApplicationUser"),
                    CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                };
                return View(blogOptionVMCategory);
            }
            return Redirect("Blogs");

        }

        public IActionResult UserBlogsByCategory(int? CategoryListId)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim.Value == null)
            {
                return Redirect("Index");
            }
            BlogOptionVM blogOptionVM = new()
            {
                BlogPost = _unitOfWork.BlogPost.GetAllFromId(u => u.ApplicationUserId == claim.Value, includeProperties: "Category,ApplicationUser"),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };
            if (CategoryListId != null && CategoryListId != 0)
            {
                BlogOptionVM blogOptionVMCategory = new()
                {
                    BlogPost = blogOptionVM.BlogPost.Where(u => u.CategoryId == CategoryListId),
                    CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                };
                return View(blogOptionVMCategory);
            }
            return View(blogOptionVM);
        }

        public IActionResult Upsert(int? id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            BlogVM blogVM = new()
            {
                BlogPost = new()
                {
                    ApplicationUserId = claim.Value
                },
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value)
            };
            if(id == null || id == 0)
            {
                return View(blogVM);
            }
            else
            {
                blogVM.BlogPost = _unitOfWork.BlogPost.GetFirstOrDefault(u => u.Id == id);
                return View(blogVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(BlogVM obj)
        {
            if (ModelState.IsValid)
            {
                if(obj.BlogPost.Id == 0)
                {
                    _unitOfWork.BlogPost.Add(obj.BlogPost);
                }
                else
                {
                    _unitOfWork.BlogPost.Update(obj.BlogPost);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product added successfully";
                return RedirectToAction("UserBlogs");
            }
            return View(obj);
        }

        public IActionResult Reaction(string? react, int id)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Reaction reaction = _unitOfWork.Reaction.GetFirstOrDefault(u => u.BlogPostId == id && u.ApplicationUserId == claim.Value);
            if(id == 0)
            {
                return Redirect("Blogs");
            }
            else
            {
                if (reaction == null)
                {
                    Reaction reactionNew = new()
                    {
                        ApplicationUserId = claim.Value,
                        BlogPostId = id,
                        Action = react
                    };
                    _unitOfWork.Reaction.Add(reactionNew);
                }
                else
                {
                    Reaction reactionNew = new()
                    {
                        Id= reaction.Id,
                        ApplicationUserId = claim.Value,
                        BlogPostId = id,
                        Action = react
                    };
                    _unitOfWork.Reaction.Update(reactionNew);
                }
                _unitOfWork.Save();
                return Redirect("Details?blogPostId="+id);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int blogPostId)
        {
            int likes = 0;
            int dislikes = 0;
            if (blogPostId == null || blogPostId == 0)
            {
                return NotFound();
            }
            IEnumerable<Reaction> count = _unitOfWork.Reaction.GetAllFromId(u=>u.BlogPostId == blogPostId);
            IEnumerable<Reaction> likeCount = count.Where(u => u.Action == "Like");
            foreach (var like in likeCount)
            {
                likes++;
            }
            IEnumerable<Reaction> dislikeCount = count.Where(u => u.Action == "Dislike");
            foreach (var like in dislikeCount)
            {
                dislikes++;
            }
            BlogVM blogVM = new()
            {
                BlogPost = _unitOfWork.BlogPost.GetFirstOrDefault(u => u.Id == blogPostId, includeProperties: "ApplicationUser,Category"),
                LikeCount = likes,
                DislikeCount = dislikes,
            };
            if (blogVM == null)
            {
                return NotFound();
            }

            return View(blogVM);
        }
    #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var blogList = _unitOfWork.BlogPost.GetAll(includeProperties: "Category");

            return Json(new { data = blogList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.BlogPost.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.BlogPost.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
    #endregion
    }

}