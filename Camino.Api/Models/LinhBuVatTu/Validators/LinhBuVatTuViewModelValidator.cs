using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhBuVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhBuVatTuViewModel>))]
    public class LinhBuVatTuViewModelValidator : AbstractValidator<LinhBuVatTuViewModel>
    {
        public LinhBuVatTuViewModelValidator(ILocalizationService localizationService, IValidator<YeuCauVatTuBenhVienViewModel> linhThuongVatTuChiTietValidator)
        {
            RuleFor(x => x.KhoNhapId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            RuleForEach(x => x.YeuCauVatTuBenhViens).SetValidator(linhThuongVatTuChiTietValidator);

        }
    }
}
