using System.Threading.Tasks;
using Camino.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Camino.Api.Infrastructure.Auth;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain;
using Camino.Services.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Api.Models.Error;
using Camino.Services.BenhVien;
using Camino.Services.KhoaPhong;
using Camino.Services.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Services.Helpers;
using Camino.Services.ExportImport;
using Camino.Api.Models.YeuCauTiepNhan;

namespace Camino.Api.Controllers
{
    public class DanhSachChoKhamController : CaminoBaseController
    {
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IBenhVienService _benhVienService;
        private readonly IExcelService _excelService;

        public DanhSachChoKhamController(IDanhSachChoKhamService danhSachChoKhamService, ILocalizationService localizationService, IJwtFactory iJwtFactory
                , IKhoaPhongService khoaPhongService, IBenhVienService benhVienService, IExcelService excelService)
        {
            _danhSachChoKhamService = danhSachChoKhamService;
            _localizationService = localizationService;
            _khoaPhongService = khoaPhongService;
            _benhVienService = benhVienService;
            _excelService = excelService;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDanhSachChoKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachChoKham)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachChoKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _danhSachChoKhamService.GetDataForGridAsyncDanhSachChoKham(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachChoKham")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhSachChoKham)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachChoKham([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _danhSachChoKhamService.GetTotalPageForGridAsyncDanhSachChoKham(queryInfo);
            return Ok(gridData);
        }

        #region InPhieuChiDinh
        //[HttpGet("InPhieuChiDinh")]
        //public async Task<ActionResult> InPhieuChiDinh(long id, string hostingName)
        //{
        //    var entity = await _danhSachChoKhamService.GetByIdAsync(id, s => s.Include(u => u.BHYTGiayMienCungChiTra)
        //    .Include(u => u.NguoiLienHeQuanHeNhanThan)
        //    .Include(o => o.YeuCauKhamBenhs)
        //    .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
        //    .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.DichVuKhamBenhBenhVien).ThenInclude(o => o.DichVuKhamBenh)
        //    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.DichVuKyThuatBenhVien).ThenInclude(o => o.DichVuKyThuat)
        //    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.DichVuGiuongBenhVien).ThenInclude(o => o.DichVuGiuong)
        //    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.VatTuBenhVien).ThenInclude(o => o.VatTus)
        //    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.DuocPhamBenhVien).ThenInclude(o => o.DuocPham)
        //    .Include(o => o.YeuCauKhamBenhs).ThenInclude(o => o.NoiDangKy).ThenInclude(o => o.KhoaPhong)
        //    .Include(o => o.YeuCauDichVuKyThuats).ThenInclude(o => o.NoiThucHien).ThenInclude(o => o.KhoaPhong)
        //    .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
        //    .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
        //    .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.NoiChiDinh).ThenInclude(o => o.KhoaPhong)
        //    .Include(u => u.NoiTiepNhan).Include(u => u.NhanVienTiepNhan).ThenInclude(o => o.User)
        //    .Include(cc => cc.PhuongXa)
        //    .Include(cc => cc.QuanHuyen)
        //    .Include(cc => cc.TinhThanh)
        //    );
        //    //var result = entity.ToModel<DanhSachChoKhamViewModel>();
        //    var now = DateTime.Now;
        //    var content = "";
        //    {
        //        var ngayThangNamSinh = string.Empty;
        //        if (entity.NgaySinh != null && entity.ThangSinh != null && entity.NamSinh != null)
        //        {
        //            ngayThangNamSinh = new DateTime(entity.NamSinh ?? 1500, entity.ThangSinh ?? 1, entity.NgaySinh ?? 1).ToString("dd/MM/yyyy");
        //        }
        //        else
        //        {
        //            ngayThangNamSinh = null;
        //        }
        //        var data = new PhieuKhamBenhViewModel
        //        {
        //            LogoUrl = hostingName + "/assets/img/logo-bacha-full.png",
        //            BarCodeImgBase64 = !string.IsNullOrEmpty(entity.BenhNhan?.MaBN) ? BarcodeHelper.GenerateBarCode(entity.BenhNhan.MaBN) : "",
        //            MaTN = entity.MaYeuCauTiepNhan,
        //            MaBN = entity.BenhNhan != null ? entity.BenhNhan.MaBN : "",
        //            Ngay = entity.ThoiDiemTiepNhan.ToString("dd"),
        //            Thang = entity.ThoiDiemTiepNhan.ToString("MM"),
        //            Nam = entity.ThoiDiemTiepNhan.ToString("yyyy"),
        //            HoTen = entity.HoTen,
        //            GioiTinh = entity?.GioiTinh,
        //            NamSinh = ngayThangNamSinh,
        //            DiaChi = AddressHelper.ApplyFormatAddress2(entity.TinhThanh != null ? entity.TinhThanh.Ten : "",
        //                entity.QuanHuyen != null ? entity.QuanHuyen.Ten : "",
        //                entity.PhuongXa != null ? entity.PhuongXa.Ten : "",
        //                entity.DiaChi),
        //            NguoiGiamHo = entity.NguoiLienHeHoTen,
        //            TenQuanHeThanNhan = entity.NguoiLienHeQuanHeNhanThan?.Ten,
        //            DienThoai = entity.SoDienThoai,
        //            //_danhSachChoKhamService.SetSoPhanTramHuongBHYT(entity.BHYTMaSoThe)
        //            DoiTuong = entity.CoBHYT != true ? "Viện phí" : "BHYT (" + entity.BHYTMucHuong.ToString() + "%)",
        //            SoTheBHYT = entity.BHYTMaSoThe + (entity.BHYTMaDKBD == null ? null : " - " + entity.BHYTMaDKBD),
        //            HanThe = (entity.BHYTNgayHieuLuc != null || entity.BHYTNgayHetHan != null) ? "từ ngày: " + (entity.BHYTNgayHieuLuc?.ToString("dd/MM/yyyy") ?? "") + " đến ngày: " + (entity.BHYTNgayHetHan?.ToString("dd/MM/yyyy") ?? "") : "",
        //            Now = now.ToString("dd/MM/yyyy HH:mm"),
        //            NowTime = now.ToString("hh:mm tt"),
        //            //GioKhamDuKien = _danhSachChoKhamService.ThoiDiemKhamDuKien(id).Find(p => p.Id == id).ThoiDiemDuKiens.ApplyFormatTime(),

