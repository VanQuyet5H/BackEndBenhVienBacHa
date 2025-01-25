using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.NhapKhoMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuNhapKhoMauViewModel>))]
    public class PhieuNhapKhoMauViewModelValidator : AbstractValidator<PhieuNhapKhoMauViewModel>
    {
        public PhieuNhapKhoMauViewModelValidator(ILocalizationService localizationService, IValidator<PhieuNhapKhoMauChiTietViewModel> nhapKhoMauChiTietValidator)
        {
            //RuleFor(x => x.SoHoaDon)
            //    .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.SoHoaDon.Required"));
            //RuleFor(x => x.NgayHoaDon)
            //    .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.NgayHoaDon.Required"));
            RuleFor(x => x.NhaThauId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoMau.NhaThauId.Required"));

            RuleForEach(x => x.NhapKhoMauChiTiets)
                .SetValidator(nhapKhoMauChiTietValidator);
        }
    }
}
