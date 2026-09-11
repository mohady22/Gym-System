using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace A_MVC01.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly ISessionServices sessionServices;

        public SessionController(ISessionServices sessionServices)
        {
            this.sessionServices = sessionServices;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await sessionServices.GetAllSessionsAsync(ct);
            return View(sessions);
        }
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownAsync(ct);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(ct);
                return View(model);
            }
            var Result = await sessionServices.CreateSessionAsync(model,ct);
            if (Result.Success)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = Result.Error;
            await PopulateDropDownAsync(ct);
            return View(model);
        }
        private async Task PopulateDropDownAsync(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList( await sessionServices.GetTrainersForDropDownAsync(ct),"Id","Name");
            ViewBag.Categories = new SelectList( await sessionServices.GetCategoriesForDropDownAsync(ct),"Id" ,"CategoryName");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var session = await sessionServices.GetSessionByIdAsync(id,ct);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(session);

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var session = await sessionServices.GetSessionToUpdateAsync(id,ct);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session Cannot Be Edit, It's Not Found";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropDownAsync(ct);
            return View(session);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(ct);
                return View(model);
            }
            var Result = await sessionServices.UpdateSessionAsync(id,model, ct);
            if (Result.Success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = Result.Error;
            await PopulateDropDownAsync(ct);
            return View(model);

        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var session = await sessionServices.GetSessionByIdAsync(id,ct);  
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(session);   
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var Result = await sessionServices.RemoveSessionAsync(id,ct);
            TempData[Result.Success ? "SuccessMessage":"ErrorMessage"] = Result.Success ? "Session Deleted Successfully!":Result.Error;
            return RedirectToAction(nameof(Index));
        }
    }
}
