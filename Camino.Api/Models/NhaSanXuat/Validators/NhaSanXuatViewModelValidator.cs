using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhaSanXuat;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Models.NhanSanXuatTheoQuocGia;

namespace Camino.Api.Models.NhaSanXuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhaSanXuatViewModel>))]
    public class NhaSanXuatViewModelValidator : AbstractValidator<NhaSanXuatViewModel>
    {
        public NhaSanXuatViewModelValidator(ILocalizationService iLocalizationService, INhaSanXuatService inhaSanXuatService , IValidator<NhaSanXuatTheoQuocGiaViewModel> nhaSanXuatTheoQuocGiaIValidator)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) => {
                    var val = await inhaSanXuatService.IsTenExists(ten, request.Id);
                    return val;
                }).WithMessage(iLocalizationService.GetResource("Common.Ten.Exists")).Length(0, 250).WithMessage(iLocalizationService
                    .GetResource("Common.Ten.Range"));
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required")).MustAsync(async (request, ma, id) => {
                    var val = await inhaSanXuatService.IsMaExists(ma, request.Id);
                    return val;
                }).WithMessage(iLocalizationService.GetResource("Common.Ma.Exists")).Length(0, 50).WithMessage(iLocalizationService
                    .GetResource("Common.Ma.Range"));

            RuleFor(x => x.NhaSanXuatTheoQuocGias)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.NhaSanXuatTheoQuocGias.Required"));
            RuleForEach(x => x.NhaSanXuatTheoQuocGias).SetValidator(nhaSanXuatTheoQuocGiaIValidator);
        }
    }
}
