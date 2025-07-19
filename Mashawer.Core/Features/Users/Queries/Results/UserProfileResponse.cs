namespace Mashawer.Core.Features.Users.Queries.Results
{
    public record UserProfileResponse
    (
       string Email,
       string FirstName,
       string LastName,
       string FullName,
       string UserName,
       string PhoneNumber,
       string? ProfilePictureUrl,
       string UserType
        );
}
