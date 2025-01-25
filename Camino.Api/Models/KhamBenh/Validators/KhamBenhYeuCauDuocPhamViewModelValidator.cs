using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.KhamBenh.Validators
{

    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhYeuCauDuocPhamViewModel>))]
    public class KhamBenhYeuCauDuocPhamViewModelValidator : AbstractValidator<KhamBenhYeuCauDuocPhamViewModel>
    {
        public KhamBenhYeuCauDuocPhamViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DuocPhamBenhVienId)
                   .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.DuocPhamBenhVienId.Required"));

            RuleFor(x => x.SoLuong)
                  .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.SoLuong.Required"));

            //RuleFor(x => x.NoiCapThuocId)
            //     .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.NoiCapThuocId.Required"));

        }
    }
}
