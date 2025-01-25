using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NgheCongViecTruocDay>))]
    public class NgheCongViecTruocDayViewModelValidator : AbstractValidator<NgheCongViecTruocDay>
    {
        public NgheCongViecTruocDayViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.TuNgay).NotEmpty().WithMessage(localizationService.GetResource("NgheCongViecTruocDay.TuNgay.Required"));
            RuleFor(x => x.DenNgay).Must((model, s) => model.TuNgay <= model.DenNgay)
                                     .WithMessage(localizationService.GetResource("NgheCongViecTruocDay.DenNgay.GreaterThan"))
                                     .When(x => x.DenNgay != null);
        }
    }
}
