using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.NhapKhoMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KetQuaXetNghiemKhac>))]
    public class KetQuaXetNghiemKhacValidator : AbstractValidator<KetQuaXetNghiemKhac>
    {
        public KetQuaXetNghiemKhacValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LoaiXetNghiem)
                .NotEmpty().WithMessage(localizationService.GetResource("KetQuaXetNghiemKhacs.LoaiXetNghiem.Required"));
        }
    }
}
