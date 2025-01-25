using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KiemTraValidationListTheBHYTViewModel>))]
    public class KiemTraValidationListTheBHYTViewModelValidator : AbstractValidator<KiemTraValidationListTheBHYTViewModel>
    {
        public KiemTraValidationListTheBHYTViewModelValidator(IValidator<NoiTruYeuCauTiepNhanTheBHYTViewModel> theBhytValidator)
        {
            RuleForEach(x => x.YeuCauTiepNhanTheBHYTs).SetValidator(theBhytValidator);
        }
    }
}
