using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;

namespace Camino.Api.Models.VatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<VatTuViewModel>))]

    public class VatTuViewModelValidator : AbstractValidator<VatTuViewModel>
    {
        public VatTuViewModelValidator(ILocalizationService localizationService , IValidator<VatTuBenhViewModel> vatTuBenhViewModelValidator
            )
        {

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));
            RuleFor(x => x.Ma)
              .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"));

            RuleFor(x => x.NhomVatTuId)
             .NotEmpty().WithMessage(localizationService.GetResource("VatTu.NhomVatTuId.Required"));
            RuleFor(x => x.TyLeBaoHiemThanhToan)
            .NotEmpty().WithMessage(localizationService.GetResource("VatTu.TyLeBaoHiemThanhToan.Required"));
            RuleFor(p => p.VatTuBenhViewModel).SetValidator(vatTuBenhViewModelValidator).When(p=>p.SuDungVatTuBenhVien == true) ;
        }
    }
}
