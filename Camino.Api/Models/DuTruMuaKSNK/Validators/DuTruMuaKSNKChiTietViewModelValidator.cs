using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using Camino.Services.YeuCauMuaDuTruVatTu;

namespace Camino.Api.Models.DuTruMuaKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuTruMuaKSNKChiTietViewModel>))]
    public class DuTruMuaKSNKChiTietViewModelValidator : AbstractValidator<DuTruMuaKSNKChiTietViewModel>
    {
        public DuTruMuaKSNKChiTietViewModelValidator(ILocalizationService localizationService,
            IYeuCauMuaDuTruVatTuService ycMuaVatTuService)
        {
            RuleFor(x => x.SoLuongDuTru)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruVatTu.SoLuongDuTru.Required"));

            RuleFor(x => x.SoLuongDuKienSuDung)
              .NotEmpty().WithMessage(localizationService.GetResource("YeuCauMuaDuTruVatTu.SoLuongDuKienSuDungTrongKy.Required"));
        }
    }
}
