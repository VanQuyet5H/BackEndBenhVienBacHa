using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet;
using Camino.Api.Models.YeuCauKhamBenhKhamBoPhanKhac;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamBenhKhamBenhViewModel>))]
    public class YeuCauKhamBenhKhamBenhViewModelValidator : AbstractValidator<YeuCauKhamBenhKhamBenhViewModel>
    {
        public YeuCauKhamBenhKhamBenhViewModelValidator(ILocalizationService localizationService, IValidator<YeuCauKhamBenhKhamBoPhanKhacViewModel> khamBoPhanKhacValidator,
            IValidator<YeuCauKhamBenhChanDoanPhanBietViewModel> chanDoanPhanBietValidator)
        {
            RuleForEach(x => x.YeuCauKhamBenhKhamBoPhanKhacs).SetValidator(khamBoPhanKhacValidator);
            RuleForEach(x => x.YeuCauKhamBenhChanDoanPhanBiets).SetValidator(chanDoanPhanBietValidator);
        }
    }
}
