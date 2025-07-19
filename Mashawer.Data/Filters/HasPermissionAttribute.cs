using Microsoft.AspNetCore.Authorization;

namespace Mashawer.Data.Filters;
public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}