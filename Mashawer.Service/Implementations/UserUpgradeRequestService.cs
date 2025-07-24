using Org.BouncyCastle.Crypto.Utilities;

namespace Mashawer.Service.Implementations
{
    public class UserUpgradeRequestService(IUnitOfWork unitOfWork) : IUserUpgradeRequestService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> CreateUpgradeRequestAsync(UserUpgradeRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.UserUpgradeRequests.AddAsync(request, cancellationToken);
            return "Created";
        }

        public async Task<string> DeleteUpgradeRequest(UserUpgradeRequest request)
        {
            await _unitOfWork.UserUpgradeRequests.Delete(request);
            return "Deleted";
        }

        public async Task<IEnumerable<UserUpgradeRequestResponse>> GetAllUpgradeRequestsAsync()
        {
            return await _unitOfWork.UserUpgradeRequests.GetTableNoTracking()
                .Select(x => new UserUpgradeRequestResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserName = x.User.FullName,
                    UserEmail = x.User.Email,
                    UserPhone = x.User.PhoneNumber,
                    UserImage = x.User.ProfilePictureUrl,
                    RequestedRole = x.RequestedRole.ToString(),
                    TargetAgentId = x.TargetAgentId,
                    TargetAgentName = x.TargetAgent != null ? x.TargetAgent.FullName : null,
                    Note = x.Note,
                    Address = x.Address,
                    CreatedAt = x.CreatedAt,
                    Status = x.Status.ToString()
                })
                .ToListAsync();
        }

        public Task<UserUpgradeRequest?> GetUpgradeRequestByIdAsync(int id)
        {
            return _unitOfWork.UserUpgradeRequests.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<UserUpgradeRequestResponse>> GetUpgradeRequestsByTargetAgentIdAsync(string targetAgentId)
        {
            return await _unitOfWork.UserUpgradeRequests.GetTableNoTracking()
                .Where(x => x.TargetAgentId == targetAgentId)
           .Select(x => new UserUpgradeRequestResponse
           {
               Id = x.Id,
               UserId = x.UserId,
               UserName = x.User.FullName,
               UserEmail = x.User.Email,
               UserPhone = x.User.PhoneNumber,
               UserImage = x.User.ProfilePictureUrl,
               RequestedRole = x.RequestedRole.ToString(),
               TargetAgentId = x.TargetAgentId,
               TargetAgentName = x.TargetAgent != null ? x.TargetAgent.FullName : null,
               Note = x.Note,
               Address = x.Address,
               CreatedAt = x.CreatedAt,
               Status = x.Status.ToString()
           })
           .ToListAsync();
        }
        public async Task<IEnumerable<UserUpgradeRequestResponse>> GetUpgradeRequestsByUserIdAsync(string userId)
        {
            return await _unitOfWork.UserUpgradeRequests.GetTableNoTracking()
                .Where(x => x.UserId == userId)
           .Select(x => new UserUpgradeRequestResponse
           {
               Id = x.Id,
               UserId = x.UserId,
               UserName = x.User.FullName,
               UserEmail = x.User.Email,
               UserPhone = x.User.PhoneNumber,
               UserImage = x.User.ProfilePictureUrl,
               RequestedRole = x.RequestedRole.ToString(),
               TargetAgentId = x.TargetAgentId,
               TargetAgentName = x.TargetAgent != null ? x.TargetAgent.FullName : null,
               Note = x.Note,
               Address = x.Address,
               CreatedAt = x.CreatedAt,
               Status = x.Status.ToString()
           })
           .ToListAsync();
        }
        public string UpdateUpgradeRequest(UserUpgradeRequest request)
        {
            _unitOfWork.UserUpgradeRequests.Update(request);
            return "Updated";
        }
        public async Task<IEnumerable<UserUpgradeRequestResponse>> GetAllUpgradeRequestsByAddressAsync(string address)
        {
            return await _unitOfWork.UserUpgradeRequests.GetTableNoTracking()
                .Where(x => x.Address == address)
                .Select(x => new UserUpgradeRequestResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserName = x.User.FullName,
                    UserEmail = x.User.Email,
                    UserPhone = x.User.PhoneNumber,
                    UserImage = x.User.ProfilePictureUrl,
                    RequestedRole = x.RequestedRole.ToString(),
                    TargetAgentId = x.TargetAgentId,
                    TargetAgentName = x.TargetAgent != null ? x.TargetAgent.FullName : null,
                    Note = x.Note,
                    Address = x.Address,
                    CreatedAt = x.CreatedAt,
                    Status = x.Status.ToString()
                })
                .ToListAsync();
        }

    }
}
