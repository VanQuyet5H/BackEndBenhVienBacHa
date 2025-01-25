using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using FluentValidation;

namespace Camino.Api.Models.KhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<KetLuanKhamBenhViewModel>))]
    public class KetLuanKhamBenhViewModelValidator : AbstractValidator<KetLuanKhamBenhViewModel>
    {
        public KetLuanKhamBenhViewModelValidator(ILocalizationService localizationService, IValidator<YeuCauKhamBenhICDKhacViewModel> yeuCauKhamBenhICDKhacValidator, IYeuCauKhamBenhService ycKhamBenhService)
        {

            RuleFor(x => x.IcdchinhId)
                .Must((model, input, s) => !(model.CheckValidator != false && model.IcdchinhId == null)).WithMessage(localizationService.GetResource("YeuCauKhamBenh.ICDChinhId.Required"));

            RuleFor(x => x.CachGiaiQuyet)
               .Must((model, input, s) => !(model.CheckValidator != false && string.IsNullOrEmpty(model.CachGiaiQuyet))).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CachGiaiQuyet.Required"));

            RuleFor(x => x.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId)
                .Must((model, input, s) => (model.CoDieuTriNgoaiTru != true 
                                    || (model.CoDieuTriNgoaiTru == true && model.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId != null ) || !model.CoInKeToa != true))
                .WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatId.Required"));

            RuleFor(x => x.YeuCauDichVuKyThuat.SoLan)
                .Must((model, input, s) => (model.CoDieuTriNgoaiTru != true || (model.CoDieuTriNgoaiTru == true && model.YeuCauDichVuKyThuat.SoLan != null) || !model.CoInKeToa != true))
                .WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.SoLan.Required"))
                .Must((request, soLanNhap, id) =>
                {
                    if (soLanNhap <= 0)
                        return false;
                    return true;
                }).WithMessage(localizationService.GetResource("DichVuKyThuat.SoLan.Range")); ;

            RuleFor(x => x.YeuCauDichVuKyThuat.ThoiDiemBatDauDieuTri)
                .Must((model, input, s) =>
                {
                    if (model.CoDieuTriNgoaiTru == true
                                && (model.YeuCauDichVuKyThuat.TrangThaiThanhToan == Core.Domain.Enums.TrangThaiThanhToan.ChuaThanhToan
                                                || model.YeuCauDichVuKyThuat.TrangThaiThanhToan == null )
                                && model.YeuCauDichVuKyThuat.ThoiDiemBatDauDieuTri >= DateTime.Today || model.YeuCauDichVuKyThuat.ThoiDiemBatDauDieuTri == null)
                        return true;
                    else if (model.CoDieuTriNgoaiTru == true
                                && (model.YeuCauDichVuKyThuat.TrangThaiThanhToan == Core.Domain.Enums.TrangThaiThanhToan.DaThanhToan))
                        return true;
                    else if (model.CoDieuTriNgoaiTru == false)
                        return true;
                    return false;
                })
                .WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.ThoiDiemBatDauDieuTri.GreaterThanOrEqualTo"))
                .Must((model, input, s) => (model.CoDieuTriNgoaiTru != true 
                                           || (model.CoDieuTriNgoaiTru == true && model.YeuCauDichVuKyThuat.ThoiDiemBatDauDieuTri != null) || !model.CoInKeToa != true))
                .WithMessage(localizationService.GetResource("DichVuKyThuatBenhVien.ThoiDiemBatDauDieuTri.Required"));

            RuleFor(x => x.KhoaPhongNhapVienId)
                .Must((model, input, s) => !(model.CoNhapVien == true && model.KhoaPhongNhapVienId == null)).WithMessage(localizationService.GetResource("YeuCauKhamBenh.KhoaPhongNhapVienId.Required"));

            RuleFor(x => x.BenhVienChuyenVienId)
                .Must((model, input, s) => !(model.CoChuyenVien == true && model.BenhVienChuyenVienId == null)).WithMessage(localizationService.GetResource("YeuCauKhamBenh.BenhVienChuyenVienId.Required"));

            RuleFor(x => x.ThoiDiemChuyenVien)
               //.LessThanOrEqualTo(DateTime.Now)
               .MustAsync(async (viewModel, ngayChuyenVien, id) =>
               {
                   var kiemTraNgayCV = await ycKhamBenhService.KiemTraNgayChuyenVien(viewModel.ThoiDiemChuyenVien, viewModel.Id);
                   return !kiemTraNgayCV;
               })
               .WithMessage(localizationService.GetResource("YeuCauKhamBenh.ThoiDiemChuyenVien.LessThanOrEqualTo"))
               .Must((model, input, s) => !(model.CoChuyenVien == true && model.ThoiDiemChuyenVien == null)).WithMessage(localizationService.GetResource("YeuCauKhamBenh.ThoiDiemChuyenVien.Required"));

            //RuleFor(x => x.NhanVienHoTongChuyenVienId)
            //   .Must((model, input, s) => !(model.CoChuyenVien == true && model.NhanVienHoTongChuyenVienId == null)).WithMessage(localizationService.GetResource("YeuCauKhamBenh.NhanVienHoTongChuyenVienId.Required"));

            RuleFor(x => x.NgayTaiKham)
                .Must((model, input, s) => !(model.CoTaiKham == true && input == null && model.CoInKeToa != true)).WithMessage(localizationService.GetResource("YeuCauKhamBenh.NgayTaiKham.Required"));

            //Ngày tái khám phải lớn hơn ngày hiện tại
            RuleFor(x => x.NgayTaiKham).GreaterThan(DateTime.Now.Date)
                .WithMessage(localizationService.GetResource("YeuCauKhamBenh.NgayTaiKham.GreaterThan"));

            RuleForEach(x => x.YeuCauKhamBenhICDKhacs).SetValidator(yeuCauKhamBenhICDKhacValidator);
        }
    }
}
