using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.PhauThuatThuThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhauThuatThuThuatVatTuThuocViewModel>))]
    public class PhauThuatThuThuatVatTuThuocViewModelValidator : AbstractValidator<PhauThuatThuThuatVatTuThuocViewModel>
    {
        public PhauThuatThuThuatVatTuThuocViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoId).NotEmpty().WithMessage(localizationService.GetResource("PhieuDieuTri.KhoId.Required"));

            RuleFor(x => x.SoLuong).NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"));

            RuleFor(x => x.VatTuThuocBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongVatTu.TenVatTu.Required"));
        }
    }
}
