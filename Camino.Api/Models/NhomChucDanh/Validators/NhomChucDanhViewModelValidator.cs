using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhomChucDanh;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhomChucDanh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhomChucDanhViewModel>))]
    public class NhomChucDanhViewModelValidator : AbstractValidator<NhomChucDanhViewModel>
    {
        public NhomChucDanhViewModelValidator(ILocalizationService iLocalizationService, INhomChucDanhService iNhomChucDanhService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) => {
                    var val = await iNhomChucDanhService.IsTenExists(ten, request.Id);
                    return val;
                }).WithMessage(iLocalizationService.GetResource("Common.Ten.Exits")).Length(0, 250).WithMessage(iLocalizationService
                    .GetResource("Common.Ten.Range"));
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.TenVietTat.Required")).MustAsync(async (request, ma, id) => {
                    var val = await iNhomChucDanhService.IsMaExists(ma, request.Id);
                    return val;
                }).WithMessage(iLocalizationService.GetResource("Common.Ma.Exists")).Length(0, 50).WithMessage(iLocalizationService
                    .GetResource("Common.Ma.Range"));

        }
    }
}
