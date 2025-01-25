using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThemDichVuKhamBenhVo>))]
    public class ThemDichVuKhamBenhVoValidator : AbstractValidator<ThemDichVuKhamBenhVo>
    {
        public ThemDichVuKhamBenhVoValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.MaDichVuGoiId)
            //    .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuModel.MaDichVuGoiId.Required")).When(x => x.LaGoi);
            RuleFor(x => x.MaDichVuId)
                .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuModel.MaDichVuId.Required")).When(x => x.LaGoi == false);
            RuleFor(x => x.LoaiGiaId)
                .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuModel.LoaiGiaId.Required")).When(x => x.LaGoi == false);
            RuleFor(x => x.NoiThucHienId)
                .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuModel.NoiThucHienId.Required")).When(x => x.LaGoi == false);
            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("themChiDinhDichVuModel.SoLuong.Required")).When(x => x.LaGoi == false);
        }
    }
}