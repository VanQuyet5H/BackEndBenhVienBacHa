using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GoiDichVuChiTietNoiThucHienViewModel>))]
    public class GoiDichVuChiTietNoiThucHienViewModelValidator : AbstractValidator<GoiDichVuChiTietNoiThucHienViewModel>
    {
        public GoiDichVuChiTietNoiThucHienViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NoiThucHienId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.NoiThucHienId.Required"));
        }
    }
}
