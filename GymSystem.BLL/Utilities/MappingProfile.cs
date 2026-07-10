using AutoMapper;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using GymSystem.DAL.Entities;
using GymSystemG03.BLL.ViewModels.MembersViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Utilities
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            MapMember();
            MapSession();
            MapPlan();
            MapTrainer();
        }
        public void MapSession()
        {
            CreateMap<Session, SessionViewModel>().ForMember(dest => dest.CategoryName,opt => opt.MapFrom(src => src.Category.CategoryName))
                                                  .ForMember(dest => dest.TrainerName,opt => opt.MapFrom(src => src.Trainer.Name))
                                                  .ForMember(dest => dest.AvailableSlots,opt => opt.Ignore()).ReverseMap();
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Session,SessionViewModel>();
            CreateMap<Session,UpdateSessionViewModel>();
            CreateMap< UpdateSessionViewModel, Session>();
        }
        public void MapPlan()
        {
            
            CreateMap<Plan, PlanViewModel>();

            CreateMap<Plan, UpdatePlanViewModel>()
                .ReverseMap();
        }
        public void MapTrainer()
        {
            CreateMap<CreateTrainerViewModel, Trainer>();

            CreateMap<Trainer, TrainerViewModel>()
                .ForMember(d => d.DateOfBirth,o => o.MapFrom(s => s.DateOfBirth.ToString()))
                .ForMember(d => d.Gender,o => o.MapFrom(s => s.Gender.ToString()))
                .ForMember(d => d.Specialties,o => o.MapFrom(s => s.Specailtize.ToString()))
                .ForMember(d => d.Address,o => o.MapFrom(s => $"{s.Address.BuildingNumber}, {s.Address.Street}, {s.Address.City}"));

            CreateMap<Trainer,TrainerToUpdateViewModel>()
                .ForMember(d => d.BuildingNumber,o => o.MapFrom(s => s.Address.BuildingNumber))
                .ForMember(d => d.City,o => o.MapFrom(s => s.Address.City))
                .ForMember(d => d.Street,o => o.MapFrom(s => s.Address.Street))
                .ReverseMap();

        }
        public void MapMember()
        {
            CreateMap<CreateMemberViewModel, Member>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
            {
                BuildingNumber = src.BuildingNumber,
                Street = src.Street,
                City = src.City
            }))
            .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel != null ? new HealthRecord
            {
                Weight = src.HealthRecordViewModel.Weight,
                Height = src.HealthRecordViewModel.Height,
                BloodType = src.HealthRecordViewModel.BloodType,
                Note = src.HealthRecordViewModel.Note
            } : null));


            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber}-{src.Address.Street}-{src.Address.City}"))
                .ForMember(dest => dest.PlanName, opt => opt.Ignore())
                .ForMember(dest => dest.MembershipStartDate, opt => opt.Ignore())
                .ForMember(dest => dest.MembershipEndDate, opt => opt.Ignore());

            CreateMap<Member, MemberToUpdateViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ReverseMap() 
            .ForPath(dest => dest.Address.BuildingNumber, opt => opt.MapFrom(src => src.BuildingNumber))
            .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Street))
            .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.City));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();
        }
    }
}
