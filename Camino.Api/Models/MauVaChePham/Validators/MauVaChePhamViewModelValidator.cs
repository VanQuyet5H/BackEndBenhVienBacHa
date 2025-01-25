using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.MauVaChePham;

namespace Camino.Api.Models.MauVaChePham.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<MauVaChePhamViewModel>))]
    public class MauVaChePhamViewModelValidator : AbstractValidator<MauVaChePhamViewModel>
    {
        public MauVaChePhamViewModelValidator(ILocalizationService iLocalizationService,
            IMauVaChePhamService iMauVaChePhamService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).MustAsync(
                    async (request, ten, id) =>
                    {
                        var val = await iMauVaChePhamService.IsTenExists(ten, request.Id);
                        return val;
                    }).WithMessage(iLocalizationService.GetResource("Common.Ten.Exists")).Length(0, 500).WithMessage(
                    iLocalizationService
                        .GetResource("Common.Ten.Range"));
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required")).MustAsync(
                    async (request, ma, id) =>
                    {
                        var val = await iMauVaChePhamService.IsMaExists(ma, request.Id);
                        return val;
                    }).WithMessage(iLocalizationService.GetResource("Common.Ma.Exists")).Length(0, 50).WithMessage(
                    iLocalizationService
                        .GetResource("Common.Ma.Range"));
            RuleFor(x => x.PhanLoaiMau)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.PhanLoaiMau.Required"));
            RuleFor(x => x.TheTich)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.TheTich.Required"));
            RuleFor(x => x.GiaTriToiDa)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.GiaTriToiDa.Required"));

            RuleFor(x => x.GhiChu).Length(0, 2000).WithMessage(iLocalizationService
                .GetResource("Common.MoTa.Range"));
        }
    }
}
