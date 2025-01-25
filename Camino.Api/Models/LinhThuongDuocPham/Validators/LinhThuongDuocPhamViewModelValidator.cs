using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhThuongDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhThuongDuocPhamViewModel>))]

    public class LinhThuongDuocPhamViewModelValidator : AbstractValidator<LinhThuongDuocPhamViewModel>
    {
        public LinhThuongDuocPhamViewModelValidator(ILocalizationService localizationService, IValidator<LinhThuongDuocPhamChiTietViewModel> linhThuongDuocPhamChiTietValidator)
        {
            RuleFor(x => x.KhoNhapId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            RuleForEach(x => x.YeuCauLinhDuocPhamChiTiets).SetValidator(linhThuongDuocPhamChiTietValidator);

        }
    }
}
