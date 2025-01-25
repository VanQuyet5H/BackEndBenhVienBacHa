using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BangKiemAnToanPhauThuatTuPhongDieuTri.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<BangKiemAnToanPhauThuatTuPhongDieuTriViewModel>))]
    public class BangKiemAnToanPhauThuatTuPhongDieuTriViewModelValidator : AbstractValidator<BangKiemAnToanPhauThuatTuPhongDieuTriViewModel>
    {
        public BangKiemAnToanPhauThuatTuPhongDieuTriViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            RuleFor(x => x.NgayNhan)
              .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayNhan.Required"))
            .MustAsync(async (viewModel, ngayNhan, id) =>
             {
                 var kiemTraNgayTiepNhan = true;
                 if (viewModel.Id != 0 && viewModel.Id != null)
                 {
                     kiemTraNgayTiepNhan = await dieuTriNoiTruService.KiemTraNgayNhan(viewModel.NgayNhan, viewModel.YeuCauTiepNhanId);
                 }
                 return kiemTraNgayTiepNhan;
             })
              .WithMessage(localizationService.GetResource("HoSoKhac.DenNgay.LessThanOrEqualTo"));
            RuleFor(x => x.NguoiGiao)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NguoiGiao.Required"));
            RuleFor(x => x.NguoiNhan)
           .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NguoiNhan.Required"));
        }
    }
}
