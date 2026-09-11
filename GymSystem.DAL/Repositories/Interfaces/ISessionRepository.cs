using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository:IGenericRepository<Session>
    {
        public Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct);
        public Task<Session> GetSessionsByIdWithTrainerAndCategoryAsync(int sessionId,CancellationToken ct);
        Task<int> GetCountOfBookedSlotAsync(int sessionId,CancellationToken ct);

    }
}
