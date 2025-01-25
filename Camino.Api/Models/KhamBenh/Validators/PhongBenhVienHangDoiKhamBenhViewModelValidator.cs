using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhongBenhVienHangDoiKhamBenhViewModel>))]
    public class PhongBenhVienHangDoiKhamBenhViewModelValidator : AbstractValidator<PhongBenhVienHangDoiKhamBenhViewModel>
    {
        public PhongBenhVienHangDoiKhamBenhViewModelValidator(ILocalizationService localizationService, IValidator<YeuCauKhamBenhKhamBenhViewModel> khamBenhValidator,
            IValidator<YeuCauTiepNhanKhamBenhViewModel> tiepNhanValidator)
        {
            RuleFor(x => x.YeuCauKhamBenh).SetValidator(khamBenhValidator);
            RuleFor(x => x.YeuCauTiepNhan).SetValidator(tiepNhanValidator);
        }
    }
}
