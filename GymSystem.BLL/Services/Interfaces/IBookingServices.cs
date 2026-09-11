using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Common;
using GymSystem.BLL.ViewModels.BookingViewModels;
using GymSystem.BLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface IBookingServices
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionAsync(CancellationToken ct=default);
        Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpComingBySessionIdAsync(int sessionId,CancellationToken ct= default);
        Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOnGoingBySessionIdAsync(int sessionId,CancellationToken ct= default);
        Task<Result> CreateNewBookingAsync(CreateBookingViewModel model,CancellationToken ct=default);
        Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDownAsync(int sessionId,CancellationToken ct=default);
        Task<Result> CancelBookingAsync(int memberId ,int sessionId,CancellationToken ct=default);
        Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default);
    }
}
