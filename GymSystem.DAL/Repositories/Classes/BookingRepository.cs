using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly GymDbContext dbContext;

        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            return dbContext.Bookings.AsNoTracking().Include(b=>b.Member).Where(b=>b.SessionId == sessionId).ToListAsync(ct);

        }
    }
}
