using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.Users.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<RoleViewModel>))]
    public class RoleViewModelValidator : AbstractValidator<RoleViewModel>
    {
        public RoleViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizationService.GetResource("Role.Name.Required"));
        }
    }
}