using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayChungNhanPhauThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GiayChungNhanPhauThuatViewModel>))]
    public class GiayChungNhanPhauThuatViewModelValidator : AbstractValidator<GiayChungNhanPhauThuatViewModel>
    {
        public GiayChungNhanPhauThuatViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DichVuPTTTId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.DichVuPTTTId.Required"));
        }
    }
}
