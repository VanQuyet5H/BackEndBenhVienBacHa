using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Services.Localization;
using FluentValidation;


namespace Camino.Api.Models.YeuCauHoanTraDuocPham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauTraDuocPhamTuTrucChiTietVo>))]
    public class YeuCauTraDuocPhamTuTrucChiTietVoValidator : AbstractValidator<YeuCauTraDuocPhamTuTrucChiTietVo>
    {
        public YeuCauTraDuocPhamTuTrucChiTietVoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SoLuongTra)
                .NotEmpty().WithMessage(localizationService.GetResource("GoiDichVuChiTietDichVuKhamBenh.SoLuong.Required"));
        }
    }
}
