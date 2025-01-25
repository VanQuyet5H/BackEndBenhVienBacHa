using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<UpdateDieuTriSoSinhRaVienViewModel>))]
    public class UpdateDieuTriSoSinhRaVienViewModelValidator : AbstractValidator<UpdateDieuTriSoSinhRaVienViewModel>
    {
        public UpdateDieuTriSoSinhRaVienViewModelValidator(ILocalizationService localizationService,
            IValidator<ThoiGianDieuTriSoSinhRaVienViewModel> thoiGianDieuTriSoSinhViewModelsValidatorr)
        {
            RuleForEach(x => x.ThoiGianDieuTriSoSinhRaVienViewModel).SetValidator(thoiGianDieuTriSoSinhViewModelsValidatorr);
        }
    }
}
