using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<userConfirmViewModel>))]
    public class userConfirmViewModelValidator : AbstractValidator<userConfirmViewModel>
    {
        public userConfirmViewModelValidator(ILocalizationService iLocalizationService)
        {
            RuleFor(x => x.userName)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("userName.Required"));
            RuleFor(x => x.pass)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("pass.Required"));
        }
    }
}
