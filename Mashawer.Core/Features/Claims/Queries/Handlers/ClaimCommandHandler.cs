
namespace Mashawer.Core.Features.Claims.Queries.Handlers
{
    public class ClaimCommandHandler(IClaimService claimService) : ResponseHandler,
        IRequestHandler<GetPermissionsByRoleIdQuery, Response<ClaimResponse>>,
        IRequestHandler<GetPermissionsQuery, Response<ClaimResponse>>
    {
        private readonly IClaimService _claimService = claimService;

        public async Task<Response<ClaimResponse>> Handle(GetPermissionsByRoleIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _claimService.GetClaimByRole(request.RoleId);
            if (result == null)
                return NotFound<ClaimResponse>("Role not found");
            return Success(result);
        }

        public async Task<Response<ClaimResponse>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var result = _claimService.GetClaim();
            return Success(result);
        }
    }
}
