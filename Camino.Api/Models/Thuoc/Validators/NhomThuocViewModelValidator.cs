using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.Thuocs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.Thuoc.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhomThuocViewModel>))]
    public class NhomThuocViewModelValidator : AbstractValidator<NhomThuocViewModel>
    {
        public NhomThuocViewModelValidator(ILocalizationService iLocalizationService, INhomThuocService iChucDanhService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).Length(0, 250).WithMessage(iLocalizationService
                .GetResource("Common.Ten.Range"));
            
        }
    }
}
