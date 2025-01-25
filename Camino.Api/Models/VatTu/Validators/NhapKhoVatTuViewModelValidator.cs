using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;

namespace Camino.Api.Models.VatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhapKhoVatTuTonKhoViewModel>))]

    public class NhapKhoVatTuViewModelValidator : AbstractValidator<NhapKhoVatTuTonKhoViewModel>
    {

        public NhapKhoVatTuViewModelValidator(ILocalizationService localizationService, IValidator<NhapKhoVatTuChiTietTonKhoViewModel> nhapKhoVatTuChiTietValidator)
        {
            RuleFor(x => x.VatTuId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongVatTu.TenVatTu.Required"));

            RuleForEach(x => x.NhapKhoVatTuChiTiets).SetValidator(nhapKhoVatTuChiTietValidator);
        }
    }
}
