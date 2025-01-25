using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using Camino.Core.Domain;

namespace Camino.Api.Models.Template.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<MessagingTemplateViewModel>))]
    public class MessagingTemplateViewModelValidator : AbstractValidator<MessagingTemplateViewModel>
    {
        public MessagingTemplateViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizationService.GetResource("Template.Ten.Required"));

            RuleFor(x => x.Title)
                .Must((model, d) => model.MessagingType != Enums.MessagingType.Email || (model.MessagingType == Enums.MessagingType.Email && !string.IsNullOrEmpty(model.Title)))
                .WithMessage(localizationService.GetResource("Template.TieuDe.Required"));

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(localizationService.GetResource("Template.NoiDung.Required"));

            //RuleFor(x => x.l)
            //    .NotEmpty().WithMessage(localizationService.GetResource("Template.TemplateType.Required"));


        }
    }
}
