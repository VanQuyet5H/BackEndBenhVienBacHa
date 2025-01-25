using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.XetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuGuiMauXetNghiemViewModel>))]
    public class PhieuGuiMauXetNghiemViewModelValidator : AbstractValidator<PhieuGuiMauXetNghiemViewModel>
    {
        public PhieuGuiMauXetNghiemViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PhongNhanMauId)
                .NotEmpty().WithMessage(localizationService.GetResource("GuiMau.PhongNhanMauId.Required"));
        }
    }
}
