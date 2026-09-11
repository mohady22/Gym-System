using GymSystem.BLL.Common;
using GymSystem.BLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface IMembershipServices
    {
        Task<IEnumerable<MembershipViewModel>> GetAllMembershipsAsync(CancellationToken ct=default);
        Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct=default);
        Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct=default);
        Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken ct=default);
        Task<Result> DeleteActiveMembershipAsync(int memberId, CancellationToken ct=default);

    }
}
