using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.Cauhinh.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<CauHinhTheoThoiGianChiTietViewModel>))]
    public class CauHinhTheoThoiGianChiTietViewModelValidator : AbstractValidator<CauHinhTheoThoiGianChiTietViewModel>
    {
        public CauHinhTheoThoiGianChiTietViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Value.Required"));
            RuleFor(x => x.FromDate)
                .NotEmpty().WithMessage(localizationService.GetResource("DichVuGiuongBenhVien.TuNgay.Required"));
        }
    }
}
