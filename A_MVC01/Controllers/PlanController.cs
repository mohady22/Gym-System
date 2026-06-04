using A_MVC01.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A_MVC01.Controllers
{
    public class PlanController : Controller
    {
        private readonly GymDbContext dbContext = new GymDbContext();
        public async Task<IActionResult> IndexAsync()
        {
            var plans = await dbContext.Plans.ToListAsync();
            return View(plans);
        }
        public async Task<IActionResult> Details(int Id)
        {
            var plans = await dbContext.Plans.FirstOrDefaultAsync(p => p.Id == Id);
            if(plans == null)
                RedirectToAction(nameof(Index));
            return View(plans);
        }
    }
}
