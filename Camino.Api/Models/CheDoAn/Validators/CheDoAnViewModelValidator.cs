using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.CheDoAn;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.CheDoAn.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CheDoAnViewModel>))]
    public class CheDoAnViewModelValidator : AbstractValidator<CheDoAnViewModel>
    {
        public CheDoAnViewModelValidator(ILocalizationService iLocalizationService, ICheDoAnService  iCheDoAnService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) => {
                var val = await iCheDoAnService.IsTenExists(ten, request.Id);
                return val;
            }).WithMessage(iLocalizationService.GetResource("Common.Ten.Exists")).Length(0, 500).WithMessage(iLocalizationService
                .GetResource("Common.Ten.Range"));
            RuleFor(x => x.KyHieu)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.KyHieu.Required")).MustAsync(async (request, ma, id) => {
                var val = await iCheDoAnService.IsMaExists(ma, request.Id);
                return val;
            }).WithMessage(iLocalizationService.GetResource("Common.KyHieu.Exists")).Length(0,50).WithMessage(iLocalizationService
                    .GetResource("CheDoAn.KyHieu.Range"));
            RuleFor(x => x.MoTa).Length(0,2000).WithMessage(iLocalizationService
                .GetResource("Common.MoTa.Range"));
        }
    }
}
