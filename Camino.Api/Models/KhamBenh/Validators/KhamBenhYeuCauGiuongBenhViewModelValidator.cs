using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhYeuCauGiuongBenhViewModel>))]
    public class KhamBenhYeuCauGiuongBenhViewModelValidator : AbstractValidator<KhamBenhYeuCauGiuongBenhViewModel>
    {
        public KhamBenhYeuCauGiuongBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NoiThucHienId)
                   .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.PhongId.Required"));

            RuleFor(x => x.DichVuGiuongBenhVienId)
                  .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.DichVuGiuongBenhVienId.Required"));

            RuleFor(x => x.GiuongBenhId)
                .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.GiuongBenhId.Required"));

            RuleFor(x => x.ThoiDiemBatDauSuDung)
                .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.ThoiDiemBatDauSuDung.Required"));
        }
    }
}
