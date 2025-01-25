using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.ICDs;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.ICDs.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChuanDoanViewModel>))]
    public class ChuanDoanViewModelValidator : AbstractValidator<ChuanDoanViewModel>
    {
        public ChuanDoanViewModelValidator(ILocalizationService localizationService, IChuanDoanService _chuanDoanService)
        {

            RuleFor(x => x.TenTiengViet)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenTiengViet.Required")).MustAsync(async (request, tenVi, id) =>
                {
                    var checkExistsTen = await _chuanDoanService.IsTenViExists(tenVi, request.Id);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.TenTiengViet.Exists"));

            RuleFor(x => x.TenTiengAnh)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenTiengAnh.Required")).MustAsync(async (request, tenEng, id) =>
                {
                    var checkExistsTen = await _chuanDoanService.IsTenEngExists(tenEng, request.Id);
                    return checkExistsTen;
                })
                .WithMessage(localizationService.GetResource("Common.TenTiengAnh.Exists"));
            RuleFor(x => x.Ma)
                 .NotEmpty().WithMessage(localizationService.GetResource("ChuanDoan.Ma.Required")).MustAsync(async (request, ma, id) =>
                 {
                     var checkExistsMa = await _chuanDoanService.IsMaExists(ma, request.Id);
                     return checkExistsMa;
                 }).WithMessage(localizationService.GetResource("Common.Ma.Exists"));
            RuleFor(x => x.DanhMucChuanDoanId)
                 .NotEmpty().WithMessage(localizationService.GetResource("ChuanDoan.DanhMucChuanDoanId.Required"));
        }
    }
}
