using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauHoanTraKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauHoanTraKSNKTuTrucViewModel>))]
    public class YeuCauHoanTraKSNKTuTrucViewModelValidator : AbstractValidator<YeuCauHoanTraKSNKTuTrucViewModel>
    {
        public YeuCauHoanTraKSNKTuTrucViewModelValidator(ILocalizationService localizationService, 
            IValidator<YeuCauTraKSNKTuTrucChiTietVo> chiTietValidator)
        {
            RuleFor(x => x.KhoXuatId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.KhoXuatId.Required"));
            RuleFor(x => x.KhoNhapId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.KhoNhapId.Required"));
            RuleFor(x => x.NhanVienYeuCauId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.NhanVienYeuCauId.Required"));
            RuleFor(x => x.NgayYeuCau)
                .NotEmpty().WithMessage(localizationService.GetResource("HoanTra.NgayYeuCau.Required"));

            RuleForEach(x => x.YeuCauHoanTraKSNKChiTiets).SetValidator(chiTietValidator);
        }
    }
}
