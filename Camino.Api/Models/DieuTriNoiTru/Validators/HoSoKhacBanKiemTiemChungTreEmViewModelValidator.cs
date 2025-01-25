using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoKhacBanKiemTiemChungTreEmViewModel>))]

    public class HoSoKhacBanKiemTiemChungTreEmViewModelValidator : AbstractValidator<HoSoKhacBanKiemTiemChungTreEmViewModel>
    {
        public HoSoKhacBanKiemTiemChungTreEmViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NhanVienThucHienId)
               .NotEmpty().WithMessage(localizationService.GetResource("BanKiemTiemChungTreEm.NhanVienThucHienId.Required"));
           
            RuleFor(x => x.ThoiDiemThucHien)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.ThoiDiemThucHien.Required"));

        }
    }
}
