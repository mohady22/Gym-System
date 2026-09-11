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
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        private readonly GymDbContext dbContext;

        public MembershipRepository(GymDbContext dbContext):base(dbContext) 
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<Membership>> GetAllMembershipWithMemberAndPlanAsync(Expression<Func<Membership, bool>>? predicate, CancellationToken ct = default)
        {
            var query = dbContext.Memberships
                .Include(m => m.Member)
                .Include(m => m.Plan).AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.ToListAsync(ct);
        }
    }
}
