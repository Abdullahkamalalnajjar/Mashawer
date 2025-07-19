namespace Mashawer.Core.Features.Users.Commands.Validators
{
    public class EditApplicationUserCommandValidator : AbstractValidator<EditApplicationUserCommand>
    {
        public EditApplicationUserCommandValidator()
        {
            EditUserCommandValidator();
        }
        public void EditUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();

        }
    }
}
