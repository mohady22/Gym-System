using AutoMapper;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.BookingViewModels;
using GymSystem.BLL.ViewModels.MembershipViewModels;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class BookingServices(IUnitOfWork unitOfWork, IMapper mapper) : IBookingServices
    {
        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetById(sessionId,ct);
            if (session is null) return Result.NotFound("Session Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot cancel a booking for a session that already started.");
            var booking = await unitOfWork.bookingRepository
                .FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId, true, ct);
            if (booking is null) return Result.NotFound("Booking Not Found");
            unitOfWork.bookingRepository.Delete(booking.Id);
            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Booking Cancel Failed");
        }

        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await unitOfWork.SessionRepository.GetById(model.SessionId,ct);
            if (session is null) return Result.NotFound("Seesion Not Found");
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot book a session that has already started.");
            var hasActiveMembership = await unitOfWork.membershipRepository
                .AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.UtcNow, ct);
            if (!hasActiveMembership)
                return Result.Fail("Member dose not have an active membership.");
            var alreadyBooked = await unitOfWork.bookingRepository
                .AnyAsync(b => b.SessionId == model.SessionId && b.MemberId == model.MemberId, ct);
            if (alreadyBooked)
                return Result.Fail("Member is already booked for this sessions.");
            var booked = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(model.SessionId, ct);
            if (booked >= session.Capacity)
                return Result.Fail("Session is Full.");
            unitOfWork.bookingRepository.Add(new Booking
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.UtcNow,
            });
            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to book Session");

        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionAsync(CancellationToken ct = default)
        {
            var bookins = await unitOfWork.SessionRepository.
                GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if (!bookins.Any()) return null;
            var mappedSession = mapper.Map<IEnumerable<SessionViewModel>>(bookins);
            foreach (var item in mappedSession)
            {
                item.AvailableSlots = item.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(item.Id,ct);
                
            }
            return mappedSession;
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDownAsync(int sessionId, CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAll(false, ct);

            if (!members.Any())
                return [];

            var activeMembers = members
                .Where(m => unitOfWork.membershipRepository
                    .AnyAsync( mb => mb.MemberId == m.Id &&  mb.EndDate > DateTime.Now,ct)
                    .GetAwaiter()
                    .GetResult());

            var bookedMemberIds = await unitOfWork.bookingRepository.GetAll(false, ct);

            var availableMembers = activeMembers.Where(m => !bookedMemberIds.Any(b => b.MemberId == m.Id &&b.SessionId == sessionId));

            return mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);

        }

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOnGoingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await unitOfWork.bookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            }).ToList();
        }

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpComingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await unitOfWork.bookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                IsAttended = b.IsAttended,
            }).ToList();
        }

        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await unitOfWork.bookingRepository
                .FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, true, ct);
            if (booking is null) return Result.NotFound("Booking not found");
            booking.IsAttended = true;
            booking.UpdateAt = DateTime.Now;
            unitOfWork.bookingRepository.Update(booking);
            var result = await unitOfWork.CompleteAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Mark As Attended.");
        }
    }
}