        //        };
        //        data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User != null ? entity.NhanVienTiepNhan.User.HoTen : "";
        //        if (entity.YeuCauKhamBenhs.Any(o => o.DichVuKhamBenhBenhVien != null) &&
        //            !entity.YeuCauDichVuKyThuats.Any(o =>
        //                o.DichVuKyThuatBenhVien != null && o.DichVuKyThuatBenhVien.DichVuKyThuat != null) &&
        //            !entity.YeuCauDichVuGiuongBenhViens.Any(o =>
        //                o.DichVuGiuongBenhVien != null && o.DichVuGiuongBenhVien.DichVuGiuong != null) &&
        //            !entity.YeuCauVatTuBenhViens.Any(o => o.VatTuBenhVien != null && o.VatTuBenhVien.VatTus != null) &&
        //            !entity.YeuCauDuocPhamBenhViens.Any(o =>
        //                o.DuocPhamBenhVien != null && o.DuocPhamBenhVien.DuocPham != null) &&
        //            entity.YeuCauGoiDichVus.All(o => o.GoiDichVu == null)
        //        )
        //        {
        //            var html = _danhSachChoKhamService.GetBodyByName("PhieuDangKyKham");
        //            foreach (var yckb in entity.YeuCauKhamBenhs.Where(o => o.DichVuKhamBenhBenhVien != null && o.TrangThaiThanhToan != Enums.TrangThaiThanhToan.HuyThanhToan))
        //            {
        //                //data.DangKyKham = (yckb.NoiDangKy != null && yckb.NoiDangKy.KhoaPhong != null ? yckb.NoiDangKy.KhoaPhong.Ten : "") + "-" + (yckb.NoiDangKy != null ? yckb.NoiDangKy.Ten : "");
        //                data.DangKyKham = yckb.TenDichVu + (yckb.NoiDangKy != null &&
        //                                                    yckb.NoiDangKy.KhoaPhong != null ? " - " + yckb.NoiDangKy.KhoaPhong.Ten : "")
        //                                                    + (yckb.NoiDangKy != null ? " - P" + yckb.NoiDangKy.Ma : "");
        //                if (content != "")
        //                {
        //                    content = content + "<div class=\"pagebreak\"> </div>";
        //                }
        //                content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
        //            }
        //        }
        //        else
        //        {

