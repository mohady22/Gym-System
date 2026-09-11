using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Common;
using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface ISessionServices
    {
        public Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct=default);
        public Task<Result> CreateSessionAsync(CreateSessionViewModel model,CancellationToken ct=default);
        public Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct=default);
        public Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct=default);
        public Task<SessionViewModel?> GetSessionByIdAsync(int sessionId, CancellationToken ct );

        public Task<UpdateSessionViewModel> GetSessionToUpdateAsync(int sessionId, CancellationToken ct=default);
        public Task<Result> UpdateSessionAsync(int sessionId,UpdateSessionViewModel model,CancellationToken ct=default);
        public Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct=default);
    }
}
