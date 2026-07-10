using AutoMapper;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using GymSystem.BLL.Common;
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
    public class TrainerServices : ITrainerServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TrainerServices(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result>
            CreateTrainerAsync(
            CreateTrainerViewModel model,
            CancellationToken ct = default)
        {
            var trainer =
                mapper.Map<Trainer>(model);

            trainer.Address = new Address
            {
                BuildingNumber = model.BuildingNumber,
                City = model.City,
                Street = model.Street
            };

            var repo =
                unitOfWork.GetRepository<Trainer>();

            repo.Add(trainer);

            var affectedRows =
                await unitOfWork.CompleteAsync();

            return affectedRows > 0
                ? Result.Ok()
                : Result.Fail("Failed To Create Trainer");
        }

        public async Task<IEnumerable<TrainerViewModel>>
            GetAllTrainersAsync(
            CancellationToken ct = default)
        {
            var trainers =
                await unitOfWork
                    .GetRepository<Trainer>()
                    .GetAll(false, ct);

            if (!trainers.Any())
                return Enumerable.Empty<TrainerViewModel>();

            return mapper.Map
                <IEnumerable<Trainer>,
                 IEnumerable<TrainerViewModel>>
                (trainers);
        }

        public async Task<TrainerViewModel?>
            GetTrainerByIdAsync(
            int trainerId,
            CancellationToken ct = default)
        {
            var trainer =
                await unitOfWork
                    .GetRepository<Trainer>()
                    .GetById(trainerId, ct);

            if (trainer == null)
                return null;

            return mapper.Map
                <TrainerViewModel>(trainer);
        }

        public async Task<TrainerToUpdateViewModel?>
            GetTrainerToUpdateAsync(
            int trainerId,
            CancellationToken ct = default)
        {
            var trainer =
                await unitOfWork
                    .GetRepository<Trainer>()
                    .GetById(trainerId, ct);

            if (trainer == null)
                return null;

            return mapper.Map
                <TrainerToUpdateViewModel>(trainer);
        }

        public async Task<Result>
            UpdateTrainerAsync(
            int trainerId,
            TrainerToUpdateViewModel model,
            CancellationToken ct = default)
        {
            var repo =
                unitOfWork.GetRepository<Trainer>();

            var trainer =
                await repo.GetById(trainerId, ct);

            if (trainer == null)
                return Result.NotFound(
                    "Trainer Not Found");

            mapper.Map(model, trainer);

            trainer.Address.BuildingNumber =
                model.BuildingNumber;

            trainer.Address.City =
                model.City;

            trainer.Address.Street =
                model.Street;

            repo.Update(trainer);

            var affectedRows =
                await unitOfWork.CompleteAsync();

            return affectedRows > 0
                ? Result.Ok()
                : Result.Fail(
                    "Failed To Update Trainer");
        }

        public async Task<Result>
            RemoveTrainerAsync(
            int trainerId,
            CancellationToken ct = default)
        {
            var repo =
                unitOfWork.GetRepository<Trainer>();

            var trainer =
                await repo.GetById(trainerId, ct);

            if (trainer == null)
                return Result.NotFound(
                    "Trainer Not Found");

            repo.Delete(trainerId);

            var affectedRows =
                await unitOfWork.CompleteAsync();

            return affectedRows > 0
                ? Result.Ok()
                : Result.Fail(
                    "Failed To Delete Trainer");
        }
    }
}
