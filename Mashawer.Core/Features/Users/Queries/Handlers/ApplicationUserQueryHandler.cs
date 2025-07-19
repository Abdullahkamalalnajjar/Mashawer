

namespace Mashawer.Core.Features.Users.Queries.Handlers
{
    public class ApplicationUserQueryHandler(IUserService userService, IMapper mapper) : ResponseHandler,
        IRequestHandler<UserProfileQuery, Response<UserProfileResponse>>,
        IRequestHandler<GetAllUserQuery, Response<IEnumerable<UserResponse>>>,
        IRequestHandler<GetAllAgentQuery, Response<IEnumerable<UserResponse>>>,
        IRequestHandler<GetAllRepresentativeQuery, Response<IEnumerable<UserResponse>>>,
        IRequestHandler<GetUserByIdQuery, Response<UserResponse>>

    {
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<UserProfileResponse>> Handle(UserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserProfileAsync(request.UserId);
            var response = _mapper.Map<UserProfileResponse>(user);
            return Success(response);
        }

        public async Task<Response<IEnumerable<UserResponse>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var response = await _userService.GetAllUsers();
            return Success(response);
        }

        public async Task<Response<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _userService.GetUserById(request.UserId);
            if (result is null)
            {
                return NotFound<UserResponse>("User not found");
            }
            return Success(result, new { request.UserId });
        }

        public async Task<Response<IEnumerable<UserResponse>>> Handle(GetAllAgentQuery request, CancellationToken cancellationToken)
        {
            var response = await _userService.GetAllAgnetAsync();
            return Success(response);
        }


        public async Task<Response<IEnumerable<UserResponse>>> Handle(GetAllRepresentativeQuery request, CancellationToken cancellationToken)
        {
            var response = await _userService.GetAllRepresentativeAsync();
            return Success(response);
        }
    }
}
