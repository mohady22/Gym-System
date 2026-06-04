using System.Diagnostics;
using A_MVC01.Models;
using Microsoft.AspNetCore.Mvc;

namespace A_MVC01.Controllers
{
    public class HomeController : Controller
    {
        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
