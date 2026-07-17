using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Entities.Enums;
using GymSystem.DAL.Repositories.Interfaces;
using GymSystemG03.BLL.ViewModels.MembersViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAttachementServices attachementServices;


        //GET
        public MemberServices(IUnitOfWork unitOfWork,IAttachementServices attachementServices)
        {
            this.unitOfWork = unitOfWork;
            this.attachementServices = attachementServices;
        }
        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAll(false, ct);
            if (!members.Any()) return [];
            var MembersViewModel = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Photo = m.Photo,
                Gender = m.Gender.ToString(),
            });
            return MembersViewModel;
            
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int id, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(id,ct);
            if(member ==null) return null;
            var memberVM = new MemberViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber}-{member.Address.Street}-{member.Address.City}",
            };
            var ActiveMembership = await unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(mb => mb.Id == id && mb.EndDate > DateTime.Now,false,ct);
            if (ActiveMembership is not null)
            {
                var ActivePlan = await unitOfWork.GetRepository<Plan>().GetById(ActiveMembership.PlanId, ct);
                memberVM.PlanName = ActivePlan?.Name;
                memberVM.MembershipStartDate = ActiveMembership.CreatedAt.ToShortDateString();
                memberVM.MembershipEndDate = ActiveMembership.EndDate.ToShortDateString();
            }
            return memberVM;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var Record = await unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(r => r.MemberId == memberId,false,ct);
            if (Record is null) return null;
            return new HealthRecordViewModel()
            {
                Weight = Record.Weight,
                Height = Record.Height,
                BloodType = Record.BloodType,
                Note = Record.Note,
            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member is null) return null;
            return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Photo = member.Photo,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City,
            };
        }
        //POST
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExsists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email,ct);
            var phoneExsists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone,ct);
            if (emailExsists || phoneExsists) return false;
            
            var member = new Member()
            {
                Name= model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City,
                },
                HealthRecord = new HealthRecord()
                {
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Note = model.HealthRecordViewModel.Note,
                }
            };
            var photoName = await attachementServices.UploadAsync(model.PhotoFile.OpenReadStream(),model.PhotoFile.FileName,"MemberPictures",ct);
            if(string.IsNullOrEmpty(photoName)) return false;   
            member.Photo = photoName;


            unitOfWork.GetRepository<Member>().Add(member);
            var Result = await unitOfWork.CompleteAsync();
            return Result > 0;
        }

        
        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            
            var member = await unitOfWork.GetRepository<Member>().GetById(id, ct);
            if (member is null) return false;
            if(await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id,ct)) return false;
            if(await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id,ct)) return false;
                        
            member.Phone = model.Phone;
            member.Email = model.Email;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdateAt = DateTime.Now;

            unitOfWork.GetRepository<Member>().Update(member);
            var Result = await unitOfWork.CompleteAsync();
            return Result > 0;

        }
        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetById(memberId, ct);
            if (member is null) return false;

            var hasFutureSession = await unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.EndDate > DateTime.Now, ct);
            if(hasFutureSession) return false;
            unitOfWork.GetRepository<Member>().Delete(memberId);

            if (member.Photo is not null)
            {
                attachementServices.Delete(member.Photo, "MemberPictures");
            }
            var Result = await unitOfWork.CompleteAsync();
            return Result > 0;
        }

    }
}
