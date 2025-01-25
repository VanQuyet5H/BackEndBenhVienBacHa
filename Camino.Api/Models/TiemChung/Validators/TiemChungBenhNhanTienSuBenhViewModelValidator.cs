using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiemChungBenhNhanTienSuBenhViewModel>))]
    public class TiemChungBenhNhanTienSuBenhViewModelValidator : AbstractValidator<TiemChungBenhNhanTienSuBenhViewModel>
    {
        public TiemChungBenhNhanTienSuBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(p => p.LoaiTienSuBenh)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhanTienSubenh.LoaiTienSuBenh.Required"));

            RuleFor(p => p.TenBenh)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhanTienSubenh.TenBenh.Required"))
                .MaximumLength(200).WithMessage(localizationService.GetResource("BenhNhanTienSubenh.TenBenh.Range.200"));
        }
    }
}