        //            var htmlDanhSachDichVu = "<table style='width:100%;border: 1px solid #020000; border-collapse: collapse;'>";
        //            htmlDanhSachDichVu += "<tr style='border: 1px solid black;  border-collapse: collapse;'>";
        //            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>STT </th>";
        //            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>TÊN DỊCH VỤ</th>";
        //            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>NƠI THỰC HIỆN</th>";
        //            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>SL</th>";
        //            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>VP</th>";
        //            htmlDanhSachDichVu += "<th style='border: 1px solid #020000; border-collapse: collapse;text-align: center;'>BHYT</th>";
        //            htmlDanhSachDichVu += "</tr>";
        //            var i = 1;
        //            if (entity.YeuCauKhamBenhs.Any(o => o.DichVuKhamBenhBenhVien != null))
        //            {
        //                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>KHÁM BỆNH</b></td>";
        //                htmlDanhSachDichVu += " </tr>";
        //                foreach (var yckb in entity.YeuCauKhamBenhs.Where(o => o.DichVuKhamBenhBenhVien != null))
        //                {
        //                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + yckb.DichVuKhamBenhBenhVien.Ten + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (yckb.NoiDangKy != null && yckb.NoiDangKy.KhoaPhong != null ? yckb.NoiDangKy.KhoaPhong.Ten : "") + "-" + (yckb.NoiDangKy != null ? yckb.NoiDangKy.Ten : "") + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>1</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " </tr>";
        //                    i++;
        //                }
        //            }
        //            if (entity.YeuCauDichVuKyThuats.Any(o => o.DichVuKyThuatBenhVien != null))
        //            {
        //                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>DỊCH VỤ KỸ THUẬT</b></td>";
        //                htmlDanhSachDichVu += " </tr>";
        //                foreach (var ycdvkt in entity.YeuCauDichVuKyThuats.Where(o => o.DichVuKyThuatBenhVien != null))
        //                {
        //                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvkt.DichVuKyThuatBenhVien.Ten + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvkt.NoiThucHien != null && ycdvkt.NoiThucHien.KhoaPhong != null ? ycdvkt.NoiThucHien.KhoaPhong.Ten : "") + "-" + (ycdvkt.NoiThucHien != null ? "-" + ycdvkt.NoiThucHien.Ten : "") + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdvkt.SoLan + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " </tr>";
        //                    i++;
        //                }
        //            }
        //            if (entity.YeuCauDichVuGiuongBenhViens.Any(o => o.DichVuGiuongBenhVien != null))
        //            {
        //                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>DỊCH VỤ GIƯỜNG</b></td>";
        //                htmlDanhSachDichVu += " </tr>";
        //                foreach (var ycdvg in entity.YeuCauDichVuGiuongBenhViens.Where(o => o.DichVuGiuongBenhVien != null))
        //                {
        //                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdvg.DichVuGiuongBenhVien.Ten + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdvg.NoiThucHien != null && ycdvg.NoiThucHien.KhoaPhong != null ? ycdvg.NoiThucHien.KhoaPhong.Ten : "") + "-" + (ycdvg.NoiThucHien != null ? "-" + ycdvg.NoiThucHien.Ten : "") + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>1</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " </tr>";
        //                    i++;
        //                }
        //            }
        //            if (entity.YeuCauVatTuBenhViens.Any(o => o.VatTuBenhVien != null && o.VatTuBenhVien.VatTus != null))
        //            {
        //                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>VẬT TƯ TIÊU HAO</b></td>";
        //                htmlDanhSachDichVu += " </tr>";
        //                foreach (var ycvt in entity.YeuCauVatTuBenhViens.Where(o => o.VatTuBenhVien != null && o.VatTuBenhVien.VatTus != null))
        //                {
        //                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycvt.VatTuBenhVien.VatTus.Ten + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycvt.NoiCapVatTu != null && ycvt.NoiCapVatTu.KhoaPhong != null ? ycvt.NoiCapVatTu.KhoaPhong.Ten : "") + "-" + (ycvt.NoiCapVatTu != null ? "-" + ycvt.NoiCapVatTu.Ten : "") + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycvt.SoLuong + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " </tr>";
        //                    i++;
        //                }
        //            }
        //            if (entity.YeuCauDuocPhamBenhViens.Any(o => o.DuocPhamBenhVien != null && o.DuocPhamBenhVien.DuocPham != null))
        //            {
        //                htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                htmlDanhSachDichVu += "<td style='border: 1px solid #020000;'colspan='6'><b>DƯỢC PHẨM</b></td>";
        //                htmlDanhSachDichVu += " </tr>";
        //                foreach (var ycdp in entity.YeuCauDuocPhamBenhViens.Where(o => o.DuocPhamBenhVien != null && o.DuocPhamBenhVien.DuocPham != null))
        //                {
        //                    htmlDanhSachDichVu += " <tr style='border: 1px solid #020000;'>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + i + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'>" + ycdp.DuocPhamBenhVien.DuocPham.Ten + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + (ycdp.NoiCapThuoc != null && ycdp.NoiCapThuoc.KhoaPhong != null ? ycdp.NoiCapThuoc.KhoaPhong.Ten : "") + "-" + (ycdp.NoiCapThuoc != null ? "-" + ycdp.NoiCapThuoc.Ten : "") + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;text-align: center;'>" + ycdp.SoLuong + "</td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " <td style='border: 1px solid #020000;'></td>";
        //                    htmlDanhSachDichVu += " </tr>";
        //                    i++;
        //                }
        //            }
        //            data.NoiYeuCau = entity.NoiTiepNhan != null ? entity.NoiTiepNhan.Ten : "";
        //            data.ChuanDoanSoBo = "Đăng ký khám bệnh";
        //            //data.NguoiChiDinh = entity.NhanVienTiepNhan != null && entity.NhanVienTiepNhan.User!=null ? entity.NhanVienTiepNhan.User.HoTen : "";
        //            data.DanhSachDichVu = htmlDanhSachDichVu;
        //            var html = _danhSachChoKhamService.GetBodyByName("PhieuChiDinh");
        //            content += TemplateHelpper.FormatTemplateWithContentTemplate(html, data);
        //        }
        //    }
        //    return Ok(content);

