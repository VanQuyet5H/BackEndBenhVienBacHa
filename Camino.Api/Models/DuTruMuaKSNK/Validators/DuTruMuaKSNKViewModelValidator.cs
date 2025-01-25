using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauMuaDuTruVatTu;
using FluentValidation;

namespace Camino.Api.Models.DuTruMuaKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuTruMuaKSNKViewModel>))]

    public class DuTruMuaVatTuViewModelValidator : AbstractValidator<DuTruMuaKSNKViewModel>
    {
        public DuTruMuaVatTuViewModelValidator(ILocalizationService localizationService,
                                                 IYeuCauMuaDuTruVatTuService ycMuaVatTuService,
                                                 IValidator<DuTruMuaKSNKChiTietViewModel> duTruMuaVatTuChiTietViewModel)
        {
            RuleFor(x => x.KhoId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.KhoId.Required"));

            RuleFor(x => x.KyDuTruMuaDuocPhamVatTuId)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.KyDuTruMuaDuocPhamVatTuId.Required"));

            RuleForEach(x => x.DuTruMuaVatTuChiTiets).SetValidator(duTruMuaVatTuChiTietViewModel);
        }
    }
}
