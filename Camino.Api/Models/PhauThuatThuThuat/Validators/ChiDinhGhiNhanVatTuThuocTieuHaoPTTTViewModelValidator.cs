using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.PhauThuatThuThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChiDinhGhiNhanVatTuThuocTieuHaoPTTTViewModel>))]
    public class ChiDinhGhiNhanVatTuThuocTieuHaoPTTTViewModelValidator : AbstractValidator<ChiDinhGhiNhanVatTuThuocTieuHaoPTTTViewModel>
    {
        public ChiDinhGhiNhanVatTuThuocTieuHaoPTTTViewModelValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.DichVuChiDinhId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("GhiNhanVatTuThuoc.DichVuChiDinhId.Required"));

            RuleFor(x => x.KhoId)
                .NotEmpty().WithMessage(localizationService.GetResource("GhiNhanVatTuThuoc.KhoId.Required"));

            RuleFor(x => x.DichVuGhiNhanId)
                .NotEmpty().WithMessage(localizationService.GetResource("GhiNhanVatTuThuoc.DichVuGhiNhanId.Required"));

            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("GhiNhanVatTuThuoc.SoLuong.Required"));

            RuleFor(x => x.strDonViTinh)
                .NotEmpty().WithMessage(localizationService.GetResource("GhiNhanVatTuThuoc.DVT.Required"));
        }
    }
}