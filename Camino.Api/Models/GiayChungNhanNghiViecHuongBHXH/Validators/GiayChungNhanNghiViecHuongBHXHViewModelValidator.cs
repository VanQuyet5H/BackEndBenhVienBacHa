using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayChungNhanNghiViecHuongBHXH.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GiayChungNhanNghiViecHuongBHXHViewModel>))]
    public class GiayChungNhanNghiViecHuongBHXHViewModelValidator : AbstractValidator<GiayChungNhanNghiViecHuongBHXHViewModel>
    {
        public GiayChungNhanNghiViecHuongBHXHViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            RuleFor(x => x.NgayThucHien)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayThucHien.Required"));
            RuleFor(x => x.NghiTuNgay)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NghiTuNgay.Required"));
            RuleFor(x => x.NghiDenNgay)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NghiDenNgay.Required"))
                .MustAsync(async (viewModel, ngayTiepNhan, id) =>
                {
                    var kiemTraNgayTiepNhan = await dieuTriNoiTruService.KiemTraNgay(viewModel.NghiTuNgay, viewModel.NghiDenNgay);
                    return kiemTraNgayTiepNhan;
                }).WithMessage(localizationService.GetResource("HoSoKhac.DenNgay.LessThanOrEqualTo"));
           
        }
    }
}
