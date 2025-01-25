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
    [TransientDependency(ServiceType = typeof(IValidator<HoSoChiTietDienBienBenhViewModel>))]
    public class HoSoChiTietDienBienBenhViewModelValidator : AbstractValidator<HoSoChiTietDienBienBenhViewModel>
    {
        public HoSoChiTietDienBienBenhViewModelValidator(ILocalizationService iLocalizationService, IBaoHiemYTeService _BHYTService)
        {
            RuleFor(x => x.DienBien)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.DienBien.Required"));
            RuleFor(x => x.NgayYLenhTime)
         .NotEmpty().WithMessage(iLocalizationService.GetResource("BHYT.NgayYLenhTime.Required"))
           .MustAsync(async (model, input, s) => !await _BHYTService.CheckValidNgayVaoRa(input).ConfigureAwait(false))
         .WithMessage(iLocalizationService.GetResource("BHYT.NgayYLenhTime.Valid"));
        }
    }
}
