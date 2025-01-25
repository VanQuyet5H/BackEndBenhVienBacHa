using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.ChucDanh;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.ChucDanh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChucDanhViewModel>))]
    public class ChucDanhViewModelValidator : AbstractValidator<ChucDanhViewModel>
    {
        public ChucDanhViewModelValidator(ILocalizationService iLocalizationService, IChucDanhService  iChucDanhService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) => {
                var val = await iChucDanhService.IsTenExists(ten, request.Id);
                return val;
            }).WithMessage(iLocalizationService.GetResource("Common.Ten.Exists")).Length(0, 250).WithMessage(iLocalizationService
                .GetResource("Common.Ten.Range"));
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required")).MustAsync(async (request, ma, id) => {
                var val = await iChucDanhService.IsMaExists(ma, request.Id);
                return val;
            }).WithMessage(iLocalizationService.GetResource("Common.Ma.Exists")).Length(0,50).WithMessage(iLocalizationService
                    .GetResource("Common.Ma.Range"));
            RuleFor(x => x.MoTa).Length(0,2000).WithMessage(iLocalizationService
                .GetResource("Common.MoTa.Range"));
            RuleFor(x => x.NhomChucDanhId)
               .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.NhomChucDanhId.Required"));
        }
    }
}
