using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Camino.Services.BaoCao
{
    public partial class BaoCaoService
    {
        public async Task<GridDataSource> GetDataBaoCaoKeToanBangKeChiTietNguoiBenhForGridAsync(QueryInfo queryInfo, bool? isToTal = false)
        {
            if (isToTal == true)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = Int32.MaxValue;
            }

            var lstTiepNhanTheoNoiGioiThieu = new List<BaoCaoKeToanBangKeChiTietNguoiBenhGridVo>();
            var lstBangKeChiTiet = new List<BaoCaoKeToanBangKeChiTietNguoiBenhGridVo>();
            var lstYeuCauTiepNhanNgoaiTruIdTheoTimKiem = new List<long>();

            #region xử lý tìm kiếm nâng cao
            var timKiemNangCaoObj = new BaoCaoKeToanBangKeChiTietNguoiBenhTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoKeToanBangKeChiTietNguoiBenhTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
                if (timKiemNangCaoObj.MaYeuCauTiepNhan == null
                    || timKiemNangCaoObj.MaYeuCauTiepNhan.Contains("undefined")
                    || timKiemNangCaoObj.MaYeuCauTiepNhan == "0")
                {
                    timKiemNangCaoObj.MaYeuCauTiepNhan = null;
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }
            #endregion

            // Riêng đối với ngoại trú:
            // -> sau khi tìm kiếm lượt đầu theo điều kiện filter, và xử lý check YCTN theo nơi giới thiệu
            // -> nếu có phát sinh YCTN chưa get data dịch vụ, thì xử lý gưt bổ sung cho các dịch vụ này
            if ((tuNgay != null && denNgay != null) || !string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan))
            {
                var hinhThucDenGioiThieu = _cauHinhService.GetSetting("CauHinhBaoCao.HinhThucDenGioiThieu");
                long.TryParse(hinhThucDenGioiThieu?.Value, out long hinhThucDenGioiThieuId);

                if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true) // xử lý get all dịch vụ rồi mới kiểm tra YCTN theo filter
                {
                    // tham khảo từ GetDataBaoCaoChiTietMienPhiTronVienForGridAsync
                    #region get chi phí theo dịch vụ

                    #region Chi phí khám
                    var chiPhiKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.YeuCauTiepNhan.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                    && x.KhongTinhPhi != true
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                    && (timKiemNangCaoObj.HinhThucDenId == null 
                                        || timKiemNangCaoObj.HinhThucDenId == 0 
                                        || (timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)

                                        || (timKiemNangCaoObj.HinhThucDenId == hinhThucDenGioiThieuId 
                                            && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId
                                            && (timKiemNangCaoObj.NoiGioiThieuId == null
                                                || timKiemNangCaoObj.NoiGioiThieuId == 0
                                                || x.YeuCauTiepNhan.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))))
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            YeucauTiepNhanId = x.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                            HoTen = x.YeuCauTiepNhan.HoTen,
                            MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                            NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                            LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                            DichVuBenhVienId = x.DichVuKhamBenhBenhVienId,
                            NoiDung = x.TenDichVu,
                            SoLuong = 1,
                            DonGiaBan = x.YeuCauGoiDichVuId != null ? x.DonGiaSauChietKhau : x.Gia,
                            MienGiam = x.SoTienMienGiam,
                            DuocHuongBHYT = x.DuocHuongBaoHiem,
                            BaoHiemChiTra = x.BaoHiemChiTra,
                            DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                            TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault()
                        })
                        .ToList();
                    #endregion

                    #region Chi phí kỹ thuật
                    var chiPhiKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.YeuCauTiepNhan.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                    && x.KhongTinhPhi != true
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                    && (timKiemNangCaoObj.HinhThucDenId == null
                                        || timKiemNangCaoObj.HinhThucDenId == 0
                                        || (timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)

                                        || (timKiemNangCaoObj.HinhThucDenId == hinhThucDenGioiThieuId
                                            && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId
                                            && (timKiemNangCaoObj.NoiGioiThieuId == null
                                                || timKiemNangCaoObj.NoiGioiThieuId == 0
                                                || x.YeuCauTiepNhan.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))))
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            Nhom = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                            TenNhom = x.LoaiDichVuKyThuat.GetDescription(),
                            YeucauTiepNhanId = x.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                            HoTen = x.YeuCauTiepNhan.HoTen,
                            MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                            NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                            LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                            DichVuBenhVienId = x.DichVuKyThuatBenhVienId,
                            NoiDung = x.TenDichVu,
                            SoLuong = x.SoLan,
                            DonGiaBan = x.YeuCauGoiDichVuId != null ? x.DonGiaSauChietKhau : x.Gia,
                            MienGiam = x.SoTienMienGiam,
                            DuocHuongBHYT = x.DuocHuongBaoHiem,
                            BaoHiemChiTra = x.BaoHiemChiTra,
                            DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                            TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault()
                        })
                        .ToList();
                    #endregion

                    #region Chi phí dược phẩm
                    var chiPhiThuocs = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.YeuCauTiepNhan.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                    && x.KhongTinhPhi != true
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                    && (timKiemNangCaoObj.HinhThucDenId == null
                                        || timKiemNangCaoObj.HinhThucDenId == 0
                                        || (timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)

                                        || (timKiemNangCaoObj.HinhThucDenId == hinhThucDenGioiThieuId
                                            && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId
                                            && (timKiemNangCaoObj.NoiGioiThieuId == null
                                                || timKiemNangCaoObj.NoiGioiThieuId == 0
                                                || x.YeuCauTiepNhan.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))))
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            Nhom = Enums.EnumNhomGoiDichVu.DuocPham,
                            YeucauTiepNhanId = x.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                            HoTen = x.YeuCauTiepNhan.HoTen,
                            MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                            NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                            LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                            DichVuBenhVienId = x.DuocPhamBenhVienId,
                            NoiDung = x.Ten,
                            DonViTinh = x.DonViTinh.Ten,
                            SoLuong = x.SoLuong,
                            DonGiaNhapKho = x.DonGiaNhap,
                            //DonGiaBan = x.DonGiaBan,
                            MienGiam = x.SoTienMienGiam,
                            DuocHuongBHYT = x.DuocHuongBaoHiem,
                            BaoHiemChiTra = x.BaoHiemChiTra,
                            DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                            TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                            // xử lý tự tính giá bán
                            LaTuTinhGiaBan = true,
                            XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietId,
                            VAT = x.VAT,
                            TiLeTheoThapGia = x.TiLeTheoThapGia,
                            PhuongPhapTinhGiaTriTonKhos = x.XuatKhoDuocPhamChiTietId != null
                                ? x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                        })
                        .ToList();
                    #endregion

                    #region Chi phí vật tư
                    var chiPhiVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                        .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.YeuCauTiepNhan.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                    && x.KhongTinhPhi != true
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                    && (timKiemNangCaoObj.HinhThucDenId == null
                                        || timKiemNangCaoObj.HinhThucDenId == 0
                                        || (timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)

                                        || (timKiemNangCaoObj.HinhThucDenId == hinhThucDenGioiThieuId
                                            && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId
                                            && (timKiemNangCaoObj.NoiGioiThieuId == null
                                                || timKiemNangCaoObj.NoiGioiThieuId == 0
                                                || x.YeuCauTiepNhan.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))))
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            Nhom = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                            YeucauTiepNhanId = x.YeuCauTiepNhanId,
                            MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                            HoTen = x.YeuCauTiepNhan.HoTen,
                            MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                            NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                            LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                            DichVuBenhVienId = x.VatTuBenhVienId,
                            NoiDung = x.Ten,
                            DonViTinh = x.DonViTinh,
                            SoLuong = x.SoLuong,
                            DonGiaNhapKho = x.DonGiaNhap,
                            //DonGiaBan = x.DonGiaBan,
                            MienGiam = x.SoTienMienGiam,
                            DuocHuongBHYT = x.DuocHuongBaoHiem,
                            BaoHiemChiTra = x.BaoHiemChiTra,
                            DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                            TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                            MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                            // xử lý tự tính giá bán
                            LaTuTinhGiaBan = true,
                            XuatKhoChiTietId = x.XuatKhoVatTuChiTietId,
                            VAT = x.VAT,
                            TiLeTheoThapGia = x.TiLeTheoThapGia,
                            PhuongPhapTinhGiaTriTonKhos = x.XuatKhoVatTuChiTietId != null
                                ? x.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Select(a => a.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                        })
                        .ToList();
                    #endregion

                    #region Chi phí đơn thuốc
                    var chiPhiDonThuocs = _donThuocThanhToanRepository.TableNoTracking
                        .Where(x => x.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT 
                                    && x.YeuCauTiepNhanId != null
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.YeuCauTiepNhan.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    && x.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                    && (timKiemNangCaoObj.HinhThucDenId == null
                                        || timKiemNangCaoObj.HinhThucDenId == 0
                                        || (timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)

                                        || (timKiemNangCaoObj.HinhThucDenId == hinhThucDenGioiThieuId
                                            && x.YeuCauTiepNhan.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId
                                            && (timKiemNangCaoObj.NoiGioiThieuId == null
                                                || timKiemNangCaoObj.NoiGioiThieuId == 0
                                                || x.YeuCauTiepNhan.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))))
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            Id = x.Id,
                            YeucauTiepNhanId = x.YeuCauTiepNhanId.Value,
                            MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                            HoTen = x.YeuCauTiepNhan.HoTen,
                            MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                            NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                            LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan
                        })
                        .ToList();

                    var donThuocThanhToanChiTiets = new List<BaoCaoKeToanBangKeChiTietNguoiBenhGridVo>();
                    var lstDonThuocId = chiPhiDonThuocs.Select(x => x.Id).ToList();
                    if (lstDonThuocId.Any())
                    {
                        donThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                            .Where(x => lstDonThuocId.Contains(x.DonThuocThanhToanId)
                                        && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.CreatedOn >= tuNgay)
                                        && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.CreatedOn <= denNgay))
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Nhom = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                                Id = x.DonThuocThanhToanId,
                                DichVuBenhVienId = x.DuocPhamId,
                                NoiDung = x.Ten,
                                DonViTinh = x.DonViTinh.Ten,
                                SoLuong = x.SoLuong,
                                DonGiaNhapKho = x.DonGiaNhap,
                                //DonGiaBan = x.DonGiaBan,
                                MienGiam = x.SoTienMienGiam,
                                DuocHuongBHYT = x.DuocHuongBaoHiem,
                                BaoHiemChiTra = x.BaoHiemChiTra,
                                DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                                // xử lý tự tính giá bán
                                LaTuTinhGiaBan = true,
                                XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTietId,
                                VAT = x.VAT,
                                TiLeTheoThapGia = x.TiLeTheoThapGia,
                                PhuongPhapTinhGiaTriTonKhos = x.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                            })
                            .ToList();
                        if (donThuocThanhToanChiTiets.Any())
                        {
                            foreach (var chiTiet in donThuocThanhToanChiTiets)
                            {
                                var donThuocThanhToan = chiPhiDonThuocs.FirstOrDefault(x => x.Id == chiTiet.Id);
                                if (donThuocThanhToan != null)
                                {
                                    chiTiet.YeucauTiepNhanId = donThuocThanhToan.YeucauTiepNhanId;
                                    chiTiet.MaYeuCauTiepNhan = donThuocThanhToan.MaYeuCauTiepNhan;
                                    chiTiet.TrangThaiYeuCauTiepNhan = donThuocThanhToan.TrangThaiYeuCauTiepNhan;
                                    chiTiet.BenhNhanId = donThuocThanhToan.BenhNhanId;
                                    chiTiet.HoTen = donThuocThanhToan.HoTen;
                                    chiTiet.MaBN = donThuocThanhToan.MaBN;
                                    chiTiet.NoiGioiThieuId = donThuocThanhToan.NoiGioiThieuId;
                                    chiTiet.NoiGioiThieuDisplay = donThuocThanhToan.NoiGioiThieuDisplay;
                                    chiTiet.TenHinhThucDen = donThuocThanhToan.TenHinhThucDen;
                                    chiTiet.LaGioiThieu = donThuocThanhToan.LaGioiThieu;
                                    chiTiet.ThoiDiemTiepNhan = donThuocThanhToan.ThoiDiemTiepNhan;
                                }
                            }
                        }
                    }
                    #endregion

                    #endregion

                    lstBangKeChiTiet = lstBangKeChiTiet
                        .Concat(chiPhiKhamBenhs)
                        .Concat(chiPhiKyThuats)
                        .Concat(chiPhiThuocs)
                        .Concat(chiPhiVatTus)
                        .Concat(donThuocThanhToanChiTiets)
                        .ToList();
                    
                    #region //xử lý bỏ những YCTN có nhập viện
                    var lstMaYCTN = lstBangKeChiTiet.Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();
                    if (lstMaYCTN.Any())
                    {
                        var lstMaYeuCauChiTiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                            .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                        && lstMaYCTN.Contains(x.MaYeuCauTiepNhan)
                                        && x.NoiTruBenhAn != null)
                            .Select(x => x.MaYeuCauTiepNhan).ToList();

                        lstBangKeChiTiet = lstBangKeChiTiet
                            .Where(x => !lstMaYeuCauChiTiepNhanNoiTru.Contains(x.MaYeuCauTiepNhan)).ToList();
                    }
                    #endregion

                    lstTiepNhanTheoNoiGioiThieu = lstBangKeChiTiet
                        .GroupBy(x => new {x.YeucauTiepNhanId})
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            YeucauTiepNhanId = x.Key.YeucauTiepNhanId,
                            MaYeuCauTiepNhan = x.First().MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.First().TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.First().BenhNhanId,
                            HoTen = x.First().HoTen,
                            MaBN = x.First().MaBN,
                            NoiGioiThieuId = x.First().NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.First().NoiGioiThieuDisplay,
                            TenHinhThucDen = x.First().TenHinhThucDen,
                            LaGioiThieu = x.First().LaGioiThieu,
                            ThoiDiemTiepNhan = x.First().ThoiDiemTiepNhan
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();

                    lstYeuCauTiepNhanNgoaiTruIdTheoTimKiem = lstTiepNhanTheoNoiGioiThieu.Select(x => x.YeucauTiepNhanId).Distinct().ToList();
                }
                else
                {
                    if (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || timKiemNangCaoObj.HinhThucDenId != hinhThucDenGioiThieuId)
                    {
                        lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                            .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                            .Where(x =>
                                        x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                        && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                        && x.NoiTruBenhAn.ThoiDiemRaVien != null
                                        && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.NoiTruBenhAn.ThoiDiemRaVien.Value >= tuNgay)
                                        && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.NoiTruBenhAn.ThoiDiemRaVien.Value <= denNgay)
                                        && (timKiemNangCaoObj.HinhThucDenId == null || timKiemNangCaoObj.HinhThucDenId == 0 || x.HinhThucDenId == timKiemNangCaoObj.HinhThucDenId)
                                        && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                        && x.BenhNhanId != null)
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                YeucauTiepNhanId = x.Id,
                                MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.BenhNhanId.Value,
                                HoTen = x.HoTen,
                                MaBN = x.BenhNhan.MaBN,
                                NoiGioiThieuId = x.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.HinhThucDen.Ten,
                                LaGioiThieu = x.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                            })
                            .OrderBy(x => x.ThoiDiemTiepNhan)
                            .ToList();
                    }
                    else
                    {
                        lstTiepNhanTheoNoiGioiThieu = _yeuCauTiepNhanRepository.TableNoTracking
                        .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.MaYeuCauTiepNhan, x => x.HoTen, x => x.BenhNhan.MaBN)
                        .Where(x => x.NoiGioiThieuId != null
                                    && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) || x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                                    && x.NoiTruBenhAn.ThoiDiemRaVien != null
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.NoiTruBenhAn.ThoiDiemRaVien.Value >= tuNgay)
                                    && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.NoiTruBenhAn.ThoiDiemRaVien.Value <= denNgay)

                                     && (timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0 || (x.NoiGioiThieuId == timKiemNangCaoObj.NoiGioiThieuId))
                                     && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                     && x.BenhNhanId != null)
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.BenhNhanId.Value,
                            HoTen = x.HoTen,
                            MaBN = x.BenhNhan.MaBN,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();
                    }
                }


                if (lstTiepNhanTheoNoiGioiThieu.Any())
                {
                    var lstBenhNhanId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.BenhNhanId).Distinct().ToList();
                    var lstTiepNhanTheoBenhNhan = _yeuCauTiepNhanRepository.TableNoTracking
                        .Where(x => x.BenhNhanId != null
                                    && x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru
                                    && lstBenhNhanId.Contains(x.BenhNhanId.Value)
                                    && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                        .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                        {
                            YeucauTiepNhanId = x.Id,
                            MaYeuCauTiepNhan = x.MaYeuCauTiepNhan,
                            TrangThaiYeuCauTiepNhan = x.TrangThaiYeuCauTiepNhan,
                            BenhNhanId = x.BenhNhanId.Value,
                            HoTen = x.HoTen,
                            MaBN = x.BenhNhan.MaBN,
                            NoiGioiThieuId = x.NoiGioiThieuId,
                            NoiGioiThieuDisplay = x.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.NoiGioiThieu.DonVi) ? $" - {x.NoiGioiThieu.DonVi}" : ""),
                            TenHinhThucDen = x.HinhThucDen.Ten,
                            LaGioiThieu = x.NoiGioiThieuId != null,
                            ThoiDiemTiepNhan = x.ThoiDiemTiepNhan,

                            // dùng để xác định người giới thiệu trước đó nếu data hiện tại ko có người giới thiệu
                            //LaDataTheoDieuKienTimKiem = (!string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan)
                            //                                && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan))
                            //                            || (!string.IsNullOrEmpty(timKiemNangCaoObj.FromDate)
                            //                                    && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate)
                            //                                    && x.ThoiDiemTiepNhan >= tuNgay
                            //                                    && x.ThoiDiemTiepNhan <= denNgay)
                            LaDataTheoDieuKienTimKiem = !string.IsNullOrEmpty(timKiemNangCaoObj.MaYeuCauTiepNhan) && x.MaYeuCauTiepNhan.Contains(timKiemNangCaoObj.MaYeuCauTiepNhan)
                        })
                        .OrderBy(x => x.ThoiDiemTiepNhan)
                        .ToList();

                    #region Xử lý get lst tiếp nhận theo khoảng thời gian tìm hoặc mã tiếp nhận
                    var lstTiepNhanIdTheoBenhNhan = lstTiepNhanTheoBenhNhan
                        .Where(x => x.LaDataTheoDieuKienTimKiem != true)
                        .Select(x => x.YeucauTiepNhanId).Distinct().ToList();
                    var lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem = new List<long>();
                    if (lstTiepNhanIdTheoBenhNhan.Any())
                    {
                        #region dịch vụ khám
                        var lstKhamYCTNId = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                        && x.KhongTinhPhi != true
                                        && lstTiepNhanIdTheoBenhNhan.Contains(x.YeuCauTiepNhanId)
                                        && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.CreatedOn >= tuNgay)
                                        && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.CreatedOn <= denNgay))
                            .Select(x => x.YeuCauTiepNhanId)
                            .Distinct().ToList();
                        lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.AddRange(lstKhamYCTNId);
                        // giảm bớt điều kiện query
                        lstTiepNhanIdTheoBenhNhan = lstTiepNhanIdTheoBenhNhan
                            .Where(x => !lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.Contains(x)).ToList();
                        #endregion

                        #region dịch vụ kỹ thuật
                        if (lstTiepNhanIdTheoBenhNhan.Any())
                        {
                            var lstKyThatYCTNId = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                            && x.KhongTinhPhi != true
                                            && lstTiepNhanIdTheoBenhNhan.Contains(x.YeuCauTiepNhanId)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.CreatedOn >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.CreatedOn <= denNgay))
                                .Select(x => x.YeuCauTiepNhanId)
                                .Distinct().ToList();
                            lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.AddRange(lstKyThatYCTNId);
                            // giảm bớt điều kiện query
                            lstTiepNhanIdTheoBenhNhan = lstTiepNhanIdTheoBenhNhan
                                .Where(x => !lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.Contains(x)).ToList();
                        }
                        #endregion

                        #region thuốc
                        if (lstTiepNhanIdTheoBenhNhan.Any())
                        {
                            var lstThuocYCTNId = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && x.KhongTinhPhi != true
                                            && lstTiepNhanIdTheoBenhNhan.Contains(x.YeuCauTiepNhanId)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.CreatedOn >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.CreatedOn <= denNgay))
                                .Select(x => x.YeuCauTiepNhanId)
                                .Distinct().ToList();
                            lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.AddRange(lstThuocYCTNId);
                            // giảm bớt điều kiện query
                            lstTiepNhanIdTheoBenhNhan = lstTiepNhanIdTheoBenhNhan
                                .Where(x => !lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.Contains(x)).ToList();
                        }
                        #endregion

                        #region vật tư
                        if (lstTiepNhanIdTheoBenhNhan.Any())
                        {
                            var lstVatTuYCTNId = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                            && x.KhongTinhPhi != true
                                            && lstTiepNhanIdTheoBenhNhan.Contains(x.YeuCauTiepNhanId)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.CreatedOn >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.CreatedOn <= denNgay))
                                .Select(x => x.YeuCauTiepNhanId)
                                .Distinct().ToList();
                            lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.AddRange(lstVatTuYCTNId);
                            // giảm bớt điều kiện query
                            lstTiepNhanIdTheoBenhNhan = lstTiepNhanIdTheoBenhNhan
                                .Where(x => !lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.Contains(x)).ToList();
                        }
                        #endregion

                        #region đơn thuốc
                        if (lstTiepNhanIdTheoBenhNhan.Any())
                        {
                            var lstDonThuocYCTNId = _donThuocThanhToanChiTietRepository.TableNoTracking
                                .Where(x => x.DonThuocThanhToan.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT
                                            && x.DonThuocThanhToan.YeuCauTiepNhanId != null
                                            && lstTiepNhanIdTheoBenhNhan.Contains(x.DonThuocThanhToan.YeuCauTiepNhanId.Value)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.CreatedOn >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.CreatedOn <= denNgay))
                                .Select(x => x.DonThuocThanhToan.YeuCauTiepNhanId.Value)
                                .Distinct().ToList();
                            lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.AddRange(lstDonThuocYCTNId);
                        }
                        #endregion
                    }

                    foreach (var tiepNhan in lstTiepNhanTheoBenhNhan)
                    {
                        tiepNhan.LaDataTheoDieuKienTimKiem =
                            //lstTiepNhanTheoNoiGioiThieu.Any(a => a.YeucauTiepNhanId == tiepNhan.YeucauTiepNhanId);
                            lstTiepNhanIdTheoBenhNhanTheoDieuKienTimKiem.Any(a => a == tiepNhan.YeucauTiepNhanId);
                    }
                    #endregion

                    //todo: update nơi giới thiệu
                    foreach (var benhNhanId in lstBenhNhanId)
                    {
                        var lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                            .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();
                        if (lanTiepNhanDauTienCoGioiThieu == null)
                        {
                            lanTiepNhanDauTienCoGioiThieu = lstTiepNhanTheoBenhNhan
                                .Where(x => x.BenhNhanId == benhNhanId && x.NoiGioiThieuId != null)
                                .OrderBy(x => x.YeucauTiepNhanId).FirstOrDefault();

                            if (lanTiepNhanDauTienCoGioiThieu == null)
                            {
                                continue;
                            }
                        }

                        var tiepNhanBenhNhans = lstTiepNhanTheoBenhNhan
                            .Where(x => x.YeucauTiepNhanId > lanTiepNhanDauTienCoGioiThieu.YeucauTiepNhanId
                                        && x.BenhNhanId == benhNhanId)
                            .ToList();

                        var khongThemNguoiGioiThieu = false;
                        var nguoiGioiThieuHienTaiId = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuId;
                        var tenNguoiGioiThieuHienTai = lanTiepNhanDauTienCoGioiThieu.NoiGioiThieuDisplay;
                        foreach (var lanTiepNhan in tiepNhanBenhNhans)
                        {
                            if (lanTiepNhan.NoiGioiThieuId != null)
                            {
                                if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId)
                                    || ((timKiemNangCaoObj.NoiGioiThieuId == null || timKiemNangCaoObj.NoiGioiThieuId == 0) && lanTiepNhan.NoiGioiThieuId != nguoiGioiThieuHienTaiId))
                                {
                                    // trường hợp tìm kiếm theo người giới thiệu thì chỉ thêm người giới thiệu đang tìm kiếm thôi
                                    if ((timKiemNangCaoObj.NoiGioiThieuId != null && timKiemNangCaoObj.NoiGioiThieuId != 0 && lanTiepNhan.NoiGioiThieuId != timKiemNangCaoObj.NoiGioiThieuId))
                                    {
                                        khongThemNguoiGioiThieu = true;
                                    }
                                    else
                                    {
                                        khongThemNguoiGioiThieu = false;
                                    }
                                    nguoiGioiThieuHienTaiId = lanTiepNhan.NoiGioiThieuId;
                                    tenNguoiGioiThieuHienTai = lanTiepNhan.NoiGioiThieuDisplay;
                                }
                                else
                                {
                                    khongThemNguoiGioiThieu = false;
                                }
                            }

                            if (!khongThemNguoiGioiThieu && lanTiepNhan.NoiGioiThieuId == null)
                            {
                                if (lstTiepNhanTheoNoiGioiThieu.All(x => x.YeucauTiepNhanId != lanTiepNhan.YeucauTiepNhanId) && lanTiepNhan.LaDataTheoDieuKienTimKiem == true)
                                {
                                    lanTiepNhan.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                    lanTiepNhan.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                    lanTiepNhan.LaGioiThieu = true;
                                    lstTiepNhanTheoNoiGioiThieu.Add(lanTiepNhan);
                                }
                                else
                                {
                                    var lanTiepNhanDaThem = lstTiepNhanTheoNoiGioiThieu.FirstOrDefault(x => x.YeucauTiepNhanId == lanTiepNhan.YeucauTiepNhanId);
                                    if (lanTiepNhanDaThem != null)
                                    {
                                        lanTiepNhanDaThem.NoiGioiThieuId = nguoiGioiThieuHienTaiId;
                                        lanTiepNhanDaThem.NoiGioiThieuDisplay = tenNguoiGioiThieuHienTai;
                                        lanTiepNhanDaThem.LaGioiThieu = true;
                                    }

                                }
                            }
                        }
                    }

                    #region xử lý check trường hợp chọn ngoại trú -> chỉ lấy các YCTN ngoại trú của người bệnh mà không có nhập viện (chưa tạo bệnh án)
                    if (timKiemNangCaoObj.LaNguoiBenhNgoaiTru == true)
                    {
                        var lstYeuCauChiTiepNhanNgoaiTruId = lstTiepNhanTheoNoiGioiThieu.Select(x => x.YeucauTiepNhanId).Distinct().ToList();

                        // xử lý get bổ sung chi phí dịch vụ
                        var lstYeuCauTiepNhanIdChuaGetDichVu = lstYeuCauChiTiepNhanNgoaiTruId.Where(x => !lstYeuCauTiepNhanNgoaiTruIdTheoTimKiem.Contains(x)).ToList();

                        #region //xử lý bỏ những YCTN có nhập viện
                        var lstMaYeuCauTiepNhanChuaGetDichVu = lstTiepNhanTheoNoiGioiThieu
                            .Where(x => lstYeuCauTiepNhanIdChuaGetDichVu.Contains(x.YeucauTiepNhanId))
                            .Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();
                        if (lstMaYeuCauTiepNhanChuaGetDichVu.Any())
                        {
                            var lstYeuCauChiTiepNhanNoiTruId = _yeuCauTiepNhanRepository.TableNoTracking
                                .Where(x => x.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNoiTru
                                            && lstMaYeuCauTiepNhanChuaGetDichVu.Contains(x.MaYeuCauTiepNhan)
                                            && x.NoiTruBenhAn != null
                                            && x.YeuCauTiepNhanNgoaiTruCanQuyetToanId != null)
                                .Select(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId.Value).ToList();

                            lstYeuCauTiepNhanIdChuaGetDichVu = lstYeuCauTiepNhanIdChuaGetDichVu.Where(x => !lstYeuCauChiTiepNhanNoiTruId.Contains(x)).ToList();
                        }
                        #endregion

                        if (lstYeuCauTiepNhanIdChuaGetDichVu.Any())
                        {
                            #region get chi phí theo dịch vụ

                            #region Chi phí khám
                            var chiPhiKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                            && x.KhongTinhPhi != true
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                            && lstYeuCauTiepNhanIdChuaGetDichVu.Contains(x.YeuCauTiepNhanId))
                                .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                                {
                                    Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                                    YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                    MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                    BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                    HoTen = x.YeuCauTiepNhan.HoTen,
                                    MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                    NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                    NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                    TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                    LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                    ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                    DichVuBenhVienId = x.DichVuKhamBenhBenhVienId,
                                    NoiDung = x.TenDichVu,
                                    SoLuong = 1,
                                    DonGiaBan = x.YeuCauGoiDichVuId != null ? x.DonGiaSauChietKhau : x.Gia,
                                    MienGiam = x.SoTienMienGiam,
                                    DuocHuongBHYT = x.DuocHuongBaoHiem,
                                    BaoHiemChiTra = x.BaoHiemChiTra,
                                    DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault()
                                })
                                .ToList();
                            if (chiPhiKhamBenhs.Any())
                            {
                                lstBangKeChiTiet.AddRange(chiPhiKhamBenhs);
                            }
                            #endregion

                            #region Chi phí kỹ thuật
                            var chiPhiKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                            && x.KhongTinhPhi != true
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                            && lstYeuCauTiepNhanIdChuaGetDichVu.Contains(x.YeuCauTiepNhanId))
                                .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                                {
                                    Nhom = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                                    TenNhom = x.LoaiDichVuKyThuat.GetDescription(),
                                    YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                    MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                    BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                    HoTen = x.YeuCauTiepNhan.HoTen,
                                    MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                    NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                    NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                    TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                    LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                    ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                    DichVuBenhVienId = x.DichVuKyThuatBenhVienId,
                                    NoiDung = x.TenDichVu,
                                    SoLuong = x.SoLan,
                                    DonGiaBan = x.YeuCauGoiDichVuId != null ? x.DonGiaSauChietKhau : x.Gia,
                                    MienGiam = x.SoTienMienGiam,
                                    DuocHuongBHYT = x.DuocHuongBaoHiem,
                                    BaoHiemChiTra = x.BaoHiemChiTra,
                                    DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault()
                                })
                                .ToList();
                            if (chiPhiKyThuats.Any())
                            {
                                lstBangKeChiTiet.AddRange(chiPhiKyThuats);
                            }
                            #endregion

                            #region Chi phí dược phẩm
                            var chiPhiThuocs = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                            && x.KhongTinhPhi != true
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                            && lstYeuCauTiepNhanIdChuaGetDichVu.Contains(x.YeuCauTiepNhanId))
                                .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                                {
                                    Nhom = Enums.EnumNhomGoiDichVu.DuocPham,
                                    YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                    MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                    BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                    HoTen = x.YeuCauTiepNhan.HoTen,
                                    MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                    NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                    NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                    TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                    LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                    ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                    DichVuBenhVienId = x.DuocPhamBenhVienId,
                                    NoiDung = x.Ten,
                                    DonViTinh = x.DonViTinh.Ten,
                                    SoLuong = x.SoLuong,
                                    DonGiaNhapKho = x.DonGiaNhap,
                                    //DonGiaBan = x.DonGiaBan,
                                    MienGiam = x.SoTienMienGiam,
                                    DuocHuongBHYT = x.DuocHuongBaoHiem,
                                    BaoHiemChiTra = x.BaoHiemChiTra,
                                    DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                                    // xử lý tự tính giá bán
                                    LaTuTinhGiaBan = true,
                                    XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietId,
                                    VAT = x.VAT,
                                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKhos = x.XuatKhoDuocPhamChiTietId != null
                                        ? x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                        : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                                })
                                .ToList();
                            if (chiPhiThuocs.Any())
                            {
                                lstBangKeChiTiet.AddRange(chiPhiThuocs);
                            }
                            #endregion

                            #region Chi phí vật tư
                            var chiPhiVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                                .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                            && x.KhongTinhPhi != true
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.ThoiDiemChiDinh >= tuNgay)
                                            && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.ThoiDiemChiDinh <= denNgay)
                                            && lstYeuCauTiepNhanIdChuaGetDichVu.Contains(x.YeuCauTiepNhanId))
                                .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                                {
                                    Nhom = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                                    YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                    MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                    BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                    HoTen = x.YeuCauTiepNhan.HoTen,
                                    MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                    NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                    NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                    TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                    LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                    ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                    DichVuBenhVienId = x.VatTuBenhVienId,
                                    NoiDung = x.Ten,
                                    DonViTinh = x.DonViTinh,
                                    SoLuong = x.SoLuong,
                                    DonGiaNhapKho = x.DonGiaNhap,
                                    //DonGiaBan = x.DonGiaBan,
                                    MienGiam = x.SoTienMienGiam,
                                    DuocHuongBHYT = x.DuocHuongBaoHiem,
                                    BaoHiemChiTra = x.BaoHiemChiTra,
                                    DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                                    // xử lý tự tính giá bán
                                    LaTuTinhGiaBan = true,
                                    XuatKhoChiTietId = x.XuatKhoVatTuChiTietId,
                                    VAT = x.VAT,
                                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKhos = x.XuatKhoVatTuChiTietId != null
                                        ? x.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Select(a => a.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                        : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                                })
                                .ToList();
                            if (chiPhiVatTus.Any())
                            {
                                lstBangKeChiTiet.AddRange(chiPhiVatTus);
                            }
                            #endregion

                            #region Chi phí đơn thuốc
                            var chiPhiDonThuocs = _donThuocThanhToanRepository.TableNoTracking
                                .Where(x => x.LoaiDonThuoc == Enums.EnumLoaiDonThuoc.ThuocBHYT
                                            && x.YeuCauTiepNhanId != null
                                            && lstYeuCauTiepNhanIdChuaGetDichVu.Contains(x.YeuCauTiepNhanId.Value))
                                .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                                {
                                    Id = x.Id,
                                    YeucauTiepNhanId = x.YeuCauTiepNhanId.Value,
                                    MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                    TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                    BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                    HoTen = x.YeuCauTiepNhan.HoTen,
                                    MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                    NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                    NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                    TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                    LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                    ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan
                                })
                                .ToList();

                            var donThuocThanhToanChiTiets = new List<BaoCaoKeToanBangKeChiTietNguoiBenhGridVo>();
                            var lstDonThuocId = chiPhiDonThuocs.Select(x => x.Id).ToList();
                            if (lstDonThuocId.Any())
                            {
                                donThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                                    .Where(x => lstDonThuocId.Contains(x.DonThuocThanhToanId)
                                                && (string.IsNullOrEmpty(timKiemNangCaoObj.FromDate) || x.CreatedOn >= tuNgay)
                                                && (string.IsNullOrEmpty(timKiemNangCaoObj.ToDate) || x.CreatedOn <= denNgay))
                                    .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                                    {
                                        Nhom = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                                        Id = x.DonThuocThanhToanId,
                                        DichVuBenhVienId = x.DuocPhamId,
                                        NoiDung = x.Ten,
                                        DonViTinh = x.DonViTinh.Ten,
                                        SoLuong = x.SoLuong,
                                        DonGiaNhapKho = x.DonGiaNhap,
                                        //DonGiaBan = x.DonGiaBan,
                                        MienGiam = x.SoTienMienGiam,
                                        DuocHuongBHYT = x.DuocHuongBaoHiem,
                                        BaoHiemChiTra = x.BaoHiemChiTra,
                                        DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                        TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                        MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                                        // xử lý tự tính giá bán
                                        LaTuTinhGiaBan = true,
                                        XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTietId,
                                        VAT = x.VAT,
                                        TiLeTheoThapGia = x.TiLeTheoThapGia,
                                        PhuongPhapTinhGiaTriTonKhos = x.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                    })
                                    .ToList();
                                if (donThuocThanhToanChiTiets.Any())
                                {
                                    foreach (var chiTiet in donThuocThanhToanChiTiets)
                                    {
                                        var donThuocThanhToan = chiPhiDonThuocs.FirstOrDefault(x => x.Id == chiTiet.Id);
                                        if (donThuocThanhToan != null)
                                        {
                                            chiTiet.YeucauTiepNhanId = donThuocThanhToan.YeucauTiepNhanId;
                                            chiTiet.MaYeuCauTiepNhan = donThuocThanhToan.MaYeuCauTiepNhan;
                                            chiTiet.TrangThaiYeuCauTiepNhan = donThuocThanhToan.TrangThaiYeuCauTiepNhan;
                                            chiTiet.BenhNhanId = donThuocThanhToan.BenhNhanId;
                                            chiTiet.HoTen = donThuocThanhToan.HoTen;
                                            chiTiet.MaBN = donThuocThanhToan.MaBN;
                                            chiTiet.NoiGioiThieuId = donThuocThanhToan.NoiGioiThieuId;
                                            chiTiet.NoiGioiThieuDisplay = donThuocThanhToan.NoiGioiThieuDisplay;
                                            chiTiet.TenHinhThucDen = donThuocThanhToan.TenHinhThucDen;
                                            chiTiet.LaGioiThieu = donThuocThanhToan.LaGioiThieu;
                                            chiTiet.ThoiDiemTiepNhan = donThuocThanhToan.ThoiDiemTiepNhan;
                                        }
                                    }

                                    lstBangKeChiTiet.AddRange(donThuocThanhToanChiTiets);
                                }
                            }
                            #endregion

                            #endregion
                        }
                    }
                    #endregion

                    else // xử lý chi phí theo trường hợp chọn loại người bệnh nội trú
                    {
                        var lstMaTiepNhan = lstTiepNhanTheoNoiGioiThieu.Select(x => x.MaYeuCauTiepNhan).Distinct().ToList();

                        // tham khảo từ GetDataBaoCaoChiTietMienPhiTronVienForGridAsync
                        #region get chi phí theo dịch vụ

                        #region Chi phí khám
                        var chiPhiKhamBenhs = _yeuCauKhamBenhRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham
                                        && x.KhongTinhPhi != true)
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                                YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                HoTen = x.YeuCauTiepNhan.HoTen,
                                MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                DichVuBenhVienId = x.DichVuKhamBenhBenhVienId,
                                NoiDung = x.TenDichVu,
                                SoLuong = 1,
                                DonGiaBan = x.YeuCauGoiDichVuId != null ? x.DonGiaSauChietKhau : x.Gia,
                                MienGiam = x.SoTienMienGiam,
                                DuocHuongBHYT = x.DuocHuongBaoHiem,
                                BaoHiemChiTra = x.BaoHiemChiTra,
                                DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault()
                            })
                            .ToList();
                        #endregion

                        #region Chi phí kỹ thuật
                        var chiPhiKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                        && x.KhongTinhPhi != true)
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Nhom = Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                                TenNhom = x.LoaiDichVuKyThuat.GetDescription(),
                                YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                HoTen = x.YeuCauTiepNhan.HoTen,
                                MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                DichVuBenhVienId = x.DichVuKyThuatBenhVienId,
                                NoiDung = x.TenDichVu,
                                SoLuong = x.SoLan,
                                DonGiaBan = x.YeuCauGoiDichVuId != null ? x.DonGiaSauChietKhau : x.Gia,
                                MienGiam = x.SoTienMienGiam,
                                DuocHuongBHYT = x.DuocHuongBaoHiem,
                                BaoHiemChiTra = x.BaoHiemChiTra,
                                DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault()
                            })
                            .ToList();
                        #endregion

                        #region Chi phí giường
                        var chiPhiGiuongs = _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan))
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Nhom = Enums.EnumNhomGoiDichVu.DichVuGiuongBenh,
                                YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                HoTen = x.YeuCauTiepNhan.HoTen,
                                MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                DichVuBenhVienId = x.DichVuGiuongBenhVienId,
                                NoiDung = x.Ten,
                                SoLuong = x.SoLuong,
                                DonGiaBan = x.YeuCauGoiDichVuId != null ? x.DonGiaSauChietKhau : x.Gia,
                                MienGiam = x.SoTienMienGiam,
                                ChiPhiBHYTDichVuGiuongs = x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Select(a => new ThongTinChiPhiBHYTDichVuVo()
                                {
                                    DuocHuongBaoHiem = a.DuocHuongBaoHiem,
                                    BaoHiemChiTra = a.BaoHiemChiTra,
                                    DonGiaBaoHiem = a.DonGiaBaoHiem,
                                    MucHuongBaoHiem = a.MucHuongBaoHiem,
                                    TiLeBaoHiemThanhToan = a.TiLeBaoHiemThanhToan
                                }).ToList()
                            })
                            .ToList();

                        if (chiPhiGiuongs.Any())
                        {
                            foreach (var chiPhiGiuong in chiPhiGiuongs)
                            {
                                if (chiPhiGiuong.ChiPhiBHYTDichVuGiuongs.Any())
                                {
                                    var bhyt = chiPhiGiuong.ChiPhiBHYTDichVuGiuongs.First();
                                    chiPhiGiuong.DuocHuongBHYT = bhyt.DuocHuongBaoHiem;
                                    chiPhiGiuong.BaoHiemChiTra = bhyt.BaoHiemChiTra;
                                    chiPhiGiuong.DonGiaBHYT = bhyt.DonGiaBaoHiem.GetValueOrDefault();
                                    chiPhiGiuong.MucHuongBaoHiem = bhyt.MucHuongBaoHiem.GetValueOrDefault();
                                    chiPhiGiuong.TiLeBaoHiemThanhToan = bhyt.TiLeBaoHiemThanhToan.GetValueOrDefault();
                                }
                            }
                        }
                        #endregion

                        #region Chi phí truyền máu
                        var chiPhiTruyenMaus = _yeuCauTruyenMauRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                        && x.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy)
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Nhom = Enums.EnumNhomGoiDichVu.TruyenMau,
                                YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                HoTen = x.YeuCauTiepNhan.HoTen,
                                MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                DichVuBenhVienId = x.MauVaChePhamId,
                                NoiDung = x.TenDichVu,
                                SoLuong = 1,
                                DonGiaNhapKho = x.DonGiaNhap,
                                DonGiaBan = x.DonGiaBan,
                                MienGiam = x.SoTienMienGiam,
                                DuocHuongBHYT = x.DuocHuongBaoHiem,
                                BaoHiemChiTra = x.BaoHiemChiTra,
                                DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault()
                            })
                            .ToList();
                        #endregion

                        #region Chi phí dược phẩm
                        var chiPhiThuocs = _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                        && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                                        && x.KhongTinhPhi != true)
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Nhom = Enums.EnumNhomGoiDichVu.DuocPham,
                                YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                HoTen = x.YeuCauTiepNhan.HoTen,
                                MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                DichVuBenhVienId = x.DuocPhamBenhVienId,
                                NoiDung = x.Ten,
                                DonViTinh = x.DonViTinh.Ten,
                                SoLuong = x.SoLuong,
                                DonGiaNhapKho = x.DonGiaNhap,
                                //DonGiaBan = x.DonGiaBan,
                                MienGiam = x.SoTienMienGiam,
                                DuocHuongBHYT = x.DuocHuongBaoHiem,
                                BaoHiemChiTra = x.BaoHiemChiTra,
                                DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                                // xử lý tự tính giá bán
                                LaTuTinhGiaBan = true,
                                XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietId,
                                VAT = x.VAT,
                                TiLeTheoThapGia = x.TiLeTheoThapGia,
                                PhuongPhapTinhGiaTriTonKhos = x.XuatKhoDuocPhamChiTietId != null
                                    ? x.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                    : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                            })
                            .ToList();
                        #endregion

                        #region Chi phí vật tư
                        var chiPhiVatTus = _yeuCauVatTuBenhVienRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                        && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                                        && x.KhongTinhPhi != true)
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Nhom = Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                                YeucauTiepNhanId = x.YeuCauTiepNhanId,
                                MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                HoTen = x.YeuCauTiepNhan.HoTen,
                                MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan,

                                DichVuBenhVienId = x.VatTuBenhVienId,
                                NoiDung = x.Ten,
                                DonViTinh = x.DonViTinh,
                                SoLuong = x.SoLuong,
                                DonGiaNhapKho = x.DonGiaNhap,
                                //DonGiaBan = x.DonGiaBan,
                                MienGiam = x.SoTienMienGiam,
                                DuocHuongBHYT = x.DuocHuongBaoHiem,
                                BaoHiemChiTra = x.BaoHiemChiTra,
                                DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                                // xử lý tự tính giá bán
                                LaTuTinhGiaBan = true,
                                    XuatKhoChiTietId = x.XuatKhoVatTuChiTietId,
                                    VAT = x.VAT,
                                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKhos = x.XuatKhoVatTuChiTietId != null
                                        ? x.XuatKhoVatTuChiTiet.XuatKhoVatTuChiTietViTris.Select(a => a.NhapKhoVatTuChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                        : new List<Enums.PhuongPhapTinhGiaTriTonKho>()
                            })
                            .ToList();
                        #endregion

                        #region Chi phí đơn thuốc
                        var chiPhiDonThuocs = _donThuocThanhToanRepository.TableNoTracking
                            .Where(x => lstMaTiepNhan.Contains(x.YeuCauTiepNhan.MaYeuCauTiepNhan)
                                        && x.TrangThai != Enums.TrangThaiDonThuocThanhToan.DaHuy)
                            .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                            {
                                Id = x.Id,
                                YeucauTiepNhanId = x.YeuCauTiepNhanId.Value,
                                MaYeuCauTiepNhan = x.YeuCauTiepNhan.MaYeuCauTiepNhan,
                                TrangThaiYeuCauTiepNhan = x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan,
                                BenhNhanId = x.YeuCauTiepNhan.BenhNhanId.Value,
                                HoTen = x.YeuCauTiepNhan.HoTen,
                                MaBN = x.YeuCauTiepNhan.BenhNhan.MaBN,
                                NoiGioiThieuId = x.YeuCauTiepNhan.NoiGioiThieuId,
                                NoiGioiThieuDisplay = x.YeuCauTiepNhan.NoiGioiThieu.Ten + (!string.IsNullOrEmpty(x.YeuCauTiepNhan.NoiGioiThieu.DonVi) ? $" - {x.YeuCauTiepNhan.NoiGioiThieu.DonVi}" : ""),
                                TenHinhThucDen = x.YeuCauTiepNhan.HinhThucDen.Ten,
                                LaGioiThieu = x.YeuCauTiepNhan.NoiGioiThieuId != null,
                                ThoiDiemTiepNhan = x.YeuCauTiepNhan.ThoiDiemTiepNhan
                            })
                            .ToList();

                        var donThuocThanhToanChiTiets = new List<BaoCaoKeToanBangKeChiTietNguoiBenhGridVo>();
                        var lstDonThuocId = chiPhiDonThuocs.Select(x => x.Id).ToList();
                        if (lstDonThuocId.Any())
                        {
                            donThuocThanhToanChiTiets = _donThuocThanhToanChiTietRepository.TableNoTracking
                                .Where(x => lstDonThuocId.Contains(x.DonThuocThanhToanId))
                                .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                                {
                                    Nhom = Enums.EnumNhomGoiDichVu.DonThuocThanhToan,
                                    Id = x.DonThuocThanhToanId,
                                    DichVuBenhVienId = x.DuocPhamId,
                                    NoiDung = x.Ten,
                                    DonViTinh = x.DonViTinh.Ten,
                                    SoLuong = x.SoLuong,
                                    DonGiaNhapKho = x.DonGiaNhap,
                                    //DonGiaBan = x.DonGiaBan,
                                    MienGiam = x.SoTienMienGiam,
                                    DuocHuongBHYT = x.DuocHuongBaoHiem,
                                    BaoHiemChiTra = x.BaoHiemChiTra,
                                    DonGiaBHYT = x.DonGiaBaoHiem.GetValueOrDefault(),
                                    TiLeBaoHiemThanhToan = x.TiLeBaoHiemThanhToan.GetValueOrDefault(),
                                    MucHuongBaoHiem = x.MucHuongBaoHiem.GetValueOrDefault(),

                                    // xử lý tự tính giá bán
                                    LaTuTinhGiaBan = true,
                                    XuatKhoChiTietId = x.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTietId,
                                    VAT = x.VAT,
                                    TiLeTheoThapGia = x.TiLeTheoThapGia,
                                    PhuongPhapTinhGiaTriTonKhos = x.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(a => a.NhapKhoDuocPhamChiTiet.PhuongPhapTinhGiaTriTonKho).ToList()
                                })
                                .ToList();
                            if (donThuocThanhToanChiTiets.Any())
                            {
                                foreach (var chiTiet in donThuocThanhToanChiTiets)
                                {
                                    var donThuocThanhToan = chiPhiDonThuocs.FirstOrDefault(x => x.Id == chiTiet.Id);
                                    if (donThuocThanhToan != null)
                                    {
                                        chiTiet.YeucauTiepNhanId = donThuocThanhToan.YeucauTiepNhanId;
                                        chiTiet.MaYeuCauTiepNhan = donThuocThanhToan.MaYeuCauTiepNhan;
                                        chiTiet.TrangThaiYeuCauTiepNhan = donThuocThanhToan.TrangThaiYeuCauTiepNhan;
                                        chiTiet.BenhNhanId = donThuocThanhToan.BenhNhanId;
                                        chiTiet.HoTen = donThuocThanhToan.HoTen;
                                        chiTiet.MaBN = donThuocThanhToan.MaBN;
                                        chiTiet.NoiGioiThieuId = donThuocThanhToan.NoiGioiThieuId;
                                        chiTiet.NoiGioiThieuDisplay = donThuocThanhToan.NoiGioiThieuDisplay;
                                        chiTiet.TenHinhThucDen = donThuocThanhToan.TenHinhThucDen;
                                        chiTiet.LaGioiThieu = donThuocThanhToan.LaGioiThieu;
                                        chiTiet.ThoiDiemTiepNhan = donThuocThanhToan.ThoiDiemTiepNhan;
                                    }
                                }
                            }
                        }
                        #endregion

                        #endregion

                        lstBangKeChiTiet = lstBangKeChiTiet
                            .Concat(chiPhiKhamBenhs)
                            .Concat(chiPhiKyThuats)
                            .Concat(chiPhiGiuongs)
                            .Concat(chiPhiTruyenMaus)
                            .Concat(chiPhiThuocs)
                            .Concat(chiPhiVatTus)
                            .Concat(donThuocThanhToanChiTiets)
                            .ToList();
                    }
                }
            }

            if (lstBangKeChiTiet.Any())
            {
                lstBangKeChiTiet = lstBangKeChiTiet
                    .GroupBy(x => new { x.MaYeuCauTiepNhan, x.Nhom, x.DichVuBenhVienId, x.DonGiaNhapKho, x.DonGiaBanThucTe })
                    .Select(x => new BaoCaoKeToanBangKeChiTietNguoiBenhGridVo()
                    {
                        Nhom = x.Key.Nhom,
                        TenNhom = x.First().TenNhom,
                        DichVuBenhVienId = x.Key.DichVuBenhVienId,
                        YeucauTiepNhanId = x.First().YeucauTiepNhanId,
                        MaYeuCauTiepNhan = x.Key.MaYeuCauTiepNhan,
                        TrangThaiYeuCauTiepNhan = x.First().TrangThaiYeuCauTiepNhan,
                        BenhNhanId = x.First().BenhNhanId,
                        HoTen = x.First().HoTen,
                        MaBN = x.First().MaBN,
                        NoiGioiThieuId = x.First().NoiGioiThieuId,
                        NoiGioiThieuDisplay = x.First().NoiGioiThieuDisplay,
                        TenHinhThucDen = x.First().TenHinhThucDen,
                        LaGioiThieu = x.First().LaGioiThieu,
                        ThoiDiemTiepNhan = x.First().ThoiDiemTiepNhan,

                        NoiDung = x.First().NoiDung,
                        DonViTinh = x.First().DonViTinh,
                        SoLuong = x.Sum(a => a.SoLuong),
                        DonGiaNhapKho = x.First().DonGiaNhapKho,
                        DonGiaBan = x.First().DonGiaBanThucTe,
                        MienGiam = x.Sum(a => a.MienGiam)
                    })
                    .OrderByDescending(x => x.ThoiDiemTiepNhan)
                    .ThenBy(x => x.MaYeuCauTiepNhan)
                    .Skip(queryInfo.Skip).Take(queryInfo.Take)
                    .ToList();
            }

            if (isToTal != true)
            {
                foreach (var chiTiet in lstBangKeChiTiet)
                {
                    if (string.IsNullOrEmpty(chiTiet.DonViTinh))
                    {
                        if (chiTiet.Nhom == Enums.EnumNhomGoiDichVu.DichVuGiuongBenh)
                        {
                            chiTiet.DonViTinh = "Ngày";
                        }
                        else
                        {
                            chiTiet.DonViTinh = "Lần";
                        }
                    }
                }

                return new GridDataSource
                {
                    Data = lstBangKeChiTiet.ToArray(),
                    TotalRowCount = lstBangKeChiTiet.Count()
                };
            }
            else
            {
                return new GridDataSource
                {
                    TotalRowCount = lstBangKeChiTiet.Count()
                };
            }
        }

        public virtual byte[] ExportBaoCaoKeToanBangKeChiTietNguoiBenh(GridDataSource gridDataSource, QueryInfo query)
        {
            var timKiemNangCaoObj = new BaoCaoKeToanBangKeChiTietNguoiBenhTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(query.AdditionalSearchString) && query.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<BaoCaoKeToanBangKeChiTietNguoiBenhTimKiemNangCaoVo>(query.AdditionalSearchString);
                if (timKiemNangCaoObj.MaYeuCauTiepNhan == null
                    || timKiemNangCaoObj.MaYeuCauTiepNhan.Contains("undefined")
                    || timKiemNangCaoObj.MaYeuCauTiepNhan == "0")
                {
                    timKiemNangCaoObj.MaYeuCauTiepNhan = null;
                }
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.FromDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.FromDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.FromDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.ToDate != null && !string.IsNullOrEmpty(timKiemNangCaoObj.ToDate))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.ToDate, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }


            var tenHinhThucDen = _hinhThucDenRepository.TableNoTracking
                .Where(x => x.Id == timKiemNangCaoObj.HinhThucDenId)
                .Select(x => x.Ten)
                .FirstOrDefault();
            var tenNguoiBenh = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.MaYeuCauTiepNhan.Equals(timKiemNangCaoObj.MaYeuCauTiepNhan))
                .Select(x => x.HoTen)
                .FirstOrDefault();

            var datas = (ICollection<BaoCaoKeToanBangKeChiTietNguoiBenhGridVo>)gridDataSource.Data;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("BÁO CÁO BẢNG KÊ CHI TIẾT NGƯỜI BỆNH");

                    // set row
                    worksheet.DefaultRowHeight = 16;

                    // SET chiều rộng cho từng cột tương ứng
                    worksheet.Column(1).Width = 5;
                    worksheet.Column(2).Width = 15;
                    worksheet.Column(3).Width = 15;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 40;
                    worksheet.Column(6).Width = 20;
                    worksheet.Column(7).Width = 10;
                    worksheet.Column(8).Width = 15;
                    worksheet.Column(9).Width = 25;
                    worksheet.Column(10).Width = 20;
                    worksheet.Column(11).Width = 20;
                    worksheet.Column(12).Width = 20;
                    worksheet.DefaultColWidth = 20;

                    //SET title BV
                    using (var range = worksheet.Cells["A1:C1"])
                    {
                        range.Worksheet.Cells["A1:C1"].Merge = true;
                        range.Worksheet.Cells["A1:C1"].Value = "BỆNH VIỆN ĐKQT BẮC HÀ";
                        range.Worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        range.Worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A1:C1"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A1:C1"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                    }

                    // SET title head cho bảng excel
                    using (var range = worksheet.Cells["A2:L3"])
                    {
                        range.Worksheet.Cells["A2:L3"].Merge = true;
                        range.Worksheet.Cells["A2:L3"].Value = "BẢNG KÊ CHI TIẾT NB";
                        range.Worksheet.Cells["A2:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A2:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A2:L3"].Style.Font.SetFromFont(new Font("Times New Roman", 16));
                        range.Worksheet.Cells["A2:L3"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A2:L3"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A4:L4"])
                    {
                        range.Worksheet.Cells["A4:L4"].Merge = true;
                        range.Worksheet.Cells["A4:L4"].Value = "Hình thức đến: " + (tenHinhThucDen ?? "Tất cả");
                        range.Worksheet.Cells["A4:L4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A4:L4"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A4:L4"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A4:L4"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                    }
                    using (var range = worksheet.Cells["A5:L5"])
                    {
                        range.Worksheet.Cells["A5:L5"].Merge = true;
                        range.Worksheet.Cells["A5:L5"].Value = "Người bệnh: " + (string.IsNullOrEmpty(tenNguoiBenh) ? "Tất cả" : $"{timKiemNangCaoObj.MaYeuCauTiepNhan} - {tenNguoiBenh}");
                        range.Worksheet.Cells["A5:L5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A5:L5"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A5:L5"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A5:L5"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A5:L5"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A6:L6"])
                    {
                        range.Worksheet.Cells["A6:L6"].Merge = true;
                        range.Worksheet.Cells["A6:L6"].Value = "Từ ngày: " + tuNgay?.FormatNgayGioTimKiemTrenBaoCao()
                                                          + " - đến ngày: " + denNgay?.FormatNgayGioTimKiemTrenBaoCao();

                        range.Worksheet.Cells["A6:L6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A6:L6"].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        range.Worksheet.Cells["A6:L6"].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        range.Worksheet.Cells["A6:L6"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A6:L6"].Style.Font.Bold = true;
                    }

                    using (var range = worksheet.Cells["A8:L8"])
                    {
                        range.Worksheet.Cells["A8:L8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Worksheet.Cells["A8:L8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        range.Worksheet.Cells["A8:L8"].Style.Font.SetFromFont(new Font("Times New Roman", 11));
                        range.Worksheet.Cells["A8:L8"].Style.Font.Color.SetColor(Color.Black);
                        range.Worksheet.Cells["A8:L8"].Style.Font.Bold = true;
                        range.Worksheet.Cells["A8:L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                        range.Worksheet.Cells["A8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["A8"].Value = "STT";

                        range.Worksheet.Cells["B8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["B8"].Value = "MÃ NB";

                        range.Worksheet.Cells["C8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["C8"].Value = "MÃ TN";

                        range.Worksheet.Cells["D8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["D8"].Value = "HỌ VÀ TÊN";

                        range.Worksheet.Cells["E8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["E8"].Value = "NỘI DUNG";

                        range.Worksheet.Cells["F8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["F8"].Value = "NHÓM DỊCH VỤ";

                        range.Worksheet.Cells["G8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["G8"].Value = "ĐVT";

                        range.Worksheet.Cells["H8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["H8"].Value = "SỐ LƯỢNG";

                        range.Worksheet.Cells["I8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["I8"].Value = "ĐƠN GIÁ NHẬP KHO";

                        range.Worksheet.Cells["J8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["J8"].Value = "ĐƠN GIÁ BÁN";

                        range.Worksheet.Cells["K8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["K8"].Value = "MIỄN GIẢM";

                        range.Worksheet.Cells["L8"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        range.Worksheet.Cells["L8"].Value = "THÀNH TIỀN BÁN";
                    }

                    //write data from line 8
                    int index = 9;
                    int stt = 1;
                    var formatCurrency = "#,##0.00";
                    if (datas.Any())
                    {
                        foreach (var item in datas)
                        {
                            // format border, font chữ,....
                            worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(index).Height = 20.5;

                            worksheet.Cells["A" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["A" + index].Value = stt;

                            worksheet.Cells["B" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["B" + index].Value = item.MaBN;

                            worksheet.Cells["C" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["C" + index].Value = item.MaYeuCauTiepNhan;

                            worksheet.Cells["D" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["D" + index].Value = item.HoTen;

                            worksheet.Cells["E" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["E" + index].Value = item.NoiDung;

                            worksheet.Cells["F" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["F" + index].Value = item.NhomDichVu;

                            worksheet.Cells["G" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["G" + index].Value = item.DonViTinh;

                            worksheet.Cells["H" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["H" + index].Value = item.SoLuong;

                            worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["I" + index].Value = item.DonGiaNhapKho;
                            worksheet.Cells["I" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["J" + index].Value = item.DonGiaBan;
                            worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["K" + index].Value = item.MienGiam;
                            worksheet.Cells["K" + index].Style.Numberformat.Format = formatCurrency;

                            worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells["L" + index].Value = item.ThanhTienBan;
                            worksheet.Cells["L" + index].Style.Numberformat.Format = formatCurrency;

                            stt++;
                            index++;
                        }

                        //total
                        worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":L" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                        worksheet.Cells["A" + index + ":L" + index].Style.Font.Color.SetColor(Color.Black);
                        worksheet.Cells["A" + index + ":L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["A" + index + ":L" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                        worksheet.Cells["A" + index + ":L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(index).Height = 20.5;

                        using (var range = worksheet.Cells["A" + index + ":H" + index])
                        {
                            range.Worksheet.Cells["A" + index + ":H" + index].Merge = true;
                            range.Worksheet.Cells["A" + index + ":H" + index].Value = "Tổng cộng";
                            range.Worksheet.Cells["A" + index + ":H" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            range.Worksheet.Cells["A" + index + ":H" + index].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                            range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.SetFromFont(new Font("Times New Roman", 10));
                            range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Color.SetColor(Color.Black);
                            range.Worksheet.Cells["A" + index + ":H" + index].Style.Font.Bold = true;
                        }



                        worksheet.Cells["I" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["I" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["I" + index].Style.Font.Bold = true;
                        worksheet.Cells["I" + index].Value = datas.Sum(x => x.DonGiaNhapKho ?? 0);
                        worksheet.Cells["I" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["J" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["J" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["J" + index].Style.Font.Bold = true;
                        worksheet.Cells["J" + index].Value = datas.Sum(x => x.DonGiaBanThucTe ?? 0);
                        worksheet.Cells["J" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["K" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["K" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["K" + index].Style.Font.Bold = true;
                        worksheet.Cells["K" + index].Value = datas.Sum(x => x.MienGiam ?? 0);
                        worksheet.Cells["K" + index].Style.Numberformat.Format = formatCurrency;

                        worksheet.Cells["L" + index].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells["L" + index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        worksheet.Cells["L" + index].Style.Font.Bold = true;
                        worksheet.Cells["L" + index].Value = datas.Sum(x => x.ThanhTienBan ?? 0);
                        worksheet.Cells["L" + index].Style.Numberformat.Format = formatCurrency;
                    }
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}
