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
            IEnumerable<BlogPost> blogList = _unitOfWork.BlogPost.GetAll(includeProperties: "Category");
            return View(blogList);
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
                })
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
                return RedirectToAction("Index");
            }
            return View(obj);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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