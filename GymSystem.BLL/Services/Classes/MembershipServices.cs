using AutoMapper;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembershipViewModels;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class MembershipServices(IUnitOfWork unitOfWork,IMapper mapper) : IMembershipServices
    {
        public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            var memberExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId, ct);
            if (!memberExists) return Result.NotFound("Member Not Found");
            var plan = await unitOfWork.GetRepository<Plan>().GetById(model.PlanId, ct);
            if (plan is null) return Result.NotFound("Plan Not Found");
            if (!plan.IsActive) return Result.Fail("Plan Is Not Active");

            var hasActive = await unitOfWork.membershipRepository
                .AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, ct);
            if (hasActive) return Result.Fail("Member Already has an active Membership.");
            var membership = new Membership
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                CreatedAt = DateTime.Now,
                EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.Duration),

            };

            unitOfWork.membershipRepository .Add(membership);
            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create New Membership.");
        }

        public async Task<Result> DeleteActiveMembershipAsync(int memberId, CancellationToken ct = default)
        {
            var active = await unitOfWork.membershipRepository
                .FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.UtcNow, true, ct);
            if (active is null) return Result.NotFound("No Active Membership for this member.");
            unitOfWork.membershipRepository.Delete(memberId);
            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete Membership.");
        }

        public async Task<IEnumerable<MembershipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default)
        {
            var membership = await unitOfWork.membershipRepository
                .GetAllMembershipWithMemberAndPlanAsync(m => m.EndDate > DateTime.UtcNow, ct);
            return mapper.Map<IEnumerable<MembershipViewModel>>(membership);
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAll(false,ct);
            return mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAll(false, ct);
            return mapper.Map<IEnumerable<PlanSelectListViewModel>>(plans);
        }
    }
}
