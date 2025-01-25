using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhongKhamChuyenDenInfoViewModel>))]
    public class PhongKhamChuyenDenInfoViewModelValidator : AbstractValidator<PhongKhamChuyenDenInfoViewModel>
    {
        public PhongKhamChuyenDenInfoViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.PhongThucHienId)
                .NotEmpty().WithMessage(localizationService.GetResource("ChuyenPhongKhamBenh.PhongThucHienId.Required"));
        }
    }
}
