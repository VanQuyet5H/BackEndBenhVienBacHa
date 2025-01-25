using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.Cauhinh.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<CauHinhDanhSachChiTietViewModel>))]
    public class CauHinhDanhSachChiTietViewModelValidator : AbstractValidator<CauHinhDanhSachChiTietViewModel>
    {
        public CauHinhDanhSachChiTietViewModelValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.KeyId).NotEmpty().WithMessage(localizationService.GetResource("CauHinh.KeyId.Required"));
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(localizationService.GetResource("CauHinh.DisplayName.Required"));
            RuleFor(x => x.Value).NotEmpty().WithMessage(localizationService.GetResource("CauHinh.Value.Required"));
        }
    }
}
