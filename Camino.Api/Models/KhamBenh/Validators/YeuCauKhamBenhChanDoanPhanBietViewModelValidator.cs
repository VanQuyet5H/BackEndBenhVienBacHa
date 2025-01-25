using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamBenhChanDoanPhanBietViewModel>))]
    public class YeuCauKhamBenhChanDoanPhanBietViewModelValidator: AbstractValidator<YeuCauKhamBenhChanDoanPhanBietViewModel>
    {
        public YeuCauKhamBenhChanDoanPhanBietViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ICDId)
                .NotEmpty().WithMessage(localizationService.GetResource("ChanDoanPhanBiet.ICDId.Required"));
        }
    }
}
