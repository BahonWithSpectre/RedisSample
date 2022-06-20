using Microsoft.AspNetCore.Mvc;

namespace RedisSample.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
