using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.QuanLyTaiKhoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<QuanLyTaiKhoanNhanVienViewModel>))]
    public class QuanLyTaiKhoanNhanVienViewModelValidator : AbstractValidator<QuanLyTaiKhoanNhanVienViewModel>
    {
        public QuanLyTaiKhoanNhanVienViewModelValidator(
            ILocalizationService localizationService
        )
        {
            RuleFor(x => x.PasswordConfirm)
                .NotEmpty().WithMessage(localizationService.GetResource("NhanVien.PasswordConfirm.Required")).When(p => p.IsChangePassword)
                .Must((model, input, s) =>
                {
                    if ((model.Password != model.PasswordConfirm) && !string.IsNullOrEmpty(model.PasswordConfirm)) 
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(localizationService.GetResource("NhanVien.PasswordConfirm.DontMatchPassword")).When(p => p.IsChangePassword);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizationService.GetResource("NhanVien.Password.Required")).When(p => p.IsChangePassword);
        }
    }
}