using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DichVuXetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KetNoiChiSoXetNghiemViewModel>))]
    public class KetNoiChiSoXetNghiemViewModelValidator : AbstractValidator<KetNoiChiSoXetNghiemViewModel>
    {
        public KetNoiChiSoXetNghiemViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.MaChiSo)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuXetNghiem.MaChiSo.Required"))
                .When(x => (!string.IsNullOrEmpty(x.TenKetNoi) && x.MauMayXetNghiemId == null) || (string.IsNullOrEmpty(x.TenKetNoi) && x.MauMayXetNghiemId != null));

            RuleFor(x => x.TenKetNoi)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuXetNghiem.TenChiSo.Required"))
                .When(x => (!string.IsNullOrEmpty(x.MaChiSo) && x.MauMayXetNghiemId == null) || (string.IsNullOrEmpty(x.MaChiSo) && x.MauMayXetNghiemId != null));

            RuleFor(x => x.MauMayXetNghiemId)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuXetNghiem.MauMayXetNghiem.Required"))
                .When(x => (!string.IsNullOrEmpty(x.MaChiSo) && !string.IsNullOrEmpty(x.TenKetNoi)) || (!string.IsNullOrEmpty(x.MaChiSo) && string.IsNullOrEmpty(x.TenKetNoi)) || (string.IsNullOrEmpty(x.MaChiSo) && !string.IsNullOrEmpty(x.TenKetNoi)));
        }
    }
}
