using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.HoanTra.HoanTraValidators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetHoanTraDuocPhamViewModel>))]
    public class DuyetHoanTraDuocPhamViewModelValidator : AbstractValidator<DuyetHoanTraDuocPhamViewModel>
    {
        public DuyetHoanTraDuocPhamViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.NhanVienNhanId)
                .NotNull().WithMessage(localizationService.GetResource("DuyetHoanTraDuocPham.NguoiNhan.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DuyetHoanTraDuocPham.NguoiNhan.Required"));

            RuleFor(p => p.NhanVienTraId)
                .NotNull().WithMessage(localizationService.GetResource("DuyetHoanTraDuocPham.NguoiTra.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DuyetHoanTraDuocPham.NguoiTra.Required"));
        }
    }
}
