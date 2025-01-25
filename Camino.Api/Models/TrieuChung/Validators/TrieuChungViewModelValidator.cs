using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.TrieuChung;
using Camino.Services.Localization;

namespace Camino.Api.Models.TrieuChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TrieuChungViewModel>))]
    public class TrieuChungViewModelValidator : AbstractValidator<TrieuChungViewModel>
    {
        public TrieuChungViewModelValidator(ILocalizationService iLocalizationService, ITrieuChungService iTrieuChungService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).MustAsync(async (request, ten, id) => {
                    var val = await iTrieuChungService.IsTenExists(ten, request.Id);
                    return val;
                }).WithMessage(iLocalizationService.GetResource("Common.Ten.Exists")).Length(0, 250).WithMessage(iLocalizationService
                    .GetResource("Common.Ten.Range"));
            RuleFor(x => x.DanhMucChuanIds)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("TrieuChung.DanhMucChuanIds.Required")).MustAsync(async (request,index,id) => {
                    if (request.DanhMucChuanIds != null) {
                        var val = iTrieuChungService.IsKiemTra(request.DanhMucChuanIds);
                        return val;
                    }
                    return true;
                }).WithMessage(iLocalizationService.GetResource("Common.DoanhMucChuan.NotExists"));
        }
    }
    
}
