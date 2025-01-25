using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KhamBenhBenhNhanViewModel>))]
    public class KhamBenhBenhNhanViewModelValidator : AbstractValidator<KhamBenhBenhNhanViewModel>
    {
        public KhamBenhBenhNhanViewModelValidator(ILocalizationService localizationService, IValidator<KhamBenhBenhNhanDiUngThuocViewModel> diUngThuocValidator,
            IValidator<KhamBenhBenhNhanTienSuBenhViewModel> tienSuBenhValidator)
        {
            RuleForEach(x => x.KhamBenhBenhNhanDiUngThuocs).SetValidator(diUngThuocValidator);
            RuleForEach(x => x.KhamBenhBenhNhanTienSuBenhs).SetValidator(tienSuBenhValidator);
        }
    }
}
