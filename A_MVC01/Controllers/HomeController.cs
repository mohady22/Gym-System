using System.Diagnostics;
using A_MVC01.Models;
using GymSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A_MVC01.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAnalyticsServices analyticsServices;

        public HomeController(IAnalyticsServices analyticsServices)
        {
            this.analyticsServices = analyticsServices;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Data = await analyticsServices.GetAnalyticsDataAsync(ct);
            return View(Data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
