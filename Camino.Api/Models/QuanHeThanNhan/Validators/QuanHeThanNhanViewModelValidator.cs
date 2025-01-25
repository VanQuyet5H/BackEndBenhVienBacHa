using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.QuanHeThanNhan;
using FluentValidation;

namespace Camino.Api.Models.QuanHeThanNhan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<QuanHeThanNhanViewModel>))]
    public class QuanHeThanNhanViewModelValidator : AbstractValidator<QuanHeThanNhanViewModel>
    {
        public QuanHeThanNhanViewModelValidator(
            ILocalizationService localizationService,
            IQuanHeThanNhanService quanHeThanNhanService
            )
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"));

            RuleFor(x => x.TenVietTat)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenVietTat.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
        }
    }
}
