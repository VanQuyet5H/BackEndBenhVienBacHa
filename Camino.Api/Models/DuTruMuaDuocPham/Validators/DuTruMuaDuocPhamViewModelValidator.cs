using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauMuaDuTruDuocPham;
using FluentValidation;

namespace Camino.Api.Models.DuTruMuaDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuTruMuaDuocPhamViewModel>))]

    public class DuTruMuaDuocPhamViewModelValidator : AbstractValidator<DuTruMuaDuocPhamViewModel>
    {
        public DuTruMuaDuocPhamViewModelValidator(ILocalizationService localizationService, 
                                                  IYeuCauMuaDuTruDuocPhamService ycMuaDuocPhamService, 
                                                  IValidator<DuTruMuaDuocPhamChiTietViewModel> duTruMuaDuocPhamChiTietViewModel)
        {
            RuleFor(x => x.NhomDuocPhamDuTru)
               .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.NhomDuocPhamDuTru.Required"));

            RuleFor(x => x.KhoId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.KhoId.Required"));

            RuleFor(x => x.KyDuTruMuaDuocPhamVatTuId)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.KyDuTruMuaDuocPhamVatTuId.Required"));

            RuleForEach(x => x.DuTruMuaDuocPhamChiTiets).SetValidator(duTruMuaDuocPhamChiTietViewModel);
        }
    }
}
