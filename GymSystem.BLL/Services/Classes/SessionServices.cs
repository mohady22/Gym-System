using AutoMapper;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SessionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            
        }

        

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Vaildation("End Date Must Be After Start Date.");
            if (model.StartDate <= DateTime.Now) return Result.Vaildation("Start Date Must Be The Future");

            var trainerRepo =  unitOfWork.GetRepository<Trainer>();
            var trainer = await trainerRepo.GetById(model.TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var categoryRepo = unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetById(model.CategoryId, ct);
            if (category == null) return Result.NotFound("Category Not Found");

            var sessions = mapper.Map<CreateSessionViewModel,Session>(model);

            var sessionRepo = unitOfWork.GetRepository<Session>();
            sessionRepo.Add(sessions);
            var rowEffected = await unitOfWork.CompleteAsync();
            return rowEffected > 0 ? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if (!sessions.Any()) return null;

            sessions = sessions.OrderByDescending(s => s.StartDate);

            var mappedSessions = mapper.Map<IEnumerable<Session>,IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id,ct);
            }
            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAll(false, ct);
            return mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct)
        {
            var session = await unitOfWork.SessionRepository.GetSessionsByIdWithTrainerAndCategoryAsync(sessionId, ct);
            if (session == null) return null;
            var mappedSession = mapper.Map<Session,SessionViewModel >(session);
            
            mappedSession.AvailableSlots = mappedSession.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(mappedSession.Id, ct);

            return mappedSession;

        }

        public async Task<UpdateSessionViewModel> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await unitOfWork.GetRepository<Session>().GetById(sessionId, ct);
            if (session is null) return null;

            if (!await IsSessionValidForUpdateAsync(session, ct)) return null;
            return mapper.Map<Session,UpdateSessionViewModel>(session);

        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetAll(false,ct);
            return mapper.Map<IEnumerable<Trainer>,IEnumerable<TrainerSelectViewModel>>(trainer);
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var repo = unitOfWork.GetRepository<Session>();
            var session = await repo.GetById(sessionId, ct);
            if (session is null) return Result.NotFound("Session Not Found");
            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Can not Delete a session that has not yet ended");
            var bookedCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(sessionId, ct);
            if (bookedCount > 0)
                return Result.Fail("Can not Delete a Session that has Bookings");
            repo.Delete(sessionId);
            var affectRows = await unitOfWork.CompleteAsync();
            return affectRows > 0 ? Result.Ok() : Result.Fail("Failed To Remove Session");


        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var sessionRepo =  unitOfWork.GetRepository<Session>();
            var session = await sessionRepo.GetById(sessionId, ct);
            if (session is null) return Result.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can not edit a session that has already started");

            var bookedCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);
            if (bookedCount > 0)
                return Result.Fail("Can not edit a session that has booked slots");
            if (model.EndDate < model.StartDate) return Result.Vaildation("End Date Must Be After Start Date");
            if (model.StartDate <= DateTime.Now) return Result.Vaildation("Start Date Must Be The Future");

            var trainerRepo = unitOfWork.GetRepository<Trainer>();
            var trainer = await trainerRepo.GetById(model.TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            session.UpdateAt = DateTime.Now;

            mapper.Map(model, session);

            sessionRepo.Update(session);
            var rowEffected = await unitOfWork.CompleteAsync();
            return rowEffected > 0 ? Result.Ok() : Result.Fail("Failed To Update Session");



        }

        private async Task<bool> IsSessionValidForUpdateAsync(Session session, CancellationToken ct)
        {
            if(session.StartDate <= DateTime.Now ) return false;
            var booked = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);
            return booked ==0;
        }
    }
}
