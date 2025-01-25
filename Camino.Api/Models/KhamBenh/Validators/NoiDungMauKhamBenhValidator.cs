using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiDungMauKhamBenhViewModel>))]
    public class NoiDungMauKhamBenhViewModelValidator : AbstractValidator<NoiDungMauKhamBenhViewModel>
    {
        public NoiDungMauKhamBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Ma).NotEmpty().WithMessage(localizationService.GetResource("NoiDungMauKhamBenh.Ma.Required"));
            RuleFor(x => x.Ten).NotEmpty().WithMessage(localizationService.GetResource("NoiDungMauKhamBenh.Ten.Required"));
            RuleFor(x => x.NoiDung).NotEmpty().WithMessage(localizationService.GetResource("NoiDungMauKhamBenh.NoiDung.Required"));
        }
    }
}
