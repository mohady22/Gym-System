using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using GymSystem.BLL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface ITrainerServices
    {
        Task<Result> CreateTrainerAsync(
            CreateTrainerViewModel model,
            CancellationToken ct = default);

        Task<IEnumerable<TrainerViewModel>>
            GetAllTrainersAsync(
            CancellationToken ct = default);

        Task<TrainerViewModel?>
            GetTrainerByIdAsync(
            int trainerId,
            CancellationToken ct = default);

        Task<TrainerToUpdateViewModel?>
            GetTrainerToUpdateAsync(
            int trainerId,
            CancellationToken ct = default);

        Task<Result>
            UpdateTrainerAsync(
            int trainerId,
            TrainerToUpdateViewModel model,
            CancellationToken ct = default);

        Task<Result>
            RemoveTrainerAsync(
            int trainerId,
            CancellationToken ct = default);
    }
}
