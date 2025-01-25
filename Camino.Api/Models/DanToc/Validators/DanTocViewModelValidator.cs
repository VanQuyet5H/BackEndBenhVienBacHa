using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.ChucDanh;
using Camino.Services.Localization;
using Camino.Services.DanToc;

namespace Camino.Api.Models.DanToc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DanTocViewModel>))]
    public class DanTocViewModelValidator : AbstractValidator<DanTocViewModel>
    {
        public DanTocViewModelValidator(ILocalizationService iLocalizationService, IDanTocService danTocService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required")).MustAsync(async (model, input, s) => !await danTocService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(iLocalizationService.GetResource("Common.Ma.Exists")).Length(0, 20).WithMessage(iLocalizationService
                .GetResource("Common.Ma.Range")); 

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).Length(0, 250).WithMessage(iLocalizationService
                .GetResource("Common.Ten.Range")); 
            RuleFor(x => x.QuocGiaId)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.QuocGiaId.Required"));
        }
    }
    
}
