using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeucCauKhamBenhChanDoanPhanBiet;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamBenhChanDoanPhanBietViewModel>))]
    public class YeuCauKhamBenhChanDoanPhanBietViewModelValidator : AbstractValidator<YeuCauKhamBenhChanDoanPhanBietViewModel>
    {
        public YeuCauKhamBenhChanDoanPhanBietViewModelValidator(ILocalizationService iLocalizationService, IYeuCauKhamBenhChanDoanPhanBietService yeuCauKhamBenhChanDoanPhanBietService)
        {

            RuleFor(x => x.ICDId)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.ICDId.Required"))
                .MustAsync(async (request, input, id) =>
                {
                    var val = await yeuCauKhamBenhChanDoanPhanBietService.IsTenExists(input, request.Id, request.YeuCauKhamBenhId);
                    return val;
                }).WithMessage(iLocalizationService.GetResource("Common.ICDId.Exists"));
        }
    }
}
