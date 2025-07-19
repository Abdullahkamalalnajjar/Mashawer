namespace Mashawer.Service.Abstracts
{
    public interface IRoleService
    {
        public Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? includeDeleted = false);
        public Task<RoleDetailsResponse> GetRolesDetailsAsync(string roleId);
        public Task<string> AddRoleWithPermissionAsync(string name, IList<string> permissions);
        public Task<string> UpdateRoleWithPermissionAsync(string id, string name, IList<string> permissions);
    }
}
