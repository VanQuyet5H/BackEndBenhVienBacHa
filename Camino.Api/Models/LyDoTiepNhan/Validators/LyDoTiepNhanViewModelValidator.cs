using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.LyDoTiepNhan;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LyDoTiepNhan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LyDoTiepNhanViewModel>))]

    public class LyDoTiepNhanViewModelValidator : AbstractValidator<LyDoTiepNhanViewModel>
    {
        public LyDoTiepNhanViewModelValidator(ILocalizationService localizationService, ILyDoTiepNhanService _lyDoTiepNhanService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, tenVi, id) =>
                {
                    var checkExistsTen = await _lyDoTiepNhanService.IsTenExists(tenVi, request.Id, request.LyDoTiepNhanChaId);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));
        }
    }
}
