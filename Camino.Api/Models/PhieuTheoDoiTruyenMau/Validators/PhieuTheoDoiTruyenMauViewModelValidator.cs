using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuTheoDoiTruyenMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuTheoDoiTruyenMauViewModel>))]
    public class PhieuTheoDoiTruyenMauViewModelValidator : AbstractValidator<PhieuTheoDoiTruyenMauViewModel>
    {
        public PhieuTheoDoiTruyenMauViewModelValidator(ILocalizationService localizationService)
    {

            RuleFor(x => x.MaDonViMauTruyenId)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.MaDonViMauTruyen.Required"));

            RuleFor(x => x.DDTruyenMauId)
               .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.DDTruyenMau.Required"));
            RuleFor(x => x.BatDauTruyenHoi)
               .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.BatDauTruyenHoi.Required"));
            //RuleForEach(p => p.ChiSoSinhTons).SetValidator(thongTinValidator);
        }
    }
}
