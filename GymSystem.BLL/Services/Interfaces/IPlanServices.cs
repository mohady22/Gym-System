using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface IPlanServices
    {
        

        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(
            CancellationToken ct = default);

        Task<PlanViewModel?> GetPlanByIdAsync(int planId,
            CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default);


    }
}
