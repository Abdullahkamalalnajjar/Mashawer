namespace Mashawer.Data.Dtos
{
    public record UserResponse
    (string Id,
        string PhoneNumber, 
        string UserImage,
        string Email,
        string FirstName,
        string LastName,
        string UserType,
        string AgentAddress,
        string RepresentativeAddress,
        IEnumerable<string> Roles);
}
