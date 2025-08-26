using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        

    }
}
