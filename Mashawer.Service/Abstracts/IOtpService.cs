public interface IOtpService
{
    Task<string> GenerateOtpAsync(string email);
    Task SendOtpAsync(string email, string otp);
    Task<bool> VerifyOtpAsync(string email, string otp);
    Task SendOtpVerifiedSuccessAsync(string email, string name);
    Task<(bool Success, string Message)> ResendOtpAsync(string email); // Add this
}