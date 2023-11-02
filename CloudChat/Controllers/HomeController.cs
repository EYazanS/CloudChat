using Microsoft.AspNetCore.Mvc;

namespace CloudChat.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
