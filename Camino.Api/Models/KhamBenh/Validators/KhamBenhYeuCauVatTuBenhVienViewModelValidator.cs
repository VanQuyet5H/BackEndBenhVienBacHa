using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhYeuCauVatTuBenhVienViewModel>))]
    public class KhamBenhYeuCauVatTuBenhVienViewModelValidator : AbstractValidator<KhamBenhYeuCauVatTuBenhVienViewModel>
    {
        public KhamBenhYeuCauVatTuBenhVienViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.VatTuBenhVienId)
                   .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.VatTuBenhVienId.Required"));

            //RuleFor(x => x.NoiChiDinhId)
            //      .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.NoiChiDinhId.Required"));

            RuleFor(x => x.SoLuong)
                 .NotEmpty().WithMessage(localizationService.GetResource("ycdvcls.SoLuong.Required"));


        }
    }
}
