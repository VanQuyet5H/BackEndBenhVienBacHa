using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;
using Camino.Services.DanToc;

namespace Camino.Api.Models.DanToc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NgayLeViewModel>))]
    public class NgayLeViewModelValidator : AbstractValidator<NgayLeViewModel>
    {
        public NgayLeViewModelValidator(ILocalizationService iLocalizationService, IDanTocService danTocService)
        {
            //RuleFor(x => x.Ma)
            //    .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required")).MustAsync(async (model, input, s) => !await danTocService.IsMaExists(
            //        !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
            //        model.Id).ConfigureAwait(false))
            //    .WithMessage(iLocalizationService.GetResource("Common.Ma.Exists")).Length(0, 20).WithMessage(iLocalizationService
            //    .GetResource("Common.Ma.Range")); 

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.TenNgayLe.Required"))
                .Length(0, 250).WithMessage(iLocalizationService
                .GetResource("Common.Ten.Range"));

            RuleFor(x => x.Thang)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.ThangNgayLe.Required"));
        }
    }

}
