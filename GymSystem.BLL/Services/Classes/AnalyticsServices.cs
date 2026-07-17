using GymManagementBLL.ViewModels.AnalyticsViewModels;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class AnalyticsServices : IAnalyticsServices
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticsServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken ct = default)
        {
            var sessions = await unitOfWork.GetRepository<Session>().GetAll(false);

            var totalMembers = await unitOfWork.GetRepository<Member>().CountAsync(ct:ct);
            var totalTrainers = await unitOfWork.GetRepository<Trainer>().CountAsync(ct:ct);
            var activeMembers = await unitOfWork.GetRepository<Membership>().CountAsync(m => m.EndDate > DateTime.Now ,ct);

            return new AnalyticsViewModel
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
                UpcomingSessions = sessions.Count( s => s.StartDate > DateTime.Now),
                OngoingSessions = sessions.Count(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now),
                CompletedSessions = sessions.Count(s => s.EndDate < DateTime.Now),
            };
        }
    }
}
