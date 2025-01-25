using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.Auth.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoginViewModel>))]
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(localizationService.GetResource("DangNhap.Username.Required"))
                .MaximumLength(200).WithMessage(localizationService.GetResource("DangNhap.Username.Range"));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Password.Required"))
                .MinimumLength(6).WithMessage(localizationService.GetResource("Common.Password.Range.Min"))
                .MaximumLength(100).WithMessage(localizationService.GetResource("Common.Password.Range"));
        }
    }
}
