using Mashawer.Core.Features.UserUpgradeRequests.Command.Models;

namespace Mashawer.Core.Features.UserUpgradeRequests.Command.Handler
{
    public class UserUpgradeRequestCommand(IUserUpgradeRequestService userUpgradeRequestService, IUnitOfWork unitOfWork, IMapper mapper) : ResponseHandler,
        IRequestHandler<CreateUserUpgradeRequestCommand, Response<string>>,
        IRequestHandler<EditUserUpgradeRequestCommand, Response<string>>,
        IRequestHandler<DeleteUserUpgradeRequestCommand, Response<string>>
    {
        private readonly IUserUpgradeRequestService _userUpgradeRequestService = userUpgradeRequestService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<string>> Handle(CreateUserUpgradeRequestCommand request, CancellationToken cancellationToken)
        {
            var userUpgradeRequest = _mapper.Map<UserUpgradeRequest>(request);
            var result = await _userUpgradeRequestService.CreateUpgradeRequestAsync(userUpgradeRequest, cancellationToken);
            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Success<string>("User upgrade request created successfully.", result);
            }
            return BadRequest<string>("Failed to create user upgrade request.");
        }

        public async Task<Response<string>> Handle(EditUserUpgradeRequestCommand request, CancellationToken cancellationToken)
        {
            var oldRequest = await _userUpgradeRequestService.GetUpgradeRequestByIdAsync(request.Id);
            if (oldRequest == null)
            {
                return NotFound<string>("User upgrade request not found.");
            }
            _mapper.Map(request, oldRequest);
            var result = _userUpgradeRequestService.UpdateUpgradeRequest(oldRequest);
            if (result == "Updated")
            {
                await _unitOfWork.CompeleteAsync();
                return Success<string>("User upgrade request updated successfully.", result);
            }
            return BadRequest<string>("Failed to update user upgrade request.");
        }

        public async Task<Response<string>> Handle(DeleteUserUpgradeRequestCommand request, CancellationToken cancellationToken)
        {
            var userRequest = await _userUpgradeRequestService.GetUpgradeRequestByIdAsync(request.Id);
            if (userRequest == null)
            {
                return NotFound<string>("User upgrade request not found.");
            }
            var result = await _userUpgradeRequestService.DeleteUpgradeRequest(userRequest);
            if (result == "Deleted")
            {
                await _unitOfWork.CompeleteAsync();
                return Success<string>("User upgrade request deleted successfully.", result);
            }
            return BadRequest<string>("Failed to delete user upgrade request.");
        }
    }
}
