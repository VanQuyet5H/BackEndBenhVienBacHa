using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThoiGianDieuTriSoSinhRaVienViewModel>))]
    public class ThoiGianDieuTriSoSinhRaVienViewModellValidator : AbstractValidator<ThoiGianDieuTriSoSinhRaVienViewModel>
    {
        public ThoiGianDieuTriSoSinhRaVienViewModellValidator(ILocalizationService localizationService,
            IValidator<ThoiGianDieuTriSoSinhViewModel> thoiGianDieuTriSoSinhRaVienViewModellValidator)
        {           
            RuleForEach(x => x.ThoiGianDieuTriSoSinhViewModels).SetValidator(thoiGianDieuTriSoSinhRaVienViewModellValidator);
        }
    }
}
