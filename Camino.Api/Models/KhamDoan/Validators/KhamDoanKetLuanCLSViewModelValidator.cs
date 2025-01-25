using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamDoanKetLuanCLSViewModel>))]

    public class KhamDoanKetLuanCLSViewModelValidator : AbstractValidator<KhamDoanKetLuanCLSViewModel>
    {
        public KhamDoanKetLuanCLSViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KSKKetQuaCanLamSang)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.KSKKetQuaCanLamSang.Required"));

            RuleFor(x => x.KSKDanhGiaCanLamSang)
                .NotEmpty().WithMessage(localizationService.GetResource("KhamDoan.KSKNhanVienDanhGiaCanLamSang.Required"));
        }

    }
}
