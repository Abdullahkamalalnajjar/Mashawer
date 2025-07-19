using Microsoft.AspNetCore.Authorization;

namespace Mashawer.Data.Filters;
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}