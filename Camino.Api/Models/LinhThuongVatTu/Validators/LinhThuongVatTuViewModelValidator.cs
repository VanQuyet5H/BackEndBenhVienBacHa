using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhThuongVatTuViewModel>))]

    public class LinhThuongVatTuViewModelValidator : AbstractValidator<LinhThuongVatTuViewModel>
    {
        public LinhThuongVatTuViewModelValidator(ILocalizationService localizationService, IValidator<LinhThuongVatTuChiTietViewModel> linhThuongVatTuChiTietValidator)
        {
            RuleFor(x => x.KhoNhapId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            RuleForEach(x => x.YeuCauLinhVatTuChiTiets).SetValidator(linhThuongVatTuChiTietValidator);

        }
    }
}
