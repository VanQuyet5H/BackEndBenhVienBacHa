using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauHoanTraVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauTraVatTuTuTrucChiTietVo>))]
    public class YeuCauTraVatTuTuTrucChiTietVoValidator : AbstractValidator<YeuCauTraVatTuTuTrucChiTietVo>
    {
        public YeuCauTraVatTuTuTrucChiTietVoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SoLuongTra)
                .NotEmpty().WithMessage(localizationService.GetResource("GoiDichVuChiTietDichVuKhamBenh.SoLuong.Required"));
        }
    }
}
