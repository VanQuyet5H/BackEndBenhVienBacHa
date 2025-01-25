using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NhapKhoDuocPhams;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoDuocPhams.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauNhapKhoDuocPhamViewModel>))]
    public class YeuCauNhapKhoDuocPhamViewModelValidator : AbstractValidator<YeuCauNhapKhoDuocPhamViewModel>
    {
        public YeuCauNhapKhoDuocPhamViewModelValidator(ILocalizationService localizationService, IYeuCauNhapKhoDuocPhamService yeuCauNhapKhoDuocPhamService)
        {
            RuleFor(x => x.SoChungTu)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.SoChungTu.Required"))
                .Must((req, soChungTu, id) => soChungTu != "0000000").WithMessage(localizationService.GetResource("NhapKho.SoChungTu.Invalid")); 
            RuleFor(x => x.NguoiNhapId)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.NguoiNhapId.Required"));
            RuleFor(x => x.NgayNhap)
              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.NgayNhap.Required"));

            RuleFor(x => x.KyHieuHoaDon)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.KyHieuHoaDon.Required"));
            RuleFor(x => x.NgayHoaDon)
              .MustAsync(async (viewModel, ngayTiepNhan, id) =>
              {
                  var kiemTraNgayTiepNhan = await yeuCauNhapKhoDuocPhamService.KiemTraNgayHoaDon(viewModel.NgayHoaDon, DateTime.Now);
                  return kiemTraNgayTiepNhan;
              })
              .WithMessage(localizationService.GetResource("NhapKhoDuocPham.NgayHoaDon.LessThanOrEqualTo")); 

            //            RuleFor(x => x.NguoiGiaoId)
            //              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.NguoiGiaoId.Required")).When(x => x.LoaiNguoiGiao == Core.Domain.Enums.LoaiNguoiGiaoNhan.TrongHeThong);
            //            RuleFor(x => x.TenNguoiGiao)
            //              .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPham.TenNguoiGiao.Required")).When(x => x.LoaiNguoiGiao == Core.Domain.Enums.LoaiNguoiGiaoNhan.NgoaiHeThong);
        }
    }
}
