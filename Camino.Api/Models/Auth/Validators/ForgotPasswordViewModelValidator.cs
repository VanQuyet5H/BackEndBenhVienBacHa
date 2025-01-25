using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.Auth.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ForgotPasswordViewModel>))]
    public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.ForgotPasswordStage)
                .Must((model, s) => Enum.IsDefined(typeof(EnumForgotPasswordStage), model.ForgotPasswordStage.ToString())).WithMessage(localizationService.GetResource("Auth.ForgotPassword.ForgotPasswordStage.Invalid"));

            RuleFor(x => x.UserName) //Sửa lại Resource khi thêm sđt
                .NotEmpty().WithMessage(localizationService.GetResource("Auth.ForgotPassword.Email.Required"))
                .Length(0, 200).WithMessage(localizationService.GetResource("Auth.ForgotPassword.Email.Range"))
                .Must((model, s) => model.UserNameType == EnumUserNameType.IsEmail).WithMessage(localizationService.GetResource("Auth.ForgotPassword.Email.Invalid"))
                .When(model => model.ForgotPasswordStage == EnumForgotPasswordStage.IsForget, ApplyConditionTo.AllValidators);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Password.Required"))
                .MinimumLength(6).WithMessage(localizationService.GetResource("Common.Password.Range.Min"))
                .Length(6, 100).WithMessage(localizationService.GetResource("Common.Password.Range"))
                .When(model => model.ForgotPasswordStage == EnumForgotPasswordStage.IsReset, ApplyConditionTo.AllValidators);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.ConfirmPassword.Required"))
                .MinimumLength(6).WithMessage(localizationService.GetResource("Common.ConfirmPassword.Range.Min"))
                .Length(6, 100).WithMessage(localizationService.GetResource("Common.ConfirmPassword.Range"))
                .Must((model, s) => model.Password == model.ConfirmPassword).When(model => !string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.ConfirmPassword) && model.ForgotPasswordStage == EnumForgotPasswordStage.IsReset, ApplyConditionTo.CurrentValidator).WithMessage(localizationService.GetResource("Common.ConfirmPassword.DontMatchPassword"))
                .When(model => model.ForgotPasswordStage == EnumForgotPasswordStage.IsReset, ApplyConditionTo.AllValidators);
        }
    }
}