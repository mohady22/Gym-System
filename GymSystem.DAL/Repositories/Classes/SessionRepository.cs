using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext dbContext;

        public SessionRepository(GymDbContext dbContext) :base(dbContext) 
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct)
        {
            var session = dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category);
            return await session.ToListAsync(ct);
        }

        public Task<int> GetCountOfBookedSlotAsync(int sessionId, CancellationToken ct)
        {
            return dbContext.Bookings.AsNoTracking().CountAsync(s => s.SessionId == sessionId);
        }

        public async Task<Session> GetSessionsByIdWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct)
        {
            var session = await dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == sessionId);
            return session;
        }
    }
}
