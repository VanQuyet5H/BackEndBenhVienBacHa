using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.GoiDichVuMarketings;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.GoiDichVu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GoiDvMarketingViewModel>))]
    public class GoiDichVuMarketingViewModelValidator : AbstractValidator<GoiDvMarketingViewModel>
    {
        public GoiDichVuMarketingViewModelValidator(ILocalizationService localizationService, IGoiDvMarketingService goiDichVuService)
        {
            RuleFor(x => x.TenGoiDv)
                .MustAsync(async (model, input, s) => !await goiDichVuService.IsTenExists(
                    model.LoaiGoiDichVu,!string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));
        }
    }
}
