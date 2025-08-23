using Microsoft.AspNetCore.Identity.UI.Services;

namespace Mashawer.Core.Features.Authentication.Command.Handlers
{
    public class AuthenticatoinCommandHandler : ResponseHandler,
        IRequestHandler<SignInCommand, Response<AuthResponse>>,
        IRequestHandler<SignUpUserCommand, Response<string>>,
        IRequestHandler<CreateNewRefreshTokenCommand, Response<AuthResponse>>,
        IRequestHandler<RevokeRefreashTokenCommand, Response<string>>,
        IRequestHandler<ConfirmEmailCommand, Response<string>>,
        IRequestHandler<ResendConfirmEmailCommand, Response<string>>,
        IRequestHandler<AddProfileImageCommand, Response<string>>,
        IRequestHandler<ResetPasswordCommand, Response<string>>,
        IRequestHandler<ChangePasswordCommand, Response<string>>


    {
        private readonly IEmailSender _emailSender;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticatoinCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;

        public AuthenticatoinCommandHandler(IEmailSender emailSender, ICurrentUserService currentUserService, IFileService fileService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IAuthService authService, ILogger<AuthenticatoinCommandHandler> logger, IMapper mapper, IJwtProvider jwtProvider, UserManager<ApplicationUser> userManager, IOtpService otpService)
        {
            _emailSender = emailSender;
            _currentUserService = currentUserService;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
            _logger = logger;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
            _userManager = userManager;
            _otpService = otpService;
        }
        public async Task<Response<AuthResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                if (result.ErrorMessage == "EmailNotConfirmed")
                    return BadRequest<AuthResponse>("Email is not confirmed. Please confirm your email first.");
                return BadRequest<AuthResponse>(result.ErrorMessage);
            }

            return Success(result.Response!, "Login successful");
        }

        public async Task<Response<string>> Handle(SignUpUserCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Check if the user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser is not null)
                    return BadRequest<string>("An account with this email already exists.");

                // Create a new user
                var newUser = _mapper.Map<ApplicationUser>(request);
                var result = await _userManager.CreateAsync(newUser, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest<string>(errors);
                }
                // Generate and send OTP
                var otp = await _otpService.GenerateOtpAsync(newUser.Email);
                await _otpService.SendOtpAsync(newUser.Email, otp);
                // Create a wallet for the user with an initial balance of 0
                var wallet = new Wallet
                {
                    UserId = newUser.Id,
                    Balance = 0
                };
                await _unitOfWork.Wallets.AddAsync(wallet, cancellationToken);
                await _unitOfWork.CompeleteAsync();
                // Commit the transaction
                await transaction.CommitAsync();

                return Success("Account created successfully. Please check your email for the OTP to verify your account.", new { id = newUser.Id });
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of any failure
                await transaction.RollbackAsync();
                // Log the exception if needed (not shown here)
                return BadRequest<string>(ex.Message.ToString());
            }
        }

        public async Task<Response<AuthResponse>> Handle(CreateNewRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var newRefreshToken = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken);
            if (newRefreshToken is null)
                return BadRequest<AuthResponse>("Invalid token or refresh token");
            return Success<AuthResponse>(newRefreshToken, "Refresh token created successfully");

        }

        public async Task<Response<string>> Handle(RevokeRefreashTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken);
            if (!result)
                return BadRequest<string>("Invalid token or refresh token");
            return Success<string>("Refresh token revoked successfully", "Refresh token revoked successfully");
        }

        public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ConfirmEmailAsync(request.UserId, request.Code);

            return result switch
            {
                "Email confirmed successfully" => Success<string>("Email Confirm Successfully"),
                "Email already confirmed" => BadRequest<string>("Email already confirmed"),
                "InvalidCode" => BadRequest<string>("Invalid confirmation code."),
                _ => BadRequest<string>($"Email confirmation failed: {result}")
            };
        }

        public async Task<Response<string>> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ResendConfirmEmailAsync(request.Email);

            return result switch
            {
                "Good" => Success(""),
                "Daplicated Confirmation" => BadRequest<string>("Daplicated Confirmation"),
                "Code Has been resend" => Success("Code has been resend to Confirmation"),
                _ => BadRequest<string>("Resend Code Failed")
            };
        }
        private async Task SendConfirmationEmail(ApplicationUser user, string code)
        {
            // استخدام عنوان الباكيند مباشرة
            var request = _httpContextAccessor.HttpContext?.Request;
            var origin = $"{request?.Scheme}://{request?.Host}";

            // توليد رابط تأكيد البريد
            var encodedCode = HttpUtility.UrlEncode(code);
            var confirmationUrl = $"{origin}/Api/V1/Authentication/ConfirmEmail?userId={user.Id}&code={encodedCode}";

            // إنشاء جسم الإيميل
            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
                new Dictionary<string, string> {
            {"{{name}}" , user.FullName },
            {"{{action_url}}" , confirmationUrl }
                });

            await _emailSender.SendEmailAsync(user.Email!, "AGECS Licensing: تأكيد البريد الإلكتروني", emailBody);
        }

        public async Task<Response<string>> VerifyOtpAndConfirmEmail(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest<string>("User not found.");

            var isValid = await _otpService.VerifyOtpAsync(email, otp);
            if (!isValid)
                return BadRequest<string>("Invalid or expired OTP.");

            // Confirm the user's email
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Success("Email confirmed successfully.");

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest<string>(errors);
        }

        public async Task<Response<string>> Handle(AddProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound<string>("User not found.");
            }
            try
            {
                // Assuming you have a method to save the image and return the URL
                var imageUrl = await _fileService.UploadImage("ProfileImages", request.Image);
                user.ProfilePictureUrl = imageUrl;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Success("Profile image updated successfully.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest<string>(errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile image for user {UserId}", request.UserId);
                return BadRequest<string>("An error occurred while updating the profile image.");
            }
        }


        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return NotFound<string>("المستخدم غير موجود");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest<string>("فشل في إعادة تعيين كلمة المرور");

            var subject = "Mashawer - Password Reset Confirmation";
            var body = $"<p>Hi,</p><p>Your password has been successfully reset. If you did not request this change, please contact support immediately.</p>";
            var emailBody = EmailBodyBuilder.GenerateEmailBody("SuccessfullyChangePassword",
        new Dictionary<string, string> {

                    {"{{name}}" , user.FullName }

    });

            try
            {
                await _emailSender.SendEmailAsync(request.Email, subject, emailBody);
                return Success("تم تغيير كلمة المرور بنجاح. تم إرسال رسالة تأكيد إلى بريدك الإلكتروني.");
            }
            catch (Exception ex)
            {
                return BadRequest<string>($"فشل في إرسال البريد الإلكتروني: {ex.Message}");
            }
        }
        public async Task<Response<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound<string>("User not found");

            var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.OldPassword);
            if (!isOldPasswordCorrect)
                return UnprocessableEntity<string>("Old password is incorrect");

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (result.Succeeded)
                return Success("Password changed successfully");

            // في حالة وجود أخطاء أخرى من Identity
            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return UnprocessableEntity<string>(errorMessage);
        }

    }
}






