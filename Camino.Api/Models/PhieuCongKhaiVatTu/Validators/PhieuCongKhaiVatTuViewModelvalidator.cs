using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuCongKhaiVatTu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuCongKhaiVatTuViewModel>))]
    public class PhieuCongKhaiVatTuViewModelvalidator : AbstractValidator<PhieuCongKhaiVatTuViewModel>
    {
        public PhieuCongKhaiVatTuViewModelvalidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {

            RuleFor(x => x.NgayRaVien)
                 .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.TuNgay.Required"));
            RuleFor(x => x.NgayVaoVien)
              .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.DenNgay.Required"))
              .MustAsync(async (viewModel, ngayTiepNhan, id) =>
              {
                  var kiemTraNgayTiepNhan = await dieuTriNoiTruService.KiemTraNgay(viewModel.NgayVaoVien, viewModel.NgayRaVien);
                  return kiemTraNgayTiepNhan;
              })
              .WithMessage(localizationService.GetResource("HoSoKhac.DenNgay.LessThanOrEqualTo"));
        }
    }
}
