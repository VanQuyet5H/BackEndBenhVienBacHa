using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamTiemChungViewModel>))]
    public class TiemChungViewModelValidator : AbstractValidator<YeuCauKhamTiemChungViewModel>
    {
        public TiemChungViewModelValidator (
            ILocalizationService localizationService, 
            IValidator<TiemChungYeuCauTiepNhanViewModel> tiemChungYeuCauTiepNhanValidator, 
            IValidator<TiemChungYeuCauDichVuKyThuatTiemChungViewModel> tiemChungYeuCauDichVuKyThuatTiemChungValidator,
            IValidator<TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel> tiemChungYeuCauDichVuKyThuatKhamSangLocValidator
        )
        {
            RuleFor(p => p.DichVuKyThuatBenhVienId)
               .NotNull().WithMessage(localizationService.GetResource("TiemChung.DichVuKyThuatBenhVienId.Required"))
               .NotEqual(0).WithMessage(localizationService.GetResource("TiemChung.DichVuKyThuatBenhVienId.Required"));

            RuleFor(p => p.NoiThucHienId)
                .NotNull().WithMessage(localizationService.GetResource("TiemChung.NoiThucHienId.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("TiemChung.NoiThucHienId.Required"));

            RuleFor(p => p.YeuCauTiepNhan).SetValidator(tiemChungYeuCauTiepNhanValidator);
            RuleFor(p => p.TiemChung).SetValidator(tiemChungYeuCauDichVuKyThuatTiemChungValidator);
            RuleFor(p => p.KhamSangLocTiemChung).SetValidator(tiemChungYeuCauDichVuKyThuatKhamSangLocValidator);
        }
    }
}