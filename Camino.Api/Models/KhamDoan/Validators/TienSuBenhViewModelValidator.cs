using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TienSuBenh>))]
    public class TienSuBenhViewModelValidator : AbstractValidator<TienSuBenh>
    {
        public TienSuBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PhatHienNam).Must((model, s) => model.PhatHienNam < DateTime.Now)
                                      .WithMessage(localizationService.GetResource("TienSuBenh.PhatHienNam.LessThan"))
                                      .When(x => x.PhatHienNam != null);            
        }
    }
}
