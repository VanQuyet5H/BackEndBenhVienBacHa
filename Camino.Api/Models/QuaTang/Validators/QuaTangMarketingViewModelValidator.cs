using Camino.Api.Models.Marketing;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.QuaTang;
using FluentValidation;

namespace Camino.Api.Models.QuaTang.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<QuaTangMarketingViewModel>))]
    public class QuaTangMarketingViewModelValidator : AbstractValidator<QuaTangMarketingViewModel>
    {
        public QuaTangMarketingViewModelValidator(ILocalizationService localizationService, IQuaTangService quaTangService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Ten)
                .NotNull().WithMessage(localizationService.GetResource("Marketing.QuaTang.Ten.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("Marketing.QuaTang.Ten.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) =>
                 {
                     var checkExistsTen = await quaTangService.IsTenExists(ten, request.Id);
                     return checkExistsTen;
                 })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Marketing.QuaTang.Ten.Range.250"));

            RuleFor(p => p.DonViTinh)
                .NotNull().WithMessage(localizationService.GetResource("Marketing.QuaTang.DonViTinh.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("Marketing.QuaTang.DonViTinh.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Marketing.QuaTang.DonViTinh.Range.50"));

            RuleFor(p => p.MoTa)
                .Length(0, 4000).WithMessage(localizationService.GetResource("Marketing.QuaTang.MoTa.Range.4000"));

            RuleFor(p => p.HieuLuc)
                .NotNull().WithMessage(localizationService.GetResource("Marketing.QuaTang.HieuLuc.Required"));
        }
    }
}
