using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.HoanTra.HoanTraValidators
{
    [TransientDependency(ServiceType = typeof(IValidator<TuChoiDuyetHoanTraDuocPhamViewModel>))]
    public class TuChoiDuyetHoanTraDuocPhamViewModelValidator : AbstractValidator<TuChoiDuyetHoanTraDuocPhamViewModel>
    {
        public TuChoiDuyetHoanTraDuocPhamViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.LyDoHuy)
                .NotNull().WithMessage(localizationService.GetResource("DuyetHoanTra.LyDoHuy.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("DuyetHoanTra.LyDoHuy.Required"));
        }
    }
}
