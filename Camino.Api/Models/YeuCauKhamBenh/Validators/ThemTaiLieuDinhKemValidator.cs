using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThemTaiLieu>))]
    public class ThemTaiLieuDinhKemValidator : AbstractValidator<ThemTaiLieu>
    {
        public ThemTaiLieuDinhKemValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TaiLieu)
                .NotEmpty().WithMessage(localizationService.GetResource("themTaiLieu.TaiLieu.Required"));
            RuleFor(x => x.LoaiId)
                .NotEmpty().WithMessage(localizationService.GetResource("themTaiLieu.LoaiId.Required"));
        }
    }
}