using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogSite.Controllers
{
    public class RoleController : Controller
    {
        [Authorize(Policy = "UserOnly")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
