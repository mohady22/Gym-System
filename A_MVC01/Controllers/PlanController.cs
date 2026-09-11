
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace A_MVC01.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Details(int id,CancellationToken ct)
        {
            var plan = await planServices.GetPlanByIdAsync(id, ct);

            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan Not Found";

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken ct)
        {
            var plan = await planServices.GetPlanToUpdateAsync(id, ct);

            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan Not Found";

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdatePlanViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await planServices.UpdatePlanAsync(id,model,ct);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Plan Updated Successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Error;

            return View(model);
        }
    }
}
