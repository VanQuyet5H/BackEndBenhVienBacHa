using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.MauVaChePham;
using FluentValidation;

namespace Camino.Api.Models.NhanSanXuatTheoQuocGia.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhaSanXuatTheoQuocGiaViewModel>))]
    public class NhaSanXuatTheoQuocGiaViewModelValidator : AbstractValidator<NhaSanXuatTheoQuocGiaViewModel>
    {
        public NhaSanXuatTheoQuocGiaViewModelValidator(ILocalizationService iLocalizationService)
        {
            RuleFor(x => x.DiaChi)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.DiaChi.Required"));
            RuleFor(x => x.QuocGiaId)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.QuocGiaId.Required"));
          
            
        }

    }
}
