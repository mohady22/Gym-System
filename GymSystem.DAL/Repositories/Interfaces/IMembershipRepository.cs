using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IMembershipRepository:IGenericRepository<Membership>
    {
        Task<IEnumerable<Membership>> GetAllMembershipWithMemberAndPlanAsync(Expression<Func<Membership,bool>>? predicate,CancellationToken ct=default);
    }
}
