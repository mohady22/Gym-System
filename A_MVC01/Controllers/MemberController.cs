using GymSystem.BLL.Services.Interfaces;
using GymSystemG03.BLL.ViewModels.MembersViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace A_MVC01.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IMemberServices memberServices;
        private readonly IAttachementServices attachementServices;

        public MemberController(IMemberServices memberServices,IAttachementServices attachementServices)
        {
            this.memberServices = memberServices;
            this.attachementServices = attachementServices;
        }
        [HttpGet]
        public async Task<IActionResult> Picture(int id,CancellationToken ct )
        {
            var member = await memberServices.GetMemberDetailsAsync(id,ct);
            if(member == null  || string.IsNullOrWhiteSpace(member.Photo))
                return NotFound();
            var result = attachementServices.GetFile(member.Photo, "MemberPictures");
            if(result == null) return NotFound();
            return File(result.Value.stream, result.Value.contentType);
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await memberServices.GetAllMembersAsync(ct);
            return View(members);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);
            var Result = await memberServices.CreateMemberAsync(model,ct);
            if (Result)
                TempData["Success"] = "Member Created Successfully";
            else
                TempData["Failed"] = "Failed To Create Member";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var member = await memberServices.GetMemberDetailsAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var record = await memberServices.GetMemberHealthRecordAsync(id, ct);
            if (record is null)
            {
                TempData["ErrorMessage"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(record);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var member = await memberServices.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);

        }
        [HttpPost]
        public async Task<IActionResult> EditMember(int id,MemberToUpdateViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(nameof(Edit),model);

            var Result = await memberServices.UpdateMemberDetailsAsync(id, model, ct);
            if (Result)
            {
                TempData["Success"] = "Member Updated Successfully";               
            }
            else
            {
                TempData["Failed"] = "Failed To Update Member";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var Result = await memberServices.GetMemberDetailsAsync(id, ct);
            if (Result == null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
                
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var Result = await memberServices.DeleteMemberAsync(id, ct);
            if (Result)
            {
                TempData["Success"] = "Member Deleted Successfully";
            }
            else
            {
                TempData["Failed"] = "Failed To Delete Member";
            }
            return RedirectToAction(nameof(Index));
            
        }

    }
}
