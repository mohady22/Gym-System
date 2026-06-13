
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
        private readonly IGenericRepository<Plan> planRepository;
        public PlanController(IGenericRepository<Plan> _planRepository)
        {
            planRepository = _planRepository;
        }
        public async Task<IActionResult> IndexAsync(CancellationToken token)
        {
            var plans = await planRepository.GetAll(false,token);
            return View(plans);
        }
        public async Task<IActionResult> Details(int Id,CancellationToken token)
        {
            var plans = await planRepository.GetById(Id,token);
            if(plans == null)
                RedirectToAction(nameof(Index));
            return View(plans);
        }
    }
}
