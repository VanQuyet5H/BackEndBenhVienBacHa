using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using Camino.Services.YeuCauMuaDuTruDuocPham;


namespace Camino.Api.Models.DuTruMuaDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuTruMuaDuocPhamChiTietViewModel>))]
    public class DuTruMuaDuocPhamChiTietViewModelValidator : AbstractValidator<DuTruMuaDuocPhamChiTietViewModel>
    {
        public DuTruMuaDuocPhamChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauMuaDuTruDuocPhamService ycMuaDuocPhamService)
        {
            RuleFor(x => x.SoLuongDuTru)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.SoLuongDuTru.Required"));

            RuleFor(x => x.SoLuongDuKienSuDung)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.SoLuongDuKienSuDungTrongKy.Required"));
        }
    }
}
