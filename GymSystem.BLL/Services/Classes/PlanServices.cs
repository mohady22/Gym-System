using AutoMapper;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    
    public class PlanServices : IPlanServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PlanServices(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        public async Task<IEnumerable<PlanViewModel>>
            GetAllPlansAsync(
            CancellationToken ct = default)
        {
            var plans =
                await unitOfWork
                    .GetRepository<Plan>()
                    .GetAll(false, ct);

            if (!plans.Any())
                return Enumerable.Empty<PlanViewModel>();

            return mapper.Map<
                IEnumerable<Plan>,
                IEnumerable<PlanViewModel>>(plans);
        }

        public async Task<PlanViewModel?>
            GetPlanByIdAsync(
            int planId,
            CancellationToken ct = default)
        {
            var plan =
                await unitOfWork
                    .GetRepository<Plan>()
                    .GetById(planId, ct);

            if (plan is null)
                return null;

            return mapper.Map<PlanViewModel>(plan);
        }

        
    }

}

