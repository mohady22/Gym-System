using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A_MVC01.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingServices _bookingServices;

        public BookingController(IBookingServices bookingServices)
        {
            _bookingServices = bookingServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View(await _bookingServices.GetAllSessionAsync(ct));
        }
        [HttpGet]
        public async Task<IActionResult> Create(int id, CancellationToken ct)
        {
            var members = await _bookingServices.GetMemberForDropDownAsync(id,ct);
            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.SessionId = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            var result = await _bookingServices.CreateNewBookingAsync(model, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
               result.Success ? "Booking Created Successfully" : result.Error;
            return RedirectToAction(nameof(GetMemberForUpComingSession),new {id = model.SessionId});
        }
        [HttpGet]
        public async Task<IActionResult> GetMemberForUpComingSession(int id, CancellationToken ct)
        {
            return View(await _bookingServices.GetMembersForUpComingBySessionIdAsync(id, ct));
        }
        [HttpGet]
        public async Task<IActionResult> GetMemberForOnGoingSession(int id, CancellationToken ct)
        {
            return View(await _bookingServices.GetMembersForOnGoingBySessionIdAsync(id, ct));
        }
        [HttpPost]
        public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingServices.MarkAttendedAsync(memberId, sessionId, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? "Attendance Recoreded" : result.Error;
            return RedirectToAction(nameof(GetMemberForOnGoingSession), new { id = sessionId });
        }

    }
}
