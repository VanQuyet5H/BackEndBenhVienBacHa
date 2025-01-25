using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhBenhNhanTienSuBenhViewModel>))]
    public class KhamBenhBenhNhanTienSuBenhViewModelValidator : AbstractValidator<KhamBenhBenhNhanTienSuBenhViewModel>
    {
        public KhamBenhBenhNhanTienSuBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LoaiTienSuBenh)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhanTienSubenh.LoaiTienSuBenh.Required"));

            RuleFor(x => x.TenBenh)
                .NotEmpty().WithMessage(localizationService.GetResource("BenhNhanTienSubenh.TenBenh.Required"));
        }
    }
}
