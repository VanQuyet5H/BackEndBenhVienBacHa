using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamBenhBoPhanTonThuongViewModel>))]
    public class YeuCauKhamBenhBoPhanTonThuongViewModelValidator : AbstractValidator<YeuCauKhamBenhBoPhanTonThuongViewModel>
    {
        public YeuCauKhamBenhBoPhanTonThuongViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.HinhAnh)
                .NotEmpty().WithMessage(localizationService.GetResource("BoPhanTonThuong.HinhAnh.Required"));
            RuleFor(x => x.MoTa)
                .NotEmpty().WithMessage(localizationService.GetResource("BoPhanTonThuong.MoTa.Required"));
        }
    }
}
