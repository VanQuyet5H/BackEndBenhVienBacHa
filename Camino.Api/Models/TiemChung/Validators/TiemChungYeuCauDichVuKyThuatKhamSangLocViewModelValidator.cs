using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel>))]
    public class TiemChungYeuCauDichVuKyThuatKhamSangLocViewModelValidator : AbstractValidator<TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel>
    {
        public TiemChungYeuCauDichVuKyThuatKhamSangLocViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(p => p.LyDoDeNghi)
                .MaximumLength(1000).WithMessage(localizationService.GetResource("TiemChung.LyDoDeNghi.Range.1000"));

            RuleFor(p => p.GhiChuKetLuan)
                .MaximumLength(1000).WithMessage(localizationService.GetResource("TiemChung.GhiChuKetLuan.Range.1000"));

            RuleFor(p => p.KetLuan)
                .NotNull().WithMessage(localizationService.GetResource("TiemChung.KetLuan.Required"));

            RuleFor(p => p.GhiChuHenTiemMuiTiepTheo)
                .MaximumLength(1000).WithMessage(localizationService.GetResource("TiemChung.GhiChuHenTiemMuiTiepTheo.Range.1000"));
        }
    }
}