using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A_MVC01.Controllers
{
    [Authorize]
    public class MembershipController(IMembershipServices membershipServices) : Controller
    {
        

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View(await membershipServices.GetAllMembershipsAsync(ct));
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownAsync(ct);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(ct);
                return View(model);
            }
            var result = await membershipServices.CreateMembershipAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully.";
                return RedirectToAction(nameof(Index));

            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropDownAsync(ct);
            return View(model);
        }

        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            var result = await membershipServices.DeleteActiveMembershipAsync(id, ct);
            TempData[result.Success? "SuccessMessage" : "ErrorMessage"] = 
                result.Success? "Membership Canceled" : result.Error;
            return RedirectToAction(nameof (Index));
        }







        private async Task PopulateDropDownAsync(CancellationToken ct)
        {
            ViewBag.Plans = new SelectList(await membershipServices.GetPlansForDropDownAsync(ct), "Id", "Name");
            ViewBag.Members = new SelectList(await membershipServices.GetMembersForDropDownAsync(ct), "Id", "Name");
        }
    }
}
