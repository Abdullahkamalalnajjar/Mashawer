namespace Mashawer.Core.Features.Users.Commands.Handlers
{
    public class ApplicationUserCommandHandler(IUserService userService, ICurrentUserService currentUserService, IMapper mapper, ApplicationDbContext context, IRoleService roleService, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork) : ResponseHandler,
        IRequestHandler<EditApplicationUserCommand, Response<string>>,
        IRequestHandler<CreateUserCommand, Response<string>>,
        IRequestHandler<UpdateUserCommand, Response<string>>,
        IRequestHandler<DeleteUserWithReasonCommand, Response<string>>,
        IRequestHandler<DeleteUserCommand, Response<string>>

    {
        private readonly IUserService _userService = userService;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IMapper _mapper = mapper;
        private readonly ApplicationDbContext _context = context;
        private readonly IRoleService _roleService = roleService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region EditProfile
        public async Task<Response<string>> Handle(EditApplicationUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound<string>("User not found");
            //  user = request.Adapt(user);
            _mapper.Map(request, user);
            var result = await _userService.UpdateProfileUser(user);
            if (result == "Updated")
                return Updated<string>("Application user has been updated succssfully");
            return UnprocessableEntity<string>("Exit error when make update");
        }
        #endregion


        public async Task<Response<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var emailIsExist = await _unitOfWork.Users.GetTableNoTracking().AnyAsync(x => x.Email == request.Email);
            if (emailIsExist) return UnprocessableEntity<string>("Email already exists");
            var allowRoles = await _roleService.GetAllRolesAsync();
            if (allowRoles is null) return UnprocessableEntity<string>("No roles available, please create roles first");
            if (request.Roles.Except(allowRoles.Select(x => x.Name)).Any())
                return UnprocessableEntity<string>("One or more roles are not valid, please check the roles provided");
            var result = await _userService.CreateUserAsync(request.Email, request.FirstName, request.LastName, request.Password, request.Roles);
            if (result == "Created")
                return Created<string>("Application user has been created successfully", new { request.Email });
            if (result == "ErrorCreatingUser")
                return UnprocessableEntity<string>("Error creating user, please try again");
            if (result == "UnknownError")
                return UnprocessableEntity<string>(result);
            return UnprocessableEntity<string>("An unknown error occurred while creating the user");
        }

        #region EditUserWithRole
        public async Task<Response<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return NotFound<string>("User not found");
            var allowRoles = await _roleService.GetAllRolesAsync();
            if (allowRoles is null) return UnprocessableEntity<string>("No roles available, please create roles first");
            if (request.Roles.Except(allowRoles.Select(x => x.Name)).Any())
                return UnprocessableEntity<string>("One or more roles are not valid, please check the roles provided");
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(request.Roles).ToList();
            await _context.Roles.Where(x => x.Id == user.Id).ExecuteDeleteAsync();
            _mapper.Map(request, user);
            var result = await _userService.UpdateUserAsync(user, request.Roles);
            if (result == "Updated")
                return Updated<string>("Application user has been updated successfully", new { request.UserId });
            return UnprocessableEntity<string>("Exit error when make update");
        }

        public async Task<Response<string>> Handle(DeleteUserWithReasonCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user = await _unitOfWork.Users.FindAsync(x => x.Id.Equals(userId));
            if (user is null)
                return NotFound<string>("User not found");
            var result = await _userService.DeleteUserWithReasone(user, request.Reason, cancellationToken);
            if (result == "Deleted")
            {
                await _unitOfWork.CompeleteAsync();
                return Deleted<string>("Application user has been deleted successfully");
            }
            return BadRequest<string>("Delete user failed, please try again");
        }

        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            var result = await _userService.DeleteUserAsync(user);
            if (result == "Deleted")
                return Deleted<string>("Deleted Succssfully");
            return BadRequest<string>("Exist error in delete");
        }

        #endregion

    }
}
