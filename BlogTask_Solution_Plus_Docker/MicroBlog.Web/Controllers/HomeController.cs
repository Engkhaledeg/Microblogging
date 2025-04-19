using Microsoft.AspNetCore.Mvc;

namespace MicroBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}
