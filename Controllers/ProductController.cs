using Microsoft.AspNetCore.Mvc;

namespace ProductAPI.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
