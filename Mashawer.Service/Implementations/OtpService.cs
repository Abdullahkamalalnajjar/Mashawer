using System.Collections.Concurrent;

public class OtpService : IOtpService
{
    private readonly IEmailSender _emailSender;
    private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();

    public OtpService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task<string> GenerateOtpAsync(string email)
    {
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(1);
        _otpStore[email] = (otp, expiry);
        return Task.FromResult(otp);
    }

    public async Task SendOtpAsync(string email, string otp)
    {
        var subject = "Your OTP Code";
        var body = EmailBodyBuilder.GenerateEmailBody(
            "SendOtp",
            new Dictionary<string, string>
            {
                { "{{name}}", email }, // Replace with user's name if available
                { "{{otp}}", otp }
            }
        );
        await _emailSender.SendEmailAsync(email, subject, body);
    }

    public Task<bool> VerifyOtpAsync(string email, string otp)
    {
        if (_otpStore.TryGetValue(email, out var entry))
        {
            if (entry.Otp == otp && entry.Expiry > DateTime.UtcNow)
            {
                _otpStore.TryRemove(email, out _);
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }

    public async Task SendOtpVerifiedSuccessAsync(string email, string name)
    {
        var subject = "Email Verified Successfully";
        var body = EmailBodyBuilder.GenerateEmailBody(
            "OtpVerifiedSuccess",
            new Dictionary<string, string>
            {
                { "{{name}}", name }
            }
        );
        await _emailSender.SendEmailAsync(email, subject, body);
    }

    public async Task<(bool Success, string Message)> ResendOtpAsync(string email)
    {
        // Check if there's an existing OTP and it hasn't expired
        if (_otpStore.TryGetValue(email, out var existingEntry))
        {
            if (existingEntry.Expiry > DateTime.UtcNow)
            {
                return (false, "Previous OTP is still valid. Please wait before requesting a new one.");
            }
            _otpStore.TryRemove(email, out _);
        }

        // Generate and send new OTP
        var newOtp = await GenerateOtpAsync(email);
        await SendOtpAsync(email, newOtp);

        return (true, "New OTP has been sent successfully.");
    }
}