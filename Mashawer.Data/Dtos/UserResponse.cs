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
        string Address,
        string AgentAddress,
        string RepresentativeAddress,
        double? RepresentativeFromLatitude,
        double? RepresentativeToLatitude,
        double? RepresentativeFromLongitude,
        double? RepresentativeToLongitude,
        int? WalletId,
        decimal? Balance,
        IEnumerable<string> Roles
        );
}

