using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]public IActionResult Add()
        {
            return View();
        }

         



        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
