using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThemDichVuKyThuatVo>))]
    public class ThemDichVuKyThuatVoValidator : AbstractValidator<ThemDichVuKyThuatVo>
    {
        public ThemDichVuKyThuatVoValidator(ILocalizationService localizationService)
        {

            RuleFor(x => x.MaDichVuId)
                .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuKyThuatModel.MaDichVuId.Required"));
            RuleFor(x => x.NoiThucHienId)
                .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuKyThuatModel.NoiThucHienId.Required"));
            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuKyThuatModel.SoLuong.Required"));
        }
    }
}