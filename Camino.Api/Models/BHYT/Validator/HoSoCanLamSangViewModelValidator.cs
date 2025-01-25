using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BHYT;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoCanLamSangViewModel>))]
    public class HoSoCanLamSangViewModelValidator : AbstractValidator<HoSoCanLamSangViewModel>
    {
        public HoSoCanLamSangViewModelValidator(ILocalizationService iLocalizationService, IBaoHiemYTeService _BHYTService)
        {
            RuleFor(x => x.MaDichVu)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.MaPhuongThucThanhToan.Required"));
            RuleFor(x => x.NgayKQTime)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayKQTime.Required")).MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false))
         .WithMessage(iLocalizationService.GetResource("BHYT.NgayKQTime.Valid"));
        }
    }
}
