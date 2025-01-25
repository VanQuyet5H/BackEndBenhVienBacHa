using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.YeuCauNhapKhoDuocPham;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;

namespace Camino.Api.Models.NhapKhoDuocPhamChiTiets.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauNhapKhoDuocPhamChiTietGridVo>))]
    public class AddOrUpdateThongTinDuocPhamNhapKhoValidator : AbstractValidator<YeuCauNhapKhoDuocPhamChiTietGridVo>
    {
        public AddOrUpdateThongTinDuocPhamNhapKhoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.HopDongThauDuocPhamId)
              .Must((req, hopDongThauId, id) =>
              {
                  if (req.LoaiNhap == 1)
                  {
                      return hopDongThauId != null && hopDongThauId != 0;
                  }

                  return true;
              }).WithMessage(localizationService.GetResource("nhapKhoChiTiet.HopDongThauDuocPhamId.Required"));
            RuleFor(x => x.NhaThauId)
                .Must((req, nhaThauId, id) =>
                {
                    if (req.LoaiNhap == 2)
                    {
                        return nhaThauId != null &&  nhaThauId != 0;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("HopDongThauDuocPham.NhaThauId.Required"));
            RuleFor(x => x.DuocPhamBenhVienId)
              .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.DuocPhamBenhVienId.Required"));
            RuleFor(x => x.LaDuocPhamBHYT)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.LaDuocPhamBHYT.Required"));
            RuleFor(x => x.Solo)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.Solo.Required"));

            RuleFor(x => x.HanSuDung)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.HanSuDung.Required"));
            RuleFor(x => x.HanSuDung)
               .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.HanSuDung.Required"))
                .Must((request, ngay, id) => {
                    if (ngay != null && ngay.Value.Date <= DateTime.Now.Date)
                        return false;
                    return true;
                }).WithMessage(localizationService.GetResource("nhapKhoChiTiet.HanSuDung.LessThanNow"));

            RuleFor(x => x.SoLuongNhap)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.SoLuongNhap.Required"));
            RuleFor(x => x.DonGiaNhap)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.DonGiaNhap.Required"));

            //RuleFor(x => x.KyHieuHoaDon)
            //.NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.KyHieuHoaDon.Required"));

            RuleFor(x => x.VAT)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.VAT.Required")).When(p => p.LaDuocPhamBHYT == false);

            RuleFor(x => x.ThanhTienSauVat)
            .NotEmpty().WithMessage(localizationService.GetResource("BHYT.ThanhToan.Required")).When(p => p.LaDuocPhamBHYT == false);

            RuleFor(x => x.ThanhTienTruocVat)
            .NotEmpty().WithMessage(localizationService.GetResource("BHYT.ThanhToan.Required"));

            RuleFor(x => x.TiLeBHYTThanhToan)
             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.TiLeBHYTThanhToan.Required")).When(p => p.LaDuocPhamBHYT == true);

            RuleFor(x => x.KhoNhapSauKhiDuyetId)
                .NotEmpty().WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.KhoNhapSauKhiDuyetId.Required"));
//            RuleFor(x => x.DuocPhamBenhVienPhanNhomId)
//             .NotEmpty().WithMessage(localizationService.GetResource("nhapKhoChiTiet.DuocPhamBenhVienPhanNhomId.Required"));
            RuleFor(x => x.NguoiNhapSauKhiDuyetId)
                .Must((req, nguoiNhapSauKhiDuyetId, id) =>
                {
                    if (req.KhoNhapSauKhiDuyetId > 0)
                    {
                        return nguoiNhapSauKhiDuyetId != null && nguoiNhapSauKhiDuyetId != 0;
                    }

                    return true;
                }).WithMessage(localizationService.GetResource("nhapKhoChiTiet.NguoiNhapSauKhiDuyetId.Required"));
        }
    }
}
