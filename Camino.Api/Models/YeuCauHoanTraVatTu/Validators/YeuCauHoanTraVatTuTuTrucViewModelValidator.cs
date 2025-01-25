using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauHoanTraVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauHoanTraVatTuTuTrucViewModel>))]
    public class YeuCauHoanTraVatTuTuTrucViewModelValidator : AbstractValidator<YeuCauHoanTraVatTuTuTrucViewModel>
    {
        public YeuCauHoanTraVatTuTuTrucViewModelValidator(ILocalizationService localizationService, IValidator<YeuCauTraVatTuTuTrucChiTietVo> chiTietValidator)
        {
            RuleFor(x => x.KhoXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.KhoXuatId.Required"));
            RuleFor(x => x.KhoNhapId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.KhoNhapId.Required"));
            RuleFor(x => x.NhanVienYeuCauId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.NhanVienYeuCauId.Required"));
            RuleFor(x => x.NgayYeuCau)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.NgayYeuCau.Required"));

            RuleForEach(x => x.YeuCauHoanTraVatTuChiTiets).SetValidator(chiTietValidator);
        }
    }
}
