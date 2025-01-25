using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayChungSinhMangThaiHo.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GiayChungSinhMangThaiHoViewModel>))]
    public class GiayChungSinhMangThaiHoViewModelValidator : AbstractValidator<GiayChungSinhMangThaiHoViewModel>
    {
        public GiayChungSinhMangThaiHoViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NgayThucHien)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayThucHien.Required"));
        }
    }
}
