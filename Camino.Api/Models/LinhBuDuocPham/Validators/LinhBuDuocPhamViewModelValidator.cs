using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LinhBuDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LinhBuDuocPhamViewModel>))]
    public class LinhBuDuocPhamViewModelValidator : AbstractValidator<LinhBuDuocPhamViewModel>
    {
        public LinhBuDuocPhamViewModelValidator(ILocalizationService localizationService, IValidator<YeuCauDuocPhamBenhVienViewModel> linhThuongDuocPhamChiTietValidator)
        {
            RuleFor(x => x.KhoNhapId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhVeKho.Required"));

            RuleFor(x => x.KhoXuatId)
               .NotEmpty().WithMessage(localizationService.GetResource("LinhThuongDuocPham.LinhTuKho.Required"));

            RuleForEach(x => x.YeuCauDuocPhamBenhViens).SetValidator(linhThuongDuocPhamChiTietValidator);

        }
    }
}
