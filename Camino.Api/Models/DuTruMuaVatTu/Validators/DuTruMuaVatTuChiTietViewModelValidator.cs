using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using Camino.Services.YeuCauMuaDuTruVatTu;

namespace Camino.Api.Models.DuTruMuaVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuTruMuaVatTuChiTietViewModel>))]
    public class DuTruMuaVatTuChiTietViewModelValidator : AbstractValidator<DuTruMuaVatTuChiTietViewModel>
    {
        public DuTruMuaVatTuChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauMuaDuTruVatTuService ycMuaVatTuService)
        {
            RuleFor(x => x.SoLuongDuTru)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruVatTu.SoLuongDuTru.Required"));

            RuleFor(x => x.SoLuongDuKienSuDung)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruVatTu.SoLuongDuKienSuDungTrongKy.Required"));
        }
    }
}
