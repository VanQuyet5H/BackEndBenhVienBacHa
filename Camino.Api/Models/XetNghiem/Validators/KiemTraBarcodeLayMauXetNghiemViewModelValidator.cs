using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.XetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KiemTraBarcodeLayMauXetNghiemViewModel>))]
    public class KiemTraBarcodeLayMauXetNghiemViewModelValidator : AbstractValidator<KiemTraBarcodeLayMauXetNghiemViewModel>
    {
        public KiemTraBarcodeLayMauXetNghiemViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BarcodeNumber)
                .Must((model, input) => (model.IsCapMoi && model.IsCapBarcodeChoDichVu != true) || ((!model.IsCapMoi || model.IsCapBarcodeChoDichVu == true) && model.BarcodeNumber != null))
                    .WithMessage(localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Required"));
                //.Must((model, input) => model.IsCapMoi 
                //                        || model.BarcodeNumber == null
                //                        || (!string.IsNullOrEmpty(model.BarcodeNumber) && model.BarcodeNumber.Length <= 4))
                //    .WithMessage(localizationService.GetResource("LayMauXetNghiem.BarcodeNumber.Length"));

            RuleFor(x => x.SoLuong)
                .Must((model, input) => !model.IsInBarcode 
                                        || input != null)
                    .WithMessage(localizationService.GetResource("LayMauXetNghiem.SoLuongBarcode.Required"))
                .Must((model, input) => !model.IsInBarcode 
                                        || input == null 
                                        || input.Value > 0)
                    .WithMessage(localizationService.GetResource("LayMauXetNghiem.SoLuongBarcode.Range"));
        }
    }
}
