namespace Mashawer.Service.Abstracts
{
    public interface IUserService
    {
        public Task<ApplicationUser> GetUserProfileAsync(string userId);
        public Task<IEnumerable<UserResponse>> GetAllUsers();
        public Task<IEnumerable<UserResponse>> GetAllAgnetAsync();
        public Task<IEnumerable<UserResponse>> GetAllRepresentativeAsync();
        public Task<UserResponse> GetUserById(string id);
        public Task<string> UpdateProfileUser(ApplicationUser user);
        public Task<string> DeleteUserWithReasone(ApplicationUser applicationUser, string reason, CancellationToken cancellationToken);
        public Task<string> DeleteUserAsync(ApplicationUser applicationUser);
        public Task<string> CreateUserAsync(string email, string firstName, string LastName, string password, IList<string> roles, CancellationToken cancellationToken);
        public Task<string> UpdateUserAsync(ApplicationUser user, IList<string> roles);


    }
}
