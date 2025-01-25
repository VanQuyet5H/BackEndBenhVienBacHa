using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.Template.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TemplateViewModel>))]
    public class TemplateViewModelValidator : AbstractValidator<TemplateViewModel>
    {
        public TemplateViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizationService.GetResource("Template.Ten.Required"));
            
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(localizationService.GetResource("Template.NoiDung.Required"));

            RuleFor(x => x.TemplateType)
                .NotEmpty().WithMessage(localizationService.GetResource("Template.TemplateType.Required"));

          
        }
    }
}
