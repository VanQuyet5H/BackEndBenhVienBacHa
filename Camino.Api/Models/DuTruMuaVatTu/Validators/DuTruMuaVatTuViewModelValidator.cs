using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauMuaDuTruVatTu;
using FluentValidation;

namespace Camino.Api.Models.DuTruMuaVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuTruMuaVatTuViewModel>))]

    public class DuTruMuaVatTuViewModelValidator : AbstractValidator<DuTruMuaVatTuViewModel>
    {
        public DuTruMuaVatTuViewModelValidator(ILocalizationService localizationService,
                                                 IYeuCauMuaDuTruVatTuService ycMuaVatTuService,
                                                 IValidator<DuTruMuaVatTuChiTietViewModel> duTruMuaVatTuChiTietViewModel)
        {
            RuleFor(x => x.KhoId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.KhoId.Required"));

            RuleFor(x => x.KyDuTruMuaDuocPhamVatTuId)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruDuocPham.KyDuTruMuaDuocPhamVatTuId.Required"));

            RuleForEach(x => x.DuTruMuaVatTuChiTiets).SetValidator(duTruMuaVatTuChiTietViewModel);
        }
    }
}
