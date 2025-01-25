using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauHoanTraKSNK.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauTraKSNKTuTrucChiTietVo>))]
    public class YeuCauTraKSNKTuTrucChiTietVoValidator : AbstractValidator<YeuCauTraKSNKTuTrucChiTietVo>
    {
        public YeuCauTraKSNKTuTrucChiTietVoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SoLuongTra)
                .NotEmpty().WithMessage(localizationService.GetResource("GoiDichVuChiTietDichVuKhamBenh.SoLuong.Required"));
        }
    }
}
