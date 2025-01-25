
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KetQuaXetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KetQuaXetNghiemViewKetQuaViewModel>))]
    public class KetQuaXetNghiemViewModelValidator : AbstractValidator<KetQuaXetNghiemViewKetQuaViewModel>
    {
        public KetQuaXetNghiemViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NhanVienThucHienId)
             .NotEmpty().WithMessage(localizationService.GetResource("KetQuaXetNghiem.NhanVienThucHienId.Required"));
        }
    }
}
