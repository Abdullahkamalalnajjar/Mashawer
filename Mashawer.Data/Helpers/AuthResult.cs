namespace Mashawer.Data.Helpers
{
    public record AuthResult
    {
        public AuthResponse? Response { get; init; }
        public string? ErrorMessage { get; init; }

        public static AuthResult Success(AuthResponse response) => new() { Response = response };
        public static AuthResult Fail(string errorMessage) => new() { ErrorMessage = errorMessage };
    }

}