        //}

        #endregion

        [HttpPost("HuyBenhNhanChoKham")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachChoKham)]
        public async Task<ActionResult> HuyBenhNhanChoKham(long id)
        {
            var ycTiepNhan = await _danhSachChoKhamService.GetByIdAsync(id, o => o.Include(p => p.BenhNhan).ThenInclude(bn => bn.TaiKhoanBenhNhan)
                    .Include(p => p.TaiKhoanBenhNhanChis)
                    .Include(p => p.TaiKhoanBenhNhanThus)
                    .Include(p => p.YeuCauDichVuGiuongBenhViens)
                    .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(k => k.PhongBenhVienHangDois)
                    .Include(p => p.YeuCauKhamBenhs).ThenInclude(k => k.PhongBenhVienHangDois)
                    .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.MienGiamChiPhis)
                    .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.MienGiamChiPhis));
            if (ycTiepNhan != null && !ycTiepNhan.TaiKhoanBenhNhanThus.Any()
                && ycTiepNhan.YeuCauDichVuGiuongBenhViens.All(o => o.TrangThai == Enums.EnumTrangThaiGiuongBenh.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiGiuongBenh.DaHuy)
                && ycTiepNhan.YeuCauDichVuKyThuats.All(o => (o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien || o.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) && (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan))
                && ycTiepNhan.YeuCauKhamBenhs.All(o => (o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham || o.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) && (o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.ChuaThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan || o.TrangThaiThanhToan == Enums.TrangThaiThanhToan.HuyThanhToan)))
            {
                foreach (var yeuCauKhamBenh in ycTiepNhan.YeuCauKhamBenhs)
                {
                    foreach (var phongBenhVienHangDoi in yeuCauKhamBenh.PhongBenhVienHangDois)
                    {
                        phongBenhVienHangDoi.WillDelete = true;
                    }
                    yeuCauKhamBenh.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham;
                    yeuCauKhamBenh.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;

                    //BVHD-3825
                    foreach (var mienGiam in yeuCauKhamBenh.MienGiamChiPhis.Where(x => x.DaHuy != true))
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;
                    }
                }
                foreach (var yeuCauDichVuKyThuat in ycTiepNhan.YeuCauDichVuKyThuats)
                {
                    foreach (var phongBenhVienHangDoi in yeuCauDichVuKyThuat.PhongBenhVienHangDois)
                    {
                        phongBenhVienHangDoi.WillDelete = true;
                    }
                    yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy;
                    yeuCauDichVuKyThuat.TrangThaiThanhToan = Enums.TrangThaiThanhToan.HuyThanhToan;

                    //BVHD-3825
                    foreach (var mienGiam in yeuCauDichVuKyThuat.MienGiamChiPhis.Where(x => x.DaHuy != true))
                    {
                        mienGiam.DaHuy = true;
                        mienGiam.WillDelete = true;
                    }
                }
                    
                ycTiepNhan.TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy;
                await _danhSachChoKhamService.UpdateAsync(ycTiepNhan);
                return NoContent();
            }
            throw new ApiException(_localizationService.GetResource("DanhSachChoKham.HuyTiepNhan"));
        }

        #region InPhieuCacDichVuKhamBenh
        [HttpGet("InPhieuCacDichVuKhamBenh")]
        public ActionResult InPhieuCacDichVuKhamBenh(long yeuCauTiepNhanId, string hostingName, bool header, bool laPhieuKhamBenh)//InPhieuCacDichVuKhamBenh
        {
            var result = _danhSachChoKhamService.InPhieuCacDichVuKhamBenh(yeuCauTiepNhanId, hostingName, header, laPhieuKhamBenh);
            return Ok(result);
        }
        #endregion

        #region InTheBenhNhan
        [HttpGet("InTheBenhNhan")]
        public ActionResult InTheBenhNhan(long yeuCauTiepNhanId, string hostingName)//InTheBenhNhan
        {
            var result = _danhSachChoKhamService.InTheBenhNhan(yeuCauTiepNhanId, hostingName);
            return Ok(result);
        }
        #endregion

        #region VongDeoTay
        [HttpGet("InVongDeoTay")]
        public ActionResult InVongDeoTay(long yeuCauTiepNhanId, string hostingName)//InTheBenhNhan
        {
            var result = _danhSachChoKhamService.InVongDeoTay(yeuCauTiepNhanId, hostingName);
            return Ok(result);
        }
        #endregion
        #region InGiayNghiHuongBHYT
        [HttpPost("XemGiayNghiHuongBHYTLien1")]
        public ActionResult XemGiayNghiHuongBHYTLien1(NghiHuongBHXHTiepNhanViewModel thongTinNgayNghi)//XemGiayNghiHuongBHYTLien1
        {
            ThongTinNgayNghiHuongBHYTTiepNhan thongTin = new ThongTinNgayNghiHuongBHYTTiepNhan
            {
                YeuCauTiepNhanId = thongTinNgayNghi.YeuCauTiepNhanId,
                ThoiDiemTiepNhan = thongTinNgayNghi.ThoiDiemTiepNhan,
                DenNgay = thongTinNgayNghi.DenNgay,
                BacSiKetLuanId = thongTinNgayNghi.BacSiKetLuanId
            };
            var result = _danhSachChoKhamService.XemGiayNghiHuongBHYTLien1(thongTin);
            return Ok(result);
        }

        [HttpPost("XemGiayNghiHuongBHYTLien2")]
        public ActionResult XemGiayNghiHuongBHYTLien2(NghiHuongBHXHTiepNhanViewModel thongTinNgayNghi)//XemGiayNghiHuongBHYTLien2
        {
            ThongTinNgayNghiHuongBHYTTiepNhan thongTin = new ThongTinNgayNghiHuongBHYTTiepNhan
            {
                YeuCauTiepNhanId = thongTinNgayNghi.YeuCauTiepNhanId,
                ThoiDiemTiepNhan = thongTinNgayNghi.ThoiDiemTiepNhan,
                DenNgay = thongTinNgayNghi.DenNgay,
                BacSiKetLuanId = thongTinNgayNghi.BacSiKetLuanId
            };
            var result = _danhSachChoKhamService.XemGiayNghiHuongBHYTLien2(thongTin);
            return Ok(result);
        }



        #endregion

        [HttpPost("BacSiKhamBenhTiepNhan")]
        public async Task<ActionResult> BacSiKhamBenhTiepNhan([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _danhSachChoKhamService.GetBacSiKhamBenhs(queryInfo);
            return Ok(lookup);
        }

        #region Get Thông tin yêu cầu tiếp nhận  
        [HttpPost("GetThongTinYeuCauTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhSachChoKham)]
        public ActionResult GetThongTinYeuCauTiepNhan(TimKiemThongTin timKiemThongTinBenhNhan)
        {
            var result = _danhSachChoKhamService.GetThongTinYeuCauTiepNhan(timKiemThongTinBenhNhan);
            return Ok(result);
        }
        #endregion

        #region Excel
        [HttpPost("ExportDanhSachTiepNhan")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachChoKham)]
        public async Task<ActionResult> ExportDanhSachTiepNhan(QueryInfo queryInfo)
        {
            var gridData = await _danhSachChoKhamService.GetDataForGridAsyncDanhSachChoKham(queryInfo, true);
            var danhSachTiepNhanData = gridData.Data.Select(p => (DanhSachChoKhamGridVo)p).ToList();
            var excelData = danhSachTiepNhanData.Map<List<DanhSachTiepNhanExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(DanhSachTiepNhanExportExcel.MaYeuCauTiepNhan), "Mã TN"),
                (nameof(DanhSachTiepNhanExportExcel.MaBenhNhan), "Mã BN"),
                (nameof(DanhSachTiepNhanExportExcel.HoTen), "Tên người bệnh"),
                (nameof(DanhSachTiepNhanExportExcel.NamSinh), "Năm sinh"),
                (nameof(DanhSachTiepNhanExportExcel.DiaChi), "Địa chỉ"),
                (nameof(DanhSachTiepNhanExportExcel.TenNhanVienTiepNhan), "Người tiếp nhận"),
                (nameof(DanhSachTiepNhanExportExcel.ThoiDiemTiepNhanDisplay), "Tiếp nhận lúc"),
                (nameof(DanhSachTiepNhanExportExcel.TrieuChungTiepNhan), "Lý do khám bệnh"),
                (nameof(DanhSachTiepNhanExportExcel.DoiTuong), "Đối tượng")
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Tiếp Nhận", 2, "DS Tiếp Nhận");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DSTiepNhan" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        #endregion
    }
}