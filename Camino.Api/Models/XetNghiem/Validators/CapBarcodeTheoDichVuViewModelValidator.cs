using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.XetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CapBarcodeTheoDichVuViewModel>))]
    public class CapBarcodeTheoDichVuViewModelValidator : AbstractValidator<CapBarcodeTheoDichVuViewModel>
    {
        public CapBarcodeTheoDichVuViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BarcodeNumber)
                .NotEmpty()
                    .WithMessage(localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Required"))
                .Must((model, input) => model.BarcodeNumber == null ||  model.BarcodeNumber <= 9999) // length: 4
                    .WithMessage(localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Length"));

            RuleFor(x => x.SoLuong)
                .NotEmpty()
                    .WithMessage(localizationService.GetResource("LayMauXetNghiem.SoLuongBarcode.Required"))
                .Must((model, input) => input == null || input.Value > 0)
                    .WithMessage(localizationService.GetResource("LayMauXetNghiem.SoLuongBarcode.Range"));
        }
    }
}
