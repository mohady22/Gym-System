
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A_MVC01.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanServices planServices;

        public PlanController(IPlanServices planServices)
        {
            this.planServices = planServices;
        }
        public async Task<IActionResult> IndexAsync(CancellationToken ct)
        {
            var plans = await planServices.GetAllPlansAsync(ct);
            return View(plans);
        }
        [HttpGet]
        public async Task<IActionResult> Details(
           int id,
           CancellationToken ct)
        {
            var plan =
                await planServices.GetPlanByIdAsync(id, ct);

            if (plan == null)
            {
                TempData["ErrorMessage"] =
                    "Plan Not Found";

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
       
    }
}
