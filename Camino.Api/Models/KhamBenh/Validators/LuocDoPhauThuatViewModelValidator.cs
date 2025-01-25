using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LuocDoPhauThuatViewModel>))]
    public class LuocDoPhauThuatViewModelValidator : AbstractValidator<LuocDoPhauThuatViewModel>
    {
        public LuocDoPhauThuatViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.MoTa)
                .Length(0, 1000).WithMessage(localizationService.GetResource("Common.MoTa.Range.1000"));
        }
    }
}
