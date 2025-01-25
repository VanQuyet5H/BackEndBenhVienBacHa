using System;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Newtonsoft.Json;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.TiemChung
{
    public partial class TiemChungService
    {
        #region Get grid
        public async Task<List<KhamBenhGoiDichVuGridVo>> GetGridDichVuKyThuatTiemChung(GridChiDinhVuKyThuatTiemChungQueryInfoVo queryInfo)
        {
            var yeuCauTiepNhan = BaseRepository.TableNoTracking
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuXetNghiem)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NoiThucHien)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhomDichVuBenhVien)?.ThenInclude(p => p.NhomDichVuBenhVienCha)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.TaiKhoanBenhNhanChis)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                     
                     //BVHD-3825
                     .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.MienGiamChiPhis).ThenInclude(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)

                     .Where(p => p.Id == queryInfo.YeuCauTiepNhanId).FirstOrDefault();

            // setup data chp grip
            if (yeuCauTiepNhan != null)
            {
                #region Kiểm tra gói dịch vụ
                var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                    .Where(x => ((x.BenhNhanId == yeuCauTiepNhan.BenhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == yeuCauTiepNhan.BenhNhanId && x.GoiSoSinh == true))
                                && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                                && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                    .ToList();
                #endregion

                var dichVus = SetDataGripViewYeuCauChiDinhKhac(yeuCauTiepNhan, queryInfo.YeuCauKhamSangLocId, queryInfo.NhomDichVuId, goiDichVus);
                return dichVus;
            }

            return null;
        }
        private List<KhamBenhGoiDichVuGridVo> SetDataGripViewYeuCauChiDinhKhac(YeuCauTiepNhan yeuCauTiepNhan, long? yeuCauKhamSangLocId, int? nhomId = null, List<YeuCauGoiDichVu> goiDichVus = null)
        {
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            // danh sách dịch vụ
            var lstDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats
                  .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) //BVHD-3284: || !string.IsNullOrEmpty(x.LyDoHuyDichVu)
                  .ToList();

            // get list nhóm dịch vụ trong gói
            var lstNhomDichVu = EnumHelper.GetListEnum<EnumNhomGoiDichVu>()
                .Select(item => new LookupItemVo()
                {
                    DisplayName = item.GetDescription(),
                    KeyId = Convert.ToInt32(item)
                }).OrderByDescending(x => nhomId == null || x.KeyId == nhomId).ThenBy(x => x.DisplayName).ToList();

            if (goiDichVus == null)
            {
                goiDichVus = new List<YeuCauGoiDichVu>();
            }

            var goiDichVuKhamBenh = new List<KhamBenhGoiDichVuGridVo>();
            var stt = 1;

            foreach (var item in lstNhomDichVu)
            {
                switch (item.KeyId)
                {
                    case (int)EnumNhomGoiDichVu.DichVuKyThuat:
                        //var sttKT = 1;
                        var lstSortNhomDichVuKyThuat = lstDichVuKyThuat.OrderBy(x => x.CreatedOn)
                            .Select(x => x.NhomDichVuBenhVienId).Distinct().ToList();
                        goiDichVuKhamBenh.AddRange(
                            lstDichVuKyThuat

                            // cập nhật 18/05/2021: sắp xếp lại các dịch vụ xét nghiệm theo số thứ tự
                            .OrderBy(x => lstSortNhomDichVuKyThuat.IndexOf(x.NhomDichVuBenhVienId))
                            .ThenBy(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem ? (x.DichVuKyThuatBenhVien.DichVuXetNghiem?.SoThuTu ?? (x.DichVuKyThuatBenhVien.DichVuXetNghiemId ?? 0)) : 0)
                            .ThenBy(x => x.CreatedOn)
                            .Select(p => new KhamBenhGoiDichVuGridVo
                            {
                                STT = stt++,
                                Id = p.Id,
                                Nhom = (string.IsNullOrEmpty(p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten) ? "" : p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten + " - ") + p.NhomDichVuBenhVien.Ten, //EnumNhomGoiDichVu.DichVuKyThuat.GetDescription(),
                                NhomId = (int)EnumNhomGoiDichVu.DichVuKyThuat,
                                LoaiYeuCauDichVuId = p.DichVuKyThuatBenhVien?.Id,
                                NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKyThuatBenhVien?.Id ?? 0,
                                Ma = p.DichVuKyThuatBenhVien?.Ma,
                                MaGiaDichVu = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.MaGia,
                                MaTT37 = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.Ma4350,
                                TenDichVu = p.DichVuKyThuatBenhVien.Ten,
                                TenTT43 = p.TenGiaDichVu,
                                TenLoaiGia = p.NhomGiaDichVuKyThuatBenhVien?.Ten,
                                LoaiGia = p.NhomGiaDichVuKyThuatBenhVienId,
                                DonGia = p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia,
                                //GiaBaoHiemThanhToan = p.GiaBaoHiemThanhToan ?? 0,
                                //ThanhTien = 0,
                                BHYTThanhToan = 0,
                                //BNThanhToan = 0,
                                NoiThucHien = p.NoiThucHien?.Ten,//String.Format("{0} - {1}", p.NoiThucHien?.Ma, p.NoiThucHien?.Ten),
                                NoiThucHienId = p.NoiThucHienId ?? 0,
                                TenNguoiThucHien = p.NhanVienThucHien?.User.HoTen,
                                NguoiThucHienId = p.NhanVienThucHienId,
                                SoLuong = Convert.ToDouble(p.SoLan),
                                TrangThaiDichVu = p.TrangThai.GetDescription(),
                                TrangThaiDichVuId = (int)p.TrangThai,
                                NhomChiPhiDichVuKyThuatId = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.NhomChiPhi,
                                KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                                isCheckRowItem = false,
                                DaThanhToan = p.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan,
                                DonGiaBaoHiem = p.DonGiaBaoHiem,
                                DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                                KhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT != null && p.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true,
                                LyDoKhongThucHien = p.YeuCauDichVuKyThuatTuongTrinhPTTT?.LyDoKhongThucHien,
                                YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                                TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                    "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                                    : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                        p.MienGiamChiPhis
                                            .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                            .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                                            .First() : null),
                                IsDichVuXetNghiem = p.NhomDichVuBenhVien.Ma == "XN" || p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ma == "XN",
                                BenhPhamXetNghiem = p.BenhPhamXetNghiem,
                                ThoiGianChiDinh = p.ThoiDiemChiDinh,
                                KhongTinhPhi = p.KhongTinhPhi,
                                ThanhTien = p.KhongTinhPhi == true ? 0 : ((p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia) * (decimal)p.SoLan),
                                IsDichVuHuyThanhToan = p.TrangThaiThanhToan != TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                                LyDoHuyDichVu = p.LyDoHuyDichVu,
                                NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

                                // gói marketing
                                CoDichVuNayTrongGoi = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                CoDichVuNayTrongGoiKhuyenMai = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                                // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
                                LoaiDichVuKyThuat = p.LoaiDichVuKyThuat,
                                LyDoHuyTrangThaiDaThucHien = p.LyDoHuyTrangThaiDaThucHien,
                                ThoiDiemThucHien = p.ThoiDiemThucHien,

                                LaDichVuKhamSangLoc = p.Id == yeuCauKhamSangLocId,
                                LaDichVuVacxin = nhomTiemChungId != null && p.NhomDichVuBenhVienId == nhomTiemChungId && p.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                            }));
                        break;
                }
            }
            return goiDichVuKhamBenh;
        }

        public async Task<GridDataSource> GetDataForGridHoanThanhTiemChungAsync(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var timKiemNangCaoObj = new HoanThanhKhamTiemChungTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<HoanThanhKhamTiemChungTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NoiTheoDoiSauTiem)
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN, 
                    x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.DiaChiDayDu, x => x.NhanVienKetLuan.User.HoTen)
                .Where(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
                //.OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemHoanThanh).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            // là hàng đợi khám sàng lọc
            if (timKiemNangCaoObj.LoaiHangDoi == LoaiHangDoiTiemVacxin.KhamSangLoc)
            {
                query = query.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                         && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null 
                                         && x.NoiThucHienId == phongHienTaiId);

                if (tuNgay != null || denNgay != null)
                {
                    query = query.Where(x => (tuNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date >= tuNgay.Value.Date))
                                             && (denNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date <= denNgay.Value.Date)));
                }
            }
            // là hàng đợi thực hiện tiêm
            else
            {
                query = query.Where(x => x.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && a.NoiThucHienId == phongHienTaiId)
                                         && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && a.NoiThucHienId == phongHienTaiId)
                                             .All(a => a.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                         && x.ThoiDiemHoanThanh != null);

                if (tuNgay != null || denNgay != null)
                {
                    query = query.Where(x => (tuNgay == null || x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                  .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                                              && a.NoiThucHienId == phongHienTaiId)
                                                  .Any(a => a.ThoiDiemHoanThanh != null && a.ThoiDiemHoanThanh.Value.Date >= tuNgay.Value.Date))
                                             && (denNgay == null || x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                     .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                                                 && a.NoiThucHienId == phongHienTaiId)
                                                     .Any(a => a.ThoiDiemHoanThanh != null && a.ThoiDiemHoanThanh.Value.Date <= denNgay.Value.Date)));
                }

                if (timKiemNangCaoObj.VacxinId != null)
                {
                    query = query.Where(x => x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                                        && a.NoiThucHienId == phongHienTaiId)
                                            .Any(a => a.DichVuKyThuatBenhVienId == timKiemNangCaoObj.VacxinId));
                }
            }

            var data = string.IsNullOrEmpty(queryInfo.SortString) 
                ? (timKiemNangCaoObj.LoaiHangDoi == LoaiHangDoiTiemVacxin.KhamSangLoc 
                    ? query.OrderByDescending(x => x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList() 
                    : query.OrderByDescending(x => x.ThoiDiemHoanThanh).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList()) 
                : query.OrderBy(queryInfo.SortString).ThenByDescending(x => x.ThoiDiemHoanThanh).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            var result = data
                .Select(item => new HoanThanhKhamTiemChungGridVo()
                {
                    Id = item.Id,
                    MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = item.YeuCauTiepNhan.HoTen,
                    NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                    ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                    NamSinh = item.YeuCauTiepNhan.NamSinh,
                    DiaChiDayDu = item.YeuCauTiepNhan.DiaChiDayDu,
                    ThoiDiemThucHien = timKiemNangCaoObj.LoaiHangDoi == LoaiHangDoiTiemVacxin.KhamSangLoc ? item.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc : item.ThoiDiemHoanThanh,
                    BacSiKhamDisplay = item.NhanVienKetLuan?.User.HoTen,
                    MuiTiem = string.Join("<br>",
                        item.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                    .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                && a.NoiThucHienId == phongHienTaiId)
                                    .Select(a => a.TenDichVu + (a.ThoiDiemHoanThanh == null ? "" : " (" + a.ThoiDiemHoanThanh?.ApplyFormatDateTimeSACH() + ")"))),
                    NoiTheoDoiSauTiem = item.KhamSangLocTiemChung.NoiTheoDoiSauTiem?.Ten,
                    PhongBenhVienId = phongHienTaiId
                }).ToArray();
            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetDataForGridHoanThanhTiemChungAsyncVer2(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            var result = new List<HoanThanhKhamTiemChungGridVo>();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var timKiemNangCaoObj = new HoanThanhKhamTiemChungTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<HoanThanhKhamTiemChungTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            // là hàng đợi khám sàng lọc
            if (timKiemNangCaoObj.LoaiHangDoi == LoaiHangDoiTiemVacxin.KhamSangLoc)
            {
                var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN,
                        x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.DiaChiDayDu, x => x.NhanVienKetLuan.User.HoTen)
                    .Where(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null
                                && x.NoiThucHienId == phongHienTaiId);

                if (tuNgay != null || denNgay != null)
                {
                    query = query.Where(x => (tuNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date >= tuNgay.Value.Date))
                                             && (denNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date <= denNgay.Value.Date)));
                }

                query = string.IsNullOrEmpty(queryInfo.SortString)
                    ? query.OrderByDescending(x => x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc).Skip(queryInfo.Skip).Take(queryInfo.Take)
                    : query.OrderBy(queryInfo.SortString).ThenByDescending(x => x.ThoiDiemHoanThanh).Skip(queryInfo.Skip).Take(queryInfo.Take);

                result = query
                    .Select(item => new HoanThanhKhamTiemChungGridVo()
                    {
                        Id = item.Id,
                        MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        DiaChiDayDu = item.YeuCauTiepNhan.DiaChiDayDu,
                        ThoiDiemThucHien = item.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc,
                        BacSiKhamDisplay = item.NhanVienKetLuan.User.HoTen,
                        //MuiTiem = string.Join("<br>",
                        //    item.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                        //        .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                        //                    && a.NoiThucHienId == phongHienTaiId)
                        //        .Select(a => a.TenDichVu + (a.ThoiDiemHoanThanh == null ? "" : " (" + a.ThoiDiemHoanThanh.Value.ApplyFormatDateTimeSACH() + ")"))),
                        NoiTheoDoiSauTiem = item.KhamSangLocTiemChung.NoiTheoDoiSauTiem.Ten,
                        PhongBenhVienId = phongHienTaiId,

                        //cập nhật 03/03/2022
                        KhamSangLocTiemChungId = item.KhamSangLocTiemChung.Id
                    }).ToList();
            }
            // là hàng đợi thực hiện tiêm
            else
            {
                var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN,
                        x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.DiaChiDayDu, x => x.NhanVienKetLuan.User.HoTen)
                    .Where(x => x.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SangLocTiemChung 
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy 
                                && x.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null
                                && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh != null
                                && (tuNgay == null || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh.Value.Date >= tuNgay.Value.Date)
                                && (denNgay == null || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh.Value.Date <= denNgay.Value.Date)
                                && (timKiemNangCaoObj.VacxinId == null || x.DichVuKyThuatBenhVienId == timKiemNangCaoObj.VacxinId)
                                && x.NoiThucHienId == phongHienTaiId
                                && (x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || x.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan || x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan))
                    .Select(item => new HoanThanhKhamTiemChungGridVo()
                    {
                        Id = item.YeuCauDichVuKyThuatKhamSangLocTiemChungId.Value,
                        MaYeuCauTiepNhan = item.YeuCauTiepNhan.MaYeuCauTiepNhan,
                        MaBN = item.YeuCauTiepNhan.BenhNhan.MaBN,
                        HoTen = item.YeuCauTiepNhan.HoTen,
                        NgaySinh = item.YeuCauTiepNhan.NgaySinh,
                        ThangSinh = item.YeuCauTiepNhan.ThangSinh,
                        NamSinh = item.YeuCauTiepNhan.NamSinh,
                        DiaChiDayDu = item.YeuCauTiepNhan.DiaChiDayDu,
                        ThoiDiemThucHien = item.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh,
                        BacSiKhamDisplay = item.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.NhanVienKetLuan.User.HoTen,
                        MuiTiem = item.TenDichVu + (item.ThoiDiemHoanThanh == null ? "" : " (" + item.ThoiDiemHoanThanh.Value.ApplyFormatDateTimeSACH() + ")"),
                        NoiTheoDoiSauTiem = item.YeuCauDichVuKyThuatKhamSangLocTiemChung.NoiTheoDoiSauTiem.Ten,
                        PhongBenhVienId = phongHienTaiId,
                        TrangThaiMuiTiemVacxin = item.TrangThai
                    })
                    .GroupBy(x => new {x.Id})
                    .Select(item => new HoanThanhKhamTiemChungGridVo()
                    {
                        Id = item.Key.Id,
                        MaYeuCauTiepNhan = item.First().MaYeuCauTiepNhan,
                        MaBN = item.First().MaBN,
                        HoTen = item.First().HoTen,
                        NgaySinh = item.First().NgaySinh,
                        ThangSinh = item.First().ThangSinh,
                        NamSinh = item.First().NamSinh,
                        DiaChiDayDu = item.First().DiaChiDayDu,
                        ThoiDiemThucHien = item.First().ThoiDiemThucHien,
                        BacSiKhamDisplay = item.First().BacSiKhamDisplay,
                        MuiTiem = string.Join("<br>",
                            item.Where(a => a.TrangThaiMuiTiemVacxin == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                .Select(a => a.MuiTiem)),
                        NoiTheoDoiSauTiem = item.First().NoiTheoDoiSauTiem,
                        PhongBenhVienId = phongHienTaiId,
                        TrangThaiMuiTiemVacxin = item.Any(a => a.TrangThaiMuiTiemVacxin != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) ? EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien,

                        //cập nhật 03/03/2022
                        KhamSangLocTiemChungId = item.Key.Id,
                    })
                    .Where(x => x.TrangThaiMuiTiemVacxin == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien);

                query = string.IsNullOrEmpty(queryInfo.SortString)
                    ? query.OrderByDescending(x => x.ThoiDiemThucHien).Skip(queryInfo.Skip).Take(queryInfo.Take)
                    : query.OrderBy(queryInfo.SortString).ThenByDescending(x => x.ThoiDiemThucHien).Skip(queryInfo.Skip).Take(queryInfo.Take);

                result = query.ToList();
            }

            return new GridDataSource
            {
                Data = result.ToArray(),
                TotalRowCount = result.Count
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridHoanThanhTiemChungAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var timKiemNangCaoObj = new HoanThanhKhamTiemChungTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<HoanThanhKhamTiemChungTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User)
                .Include(x => x.YeuCauTiepNhan).ThenInclude(x => x.BenhNhan)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.NoiTheoDoiSauTiem)
                .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN,
                    x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.DiaChiDayDu, x => x.NhanVienKetLuan.User.HoTen)
                .Where(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy);
            //.OrderBy(queryInfo.SortString).ThenBy(x => x.ThoiDiemHoanThanh).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();

            // là hàng đợi khám sàng lọc
            if (timKiemNangCaoObj.LoaiHangDoi == LoaiHangDoiTiemVacxin.KhamSangLoc)
            {
                query = query.Where(x => (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                         && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null
                                         && x.NoiThucHienId == phongHienTaiId);

                if (tuNgay != null || denNgay != null)
                {
                    query = query.Where(x => (tuNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date >= tuNgay.Value.Date))
                                             && (denNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date <= denNgay.Value.Date)));
                }
            }
            // là hàng đợi thực hiện tiêm
            else
            {
                query = query.Where(x => x.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && a.NoiThucHienId == phongHienTaiId)
                                         && x.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && a.NoiThucHienId == phongHienTaiId)
                                             .All(a => a.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                         && x.ThoiDiemHoanThanh != null);

                if (tuNgay != null || denNgay != null)
                {
                    query = query.Where(x => (tuNgay == null || x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                  .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                              && a.NoiThucHienId == phongHienTaiId)
                                                  .Any(a => a.ThoiDiemHoanThanh != null && a.ThoiDiemHoanThanh.Value.Date >= tuNgay.Value.Date))
                                             && (denNgay == null || x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                                     .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                 && a.NoiThucHienId == phongHienTaiId)
                                                     .Any(a => a.ThoiDiemHoanThanh != null && a.ThoiDiemHoanThanh.Value.Date <= denNgay.Value.Date)));
                }

                if (timKiemNangCaoObj.VacxinId != null)
                {
                    query = query.Where(x => x.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                            .Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                        && a.NoiThucHienId == phongHienTaiId)
                                            .Any(a => a.DichVuKyThuatBenhVienId == timKiemNangCaoObj.VacxinId));
                }
            }

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        public async Task<GridDataSource> GetTotalPageForGridHoanThanhTiemChungAsyncVer2(QueryInfo queryInfo)
        {
            //BuildDefaultSortExpression(queryInfo);
            IQueryable<HoanThanhKhamTiemChungGridVo> result = null;
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var timKiemNangCaoObj = new HoanThanhKhamTiemChungTimKiemNangCaoVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<HoanThanhKhamTiemChungTimKiemNangCaoVo>(queryInfo.AdditionalSearchString);
            }

            DateTime? tuNgay = null;
            DateTime? denNgay = null;
            //kiểm tra tìm kiếm nâng cao
            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.TuNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.TuNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var tuNgayTemp);
                tuNgay = tuNgayTemp;
            }

            if (timKiemNangCaoObj.TuNgayDenNgay != null && !string.IsNullOrEmpty(timKiemNangCaoObj.TuNgayDenNgay.DenNgay))
            {
                DateTime.TryParseExact(timKiemNangCaoObj.TuNgayDenNgay.DenNgay, "MM/dd/yyyy hh:mm tt", null, DateTimeStyles.None, out var denNgayTemp);
                denNgay = denNgayTemp;
            }

            // là hàng đợi khám sàng lọc
            if (timKiemNangCaoObj.LoaiHangDoi == LoaiHangDoiTiemVacxin.KhamSangLoc)
            {
                var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN,
                        x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.DiaChiDayDu, x => x.NhanVienKetLuan.User.HoTen)
                    .Where(x => x.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && (x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || x.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                                && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null
                                && x.NoiThucHienId == phongHienTaiId);

                if (tuNgay != null || denNgay != null)
                {
                    query = query.Where(x => (tuNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date >= tuNgay.Value.Date))
                                             && (denNgay == null || (x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc != null && x.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc.Value.Date <= denNgay.Value.Date)));
                }

                result = query
                    .Select(item => new HoanThanhKhamTiemChungGridVo()
                    {
                        Id = item.Id
                    });
            }
            // là hàng đợi thực hiện tiêm
            else
            {
                result = _yeuCauDichVuKyThuatRepository.TableNoTracking
                    .ApplyLike(timKiemNangCaoObj.SearchString?.Trim(), x => x.YeuCauTiepNhan.MaYeuCauTiepNhan, x => x.YeuCauTiepNhan.BenhNhan.MaBN,
                        x => x.YeuCauTiepNhan.HoTen, x => x.YeuCauTiepNhan.DiaChiDayDu, x => x.NhanVienKetLuan.User.HoTen)
                    .Where(x => x.LoaiDichVuKyThuat != LoaiDichVuKyThuat.SangLocTiemChung
                                && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                && x.YeuCauDichVuKyThuatKhamSangLocTiemChungId != null
                                && x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh != null
                                && (tuNgay == null || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh.Value.Date >= tuNgay.Value.Date)
                                && (denNgay == null || x.YeuCauDichVuKyThuatKhamSangLocTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh.Value.Date <= denNgay.Value.Date)
                                && (timKiemNangCaoObj.VacxinId == null || x.DichVuKyThuatBenhVienId == timKiemNangCaoObj.VacxinId)
                                && x.NoiThucHienId == phongHienTaiId
                                && (x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || x.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan || x.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan))
                    .Select(item => new HoanThanhKhamTiemChungGridVo()
                    {
                        Id = item.YeuCauDichVuKyThuatKhamSangLocTiemChungId.Value,
                        TrangThaiMuiTiemVacxin = item.TrangThai
                    })
                    .GroupBy(x => new { x.Id })
                    .Select(item => new HoanThanhKhamTiemChungGridVo()
                    {
                        Id = item.Key.Id,
                        TrangThaiMuiTiemVacxin = item.Any(a => a.TrangThaiMuiTiemVacxin != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien) ? EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien : EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                    })
                    .Where(x => x.TrangThaiMuiTiemVacxin == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien);
            }

            var countTask = result.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region Get data
        public List<LookupItemVo> GetKetLuans(DropDownListRequestModel queryInfo)
        {
            var ketLuans = EnumHelper.GetListEnum<LoaiKetLuanKhamSangLocTiemChung>();

            var lstKetLuan = ketLuans.Select(item => new LookupItemVo
            {
                KeyId = (int)item,
                DisplayName = item.GetDescription()
            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstKetLuan = lstKetLuan.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                       .ToList();
            }

            return lstKetLuan;
        }

        public List<LookupItemVo> GetViTriTiems(DropDownListRequestModel queryInfo)
        {
            var viTriTiems = EnumHelper.GetListEnum<ViTriTiem>();

            var lstViTriTiem = viTriTiems.Select(item => new LookupItemVo
            {
                KeyId = (int)item,
                DisplayName = item.GetDescription()
            }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                lstViTriTiem = lstViTriTiem.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().Trim()))
                                           .ToList();
            }

            return lstViTriTiem;
        }

        public async Task<string> GetTemplateKhamSangLocTiemChungAsync(string ten)
        {
            return await _templateKhamSangLocTiemChungRepository.TableNoTracking.Where(p => string.IsNullOrEmpty(ten) || p.Ten.Equals(ten))
                                                                                .Select(p => p.ComponentDynamics)
                                                                                .FirstOrDefaultAsync();
        }

        public async Task<List<VacxinTiemChungVo>> GetVaccinesAsync(DropDownListRequestModel queryInfo)
        {
            var khoVacxin = await _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin).FirstOrDefaultAsync();

            if(khoVacxin == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var duocPhamVacxins = await _duocPhamVaVatTuBenhVienService.GetDuocPhamVaVatTuTrongKho(false, queryInfo.Query, khoVacxin.Id, queryInfo.Take);

            var duocPhamVacxinTheoDichVuKyThuat = await _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.DichVuKyThuatVuBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date)))
                                                                                                        .SelectMany(p => p.DichVuKyThuatBenhVienTiemChungs)
                                                                                                        .Where(p => duocPhamVacxins.Any(o => o.Id == p.DuocPhamBenhVienId))
                                                                                                        .Select(p => new VacxinTiemChungVo
                                                                                                        {
                                                                                                            KeyId = p.DichVuKyThuatBenhVienId,
                                                                                                            DisplayName = p.DichVuKyThuatBenhVien.Ten,
                                                                                                            DuocPhamBenhVienId = p.DuocPhamBenhVienId,
                                                                                                            NhomDichVuBenhVienId = p.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                                                                                                            DonViTinh = p.DuocPhamBenhVien.DuocPham.DonViTinh.Ten,
                                                                                                            NuocSanXuat = p.DuocPhamBenhVien.DuocPham.NuocSanXuat,
                                                                                                            SoLuongTon = duocPhamVacxins.Where(o => o.Id == p.DuocPhamBenhVienId).First().SoLuongTon,
                                                                                                            KhoId = khoVacxin.Id
                                                                                                        }).ToListAsync();

            return duocPhamVacxinTheoDichVuKyThuat;
        }

        public async Task<int> GetSoThuTuTiepTheoTrongHangDoiTiemChungAsync(long phongBenhVienId)
        {
            var sttMax = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(x => x.PhongBenhVienId == phongBenhVienId)
                .OrderByDescending(x => x.SoThuTu)
                .Select(x => x.SoThuTu)
                .FirstOrDefaultAsync();
            return sttMax + 1;
        }

        public async Task<LookupItemVo> GetBacSiThucHienMacDinhAsync(long noiThucHienId)
        {
            var nhanVien = await _hoatDongNhanVienRepository.TableNoTracking.Include(hd => hd.NhanVien).ThenInclude(nv => nv.User)
                                                                            .Include(hd => hd.NhanVien).ThenInclude(nv => nv.ChucDanh)
                                                                            .Include(hd => hd.PhongBenhVien)
                                                                            .Where(hd => hd.PhongBenhVienId == noiThucHienId && hd.NhanVien.ChucDanh.NhomChucDanhId == (long)EnumNhomChucDanh.BacSi)
                                                                            .Select(hd => hd.NhanVien)
                                                                            .Select(s => new LookupItemVo
                                                                            {
                                                                                DisplayName = s.User.HoTen,
                                                                                KeyId = s.Id
                                                                            })
                                                                            .FirstOrDefaultAsync();

            return nhanVien;
        }
        #endregion

        #region Kiểm tra data
        public void KiemTraKetLuanPhuHopVoiHuongDan(YeuCauDichVuKyThuat yeuCauDichVuKyThuat)
        {
            var thongTin = JsonConvert.DeserializeObject<BanKiemTruocTiemChungDataVo>(yeuCauDichVuKyThuat.KhamSangLocTiemChung.ThongTinKhamSangLocTiemChungData);
            var phanLoai = JsonConvert.DeserializeObject<PhanLoaiBanKiemTruocTiemChungDataVo>(yeuCauDichVuKyThuat.KhamSangLocTiemChung.ThongTinKhamSangLocTiemChungTemplate);

            var soThangTuoi = CalculateHelper.TinhTongSoThangCuaTuoi(yeuCauDichVuKyThuat.YeuCauTiepNhan.NgaySinh, yeuCauDichVuKyThuat.YeuCauTiepNhan.ThangSinh, yeuCauDichVuKyThuat.YeuCauTiepNhan.NamSinh);
            var isHopLe = true;

            if (soThangTuoi >= 1)
            {
                if (phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.TrongBenhVien)
                {
                    switch (yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan.Value)
                    {
                        case LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Co" || p.Id == "Group8Co") &&
                                                                               p.Value == "true")
                                                                   .Count() == 2;

                            if(!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.TrenMotThangTuoi.TrongBenhVien.ChongChiDinh"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Khong" || p.Id == "Group2Khong" || p.Id == "Group3Khong" || p.Id == "Group4Khong" || p.Id == "Group5Khong" || p.Id == "Group6Khong" || p.Id == "Group7Khong" || p.Id == "Group8Khong") &&
                                                                               p.Value == "true")
                                                                   .Count() == 8;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.TrenMotThangTuoi.TrongBenhVien.DuDieuKienTiem"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.KhongDongYTiem:
                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group2Co" || p.Id == "Group3Co" || p.Id == "Group4Co" || p.Id == "Group5Co" || p.Id == "Group6Co" || p.Id == "Group7Co") &&
                                                                             p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.TrenMotThangTuoi.TrongBenhVien.TamHoanTiemChung"));
                            }

                            break;
                    }
                }
                else if(phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.NgoaiBenhVien)
                {
                    switch (yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan.Value)
                    {
                        case LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Co" || p.Id == "Group9Co") &&
                                                                               p.Value == "true")
                                                                   .Count() == 2;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.TrenMotThangTuoi.NgoaiBenhVien.ChongChiDinh"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Khong" || p.Id == "Group2Khong" || p.Id == "Group3Khong" || p.Id == "Group4Khong" || p.Id == "Group5Khong" || p.Id == "Group6Khong" || p.Id == "Group7Khong" || p.Id == "Group8Khong" || p.Id == "Group9Khong") &&
                                                                               p.Value == "true")
                                                                   .Count() == 9;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.TrenMotThangTuoi.NgoaiBenhVien.DuDieuKienTiem"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.KhongDongYTiem:
                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group2Co" || p.Id == "Group3Co" || p.Id == "Group4Co" || p.Id == "Group5Co" || p.Id == "Group6Co" || p.Id == "Group7Co" || p.Id == "Group8Co") &&
                                                                             p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.TrenMotThangTuoi.NgoaiBenhVien.TamHoanTiemChung"));
                            }

                            break;
                    }
                }
                else
                {
                    switch (yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan.Value)
                    {
                        case LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => p.Id == "Group1Co" && p.Value == "true")
                                                                   .Count() == 1;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuanSangLocTiemCovid.ChongChiDinh"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Khong" || p.Id == "Group2Khong" || p.Id == "Group313TuanKhong" || p.Id == "Group3Hon13TuanKhong" || p.Id == "Group4Khong" || p.Id == "Group5Khong" || p.Id == "Group6Khong" || p.Id == "Group7Khong" || p.Id == "Group8Khong" || p.Id == "Group9Khong" || p.Id == "Group10Khong")
                                                                               && p.Value == "true")
                                                                   .Count() == 11;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuanSangLocTiemCovid.DuDieuKienTiem"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.KhongDongYTiem:
                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group2Co" || p.Id == "Group313TuanCo")
                                                                             && p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuanSangLocTiemCovid.TamHoanTiemChung"));
                            }

                            break;
                    }
                }
            }
            else
            {
                if (phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.TrongBenhVien)
                {
                    switch (yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan.Value)
                    {
                        case LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9Co" &&
                                                                             p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.SoSinh.TrongBenhVien.ChongChiDinh"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Khong" || p.Id == "Group2Khong" || p.Id == "Group3Khong" || p.Id == "Group4Khong" || p.Id == "Group5Khong" || p.Id == "Group6Khong" || p.Id == "Group7Khong" || p.Id == "Group8Khong" || p.Id == "Group9Khong") &&
                                                                               p.Value == "true")
                                                                   .Count() == 9;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.SoSinh.TrongBenhVien.DuDieuKienTiem"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.KhongDongYTiem:
                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group1Co" || p.Id == "Group2Co" || p.Id == "Group3Co" || p.Id == "Group4Co" || p.Id == "Group5Co" || p.Id == "Group6Co" || p.Id == "Group7Co" || p.Id == "Group8Co") &&
                                                                             p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.SoSinh.TrongBenhVien.TamHoanTiemChung"));
                            }

                            break;
                    }
                }
                else if(phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.NgoaiBenhVien)
                {
                    switch (yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan.Value)
                    {
                        case LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group8Co" &&
                                                                             p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.SoSinh.NgoaiBenhVien.ChongChiDinh"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Khong" || p.Id == "Group2Khong" || p.Id == "Group3Khong" || p.Id == "Group4Khong" || p.Id == "Group5Khong" || p.Id == "Group6Khong" || p.Id == "Group7Khong" || p.Id == "Group8Khong") &&
                                                                               p.Value == "true")
                                                                   .Count() == 8;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.SoSinh.NgoaiBenhVien.DuDieuKienTiem"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.KhongDongYTiem:
                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group1Co" || p.Id == "Group2Co" || p.Id == "Group3Co" || p.Id == "Group4Co" || p.Id == "Group5Co" || p.Id == "Group6Co" || p.Id == "Group7Co") &&
                                                                             p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuan.SoSinh.NgoaiBenhVien.TamHoanTiemChung"));
                            }

                            break;
                    }
                }
                else
                {
                    switch (yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan.Value)
                    {
                        case LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => p.Id == "Group1Co" && p.Value == "true")
                                                                   .Count() == 1;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuanSangLocTiemCovid.ChongChiDinh"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem:
                            isHopLe = thongTin.DataKhamTheoTemplate.Where(p => (p.Id == "Group1Khong" || p.Id == "Group2Khong" || p.Id == "Group313TuanKhong" || p.Id == "Group3Hon13TuanKhong" || p.Id == "Group4Khong" || p.Id == "Group5Khong" || p.Id == "Group6Khong" || p.Id == "Group7Khong" || p.Id == "Group8Khong" || p.Id == "Group9Khong" || p.Id == "Group10Khong")
                                                                               && p.Value == "true")
                                                                   .Count() == 11;

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuanSangLocTiemCovid.DuDieuKienTiem"));
                            }

                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.KhongDongYTiem:
                            break;
                        case LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung:
                            isHopLe = thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group2Co" || p.Id == "Group313TuanCo")
                                                                             && p.Value == "true");

                            if (!isHopLe)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.KetLuanSangLocTiemCovid.TamHoanTiemChung"));
                            }

                            break;
                    }
                }
            }
        }

        public async Task KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(TiemChungChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo)
        {
            var lstDichVuDaChon = yeuCauVo.DichVus;
            var lstDichVuKhamBenhTrongGoi = new List<TiemChungDichVuBenhVienTheoGoiDichVuVo>();
            var lstDichVuSoLuongConLaiKhongDu = new List<string>();
            var lstDichVuKhongCoTrongGoi = new List<string>();

            var lstYeuCauGoiDichVuIds = lstDichVuDaChon.Select(a => a.YeuCauGoiDichVuId).ToList();
            var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien)//.ThenInclude(t => t.YeuCauDichVuKyThuats)
                .Where(x => lstYeuCauGoiDichVuIds.Contains(x.Id))
                .ToList();

            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId != null
                            && lstYeuCauGoiDichVuIds.Contains(x.YeuCauGoiDichVuId.Value))
                .Select(item => new ThongTinSuDungDichVuTronGoiVo()
                {
                    YeuCauGoiDichVuId = item.YeuCauGoiDichVuId.Value,
                    DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                    SoLan = item.SoLan
                })
                .ToList();

            foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
            {
                // dịch vụ kỹ thuật
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                    .Select(item => new TiemChungDichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                        //SoLanDaSuDung = item.DichVuKyThuatBenhVien.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                        //                                                                     && a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id).Sum(a => a.SoLan),
                        SoLanDaSuDung = dichVuKyThuatDaChiDinhs.Where(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id && a.DichVuBenhVienId == item.DichVuKyThuatBenhVienId).Sum(b => b.SoLan),
                        SoLanTheoGoi = item.SoLan
                    }));
            }

            foreach (var dichVuDaChon in lstDichVuDaChon)
            {
                var dichVuTrongGoi = lstDichVuKhamBenhTrongGoi.FirstOrDefault(x => x.YeuCauGoiDichVuId == dichVuDaChon.YeuCauGoiDichVuId
                                                                                   && x.DichVuBenhVienId == dichVuDaChon.DichVuBenhVienId);
                if (dichVuTrongGoi == null)
                {
                    lstDichVuKhongCoTrongGoi.Add(dichVuDaChon.TenDichVu);
                }
                else if (dichVuTrongGoi.SoLanConLai < dichVuDaChon.SoLuongSuDung)
                {
                    lstDichVuSoLuongConLaiKhongDu.Add(dichVuDaChon.TenDichVu);
                }
            }

            if (lstDichVuKhongCoTrongGoi.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVu.NotExists"), lstDichVuKhongCoTrongGoi.Join(",")));
            }
            if (lstDichVuSoLuongConLaiKhongDu.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.DichVuSoLuongConLai.NotEnough"), lstDichVuSoLuongConLaiKhongDu.Join(",")));
            }

            var lstGoiDaQuyetToan = yeuCauGoiDichVus.Where(x => x.DaQuyetToan == true).ToList();
            if (lstGoiDaQuyetToan.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.DaQuetToan"), lstGoiDaQuyetToan.Select(x => x.TenChuongTrinh).Join(",")));
            }

            var lstGoiDangNgungSuDung = yeuCauGoiDichVus.Where(x => x.NgungSuDung == true).ToList();
            if (lstGoiDangNgungSuDung.Any())
            {
                throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.YeuCauGoiDichVu.NgungSuDung"), lstGoiDangNgungSuDung.Select(x => x.TenChuongTrinh).Join(",")));
            }
        }

        public async Task<bool> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId)
        {
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            // todo: có cập nhật bỏ await
            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId != null)
                .Include(x => x.DichVuKyThuatBenhVien)
                .ToList();

            var goiDichVuDaDangKyKhaDung = _yeuCauGoiDichVuRepository.TableNoTracking
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && x.DaQuyetToan != true
                            && (x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(a => nhomTiemChungId != null && a.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId &&
                                                                                                   a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id &&
                                                                                                                                                b.DichVuKyThuatBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan))
                           )
                            
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                            )
                .Any();

            //var test = _yeuCauGoiDichVuRepository.TableNoTracking
            //    .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
            //                && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
            //                && x.DaQuyetToan != true)
            //    .Include(x => x.ChuongTrinhGoiDichVu)
            //    .ThenInclude(x => x.ChuongTrinhGoiDichVuDichVuKyThuats)
            //    .ToList();

            return goiDichVuDaDangKyKhaDung;
        }
        #endregion

        #region Xử lý data
        public async Task KiemTraDatayeuCauTiemChungAsync(long yeuCauDichVuKyThuatId, long phongBenhVienHangDoiId = 0, Enums.EnumTrangThaiHangDoi trangThaiHangDoi = Enums.EnumTrangThaiHangDoi.DangKham, long? yeuCauDichVuKyThuatVacxinId = null)
        {
            var resourceName = "";
            var yeuCauTiemChung = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Include(x => x.PhongBenhVienHangDois)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .FirstOrDefault(x => x.Id == yeuCauDichVuKyThuatId);

            if (yeuCauTiemChung == null)
            {
                resourceName = "TiemChung.YeuCauDichVuKyThuat.NotExists";
            }
            else
            {
                switch (yeuCauTiemChung.TrangThai)
                {
                    case EnumTrangThaiYeuCauDichVuKyThuat.DaHuy:
                        resourceName = "TiemChung.YeuCauDichVuKyThuat.DaHuy"; break;
                    case EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien:
                        if (yeuCauDichVuKyThuatVacxinId == null)
                        {
                            resourceName = "TiemChung.YeuCauDichVuKyThuat.DaHoanThanhKham";
                        }
                        break;

                    default:
                        resourceName = ""; break;
                }

                // kiểm tra trạng thái hàng đợi, nếu hàng đợi ko phải đang khám thì báo lỗi thay đổi trạng thái
                if (trangThaiHangDoi == Enums.EnumTrangThaiHangDoi.DangKham
                    && string.IsNullOrEmpty(resourceName)
                    && ((yeuCauDichVuKyThuatVacxinId == null 
                         && yeuCauTiemChung.PhongBenhVienHangDois
                             .Any(x => x.YeuCauKhamBenhId == null && (phongBenhVienHangDoiId == 0 || yeuCauTiemChung.NoiThucHienId == null || x.Id == yeuCauTiemChung.NoiThucHienId)))
                        || (yeuCauDichVuKyThuatVacxinId != null 
                            && yeuCauTiemChung.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                .Where(x => x.Id == yeuCauDichVuKyThuatVacxinId)
                                .Any(a => a.PhongBenhVienHangDois.Any(x => x.YeuCauKhamBenhId == null && (phongBenhVienHangDoiId == 0 || yeuCauTiemChung.NoiThucHienId == null || x.Id == yeuCauTiemChung.NoiThucHienId))))))
                {
                    var phongBenhVienHangDoiHienTai = new PhongBenhVienHangDoi();
                    if (yeuCauDichVuKyThuatVacxinId == null)
                    {
                        phongBenhVienHangDoiHienTai =
                            yeuCauTiemChung.PhongBenhVienHangDois
                                .Where(x => x.YeuCauKhamBenhId == null && (phongBenhVienHangDoiId == 0 || yeuCauTiemChung.NoiThucHienId == null || x.Id == yeuCauTiemChung.NoiThucHienId))
                                .OrderByDescending(x => x.Id)
                                .First();
                    }
                    else
                    {
                        phongBenhVienHangDoiHienTai =
                            yeuCauTiemChung.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                                .Where(x => x.Id == yeuCauDichVuKyThuatVacxinId)
                                .SelectMany(a => a.PhongBenhVienHangDois)
                                .Where(x => x.YeuCauKhamBenhId == null && (phongBenhVienHangDoiId == 0 || yeuCauTiemChung.NoiThucHienId == null || x.Id == yeuCauTiemChung.NoiThucHienId))
                                .OrderByDescending(x => x.TrangThai == EnumTrangThaiHangDoi.DangKham).ThenBy(x => x.Id)
                                .First();
                    }

                    if (phongBenhVienHangDoiHienTai.TrangThai != Enums.EnumTrangThaiHangDoi.DangKham)
                    {
                        resourceName = "TiemChung.PhongBenhVienHangDoi.TrangThaiBiThayDoi";
                    }

                }
            }

            if (!string.IsNullOrEmpty(resourceName))
            {
                throw new Exception(_localizationService.GetResource(resourceName));
            }
        }

        public async Task<bool> KiemTraCoBenhNhanKhacDangKhamTiemChungTrongPhong(long? yeuCauDichVuKyThuatId, long phongBenhVienId, Enums.LoaiHangDoiTiemVacxin loaiHangDoi = Enums.LoaiHangDoiTiemVacxin.KhamSangLoc)
        {
            var coBenhNhanKhacDangKham = await _phongBenhVienHangDoiRepository.TableNoTracking
                .AnyAsync(x => //(yeuCauDichVuKyThuatId == null || x.YeuCauDichVuKyThuatId != yeuCauDichVuKyThuatId)
                                x.YeuCauDichVuKyThuatId != null
                               && x.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.SangLocTiemChung
                               && x.PhongBenhVienId == phongBenhVienId
                               && x.TrangThai == Enums.EnumTrangThaiHangDoi.DangKham
                               
                               && ((loaiHangDoi == LoaiHangDoiTiemVacxin.KhamSangLoc 
                                    && (x.YeuCauDichVuKyThuat.KhamSangLocTiemChung == null || x.YeuCauDichVuKyThuat.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && (yeuCauDichVuKyThuatId == null || x.YeuCauDichVuKyThuatId != yeuCauDichVuKyThuatId))
                                   || (loaiHangDoi == LoaiHangDoiTiemVacxin.ThucHienTiem 
                                       && x.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChung != null 
                                       && x.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                                       && (yeuCauDichVuKyThuatId == null || x.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChung.Id != yeuCauDichVuKyThuatId))));
            return coBenhNhanKhacDangKham;
        }

        public async Task XuLyHoanThanhCongDoanKhamTiemChungHienTaiCuaBenhNhan(long yeuCauDichVuKyThuatId, bool hoanThanhKham = false, long? yeuCauDichVuKyThuatVacxinId = null)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var yeuCauDichVuyThuat = _yeuCauDichVuKyThuatRepository.Table
                .Include(x => x.PhongBenhVienHangDois)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.KhamSangLocTiemChung).ThenInclude(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung)
                .First(x => x.Id == yeuCauDichVuKyThuatId);

            var yeuCauDichVuyThuatVacxin = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(x => x.Id == yeuCauDichVuKyThuatVacxinId).FirstOrDefault();

            var hangDoiDangKham = new PhongBenhVienHangDoi();
            if (yeuCauDichVuKyThuatVacxinId == null)
            {
                hangDoiDangKham = yeuCauDichVuyThuat.PhongBenhVienHangDois
                    .Where(x => x.PhongBenhVienId == yeuCauDichVuyThuat.NoiThucHienId)
                    .OrderByDescending(x => x.Id)
                    .First();
            }
            else
            {
                phongHienTaiId = yeuCauDichVuyThuatVacxin?.NoiThucHienId ?? phongHienTaiId;
                hangDoiDangKham = yeuCauDichVuyThuat.KhamSangLocTiemChung.YeuCauDichVuKyThuats
                    .Where(x => x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                    .SelectMany(a => a.PhongBenhVienHangDois)
                    .Where(x => x.PhongBenhVienId == phongHienTaiId)
                    .OrderByDescending(x => x.TrangThai == EnumTrangThaiHangDoi.DangKham).ThenBy(x => x.Id)
                    .First();
            }
            if (hoanThanhKham) // trường hợp nhấn nút hoàn thành khám
            {
                if (yeuCauDichVuKyThuatVacxinId == null)
                {
                    if (yeuCauDichVuyThuat.KhamSangLocTiemChung.KetLuan != LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem)
                    {
                        if (yeuCauDichVuyThuat.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(p => p.TiemChung != null))
                        {
                            throw new Exception(_localizationService.GetResource("TiemChung.KhongDongY.TamHoanTiemChung.ChongChiDinh.Vacxin"));
                        }
                    }
                    else
                    {
                        if (!yeuCauDichVuyThuat.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(p => p.TiemChung != null))
                        {
                            throw new Exception(_localizationService.GetResource("TiemChung.DongY.Vacxin"));
                        }
                    }

                    yeuCauDichVuyThuat.ThoiDiemKetLuan = DateTime.Now;
                    yeuCauDichVuyThuat.NhanVienKetLuanId = currentUserId;

                    yeuCauDichVuyThuat.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc = DateTime.Now;
                    yeuCauDichVuyThuat.KhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId = currentUserId;

                    if (yeuCauDichVuyThuat.KhamSangLocTiemChung.KetLuan != LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem || 
                        yeuCauDichVuyThuat.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy).All(p => p.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien))
                    {
                        yeuCauDichVuyThuat.ThoiDiemHoanThanh = DateTime.Now;
                        yeuCauDichVuyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                    }

                    foreach(var hangDoi in yeuCauDichVuyThuat.PhongBenhVienHangDois)
                    {
                        hangDoi.WillDelete = true;
                    }

                    await _yeuCauDichVuKyThuatRepository.Context.SaveChangesAsync();
                }
            }
            else // trường hợp lưu thông tin cho lần khám hiện tại, và khám yêu cầu khác đồng thời cập nhật lại ví trí hàng chờ
            {
                var laHangDoiKhamSangLoc = yeuCauDichVuKyThuatVacxinId == null;
                var laHangDoiTiemChung = yeuCauDichVuKyThuatVacxinId != null;

                var lstHangDoiTheoPhongKham = await _phongBenhVienHangDoiRepository.Table
                    .Include(y => y.YeuCauDichVuKyThuat)
                    .Where(x => x.PhongBenhVienId == hangDoiDangKham.PhongBenhVienId
                                && x.YeuCauDichVuKyThuat != null
                                && x.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy
                                && (
                                (laHangDoiKhamSangLoc
                                    && x.YeuCauDichVuKyThuat.TiemChung == null
                                    && (x.YeuCauDichVuKyThuat.KhamSangLocTiemChung == null || x.YeuCauDichVuKyThuat.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                                    && x.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                    // hàng chờ khám sàng lọc chỉ lấy những dv đang thực hiện
                                    && x.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                                    || (laHangDoiTiemChung
                                        && x.YeuCauDichVuKyThuat.TiemChung != null
                                        && x.YeuCauDichVuKyThuat.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung
                                        && x.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChung != null
                                        && x.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatKhamSangLocTiemChung.NhanVienHoanThanhKhamSangLocId != null
                                        // hàng chờ thực hiện tiêm,chỉ lấy những dv khác hủy -> khi hoàn thành tiêm thì sẽ xóa hết hàng đợi
                                        )
                             )
                                )
                    .OrderBy(x => x.SoThuTu).ToListAsync();

                var viTriHangDoiDangKham = lstHangDoiTheoPhongKham.FindLastIndex(x => x.Id == hangDoiDangKham.Id);
                if (viTriHangDoiDangKham != 0 && lstHangDoiTheoPhongKham.Count() > 1)
                {
                    var sttDauTien = 0;
                    foreach (var hangDoi in lstHangDoiTheoPhongKham)
                    {
                        sttDauTien = sttDauTien == 0 ? hangDoi.SoThuTu : sttDauTien;
                        if (hangDoi.Id == hangDoiDangKham.Id)
                        {
                            hangDoi.SoThuTu = sttDauTien;
                            hangDoi.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                            hangDoi.YeuCauDichVuKyThuat.LastTime = DateTime.Now;
                        }
                        else
                        {
                            hangDoi.SoThuTu += 1;
                        }
                    }
                }
                else if (viTriHangDoiDangKham == 0)
                {
                    foreach (var hangDoi in lstHangDoiTheoPhongKham)
                    {
                        if (hangDoi.Id == hangDoiDangKham.Id)
                        {
                            hangDoi.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                            hangDoi.YeuCauDichVuKyThuat.LastTime = DateTime.Now;
                            break;
                        }
                    }
                }
                //await _phongBenhVienHangDoiRepository.UpdateAsync(lstHangDoiTheoPhongKham);
                await _phongBenhVienHangDoiRepository.Context.SaveChangesAsync();
            }
        }

        public async Task<YeuCauDichVuKyThuat> ThemChiDinhVacxinAsync(YeuCauKhamTiemChungVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhan)
        {
            //var coBHYT = yeuCauTiepNhan.CoBHYT ?? false;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentPhongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var noiThucHien = await _phongBenhVienRepository.GetByIdAsync(yeuCauVo.NoiThucHienId.GetValueOrDefault());
            var noiChiDinh = await _phongBenhVienRepository.GetByIdAsync(currentPhongLamViecId);

            var nhanVienChiDinh = await _nhanVienRepository.GetByIdAsync(currentUserId, o => o.Include(p => p.User));

            var dichVuKyThuatBenhVien = await _dichVuKyThuatBenhVienRepository.GetByIdAsync(yeuCauVo.DichVuKyThuatBenhVienId);

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            /* Thêm yêu cầu dịch vụ ký thuật */
            var newYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
            {
                DichVuKyThuatBenhVienId = yeuCauVo.DichVuKyThuatBenhVienId,
                DichVuKyThuatBenhVien = dichVuKyThuatBenhVien,
                NhomDichVuBenhVienId = yeuCauVo.NhomDichVuBenhVienId,
                NoiThucHienId = yeuCauVo.NoiThucHienId,
                NoiThucHien = noiThucHien,

                YeuCauGoiDichVuId = yeuCauVo.YeuCauGoiDichVuId,
                DonGiaTruocChietKhau = yeuCauVo.DonGiaTruocChietKhau,
                DonGiaSauChietKhau = yeuCauVo.DonGiaSauChietKhau
            };

            var dvkt = await _dichVuKyThuatBenhVienRepository.GetByIdAsync(newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId, x => x.Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(o => o.DichVuKyThuatVuBenhVienGiaBenhViens)
                .Include(o => o.DichVuKyThuat));

            var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
            long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);

            var dvktGiaBH = dvkt.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(p => p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || DateTime.Now.Date <= p.DenNgay.Value.Date));
            var dvktGiaBV = dvkt.DichVuKyThuatVuBenhVienGiaBenhViens.Where(p => p.TuNgay.Date <= DateTime.Now.Date && (p.DenNgay == null || DateTime.Now.Date <= p.DenNgay.Value.Date))
                                                                    .OrderByDescending(p => p.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                                                                    .ThenBy(p => p.CreatedOn)
                                                                    .First();

            var dtudDVKTBV = yeuCauTiepNhan.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o => o.DichVuKyThuatBenhVienId == newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVuKyThuatBenhVien.CoUuDai == true);
            var duocHuongBaoHiem = false;
            //var duocHuongBaoHiem = yeuCauTiepNhan.NoiTruBenhAn == null && yeuCauTiepNhan.QuyetToanTheoNoiTru != true ? false : (coBHYT && dvktGiaBH != null && dvktGiaBH.Gia != 0);

            newYeuCauDichVuKyThuat.BaoHiemChiTra = null;
            newYeuCauDichVuKyThuat.DuocHuongBaoHiem = duocHuongBaoHiem;

            newYeuCauDichVuKyThuat.YeuCauTiepNhanId = yeuCauTiepNhan.Id;
            newYeuCauDichVuKyThuat.MaDichVu = dvkt.Ma;
            newYeuCauDichVuKyThuat.TenDichVu = dvkt.Ten;
            newYeuCauDichVuKyThuat.Gia = yeuCauVo.Gia ?? dvktGiaBV.Gia;
            newYeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId = dvktGiaBV.NhomGiaDichVuKyThuatBenhVienId;
            newYeuCauDichVuKyThuat.NhomChiPhi = dvkt.DichVuKyThuat != null ? dvkt.DichVuKyThuat.NhomChiPhi : EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe;
            newYeuCauDichVuKyThuat.SoLan = 1;
            newYeuCauDichVuKyThuat.TiLeUuDai = dtudDVKTBV?.TiLeUuDai;
            newYeuCauDichVuKyThuat.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
            newYeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            newYeuCauDichVuKyThuat.NhanVienChiDinhId = currentUserId;
            newYeuCauDichVuKyThuat.NhanVienChiDinh = nhanVienChiDinh;
            newYeuCauDichVuKyThuat.NoiChiDinhId = currentPhongLamViecId;
            newYeuCauDichVuKyThuat.NoiChiDinh = noiChiDinh;
            newYeuCauDichVuKyThuat.ThoiDiemChiDinh = DateTime.Now;
            newYeuCauDichVuKyThuat.ThoiDiemDangKy = DateTime.Now;
            newYeuCauDichVuKyThuat.NhomDichVuBenhVienId = dvkt.NhomDichVuBenhVienId;
            newYeuCauDichVuKyThuat.LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(newYeuCauDichVuKyThuat.NhomDichVuBenhVienId, lstNhomDichVuBenhVien);
            newYeuCauDichVuKyThuat.MaGiaDichVu = dvkt.DichVuKyThuat?.MaGia;
            newYeuCauDichVuKyThuat.TenGiaDichVu = dvkt.DichVuKyThuat?.TenGia;
            /* */

            /* Get người thực hiện mặc định */
            var nguoiThucHien = await GetBacSiThucHienMacDinhAsync(newYeuCauDichVuKyThuat.NoiThucHienId ?? 0);
            if (nguoiThucHien != null)
            {
                var nhanVienThucHien = await _nhanVienRepository.GetByIdAsync(nguoiThucHien.KeyId, o => o.Include(p => p.User));
                
                newYeuCauDichVuKyThuat.NhanVienThucHienId = nguoiThucHien.KeyId;
                newYeuCauDichVuKyThuat.NhanVienThucHien = nhanVienThucHien;
            }

            if (dvktGiaBH != null)
            {
                newYeuCauDichVuKyThuat.DonGiaBaoHiem = dvktGiaBH.Gia;
                newYeuCauDichVuKyThuat.TiLeBaoHiemThanhToan = dvktGiaBH.TiLeBaoHiemThanhToan;
            }
            /* */

            /* Thêm yêu cầu dịch vụ kỹ thuật tiêm chủng (dược phẩm) */
            var duocPhamBenhVien = await _duocPhamBenhVienRepository.GetByIdAsync(yeuCauVo.TiemChung.DuocPhamBenhVienId, o => o.Include(p => p.DuocPham));

            var newYeuCauDichVuKyThuatTiemChung = new YeuCauDichVuKyThuatTiemChung();
            newYeuCauDichVuKyThuatTiemChung.DuocPhamBenhVienId = duocPhamBenhVien.Id;
            newYeuCauDichVuKyThuatTiemChung.TenDuocPham = duocPhamBenhVien.DuocPham.Ten;
            newYeuCauDichVuKyThuatTiemChung.TenDuocPhamTiengAnh = duocPhamBenhVien.DuocPham.TenTiengAnh;
            newYeuCauDichVuKyThuatTiemChung.SoDangKy = duocPhamBenhVien.DuocPham.SoDangKy;
            newYeuCauDichVuKyThuatTiemChung.STTHoatChat = duocPhamBenhVien.DuocPham.STTHoatChat;
            newYeuCauDichVuKyThuatTiemChung.MaHoatChat = duocPhamBenhVien.DuocPham.MaHoatChat;
            newYeuCauDichVuKyThuatTiemChung.HoatChat = duocPhamBenhVien.DuocPham.HoatChat;
            newYeuCauDichVuKyThuatTiemChung.LoaiThuocHoacHoatChat = duocPhamBenhVien.DuocPham.LoaiThuocHoacHoatChat;
            newYeuCauDichVuKyThuatTiemChung.NhaSanXuat = duocPhamBenhVien.DuocPham.NhaSanXuat;
            newYeuCauDichVuKyThuatTiemChung.NuocSanXuat = duocPhamBenhVien.DuocPham.NuocSanXuat;
            newYeuCauDichVuKyThuatTiemChung.DuongDungId = duocPhamBenhVien.DuocPham.DuongDungId;
            newYeuCauDichVuKyThuatTiemChung.HamLuong = duocPhamBenhVien.DuocPham.HamLuong;
            newYeuCauDichVuKyThuatTiemChung.QuyCach = duocPhamBenhVien.DuocPham.QuyCach;
            newYeuCauDichVuKyThuatTiemChung.TieuChuan = duocPhamBenhVien.DuocPham.TieuChuan;
            newYeuCauDichVuKyThuatTiemChung.DangBaoChe = duocPhamBenhVien.DuocPham.DangBaoChe;
            newYeuCauDichVuKyThuatTiemChung.DonViTinhId = duocPhamBenhVien.DuocPham.DonViTinhId;
            newYeuCauDichVuKyThuatTiemChung.HuongDan = duocPhamBenhVien.DuocPham.HuongDan;
            newYeuCauDichVuKyThuatTiemChung.MoTa = duocPhamBenhVien.DuocPham.MoTa;
            newYeuCauDichVuKyThuatTiemChung.ChiDinh = duocPhamBenhVien.DuocPham.ChiDinh;
            newYeuCauDichVuKyThuatTiemChung.ChongChiDinh = duocPhamBenhVien.DuocPham.ChongChiDinh;
            newYeuCauDichVuKyThuatTiemChung.LieuLuongCachDung = duocPhamBenhVien.DuocPham.LieuLuongCachDung;
            newYeuCauDichVuKyThuatTiemChung.TacDungPhu = duocPhamBenhVien.DuocPham.TacDungPhu;
            newYeuCauDichVuKyThuatTiemChung.ChuYdePhong = duocPhamBenhVien.DuocPham.ChuYDePhong;

            //newYeuCauDichVuKyThuatTiemChung.HopDongThauDuocPhamId = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.NhaThauId = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.SoHopDongThau = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.SoQuyetDinhThau = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.LoaiThau = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.LoaiThuocThau = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.NhomThau = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.GoiThau = duocPhamBenhVien.Id;
            //newYeuCauDichVuKyThuatTiemChung.NamThau = duocPhamBenhVien.Id;

            newYeuCauDichVuKyThuatTiemChung.ViTriTiem = yeuCauVo.TiemChung.ViTriTiem;
            newYeuCauDichVuKyThuatTiemChung.MuiSo = yeuCauVo.TiemChung.MuiSo;
            newYeuCauDichVuKyThuatTiemChung.TrangThaiTiemChung = yeuCauVo.TiemChung.TrangThaiTiemChung;
            newYeuCauDichVuKyThuatTiemChung.LieuLuong = yeuCauVo.TiemChung.LieuLuong;
            //newYeuCauDichVuKyThuatTiemChung.NhanVienTiemId = null;

            /***** Xử lý số lượng *****/
            //var nhapChiTietTheoDuocPham = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == yeuCauVo.KhoId &&
            //                                                                                                    x.DuocPhamBenhVienId == duocPhamBenhVien.Id &&
            //                                                                                                    x.NhapKhoDuocPhams.DaHet != true &&
            //                                                                                                    x.LaDuocPhamBHYT == false && //Tạm thời DP dạng vắcxin k có BHYT
            //                                                                                                                                 //x.LaDuocPhamBHYT == yeuCauVo.LaDuocPhamBHYT &&
            //                                                                                                    x.SoLuongNhap > x.SoLuongDaXuat)
            //                                                                                        .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                                                        .FirstOrDefaultAsync();

            //var soLuongTonTrongKho = Math.Round(nhapChiTietTheoDuocPham.SoLuongNhap - nhapChiTietTheoDuocPham.SoLuongDaXuat, 2);

            //if ((soLuongTonTrongKho < 0) || (soLuongTonTrongKho < yeuCauVo.TiemChung.SoLuong))
            //{
            //    throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
            //}

            //// thêm xuất kho dược phẩm chi tiết
            //var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
            //{
            //    DuocPhamBenhVienId = yeuCauVo.DuocPhamBenhVienId
            //};

            //var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
            //{
            //    NhapKhoDuocPhamChiTietId = nhapChiTietTheoDuocPham.Id
            //};

            //xuatViTri.SoLuongXuat = yeuCauVo.SoLuong;
            //nhapChiTietTheoDuocPham.SoLuongDaXuat = Math.Round(nhapChiTietTheoDuocPham.SoLuongDaXuat + yeuCauVo.SoLuong, 2);

            //xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);

            //newYeuCauDichVuKyThuatTiemChung.XuatKhoDuocPhamChiTiet = xuatChiTiet;
            newYeuCauDichVuKyThuatTiemChung.SoLuong = yeuCauVo.TiemChung.SoLuong;
            //newYeuCauDichVuKyThuatTiemChung.HopDongThauDuocPhamId = nhapChiTietTheoDuocPham.HopDongThauDuocPhamId;
            ////newYeuCauDichVuKyThuatTiemChung.SoLuong = Math.Round(newYeuCauDichVuKyThuatTiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Sum(x => x.SoLuongXuat), 2);
            newYeuCauDichVuKyThuat.TiemChung = newYeuCauDichVuKyThuatTiemChung;
            //yeuCauVo.NhapKhoDuocPhamChiTiets.Add(nhapChiTietTheoDuocPham);
            /***** *****/
            /* */

            //yeuCauDichVuKyThuat.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Add(newYeuCauDichVuKyThuat);

            //BVHD-3825
            var mienGiam = yeuCauVo.MienGiamChiPhis
                .FirstOrDefault(x => x.DaHuy != true 
                                     && x.YeuCauGoiDichVuId == yeuCauVo.YeuCauGoiDichVuKhuyenMaiId);
            if (mienGiam != null)
            {
                var chuongTrinhKhuyenMai = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => x.Id == mienGiam.YeuCauGoiDichVuId)
                    .SelectMany(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                    .FirstOrDefault(x => x.DichVuKyThuatBenhVienId == yeuCauVo.DichVuKyThuatBenhVienId
                                         && x.NhomGiaDichVuKyThuatBenhVienId == newYeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId);

                if (chuongTrinhKhuyenMai != null)
                {
                    var thanhTien = newYeuCauDichVuKyThuat.SoLan * newYeuCauDichVuKyThuat.Gia;
                    var thanhTienMienGiam = newYeuCauDichVuKyThuat.SoLan * chuongTrinhKhuyenMai.DonGiaKhuyenMai;
                    var soTienMienGiam = (thanhTien - thanhTienMienGiam) > 0 ? (thanhTien - thanhTienMienGiam) : 0;

                    newYeuCauDichVuKyThuat.SoTienMienGiam = soTienMienGiam;
                    newYeuCauDichVuKyThuat.MienGiamChiPhis.Add(new MienGiamChiPhi()
                    {
                        YeuCauTiepNhanId = mienGiam.YeuCauTiepNhanId,
                        YeuCauGoiDichVuId = mienGiam.YeuCauGoiDichVuId,
                        SoTien = soTienMienGiam,
                        LoaiMienGiam = mienGiam.LoaiMienGiam.Value,
                        LoaiChietKhau = mienGiam.LoaiChietKhau.Value
                    });
                }
            }

            return newYeuCauDichVuKyThuat;
        }

        public async Task XuLySoLuongChiDinhVacxinAsync(ICollection<YeuCauDichVuKyThuat> yeuCauDichVuKyThuats, List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets)
        {
            var nhapChiTietVacxinTiemChung = new List<NhapChiTietVacxinTiemChungVo>();
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();

            var khoVacxin = await _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin).FirstOrDefaultAsync();

            if (khoVacxin == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            /***** *****/

            foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
            {
                if (yeuCauDichVuKyThuat.Id != 0)
                {
                    if (yeuCauDichVuKyThuat.WillDelete == true)
                    {
                        await XuLyXoaSoLuongChiDinhVacxinAsync(yeuCauDichVuKyThuat, nhapChiTietVacxinTiemChung);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    await XuLyThemSoLuongChiDinhVacxinAsync(yeuCauDichVuKyThuat, nhapChiTietVacxinTiemChung, cauHinhChung, khoVacxin.Id);
                }
            }

            nhapChiTietVacxinTiemChung = nhapChiTietVacxinTiemChung.GroupBy(p => p.NhapKhoDuocPhamChiTietId)
                                                                   .Select(p => new NhapChiTietVacxinTiemChungVo
                                                                   {
                                                                       NhapKhoDuocPhamChiTietId = p.First().NhapKhoDuocPhamChiTietId,
                                                                       NhapKhoDuocPhamChiTiet = p.First().NhapKhoDuocPhamChiTiet,
                                                                       SoLuongXuat = p.Sum(o => o.SoLuongXuat)
                                                                   })
                                                                   .ToList();

            foreach (var item in nhapChiTietVacxinTiemChung)
            {
                //item.NhapKhoDuocPhamChiTiet.SoLuongDaXuat += item.SoLuongXuat;
                item.NhapKhoDuocPhamChiTiet.SoLuongDaXuat = Math.Round(item.NhapKhoDuocPhamChiTiet.SoLuongDaXuat + item.SoLuongXuat, 2);
                nhapKhoDuocPhamChiTiets.Add(item.NhapKhoDuocPhamChiTiet);
            }
        }

        public async Task XuLyThemSoLuongChiDinhVacxinAsync(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapChiTietVacxinTiemChungVo> nhapChiTietVacxin, CauHinhChung cauHinhChung, long khoVacXinId)
        {
            /***** Xử lý số lượng *****/
            var lstNhapChiTietTheoDuocPham = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == khoVacXinId &&
                                                                                                                x.DuocPhamBenhVienId == yeuCauDichVuKyThuat.TiemChung.DuocPhamBenhVienId &&
                                                                                                                x.NhapKhoDuocPhams.DaHet != true &&//Tạm thời DP dạng vắcxin k có BHYT
                                                                                                                //x.LaDuocPhamBHYT == yeuCauVo.LaDuocPhamBHYT &&
                                                                                                                x.LaDuocPhamBHYT == false &&
                                                                                                                x.SoLuongNhap > x.SoLuongDaXuat

                                                                                                                //BVHD-3821
                                                                                                                // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                                                                                                && x.HanSuDung.Date >= DateTime.Now.Date)
                                                                                                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                                                    .ToListAsync();

            //var nhapChiTietTheoDuocPham = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(x => x.NhapKhoDuocPhams.KhoId == khoVacxin.Id &&
            //                                                                                                 x.DuocPhamBenhVienId == yeuCauDichVuKyThuat.TiemChung.DuocPhamBenhVienId &&
            //                                                                                                 x.NhapKhoDuocPhams.DaHet != true &&
            //                                                                                                 x.LaDuocPhamBHYT == false && //Tạm thời DP dạng vắcxin k có BHYT
            //                                                                                                                              //x.LaDuocPhamBHYT == yeuCauVo.LaDuocPhamBHYT &&
            //                                                                                                 x.SoLuongNhap > x.SoLuongDaXuat)
            //                                                                                     .Include(p => p.NhapKhoDuocPhams)
            //                                                                                     .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
            //                                                                                     .FirstOrDefaultAsync();

            var tongSoLuongTonTrongKho = Math.Round(lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat), 2);
            if ((tongSoLuongTonTrongKho < 0) || (tongSoLuongTonTrongKho < yeuCauDichVuKyThuat.TiemChung.SoLuong))
            {
                throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
            }

            var soLuongXuatLanNay = Math.Round(yeuCauDichVuKyThuat.TiemChung.SoLuong, 2);
            foreach (var nhapChiTietTheoDuocPham in lstNhapChiTietTheoDuocPham)
            {
                var soLuongDaXuatTrongDot = nhapChiTietVacxin.Where(x => x.NhapKhoDuocPhamChiTietId == nhapChiTietTheoDuocPham.Id)
                                                             .Sum(p => p.SoLuongXuat);

                var soLuongTonTrongKho = Math.Round(nhapChiTietTheoDuocPham.SoLuongNhap - nhapChiTietTheoDuocPham.SoLuongDaXuat - soLuongDaXuatTrongDot, 2);

                if ((soLuongTonTrongKho < 0)) // || (soLuongTonTrongKho < yeuCauDichVuKyThuat.TiemChung.SoLuong))
                {
                    continue;
                }

                // thêm xuất kho dược phẩm chi tiết
                var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = yeuCauDichVuKyThuat.TiemChung.DuocPhamBenhVienId
                };

                var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
                {
                    NhapKhoDuocPhamChiTietId = nhapChiTietTheoDuocPham.Id
                };

                double soLuongXuatTheoNhap = 0;
                if (soLuongXuatLanNay > soLuongTonTrongKho)
                {
                    soLuongXuatTheoNhap = soLuongTonTrongKho;
                    soLuongXuatLanNay = Math.Round(soLuongXuatLanNay - soLuongTonTrongKho, 2);
                }
                else
                {
                    soLuongXuatTheoNhap = soLuongXuatLanNay;
                    soLuongXuatLanNay = 0;
                }

                xuatViTri.SoLuongXuat = soLuongXuatTheoNhap; //yeuCauDichVuKyThuat.TiemChung.SoLuong;
                //nhapChiTietTheoDuocPham.SoLuongDaXuat = Math.Round(nhapChiTietTheoDuocPham.SoLuongDaXuat + yeuCauDichVuKyThuat.TiemChung.SoLuong, 2);

                xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);

                yeuCauDichVuKyThuat.TiemChung.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                yeuCauDichVuKyThuat.TiemChung.HopDongThauDuocPhamId = nhapChiTietTheoDuocPham.HopDongThauDuocPhamId;

                nhapChiTietVacxin.Add(new NhapChiTietVacxinTiemChungVo
                {
                    NhapKhoDuocPhamChiTietId = nhapChiTietTheoDuocPham.Id,
                    NhapKhoDuocPhamChiTiet = nhapChiTietTheoDuocPham,
                    SoLuongXuat = soLuongXuatTheoNhap //yeuCauDichVuKyThuat.TiemChung.SoLuong
                });

                if (soLuongXuatLanNay < 0 || soLuongXuatLanNay.AlmostEqual(0))
                {
                    break;
                }
            }

            if (soLuongXuatLanNay > 0)
            {
                throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
            }
            /***** *****/
        }

        public async Task XuLyXoaSoLuongChiDinhVacxinAsync(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapChiTietVacxinTiemChungVo> nhapChiTietVacxin)
        {
            foreach(var item in yeuCauDichVuKyThuat.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
            {
                var nhapChiTietTheoDuocPham = await _nhapKhoDuocPhamChiTietRepository.TableNoTracking.Where(p => p.Id == item.NhapKhoDuocPhamChiTietId)
                                                                                                     .FirstOrDefaultAsync();

                if(nhapChiTietTheoDuocPham == null)
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                nhapChiTietVacxin.Add(new NhapChiTietVacxinTiemChungVo
                {
                    NhapKhoDuocPhamChiTietId = nhapChiTietTheoDuocPham.Id,
                    NhapKhoDuocPhamChiTiet = nhapChiTietTheoDuocPham,
                    SoLuongXuat = -yeuCauDichVuKyThuat.TiemChung.SoLuong
                });
                //nhapChiTietTheoDuocPham.SoLuongDaXuat = Math.Round(nhapChiTietTheoDuocPham.SoLuongDaXuat - yeuCauDichVuKyThuat.TiemChung.SoLuong, 2);

                //nhapKhoDuocPhamChiTiets.Add(nhapChiTietTheoDuocPham);
            }
            /***** *****/
        }
        #endregion

        #region In
        public async Task<string> InBanKiemTruocTiemChungDoiVoiTreEm(long yeuCauDichVuKyThuatKhamSangLocId, string hosting)
        {
            var today = DateTime.Now;

            var yeuCauDichVuKyThuat = await _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuatKhamSangLocId)
                                                                                          .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.YeuCauNhapVien).ThenInclude(p => p.YeuCauTiepNhanMe).ThenInclude(p => p.NoiTruBenhAn)
                                                                                          .Include(p => p.KhamSangLocTiemChung).ThenInclude(p => p.YeuCauDichVuKyThuats)

                                                                                          //BVHD-3812
                                                                                          .Include(p => p.YeuCauTiepNhan).ThenInclude(p => p.NgheNghiep)
                                                                                          .FirstOrDefaultAsync();

            if(yeuCauDichVuKyThuat == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var template = new Template();

            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentUser = await _nhanVienRepository.GetByIdAsync(currentUserId, o => o.Include(p => p.User).Include(d=>d.HocHamHocVi));

            var currentDateTime = DateTime.Now;

            var quanHeNhanThanBoMe = await _quanHeThanNhanRepository.TableNoTracking.Where(p => p.TenVietTat.Equals("ChaDe") || p.TenVietTat.Equals("MeDe")).ToListAsync();

            var thongTin = JsonConvert.DeserializeObject<BanKiemTruocTiemChungDataVo>(yeuCauDichVuKyThuat.KhamSangLocTiemChung.ThongTinKhamSangLocTiemChungData);
            var phanLoai = JsonConvert.DeserializeObject<PhanLoaiBanKiemTruocTiemChungDataVo>(yeuCauDichVuKyThuat.KhamSangLocTiemChung.ThongTinKhamSangLocTiemChungTemplate);

            var soThangTuoi = CalculateHelper.TinhTongSoThangCuaTuoi(yeuCauDichVuKyThuat.YeuCauTiepNhan.NgaySinh, yeuCauDichVuKyThuat.YeuCauTiepNhan.ThangSinh, yeuCauDichVuKyThuat.YeuCauTiepNhan.NamSinh);

            if (soThangTuoi >= 1)
            {
                if (phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.TrongBenhVien)
                {
                    template = await _templateRepository.TableNoTracking.FirstAsync(p => p.Name.Equals("BanKiemTruocTiemChungTrongBenhVienTren1Thang") && p.IsDisabled != true);
                }
                else if(phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.NgoaiBenhVien)
                {
                    template = await _templateRepository.TableNoTracking.FirstAsync(p => p.Name.Equals("BanKiemTruocTiemChungNgoaiBenhVienTren1Thang") && p.IsDisabled != true);
                }
                else
                {
                    template = await _templateRepository.TableNoTracking.FirstAsync(p => p.Name.Equals("BanKiemTruocTiemChungDoiTuongCovid") && p.IsDisabled != true);
                }
            }
            else
            {
                if (phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.TrongBenhVien)
                {
                    template = await _templateRepository.TableNoTracking.FirstAsync(p => p.Name.Equals("BanKiemTruocTiemChungTrongBenhVienSoSinh") && p.IsDisabled != true);
                }
                else if (phanLoai.NhomKhamSangLoc == NhomKhamSangLoc.NgoaiBenhVien)
                {
                    template = await _templateRepository.TableNoTracking.FirstAsync(p => p.Name.Equals("BanKiemTruocTiemChungNgoaiBenhVienSoSinh") && p.IsDisabled != true);
                }
                else
                {
                    template = await _templateRepository.TableNoTracking.FirstAsync(p => p.Name.Equals("BanKiemTruocTiemChungDoiTuongCovid") && p.IsDisabled != true);
                }
            }

            var thongTinTreSoSinh = new TiemChungThongTinTreSoSinhVo();

            if(!string.IsNullOrEmpty(yeuCauDichVuKyThuat.YeuCauTiepNhan?.YeuCauNhapVien?.YeuCauTiepNhanMe?.NoiTruBenhAn?.ThongTinTongKetBenhAn))
            {
                thongTinTreSoSinh = JsonConvert.DeserializeObject<TiemChungThongTinTreSoSinhVo>(yeuCauDichVuKyThuat.YeuCauTiepNhan.YeuCauNhapVien.YeuCauTiepNhanMe.NoiTruBenhAn.ThongTinTongKetBenhAn);
            }

            var data = new BanKiemTruocTiemChungDoiVoiTreEmVo
            {
                ImageSrc = $"{hosting}/assets/img/logo-bacha-full.png",
                Barcode = BarcodeHelper.GenerateBarCode(yeuCauDichVuKyThuat.YeuCauTiepNhan.MaYeuCauTiepNhan),
                MaTiepNhan = yeuCauDichVuKyThuat.YeuCauTiepNhan.MaYeuCauTiepNhan,
                HoTen = yeuCauDichVuKyThuat.YeuCauTiepNhan.HoTen,
                GioiTinhNam = yeuCauDichVuKyThuat.YeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNam ? "X" : "&nbsp;",
                GioiTinhNu = yeuCauDichVuKyThuat.YeuCauTiepNhan.GioiTinh == LoaiGioiTinh.GioiTinhNu ? "X" : "&nbsp;",
                NgaySinh = yeuCauDichVuKyThuat.YeuCauTiepNhan.NgaySinh,
                ThangSinh = yeuCauDichVuKyThuat.YeuCauTiepNhan.ThangSinh,
                NamSinh = yeuCauDichVuKyThuat.YeuCauTiepNhan.NamSinh,
                GioSinh = thongTinTreSoSinh.DacDiemTreSoSinhs.Any(p => p.YeuCauTiepNhanConId == yeuCauDichVuKyThuat.YeuCauTiepNhanId && p.DeLuc != null) ? thongTinTreSoSinh.DacDiemTreSoSinhs.First(p => p.YeuCauTiepNhanConId == yeuCauDichVuKyThuat.YeuCauTiepNhanId).DeLuc.Value.Hour : (int?)null,
                PhutSinh = thongTinTreSoSinh.DacDiemTreSoSinhs.Any(p => p.YeuCauTiepNhanConId == yeuCauDichVuKyThuat.YeuCauTiepNhanId && p.DeLuc != null) ? thongTinTreSoSinh.DacDiemTreSoSinhs.First(p => p.YeuCauTiepNhanConId == yeuCauDichVuKyThuat.YeuCauTiepNhanId).DeLuc.Value.Minute : (int?)null,
                Tuoi = CalculateHelper.TinhTuoi(yeuCauDichVuKyThuat.YeuCauTiepNhan.NgaySinh, yeuCauDichVuKyThuat.YeuCauTiepNhan.ThangSinh, yeuCauDichVuKyThuat.YeuCauTiepNhan.NamSinh),
                DiaChi = yeuCauDichVuKyThuat.YeuCauTiepNhan.DiaChiDayDu,
                HoTenBoMe = quanHeNhanThanBoMe.Any(p => p.Id == yeuCauDichVuKyThuat.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId) ? yeuCauDichVuKyThuat.YeuCauTiepNhan.NguoiLienHeHoTen : "&nbsp;",
                DienThoaiBoMe = quanHeNhanThanBoMe.Any(p => p.Id == yeuCauDichVuKyThuat.YeuCauTiepNhan.NguoiLienHeQuanHeNhanThanId) ? yeuCauDichVuKyThuat.YeuCauTiepNhan.NguoiLienHeSoDienThoai : "&nbsp;",

                CanNang = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "CanNang") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "CanNang").First().Value : "&nbsp;",
                ThanNhiet = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "ThanNhiet") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "ThanNhiet").First().Value : "&nbsp;",
                TuoiThaiKhiSinh = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "TuoiThaiKhiSinh") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "TuoiThaiKhiSinh").First().Value : "&nbsp;",
                GroupXetNghiemHbsAgKhong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "GroupXetNghiemHbsAgKhong" && p.Value == "true") ? "X" : "&nbsp;",
                GroupXetNghiemHbsAgCo = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "GroupXetNghiemHbsAgCo" && p.Value == "true") ? "X" : "&nbsp;",
                GroupKetQuaHbsAgCoChild = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "GroupKetQuaHbsAgCoChild" && p.Value == "true") ? "X" : "&nbsp;",
                GroupKetQuaHbsAgKhongChild = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "GroupKetQuaHbsAgKhongChild" && p.Value == "true") ? "X" : "&nbsp;",

                Group1Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group1Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group1Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group1Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group2Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group2Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group2Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group2Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group3Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group3Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group3Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group3Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group4Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group4Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group4Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group4Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group5Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group5Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group5Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group5Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group6Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group6Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group6Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group6Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group7Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group7Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group7Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group7Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group8Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group8Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group8Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group8Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group8Text = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group8Text") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "Group8Text").First().Value : "&nbsp;",
                Group9Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9Co" && p.Value == "true") ? "X" : "&nbsp;",
                Group9Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group9Text = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9Text") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "Group9Text").First().Value : "&nbsp;",

                GroupKhamSangLocChuyenKhoaKhong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "GroupKhamSangLocChuyenKhoaKhong" && p.Value == "true") ? "X" : "&nbsp;",
                GroupKhamSangLocChuyenKhoaCo = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "GroupKhamSangLocChuyenKhoaCo" && p.Value == "true") ? "X" : "&nbsp;",
                GroupChuyenKhoaText = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "GroupChuyenKhoaText") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "GroupChuyenKhoaText").First().Value : "&nbsp;",
                LyDo = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "LyDo") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "LyDo").First().Value : "&nbsp;",
                KetQua = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "KetQua") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "KetQua").First().Value : "&nbsp;",
                KetLuan = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "KetLuan") ? thongTin.DataKhamTheoTemplate.Where(p => p.Id == "KetLuan").First().Value : "&nbsp;",
                
                TenVacxin = yeuCauDichVuKyThuat.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any() ? string.Join("; ", yeuCauDichVuKyThuat.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Select(o => o.TenDichVu).GroupBy(o => o).Select(o => o.Key).ToList()) : "&nbsp;",
                KhongCoBatThuong = yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan == LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem ? "X" : "&nbsp;",
                ChongChiDinh = yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan == LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh ? "X" : "&nbsp;",
                TamHoan = yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan == LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung ? "X" : "&nbsp;",

                CoKhamSangLoc = yeuCauDichVuKyThuat.KhamSangLocTiemChung.BenhNhanDeNghi == true ? "X" : "&nbsp;",
                KhongKhamSangLoc = yeuCauDichVuKyThuat.KhamSangLocTiemChung.BenhNhanDeNghi == false ? "X" : "&nbsp;",
                LyDoKhamSangLoc = yeuCauDichVuKyThuat.KhamSangLocTiemChung.LyDoDeNghi,

                Hoi = $"{currentDateTime.ToString("HH")} giờ {currentDateTime.Minute} phút",
                NgayThangHienTai = $"ngày {currentDateTime.Day} tháng {currentDateTime.Month} năm {currentDateTime.Year}",
                HoTenBacSi = currentUser.HocHamHocVi != null ? !string.IsNullOrEmpty(currentUser.HocHamHocVi.Ma) ? currentUser.HocHamHocVi.Ma + " " + currentUser.User.HoTen :  currentUser.User.HoTen :  currentUser.User.HoTen,

                //BVHD-3812
                CanCuocCongDan = yeuCauDichVuKyThuat.YeuCauTiepNhan.SoChungMinhThu,
                NgheNghiep = yeuCauDichVuKyThuat.YeuCauTiepNhan.NgheNghiep?.Ten,
                SoDienThoai = yeuCauDichVuKyThuat.YeuCauTiepNhan.SoDienThoai,
                DonViCongTac = yeuCauDichVuKyThuat.YeuCauTiepNhan.NoiLamViec,

                DaTiemMui1GroupKhong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupKhong" && p.Value == "true") ? "X" : "&nbsp;",
                DaTiemMui1GroupCo1 = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo1" && p.Value == "true") ? "X" : "&nbsp;",
                DaTiemMui1GroupCo1Text = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo1Text") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "DaTiemMui1GroupCo1Text").Value : "&nbsp;",
                DaTiemMui1GroupCo1DateValue = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo1Date") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "DaTiemMui1GroupCo1Date").Value : "",

                DaTiemMui1GroupCo2 = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo2" && p.Value == "true") ? "X" : "&nbsp;",
                DaTiemMui1GroupCo2Text = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo2Text") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "DaTiemMui1GroupCo2Text").Value : "&nbsp;",
                DaTiemMui1GroupCo2DateValue = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo2Date") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "DaTiemMui1GroupCo2Date").Value : "",

                DaTiemMui1GroupCo3 = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo3" && p.Value == "true") ? "X" : "&nbsp;",
                DaTiemMui1GroupCo3Text = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo3Text") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "DaTiemMui1GroupCo3Text").Value : "&nbsp;",
                DaTiemMui1GroupCo3DateValue = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "DaTiemMui1GroupCo3Date") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "DaTiemMui1GroupCo3Date").Value : "",

                Group313TuanKhong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group313TuanKhong" && p.Value == "true") ? "X" : "&nbsp;",
                Group313TuanCo = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group313TuanCo" && p.Value == "true") ? "X" : "&nbsp;",
                Group3Hon13TuanKhong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group3Hon13TuanKhong" && p.Value == "true") ? "X" : "&nbsp;",
                Group3Hon13TuanCo = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group3Hon13TuanCo" && p.Value == "true") ? "X" : "&nbsp;",
                Group4Text = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group4Text") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "Group4Text").Value : "&nbsp;",
                
                Group9NhietDo = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9NhietDo") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "Group9NhietDo").Value : "&nbsp;",
                Group9Mach = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9Mach") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "Group9Mach").Value : "&nbsp;",
                Group9HuyetAp = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9HuyetAp") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "Group9HuyetAp").Value : "&nbsp;",
                Group9NhipTho = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group9NhipTho") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "Group9NhipTho").Value : "&nbsp;",

                Group10Text = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group10Text") ? thongTin.DataKhamTheoTemplate.First(p => p.Id == "Group10Text").Value : "&nbsp;",
                Group10Khong = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group10Khong" && p.Value == "true") ? "X" : "&nbsp;",
                Group10Co = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group10Co" && p.Value == "true") ? "X" : "&nbsp;",

                KhongCoBatThuongTiemCovid = yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan == LoaiKetLuanKhamSangLocTiemChung.DuDieuKienTiem ? "X" : "&nbsp;",
                ChongChiDinhTiemCovid = yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan == LoaiKetLuanKhamSangLocTiemChung.ChongChiDinh ? "X" : "&nbsp;",
                TamHoanTiemCovid = yeuCauDichVuKyThuat.KhamSangLocTiemChung.KetLuan == LoaiKetLuanKhamSangLocTiemChung.TamHoanTiemChung ? "X" : "&nbsp;",
                                    //thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group2Co" && p.Value == "true") 
                                    //                                      || (p.Id == "Group3Co" && p.Value == "true") 
                                    //                                      || (p.Id == "Group413TuanCo" && p.Value == "true")) ? "X" : "&nbsp;",
                ChiDinhCoSoYTeCoDieuKienTiemCovid = thongTin.DataKhamTheoTemplate.Any(p => p.Id == "Group4Co" && p.Value == "true") ? "X" : "&nbsp;",
                NhomThanTrongTiemCovid = thongTin.DataKhamTheoTemplate.Any(p => (p.Id == "Group3Hon13TuanCo" && p.Value == "true")
                                                                                || (p.Id == "Group5Co" && p.Value == "true")
                                                                                || (p.Id == "Group6Co" && p.Value == "true")
                                                                                || (p.Id == "Group7Co" && p.Value == "true")
                                                                                || (p.Id == "Group8Co" && p.Value == "true")
                                                                                || (p.Id == "Group9Co" && p.Value == "true")) ? "X" : "&nbsp;",
            };

            if ((data.NgaySinh != null && data.NgaySinh.Value != 0) && (data.ThangSinh != null && data.ThangSinh.Value != 0) && (data.NamSinh != null && data.NamSinh.Value != 0))
            {
                data.SinhNgay = $"sinh ngày {data.NgaySinh} tháng {data.ThangSinh} năm {data.NamSinh}";
            }
            else
            {
                data.SinhNgay = $"sinh năm {data.NamSinh}";
            }

            var content = TemplateHelpper.FormatTemplateWithContentTemplate(template.Body, data);

            return content;
        }
        #endregion

        #region Gói dịch vụ
        public async Task<List<TiemChungChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>> GetThongTinSuDungDichVuGiuongTrongGoiAsync(long benhNhanId)
        {
            var yeuCauTiepNhans = BaseRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.KhoaPhong)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.PhongBenhVien)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.GiuongBenh)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.KhoaPhong)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.PhongBenhVien)
                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.GiuongBenh)
                .Include(o => o.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.NoiChiDinh).ThenInclude(gb => gb.KhoaPhong)
                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong)
                .Where(x => x.BenhNhanId == benhNhanId
                            && x.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DaHuy)
                .ToList();
            var lstSuDung = new List<TiemChungChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>();
            foreach (var yeuCauTiepNhan in yeuCauTiepNhans)
            {
                if (yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)
                {
                    var chiPhiGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
                    lstSuDung.AddRange(chiPhiGiuong.Item1
                        .Where(x => x.YeuCauGoiDichVuId != null)
                        .Select(s => new TiemChungChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo
                        {
                            YeuCauGoiDichVuId = s.YeuCauGoiDichVuId.Value,
                            DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                            NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                            SoLuongDaSuDung = s.SoLuong
                        }).ToList());
                }
                else
                {
                    lstSuDung.AddRange(yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens
                        .Where(x => x.YeuCauGoiDichVuId != null)
                        .Select(s => new TiemChungChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo
                        {
                            YeuCauGoiDichVuId = s.YeuCauGoiDichVuId.Value,
                            DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                            NhomGiaDichVuBenhVienId = s.NhomGiaDichVuGiuongBenhVienId,
                            SoLuongDaSuDung = s.SoLuong
                        }).ToList());
                }
            }

            return lstSuDung;
        }

        public async Task<GridDataSource> GetGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo)
        {
            //Có cập nhật bỏ await
            BuildDefaultSortExpression(queryInfo);

            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var arrParam = queryInfo.AdditionalSearchString.Split(";");
            var benhNhanId = long.Parse(arrParam[0]);
            bool isCapGiuong = arrParam[1].ToLower() == "true";

            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId != null)
                .ToList();

            var query = _yeuCauGoiDichVuRepository.TableNoTracking.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                                                                  .Include(x => x.YeuCauDichVuKyThuats)
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && x.DaQuyetToan != true // cập nhật 10/06/2021: ko hiển thị gói đã quyết toán
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                    )
                .ToList();

            var result = query.Where(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(a => a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id &&
                                                                                                                                                          b.DichVuKyThuatBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan) &&
                                                                                                                                                          nhomTiemChungId != null &&
                                                                                                                                                          a.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId))
                              .Select(item => new GoiDichVuTheoBenhNhanGridVo()
                              {
                                  Id = item.Id,
                                  // cập nhật 25/05/2021: chỉ hiện tên chương trình
                                  TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten, // + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                                  TongCong = item.ChuongTrinhGoiDichVu.GiaTruocChietKhau,
                                  GiaGoi = item.ChuongTrinhGoiDichVu.GiaSauChietKhau,
                                  BenhNhanDaThanhToan = item.SoTienBenhNhanDaChi ?? 0,
                                  DangDung = item.YeuCauKhamBenhs.Where(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                             // cập nhật: tính tổng dv đã sử dụng trong gói -> bệnh nhân có cần đóng thêm tiền ko
                                                                             //&& a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                                                                             && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.DonGiaSauChietKhau ?? 0)
                                             + item.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                    //&& a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                                                                                    && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0))
                                              + item.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                                                                            //&& a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                                                                                            && a.YeuCauGoiDichVuId == item.Id).Sum(a => a.DonGiaSauChietKhau ?? 0),

                                  //BVHD-3268
                                  GiaGoiDichVuTiemChung = item.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Where(a => nhomTiemChungId != null
                                                                                                                                  && a.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId)
                                      .Sum(a => a.SoLan * a.DonGiaSauChietKhau)
                              })
                              .ToArray();

            return new GridDataSource { Data = result, TotalRowCount = result.Length };
        }
        public async Task<GridDataSource> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //Có cập nhật bỏ await
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var arrParam = queryInfo.AdditionalSearchString.Split(";");
            var benhNhanId = long.Parse(arrParam[0]);
            bool isCapGiuong = arrParam[1].ToLower() == "true";

            var dichVuKyThuatDaChiDinhs = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId != null)
                .ToList();

            var query = _yeuCauGoiDichVuRepository.TableNoTracking.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(y => y.DichVuKyThuatBenhVien)
                                                                  .Include(x => x.YeuCauDichVuKyThuats)
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai == EnumTrangThaiYeuCauGoiDichVu.DangThucHien
                            && x.DaQuyetToan != true // cập nhật 10/06/2021: ko hiển thị gói đã quyết toán
                            && x.NgungSuDung != true // cập nhật 26/11/2021: ko hiển thị gói đã ngưng sử dụng
                    )
                .ToList();

            var result = query.Where(x => x.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(a => a.SoLan > dichVuKyThuatDaChiDinhs.Where(b => b.YeuCauGoiDichVuId == x.Id &&
                                                                                                                                                          b.DichVuKyThuatBenhVienId == a.DichVuKyThuatBenhVienId).Sum(b => b.SoLan) &&
                                                                                                                                                          nhomTiemChungId != null &&
                                                                                                                                                          a.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId))
                              .Select(item => new GoiDichVuTheoBenhNhanGridVo()
                              {
                                  Id = item.Id,
                                  TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten,// + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu
                              })
                              .ToArray();

            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = result.Length };
        }

        public async Task<TiemChungGridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false, List<TiemChungChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null)
        {
            BuildDefaultSortExpression(queryInfo);

            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;

            var lstDichVuDangChon = new List<TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanVo>();
            if (!string.IsNullOrEmpty(searchObj[1]) && searchObj[1] != "undefined" && searchObj[1] != "null")
            {
                lstDichVuDangChon = JsonConvert.DeserializeObject<List<TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanVo>>(searchObj[1]);
            }

            var dichVuKyThuatDaChiDinhs = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                .ToListAsync();

            var isPTTT = searchObj[2].ToLower() == "true";
            var isCapGiuong = searchObj[3].ToLower() == "true";

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var query = _chuongTrinhGoiDichVuKyThuatRepository.TableNoTracking
                        //.Where(x => (!isCapGiuong && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId) || (isCapGiuong && x.ChuongTrinhGoiDichVuId == 0))// cheat điều kiện, nếu là cấp giường thì ko hiện dv khác dv giường
                        .Where(x => nhomTiemChungId != null && x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId)
                        .ApplyLike(queryInfo.SearchTerms, x => x.DichVuKyThuatBenhVien.Ma, x => x.DichVuKyThuatBenhVien.Ten)
                        .Select(item => new TiemChungChiTietGoiDichVuTheoBenhNhanGridVo()
                        {
                            YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                            TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                            ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                            ChuongTrinhGoiDichVuChiTietId = item.Id,
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                            TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            SoLuong = item.SoLan,
                            DonGia = item.DonGiaSauChietKhau,
                            SoLuongDaDung = dichVuKyThuatDaChiDinhs.Where(a => a.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId).Sum(b => b.SoLan),
                            SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                                       && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung : 0,
                            IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat),
                            IsPTTT = isPTTT,
                            IsDieuTriNoiTru = isDieuTriNoiTru,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                            IsNhomTiemChung = nhomTiemChungId == null ? false : item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                        });

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString)
                .Skip(queryInfo.Skip).Take(queryInfo.Take)
                .ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new TiemChungGridDataSourceChiTietGoiDichVuTheoBenhNhan { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
        public async Task<TiemChungGridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false)
        {
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;

            var searchObj = queryInfo.AdditionalSearchString.Split(';');
            var yeuCauGoiDichVuId = long.Parse(searchObj[0]);
            var yeuCauGoiDichVu = await _yeuCauGoiDichVuRepository.TableNoTracking.FirstAsync(x => x.Id == yeuCauGoiDichVuId);
            var benhNhanId = yeuCauGoiDichVu.GoiSoSinh == true ? yeuCauGoiDichVu.BenhNhanSoSinhId : yeuCauGoiDichVu.BenhNhanId;

            var lstDichVuDangChon = new List<TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanVo>();
            if (!string.IsNullOrEmpty(searchObj[1]) && searchObj[1] != "undefined" && searchObj[1] != "null")
            {
                lstDichVuDangChon = JsonConvert.DeserializeObject<List<TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanVo>>(searchObj[1]);
            }

            var dichVuKyThuatDaChiDinhs = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhan.BenhNhanId == benhNhanId //yeuCauGoiDichVu.BenhNhanId
                            && x.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.YeuCauGoiDichVuId == yeuCauGoiDichVuId)
                .ToListAsync();

            var isPTTT = searchObj[2].ToLower() == "true";
            var isCapGiuong = searchObj[3].ToLower() == "true";

            var lstNhomDichVuBenhVien = await _nhomDichVuBenhVienRepository.TableNoTracking.ToListAsync();

            var query = _chuongTrinhGoiDichVuKyThuatRepository.TableNoTracking
                        //.Where(x => (!isCapGiuong && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId) || (isCapGiuong && x.ChuongTrinhGoiDichVuId == 0))// cheat điều kiện, nếu là cấp giường thì ko hiện dv khác dv giường
                        .Where(x => nhomTiemChungId != null && x.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId && x.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId)
                        .ApplyLike(queryInfo.SearchTerms, x => x.DichVuKyThuatBenhVien.Ma, x => x.DichVuKyThuatBenhVien.Ten)
                        .Select(item => new TiemChungChiTietGoiDichVuTheoBenhNhanGridVo()
                        {
                            YeuCauGoiDichVuId = yeuCauGoiDichVuId,
                            TenGoiDichVu = item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu,
                            ChuongTrinhGoiDichVuId = yeuCauGoiDichVu.ChuongTrinhGoiDichVuId,
                            ChuongTrinhGoiDichVuChiTietId = item.Id,
                            DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                            MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                            TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                            TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                            SoLuong = item.SoLan,
                            DonGia = item.DonGiaSauChietKhau,
                            SoLuongDaDung = dichVuKyThuatDaChiDinhs.Where(a => a.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId).Sum(b => b.SoLan),
                            SoLuongDungLanNay = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                           && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                           && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat) ? lstDichVuDangChon.FirstOrDefault(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                                                                                                                       && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                                                                                                                       && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung : 0,
                            IsChecked = lstDichVuDangChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuId == yeuCauGoiDichVu.ChuongTrinhGoiDichVuId
                                                                   && a.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                                   && a.NhomGoiDichVu == (int)EnumNhomGoiDichVu.DichVuKyThuat),
                            IsPTTT = isPTTT,
                            IsDieuTriNoiTru = isDieuTriNoiTru,
                            LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                            IsNhomTiemChung = nhomTiemChungId == null ? false : item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId == nhomTiemChungId
                        });
            //.ApplyLike(queryInfo.SearchTerms, x => x.MaDichVu, x => x.TenDichVu);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new TiemChungGridDataSourceChiTietGoiDichVuTheoBenhNhan { TotalRowCount = countTask.Result };
        }

        public async Task XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(YeuCauTiepNhan yeuCauTiepNhan, TiemChungChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo)
        {
            //todo: có cập nhật bỏ await
            var coBHYT = yeuCauTiepNhan.CoBHYT ?? false;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thoiDiemHienTai = DateTime.Now;
            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();
            var lstDichVuDaChon = yeuCauVo.DichVus;

            var lstDichVuKhamBenhTrongGoi = new List<TiemChungDichVuBenhVienTheoGoiDichVuVo>();

            //có cập nhật: bỏ await
            var lstYeuCauGoiDichVuIds = lstDichVuDaChon.Select(x => x.YeuCauGoiDichVuId).ToList();
            var yeuCauGoiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.DichVuKyThuat)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.YeuCauDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats).ThenInclude(z => z.DichVuKyThuatBenhVien).ThenInclude(t => t.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(x => x.YeuCauDichVuKyThuats)
                .Where(x => lstYeuCauGoiDichVuIds.Contains(x.Id)) //lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == x.Id))
                .ToList();

            foreach (var yeuCauGoiDichVu in yeuCauGoiDichVus)
            {
                // dịch vụ kỹ thuật
                lstDichVuKhamBenhTrongGoi.AddRange(yeuCauGoiDichVu.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats
                    .Where(x => lstDichVuDaChon.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                         && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                         && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                         && a.DichVuBenhVienId == x.DichVuKyThuatBenhVienId
                                                         && a.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat)
                                && !yeuCauVo.DichVuKhongThems.Any(a => a.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                                        && a.ChuongTrinhGoiDichVuId == x.ChuongTrinhGoiDichVuId
                                                                        && a.ChuongTrinhGoiDichVuChiTietId == x.Id
                                                                        && a.DichVuId == x.DichVuKyThuatBenhVienId
                                                                        && a.NhomGoiDichVuValue == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat))
                    .Select(item => new TiemChungDichVuBenhVienTheoGoiDichVuVo()
                    {
                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuKyThuat,
                        DichVuBenhVienId = item.DichVuKyThuatBenhVienId,
                        MaDichVu = item.DichVuKyThuatBenhVien.Ma,
                        TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                        MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia : "",
                        TenGia = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.TenGia : "",
                        Ma4350 = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350 : "",
                        NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                        DonGiaBenhVien = item.DonGia,
                        DonGiaTruocChietKhau = item.DonGiaTruocChietKhau,
                        DonGiaSauChietKhau = item.DonGiaSauChietKhau,
                        DonGiaBaoHiem = item.DichVuKyThuatBenhVien
                                        .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                              && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                    item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                    && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).Gia : (decimal?)null,
                        CoBHYT = coBHYT,
                        SoLuong = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                             && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                             && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                             && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                             && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).SoLuongSuDung,
                        TiLeBaoHiemThanhToan = item.DichVuKyThuatBenhVien
                                               .DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                     && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)) != null ?
                                            item.DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(a => a.TuNgay.Date <= DateTime.Now.Date
                                                                                                                            && (a.DenNgay == null || a.DenNgay.Value.Date >= DateTime.Now.Date)).TiLeBaoHiemThanhToan : (int?)null,
                        NhomChiPhiDichVuKyThuat = item.DichVuKyThuatBenhVien.DichVuKyThuat != null ? item.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi : Enums.EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe,
                        NhomDichVuBenhVienId = item.DichVuKyThuatBenhVien.NhomDichVuBenhVienId,
                        YeuCauGoiDichVuId = yeuCauGoiDichVu.Id,
                        SoLanDaSuDung = yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                             && a.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId).Sum(a => a.SoLan),
                        SoLanTheoGoi = item.SoLan,
                        NoiThucHienId = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                            && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                            && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                            && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                            && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).NoiThucHienId.GetValueOrDefault(),
                        ViTriTiem = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                            && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                            && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                            && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                            && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).ViTriTiem.GetValueOrDefault(),
                        MuiSo = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                            && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                            && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                            && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                            && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).MuiSo.GetValueOrDefault(),
                        LieuLuong = lstDichVuDaChon.FirstOrDefault(b => b.YeuCauGoiDichVuId == yeuCauGoiDichVu.Id
                                                            && b.ChuongTrinhGoiDichVuId == item.ChuongTrinhGoiDichVuId
                                                            && b.ChuongTrinhGoiDichVuChiTietId == item.Id
                                                            && b.DichVuBenhVienId == item.DichVuKyThuatBenhVienId
                                                            && b.NhomGoiDichVu == (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat).LieuLuong,
                    }));
            }

            if (!lstDichVuKhamBenhTrongGoi.Any())
            {
                throw new Exception(_localizationService.GetResource("ChiDihNhomDichVuThuongDung.DichVu.Required"));
            }

            var bacSiDangKys = await _hoatDongNhanVienRepository.TableNoTracking
                            .Where(x => x.NhanVien.ChucDanh.NhomChucDanhId == (long)Enums.EnumNhomChucDanh.BacSi
                                        && x.NhanVien.User.IsActive).ToListAsync();

            //var noiThucHienDVKTs = await _dichVuKyThuatBenhVienRepository.TableNoTracking
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
            //    .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.KhoaPhong).ThenInclude(z => z.PhongBenhViens)
            //    .ToListAsync();

            foreach (var dichVuBenhVien in lstDichVuKhamBenhTrongGoi)
            {
                switch (dichVuBenhVien.NhomGoiDichVu)
                {
                    case EnumNhomGoiDichVu.DichVuKyThuat:
                        if (dichVuBenhVien.SoLanConLai < dichVuBenhVien.SoLuong)
                        {
                            throw new Exception(string.Format(_localizationService.GetResource("ChiDinhGoiDichVuMarketing.SoLuongConLai.NotEnough"), dichVuBenhVien.TenDichVu));
                        }

                        var bacSiDangKyDVKT = bacSiDangKys.FirstOrDefault(x => x.PhongBenhVienId == dichVuBenhVien.NoiThucHienId);
                        var noiThucHien = await _phongBenhVienRepository.GetByIdAsync(dichVuBenhVien.NoiThucHienId);
                        var noiChiDinh = await _phongBenhVienRepository.GetByIdAsync(phongHienTaiId);

                        var nhanVienChiDinh = await _nhanVienRepository.GetByIdAsync(currentUserId, o => o.Include(p => p.User));

                        var dichVuKyThuatBenhVien = _dichVuKyThuatBenhVienRepository.GetById(dichVuBenhVien.DichVuBenhVienId);

                        for (int i = 1; i <= dichVuBenhVien.SoLuong; i++)
                        {
                            var entityYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                            {
                                DichVuKyThuatBenhVienId = dichVuBenhVien.DichVuBenhVienId,
                                DichVuKyThuatBenhVien = dichVuKyThuatBenhVien,
                                NhomDichVuBenhVienId = dichVuBenhVien.NhomDichVuBenhVienId,
                                NoiThucHienId = dichVuBenhVien.NoiThucHienId,
                                NoiThucHien = noiThucHien,
                                YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                                YeuCauKhamBenhId = yeuCauVo.YeuCauKhamBenhId,
                                MaDichVu = dichVuBenhVien.MaDichVu,
                                TenDichVu = dichVuBenhVien.TenDichVu,
                                Gia = dichVuBenhVien.DonGiaBenhVien.Value,
                                NhomGiaDichVuKyThuatBenhVienId = dichVuBenhVien.NhomGiaDichVuBenhVienId,
                                NhomChiPhi = dichVuBenhVien.NhomChiPhiDichVuKyThuat,
                                SoLan = 1,
                                //SoLan = dichVuBenhVien.SoLuong,
                                TiLeUuDai = null,
                                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                                TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien,
                                NhanVienChiDinhId = currentUserId,
                                NhanVienChiDinh = nhanVienChiDinh,
                                NhanVienThucHienId = bacSiDangKyDVKT?.NhanVienId,
                                NhanVienThucHien = bacSiDangKyDVKT != null ? await _nhanVienRepository.GetByIdAsync(bacSiDangKyDVKT.NhanVienId, o => o.Include(p => p.User)) : null,
                                NoiChiDinhId = phongHienTaiId,
                                NoiChiDinh = noiChiDinh,
                                ThoiDiemChiDinh = DateTime.Now,
                                ThoiDiemDangKy = DateTime.Now,
                                LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(dichVuBenhVien.NhomDichVuBenhVienId, lstNhomDichVuBenhVien),
                                MaGiaDichVu = dichVuBenhVien.MaGiaDichVu,
                                TenGiaDichVu = dichVuBenhVien.TenGia,
                                Ma4350DichVu = dichVuBenhVien.Ma4350,
                                DonGiaTruocChietKhau = dichVuBenhVien.DonGiaTruocChietKhau,
                                DonGiaSauChietKhau = dichVuBenhVien.DonGiaSauChietKhau,
                                DonGiaBaoHiem = dichVuBenhVien.DonGiaBaoHiem,
                                DuocHuongBaoHiem = dichVuBenhVien.DuocHuongBaoHiem,
                                TiLeBaoHiemThanhToan = dichVuBenhVien.TiLeBaoHiemThanhToan,
                                BaoHiemChiTra = null,
                                YeuCauGoiDichVuId = dichVuBenhVien.YeuCauGoiDichVuId,
                                //YeuCauDichVuKyThuatKhamSangLocTiemChungId = yeuCauVo.YeuCauDichVuKyThuatKhamSangLocTiemChungId
                            };
                            /* */

                            // trường hợp chỉ định trong nội trú
                            if (yeuCauVo.NoiTruPhieuDieuTriId != null)
                            {
                                var ngayDieuTri = yeuCauTiepNhan.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.NoiTruPhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;
                                var newThoiDiemDangKy = ngayDieuTri.Date == thoiDiemHienTai.Date ? thoiDiemHienTai : ngayDieuTri; //ngayDieuTri.Date.AddSeconds(thoiDiemHienTai.Hour * 3600 + thoiDiemHienTai.Minute * 60);
                                entityYeuCauDichVuKyThuat.ThoiDiemDangKy = newThoiDiemDangKy;
                                entityYeuCauDichVuKyThuat.NoiTruPhieuDieuTriId = yeuCauVo.NoiTruPhieuDieuTriId;
                            }

                            /* Thêm yêu cầu dịch vụ kỹ thuật tiêm chủng (dược phẩm) */
                            var duocPhamBenhVienTiemChung = _dichVuKyThuatBenhVienRepository.TableNoTracking.Where(p => p.Id == dichVuBenhVien.DichVuBenhVienId)
                                                                                                                  .SelectMany(p => p.DichVuKyThuatBenhVienTiemChungs)
                                                                                                                  .LastOrDefault();

                            if(duocPhamBenhVienTiemChung == null)
                            {
                                throw new Exception(_localizationService.GetResource("TiemChung.DichVuKyThuatBenhVienTiemChung.DichVuKyThuatBenhVien.Null"));
                            }

                            var duocPhamBenhVien = await _duocPhamBenhVienRepository.GetByIdAsync(duocPhamBenhVienTiemChung.DuocPhamBenhVienId, o => o.Include(p => p.DuocPham));

                            var newYeuCauDichVuKyThuatTiemChung = new YeuCauDichVuKyThuatTiemChung();
                            newYeuCauDichVuKyThuatTiemChung.DuocPhamBenhVienId = duocPhamBenhVien.Id;
                            newYeuCauDichVuKyThuatTiemChung.TenDuocPham = duocPhamBenhVien.DuocPham.Ten;
                            newYeuCauDichVuKyThuatTiemChung.TenDuocPhamTiengAnh = duocPhamBenhVien.DuocPham.TenTiengAnh;
                            newYeuCauDichVuKyThuatTiemChung.SoDangKy = duocPhamBenhVien.DuocPham.SoDangKy;
                            newYeuCauDichVuKyThuatTiemChung.STTHoatChat = duocPhamBenhVien.DuocPham.STTHoatChat;
                            newYeuCauDichVuKyThuatTiemChung.MaHoatChat = duocPhamBenhVien.DuocPham.MaHoatChat;
                            newYeuCauDichVuKyThuatTiemChung.HoatChat = duocPhamBenhVien.DuocPham.HoatChat;
                            newYeuCauDichVuKyThuatTiemChung.LoaiThuocHoacHoatChat = duocPhamBenhVien.DuocPham.LoaiThuocHoacHoatChat;
                            newYeuCauDichVuKyThuatTiemChung.NhaSanXuat = duocPhamBenhVien.DuocPham.NhaSanXuat;
                            newYeuCauDichVuKyThuatTiemChung.NuocSanXuat = duocPhamBenhVien.DuocPham.NuocSanXuat;
                            newYeuCauDichVuKyThuatTiemChung.DuongDungId = duocPhamBenhVien.DuocPham.DuongDungId;
                            newYeuCauDichVuKyThuatTiemChung.HamLuong = duocPhamBenhVien.DuocPham.HamLuong;
                            newYeuCauDichVuKyThuatTiemChung.QuyCach = duocPhamBenhVien.DuocPham.QuyCach;
                            newYeuCauDichVuKyThuatTiemChung.TieuChuan = duocPhamBenhVien.DuocPham.TieuChuan;
                            newYeuCauDichVuKyThuatTiemChung.DangBaoChe = duocPhamBenhVien.DuocPham.DangBaoChe;
                            newYeuCauDichVuKyThuatTiemChung.DonViTinhId = duocPhamBenhVien.DuocPham.DonViTinhId;
                            newYeuCauDichVuKyThuatTiemChung.HuongDan = duocPhamBenhVien.DuocPham.HuongDan;
                            newYeuCauDichVuKyThuatTiemChung.MoTa = duocPhamBenhVien.DuocPham.MoTa;
                            newYeuCauDichVuKyThuatTiemChung.ChiDinh = duocPhamBenhVien.DuocPham.ChiDinh;
                            newYeuCauDichVuKyThuatTiemChung.ChongChiDinh = duocPhamBenhVien.DuocPham.ChongChiDinh;
                            newYeuCauDichVuKyThuatTiemChung.LieuLuongCachDung = duocPhamBenhVien.DuocPham.LieuLuongCachDung;
                            newYeuCauDichVuKyThuatTiemChung.TacDungPhu = duocPhamBenhVien.DuocPham.TacDungPhu;
                            newYeuCauDichVuKyThuatTiemChung.ChuYdePhong = duocPhamBenhVien.DuocPham.ChuYDePhong;
                            newYeuCauDichVuKyThuatTiemChung.ViTriTiem = dichVuBenhVien.ViTriTiem;
                            newYeuCauDichVuKyThuatTiemChung.MuiSo = dichVuBenhVien.MuiSo;
                            newYeuCauDichVuKyThuatTiemChung.TrangThaiTiemChung = TrangThaiTiemChung.ChuaTiemChung;
                            newYeuCauDichVuKyThuatTiemChung.LieuLuong = dichVuBenhVien.LieuLuong;

                            /***** Xử lý số lượng *****/
                            newYeuCauDichVuKyThuatTiemChung.SoLuong = 1;
                            entityYeuCauDichVuKyThuat.TiemChung = newYeuCauDichVuKyThuatTiemChung;
                            /* */

                            yeuCauVo.YeuCauDichVuKyThuatNews.Add(entityYeuCauDichVuKyThuat);
                            yeuCauTiepNhan.YeuCauDichVuKyThuats.Add(entityYeuCauDichVuKyThuat);
                        }
                        
                        break;
                }
            }

            #region kiểm tra có gói nào chỉ định dịch vụ vượt quá số tiền bảo lãnh còn lại
            var lstTongChiTheoGoiDichVu = lstDichVuKhamBenhTrongGoi
                .GroupBy(x => new { x.YeuCauGoiDichVuId })
                .Select(x => new { x.First().YeuCauGoiDichVuId, Sum = x.Sum(y => (y.DonGiaSauChietKhau ?? 0) * y.SoLuong) })
                .ToList();
            foreach (var goi in lstTongChiTheoGoiDichVu)
            {
                var yeuCauGoiDichVu = yeuCauGoiDichVus.Where(x => x.Id == goi.YeuCauGoiDichVuId).First();
                var tongDaChi = yeuCauGoiDichVu.YeuCauKhamBenhs.Where(a => a.TrangThai != EnumTrangThaiYeuCauKhamBenh.HuyKham
                                                                           && a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                                                                           && a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.DonGiaSauChietKhau ?? 0)
                              + yeuCauGoiDichVu.YeuCauDichVuKyThuats.Where(a => a.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                     && a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                                                                     && a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.SoLan * (a.DonGiaSauChietKhau ?? 0))
                              + yeuCauGoiDichVu.YeuCauDichVuGiuongBenhViens.Where(a => a.TrangThai != EnumTrangThaiGiuongBenh.DaHuy
                                                                            && a.TrangThaiThanhToan != TrangThaiThanhToan.ChuaThanhToan
                                                                            && a.YeuCauGoiDichVuId == goi.YeuCauGoiDichVuId).Sum(a => a.DonGiaSauChietKhau ?? 0);
                var soTienBaoLanhConLai = (yeuCauGoiDichVu.SoTienBenhNhanDaChi ?? 0) - tongDaChi;
                if (soTienBaoLanhConLai < goi.Sum)
                {
                    yeuCauVo.IsVuotQuaBaoLanhGoi = true;
                    break;
                }
            }
            #endregion

            if (yeuCauVo.YeuCauKhamBenhId != null)
            {
                var yeuCauKhamBenh = yeuCauTiepNhan.YeuCauKhamBenhs.FirstOrDefault(x => x.Id == yeuCauVo.YeuCauKhamBenhId
                                                                               && x.YeuCauTiepNhanId == yeuCauVo.YeuCauTiepNhanId
                                                                               && x.TrangThai == EnumTrangThaiYeuCauKhamBenh.ChuaKham);
                if (yeuCauKhamBenh != null)
                {
                    yeuCauKhamBenh.TrangThai = EnumTrangThaiYeuCauKhamBenh.DangKham;
                    yeuCauKhamBenh.NoiThucHienId = yeuCauKhamBenh.NoiDangKyId; // _userAgentHelper.GetCurrentNoiLLamViecId();
                    yeuCauKhamBenh.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                    yeuCauKhamBenh.ThoiDiemThucHien = DateTime.Now;

                    YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = yeuCauKhamBenh.TrangThai,
                        MoTa = yeuCauKhamBenh.TrangThai.GetDescription()
                    };
                    yeuCauKhamBenh.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);
                }
            }
        }

        public async Task XuLyTiemChungQuayLaiChuaKhamAsync(long yeuCauDichVuKyThuatId)
        {
            var yeuCauDichVuKyThuatHienTai = await _yeuCauDichVuKyThuatRepository.Table.Where(p => p.Id == yeuCauDichVuKyThuatId)
                                                                                       .Include(p => p.PhongBenhVienHangDois)
                                                                                       .Include(p => p.YeuCauDuocPhamBenhViens)
                                                                                       .Include(p => p.YeuCauVatTuBenhViens)
                                                                                       .Include(p => p.KhamSangLocTiemChung).ThenInclude(p => p.KetQuaSinhHieus)
                                                                                       .Include(p => p.KhamSangLocTiemChung).ThenInclude(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.TiemChung)
                                                                                       .FirstOrDefaultAsync();

            if (yeuCauDichVuKyThuatHienTai == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.ConcurrencyError"));
            }

            if (yeuCauDichVuKyThuatHienTai.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new Exception(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHuy")); //check
            }

            if (yeuCauDichVuKyThuatHienTai.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
            {
                throw new Exception(_localizationService.GetResource("KhamBenh.YeuCauKhamBenh.DaHoanThanhKham")); // check
            }

            if(yeuCauDichVuKyThuatHienTai.KhamSangLocTiemChung.KetQuaSinhHieus.Any())
            {
                throw new Exception(_localizationService.GetResource("TiemChung.QuayLaiChuaKham.KetQuaSinhHieu"));
            }

            if(yeuCauDichVuKyThuatHienTai.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(o => o.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            {
                throw new Exception(_localizationService.GetResource("TiemChung.QuayLaiChuaKham.Vaccine"));
            }

            if(yeuCauDichVuKyThuatHienTai.YeuCauDuocPhamBenhViens.Any(o => o.TrangThai != EnumYeuCauDuocPhamBenhVien.DaHuy) || yeuCauDichVuKyThuatHienTai.YeuCauVatTuBenhViens.Any(o => o.TrangThai != EnumYeuCauVatTuBenhVien.DaHuy))
            {
                throw new Exception(_localizationService.GetResource("TiemChung.QuayLaiChuaKham.DuocPhamVatTu"));
            }

            yeuCauDichVuKyThuatHienTai.KhamSangLocTiemChung.WillDelete = true;
            yeuCauDichVuKyThuatHienTai.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            yeuCauDichVuKyThuatHienTai.NhanVienThucHienId = null;
            yeuCauDichVuKyThuatHienTai.ThoiDiemThucHien = null;
            yeuCauDichVuKyThuatHienTai.ThoiDiemKetLuan = null;
            yeuCauDichVuKyThuatHienTai.NhanVienKetLuanId = null;
            yeuCauDichVuKyThuatHienTai.ThoiDiemHoanThanh = null;

            foreach(var hangDoi in yeuCauDichVuKyThuatHienTai.PhongBenhVienHangDois)
            {
                hangDoi.TrangThai = EnumTrangThaiHangDoi.ChoKham;
            }

            await _yeuCauDichVuKyThuatRepository.Context.SaveChangesAsync();
        }

        public async Task<List<LookupItemVo>> GetListTenGoiDichVu(List<long> yeuCauGoiDichVuIds)
        {
            var lstTenGoi = new List<LookupItemVo>();
            if (yeuCauGoiDichVuIds.Any())
            {
                lstTenGoi = _yeuCauGoiDichVuRepository.TableNoTracking
                    .Where(x => yeuCauGoiDichVuIds.Contains(x.Id))
                    .Select(item => new LookupItemVo()
                    {
                        KeyId = item.Id,
                        DisplayName = "Dịch vụ chọn từ gói: " + (item.ChuongTrinhGoiDichVu.Ten + " - " + item.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper()
                    })
                    .ToList();
            }

            return lstTenGoi;
        }
        #endregion

        #region Cập nhật xử lý lưu nhập xuất chung 1 lần
        public async Task XuLySoLuongChiDinhVacxinAsyncVer2(ICollection<YeuCauDichVuKyThuat> yeuCauDichVuKyThuats)
        {
            var cauHinhChung = _cauHinhService.LoadSetting<CauHinhChung>();
            var lstDuocPhamBenhVienId = yeuCauDichVuKyThuats
                                        .Where(x => x.TiemChung != null)
                                        .Select(x => x.TiemChung.DuocPhamBenhVienId)
                                        .Distinct().ToList();
            var lstNhapChiTietTheoDuocPham = await _nhapKhoDuocPhamChiTietRepository.Table
                .Include(x => x.NhapKhoDuocPhams)
                .Where(x => lstDuocPhamBenhVienId.Contains(x.DuocPhamBenhVienId))
                .ToListAsync();

            var khoVacxin = await _khoRepository.TableNoTracking.Where(p => p.LoaiKho == EnumLoaiKhoDuocPham.KhoVacXin).FirstOrDefaultAsync();

            if (khoVacxin == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            foreach (var yeuCauDichVuKyThuat in yeuCauDichVuKyThuats)
            {
                if (yeuCauDichVuKyThuat.Id != 0)
                {
                    if (yeuCauDichVuKyThuat.WillDelete == true)
                    {
                        await XuLyXoaSoLuongChiDinhVacxinAsyncVer2(yeuCauDichVuKyThuat, lstNhapChiTietTheoDuocPham);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    await XuLyThemSoLuongChiDinhVacxinAsyncVer2(yeuCauDichVuKyThuat, lstNhapChiTietTheoDuocPham, cauHinhChung, khoVacxin.Id);
                }
            }
        }

        public async Task XuLyXoaSoLuongChiDinhVacxinAsyncVer2(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets)
        {
            foreach (var item in yeuCauDichVuKyThuat.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris)
            {
                var nhapChiTietTheoDuocPham = nhapKhoDuocPhamChiTiets.FirstOrDefault(p => p.Id == item.NhapKhoDuocPhamChiTietId);

                if (nhapChiTietTheoDuocPham == null)
                {
                    throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
                }

                nhapChiTietTheoDuocPham.SoLuongDaXuat = Math.Round(nhapChiTietTheoDuocPham.SoLuongDaXuat - item.SoLuongXuat, 2);
            }
        }

        public async Task XuLyThemSoLuongChiDinhVacxinAsyncVer2(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets, CauHinhChung cauHinhChung, long khoVacXinId)
        {
            /***** Xử lý số lượng *****/
            var lstNhapChiTietTheoDuocPham = nhapKhoDuocPhamChiTiets.Where(x => x.NhapKhoDuocPhams.KhoId == khoVacXinId &&
                                                                                                                x.DuocPhamBenhVienId == yeuCauDichVuKyThuat.TiemChung.DuocPhamBenhVienId &&
                                                                                                                x.NhapKhoDuocPhams.DaHet != true &&
                                                                                                                x.LaDuocPhamBHYT == false &&
                                                                                                                x.SoLuongNhap > x.SoLuongDaXuat

                                                                                                                //BVHD-3821
                                                                                                                // trường hợp xuất cho người bệnh thì phải check còn hạn sử dụng
                                                                                                                && x.HanSuDung.Date >= DateTime.Now.Date)
                                                                                                    .OrderBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.HanSuDung : p.NgayNhapVaoBenhVien).ThenBy(p => cauHinhChung.UuTienXuatKhoTheoHanSuDung ? p.NgayNhapVaoBenhVien : p.HanSuDung)
                                                                                                    .ToList();

            var tongSoLuongTonTrongKho = Math.Round(lstNhapChiTietTheoDuocPham.Sum(x => x.SoLuongNhap - x.SoLuongDaXuat), 2);
            if ((tongSoLuongTonTrongKho < 0) || (tongSoLuongTonTrongKho < yeuCauDichVuKyThuat.TiemChung.SoLuong))
            {
                throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
            }

            var soLuongXuatLanNay = Math.Round(yeuCauDichVuKyThuat.TiemChung.SoLuong, 2);
            foreach (var nhapChiTietTheoDuocPham in lstNhapChiTietTheoDuocPham)
            {
                var soLuongTonTrongKho = Math.Round(nhapChiTietTheoDuocPham.SoLuongNhap - nhapChiTietTheoDuocPham.SoLuongDaXuat, 2);
                if ((soLuongTonTrongKho < 0))
                {
                    continue;
                }

                // thêm xuất kho dược phẩm chi tiết
                var xuatChiTiet = new XuatKhoDuocPhamChiTiet()
                {
                    DuocPhamBenhVienId = yeuCauDichVuKyThuat.TiemChung.DuocPhamBenhVienId
                };

                var xuatViTri = new XuatKhoDuocPhamChiTietViTri()
                {
                    NhapKhoDuocPhamChiTietId = nhapChiTietTheoDuocPham.Id
                };

                double soLuongXuatTheoNhap = 0;
                if (soLuongXuatLanNay > soLuongTonTrongKho)
                {
                    soLuongXuatTheoNhap = soLuongTonTrongKho;
                    soLuongXuatLanNay = Math.Round(soLuongXuatLanNay - soLuongTonTrongKho, 2);
                    nhapChiTietTheoDuocPham.SoLuongDaXuat = Math.Round(nhapChiTietTheoDuocPham.SoLuongDaXuat + soLuongTonTrongKho, 2);
                }
                else
                {
                    soLuongXuatTheoNhap = soLuongXuatLanNay;
                    nhapChiTietTheoDuocPham.SoLuongDaXuat = Math.Round(nhapChiTietTheoDuocPham.SoLuongDaXuat + soLuongXuatLanNay, 2);
                    soLuongXuatLanNay = 0;
                }

                xuatViTri.SoLuongXuat = soLuongXuatTheoNhap;

                xuatChiTiet.XuatKhoDuocPhamChiTietViTris.Add(xuatViTri);

                yeuCauDichVuKyThuat.TiemChung.XuatKhoDuocPhamChiTiet = xuatChiTiet;
                yeuCauDichVuKyThuat.TiemChung.HopDongThauDuocPhamId = nhapChiTietTheoDuocPham.HopDongThauDuocPhamId;

                //nhapChiTietVacxin.Add(new NhapChiTietVacxinTiemChungVo
                //{
                //    NhapKhoDuocPhamChiTietId = nhapChiTietTheoDuocPham.Id,
                //    NhapKhoDuocPhamChiTiet = nhapChiTietTheoDuocPham,
                //    SoLuongXuat = soLuongXuatTheoNhap
                //});

                if (soLuongXuatLanNay < 0 || soLuongXuatLanNay.AlmostEqual(0))
                {
                    break;
                }
            }

            if (soLuongXuatLanNay > 0)
            {
                throw new Exception(_localizationService.GetResource("GhiNhanVatTuThuoc.SoLuongTon.KhongDu"));
            }
        }
        #endregion
    }
}