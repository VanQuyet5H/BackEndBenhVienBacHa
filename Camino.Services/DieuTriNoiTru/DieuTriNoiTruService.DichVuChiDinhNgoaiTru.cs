using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuChiDinhNgoaiTru;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KetQuaCLS;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        #region Get data
        public async Task<List<KhamBenhGoiDichVuGridVo>> GetDataDefaulDichVuChiDinhNgoaiTru(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo)
        {
            var yeuCauTiepNhan = _yeuCauTiepNhanRepository.TableNoTracking
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBenhViens)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenhBenhVienGiaBaoHiems)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKhamBenhBenhViens)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)?.ThenInclude(p => p.DichVuKhamBenh)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.DichVuKhamBenhBenhVien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NhomGiaDichVuKhamBenhBenhVien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiThucHien)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NoiDangKy)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiThucHien)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.BacSiDangKy)?.ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.TaiKhoanBenhNhanChis)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                     .Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)

                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatVuBenhVienGiaBenhViens)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuatBenhVienGiaBaoHiems)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DoiTuongUuDaiDichVuKyThuatBenhViens)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuKyThuat)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.DichVuKyThuatBenhVien)?.ThenInclude(p => p.DichVuXetNghiem)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomGiaDichVuKyThuatBenhVien)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NoiThucHien)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienThucHien)?.ThenInclude(p => p.User)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauDichVuKyThuatTuongTrinhPTTT)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhomDichVuBenhVien)?.ThenInclude(p => p.NhomDichVuBenhVienCha)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.TaiKhoanBenhNhanChis)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.NhanVienChiDinh).ThenInclude(p => p.User)
                     //.Include(p => p.YeuCauKhamBenhs).ThenInclude(p => p.YeuCauDichVuKyThuats)?.ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
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
                     .Include(p => p.YeuCauDichVuKyThuats).ThenInclude(p => p.MienGiamChiPhis).ThenInclude(p => p.YeuCauGoiDichVu).ThenInclude(p => p.ChuongTrinhGoiDichVu)
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
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                                && x.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                    .ToList();


                #endregion

                var dichVus = SetDataGripViewYeuCauChiDinhKhac(yeuCauTiepNhan, queryInfo.NhomDichVuId, goiDichVus);
                return dichVus;
            }

            return null;
        }
        private List<KhamBenhGoiDichVuGridVo> SetDataGripViewYeuCauChiDinhKhac(YeuCauTiepNhan yeuCauTiepNhan, Enums.EnumNhomGoiDichVu? nhomId = null, List<YeuCauGoiDichVu> goiDichVus = null)
        {
            // list chỉ định dịch vụ khám
            var lstYeuCauKhamBenhChiDinh = yeuCauTiepNhan.YeuCauKhamBenhs
                                            .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham) //BVHD-3284:  || !string.IsNullOrEmpty(x.LyDoHuyDichVu)
                                            .OrderBy(x => x.CreatedOn);

            // danh sách dịch vụ
            var lstDichVuKyThuat = yeuCauTiepNhan.YeuCauDichVuKyThuats
                  .Where(x => x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy) //BVHD-3284:  || !string.IsNullOrEmpty(x.LyDoHuyDichVu)
                  .ToList();

            // get list nhóm dịch vụ trong gói
            //var lstNhomDichVu = EnumHelper.GetListEnum<Enums.EnumNhomGoiDichVu>()
            //    .Select(item => new LookupItemVo()
            //    {
            //        DisplayName = item.GetDescription(),
            //        KeyId = Convert.ToInt32(item)
            //    }).OrderByDescending(x => nhomId == null || x.KeyId == nhomId).ThenBy(x => x.DisplayName).ToList();

            if (goiDichVus == null)
            {
                goiDichVus = new List<YeuCauGoiDichVu>();
            }

            var goiDichVuKhamBenh = new List<KhamBenhGoiDichVuGridVo>();
            var stt = 1;

            //foreach (var item in lstNhomDichVu)
            //{
                switch (nhomId)
                {
                    case Enums.EnumNhomGoiDichVu.DichVuKhamBenh:
                        goiDichVuKhamBenh.AddRange(lstYeuCauKhamBenhChiDinh.Select(p => new KhamBenhGoiDichVuGridVo
                        {
                            STT = stt++,
                            Id = p.Id,
                            Nhom = Enums.EnumNhomGoiDichVu.DichVuKhamBenh.GetDescription(),
                            NhomId = (int)Enums.EnumNhomGoiDichVu.DichVuKhamBenh,
                            LoaiYeuCauDichVuId = p.DichVuKhamBenhBenhVienId,
                            NhomGiaDichVuBenhVienId = p.NhomGiaDichVuKhamBenhBenhVienId,
                            Ma = p.MaDichVu,
                            TenDichVu = p.TenDichVu,
                            TenLoaiGia = p.NhomGiaDichVuKhamBenhBenhVien?.Ten,
                            LoaiGia = p.NhomGiaDichVuKhamBenhBenhVienId,
                            DonGia = p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia,
                            BHYTThanhToan = 0,
                            NoiThucHien = p.NoiThucHien == null ? p.NoiDangKy?.Ten : p.NoiThucHien?.Ten,
                            NoiThucHienId = p.NoiThucHienId == null ? (p.NoiDangKyId ?? 0) : (p.NoiThucHienId ?? 0),
                            TenNguoiThucHien = p.BacSiThucHien == null ? p.BacSiDangKy?.User?.HoTen : p.BacSiThucHien?.User?.HoTen,
                            NguoiThucHienId = p.BacSiThucHienId == null ? p.BacSiDangKyId : p.BacSiThucHienId,
                            SoLuong = 1,
                            TrangThaiDichVu = p.TrangThai.GetDescription(),
                            TrangThaiDichVuId = (int)p.TrangThai,
                            KhongTinhPhi = p.KhongTinhPhi,
                            KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                            isCheckRowItem = false,
                            DaThanhToan = p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan,
                            DonGiaBaoHiem = p.DonGiaBaoHiem,
                            DuocHuongBaoHiem = p.DuocHuongBaoHiem,
                            YeuCauGoiDichVuId = p.YeuCauGoiDichVuId,
                            TenGoiDichVu = p.YeuCauGoiDichVu != null ?
                                "Dịch vụ chọn từ gói: " + (p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + p.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper()
                                : (p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                    p.MienGiamChiPhis
                                        .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                        .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.Ten + " - " + a.YeuCauGoiDichVu?.ChuongTrinhGoiDichVu?.TenGoiDichVu).ToUpper())
                                        .First() : null),
                            ThoiGianChiDinh = p.ThoiDiemChiDinh,
                            ThanhTien = p.KhongTinhPhi == true ? 0 : (p.YeuCauGoiDichVuId != null ? p.DonGiaSauChietKhau : p.Gia),
                            IsDichVuHuyThanhToan = p.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                            LyDoHuyDichVu = p.LyDoHuyDichVu,
                            NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

                            // gói marketing
                            CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs.Any(b => b.DichVuKhamBenhBenhVienId == p.DichVuKhamBenhBenhVienId)),
                            CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                        }));
                        break;
                    case Enums.EnumNhomGoiDichVu.DichVuKyThuat:
                        var lstSortNhomDichVuKyThuat = lstDichVuKyThuat.OrderBy(x => x.CreatedOn)
                            .Select(x => x.NhomDichVuBenhVienId).Distinct().ToList();
                        goiDichVuKhamBenh.AddRange(
                            lstDichVuKyThuat

                            // cập nhật 18/05/2021: sắp xếp lại các dịch vụ xét nghiệm theo số thứ tự
                            .OrderBy(x => lstSortNhomDichVuKyThuat.IndexOf(x.NhomDichVuBenhVienId))
                            .ThenBy(x => x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem ? (x.DichVuKyThuatBenhVien.DichVuXetNghiem?.SoThuTu ?? (x.DichVuKyThuatBenhVien.DichVuXetNghiemId ?? 0)) : 0)
                            .ThenBy(x => x.CreatedOn)
                            .Select(p => new KhamBenhGoiDichVuGridVo
                            {
                                STT = stt++,
                                Id = p.Id,
                                Nhom = (string.IsNullOrEmpty(p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten) ? "" : p.NhomDichVuBenhVien.NhomDichVuBenhVienCha?.Ten + " - ") + p.NhomDichVuBenhVien.Ten,
                                NhomId = (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat,
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
                                BHYTThanhToan = 0,
                                NoiThucHien = p.NoiThucHien?.Ten,
                                NoiThucHienId = p.NoiThucHienId ?? 0,
                                TenNguoiThucHien = p.NhanVienThucHien?.User.HoTen,
                                NguoiThucHienId = p.NhanVienThucHienId,
                                SoLuong = Convert.ToDouble(p.SoLan),
                                TrangThaiDichVu = p.TrangThai.GetDescription(),
                                TrangThaiDichVuId = (int)p.TrangThai,
                                NhomChiPhiDichVuKyThuatId = p.DichVuKyThuatBenhVien?.DichVuKyThuat?.NhomChiPhi,
                                KiemTraBHYTXacNhan = p.BaoHiemChiTra != null,
                                isCheckRowItem = false,
                                DaThanhToan = p.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan,
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
                                IsDichVuHuyThanhToan = p.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && p.TaiKhoanBenhNhanChis.Any(),
                                LyDoHuyDichVu = p.LyDoHuyDichVu,
                                NguoiChiDinhDisplay = p.NhanVienChiDinh?.User?.HoTen,

                                // gói marketing
                                CoDichVuNayTrongGoi = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                CoDichVuNayTrongGoiKhuyenMai = !goiDichVus.Any() ? false : goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == p.DichVuKyThuatBenhVienId)),
                                CoThongTinMienGiam = p.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                                // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
                                LoaiDichVuKyThuat = p.LoaiDichVuKyThuat,
                                LyDoHuyTrangThaiDaThucHien = p.LyDoHuyTrangThaiDaThucHien,
                                ThoiDiemThucHien = p.ThoiDiemThucHien
                            }));
                        break;
                }
            //}
            return goiDichVuKhamBenh;
        }

        public async Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTruAsync(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo)
        {
            var lstGhiNhanVTHHThuoc = new List<GhiNhanVatTuTieuHaoThuocGridVo>();
            if (queryInfo.NhomDichVuId == Enums.EnumNhomGoiDichVu.DuocPham)
            {
                lstGhiNhanVTHHThuoc = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                            && x.YeuCauTiepNhanId == queryInfo.YeuCauTiepNhanId)
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)Enums.EnumNhomGoiDichVu.DuocPham,
                    TenNhomYeuCau = Enums.EnumNhomGoiDichVu.DuocPham.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.DuocPhamBenhVien.DuocPham.SoDangKy,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,
                    DonViTinh = item.DonViTinh.Ten,
                    TenDuongDung = item.DuongDung.Ten,

                    DonGia = item.DonGiaBan,//item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaDuocPhamBHYT,

                    PhieuLinh = item.YeuCauLinhDuocPham != null ?
                        item.YeuCauLinhDuocPham.SoPhieu : (item.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false) ? item.YeuCauLinhDuocPhamChiTiets.Where(a => a.YeuCauLinhDuocPham.DuocDuyet != false).Select(a => a.YeuCauLinhDuocPham.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    //Ma = item.Ma,
                    Ten = item.Ten,
                    //DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.DuocPhamBenhVienId,
                    TinhTrang = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu,
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any(),
                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToListAsync();
            }
            else
            {
                lstGhiNhanVTHHThuoc = await _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                            && x.YeuCauTiepNhanId == queryInfo.YeuCauTiepNhanId)
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                    TenNhomYeuCau = Enums.EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.Ma,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,

                    DonViTinh = item.DonViTinh,
                    TenDuongDung = "", //item.DuongDung,

                    DonGia = item.DonGiaBan, //item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaVatTuBHYT,

                    PhieuLinh = item.YeuCauLinhVatTu != null ?
                        item.YeuCauLinhVatTu.SoPhieu : (item.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false) ? item.YeuCauLinhVatTuChiTiets.Where(a => a.YeuCauLinhVatTu.DuocDuyet != false).Select(a => a.YeuCauLinhVatTu.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoVatTuChiTiet.XuatKhoVatTu != null ? item.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.VatTuBenhVienId,
                    TinhTrang = item.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu,
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraVatTuTuBenhNhanChiTiets.Any()

                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToListAsync();
            }

            return lstGhiNhanVTHHThuoc;
        }
        public async Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocChiDinhNgoaiTruAsyncVer2(GridChiDinhDichVuNgoaiTruQueryInfoVo queryInfo)
        {
            var lstGhiNhanVTHHThuoc = new List<GhiNhanVatTuTieuHaoThuocGridVo>();
            if (queryInfo.NhomDichVuId == Enums.EnumNhomGoiDichVu.DuocPham)
            {
                lstGhiNhanVTHHThuoc = await _yeuCauDuocPhamBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy
                            && x.YeuCauTiepNhanId == queryInfo.YeuCauTiepNhanId)
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)Enums.EnumNhomGoiDichVu.DuocPham,
                    TenNhomYeuCau = Enums.EnumNhomGoiDichVu.DuocPham.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.DuocPhamBenhVien.DuocPham.SoDangKy,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,
                    DonViTinh = item.DonViTinh.Ten,
                    TenDuongDung = item.DuongDung.Ten,

                    DonGia = item.DonGiaBan,//item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaDuocPhamBHYT,

                    PhieuLinh = item.YeuCauLinhDuocPham != null ?
                        item.YeuCauLinhDuocPham.SoPhieu : (item.YeuCauLinhDuocPhamChiTiets.Any(a => a.YeuCauLinhDuocPham.DuocDuyet != false) ? item.YeuCauLinhDuocPhamChiTiets.Where(a => a.YeuCauLinhDuocPham.DuocDuyet != false).Select(a => a.YeuCauLinhDuocPham.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham != null ? item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    //Ma = item.Ma,
                    Ten = item.Ten,
                    //DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.DuocPhamBenhVienId,
                    TinhTrang = item.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu,
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any(),
                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToListAsync();
            }
            else
            {
                lstGhiNhanVTHHThuoc = await _yeuCauVatTuBenhVienRepository.TableNoTracking
                .Where(x => x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy
                            && x.YeuCauTiepNhanId == queryInfo.YeuCauTiepNhanId)
                .Select(item => new GhiNhanVatTuTieuHaoThuocGridVo()
                {
                    YeuCauId = item.Id,
                    NhomYeuCauId = (int)Enums.EnumNhomGoiDichVu.VatTuTieuHao,
                    TenNhomYeuCau = Enums.EnumNhomGoiDichVu.VatTuTieuHao.GetDescription(),
                    TenDichVu = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ma + " - " + item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVien.Ten : item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ma + " - " + item.YeuCauKhamBenh.DichVuKhamBenhBenhVien.Ten,
                    DichVuId = item.YeuCauDichVuKyThuat != null ? item.YeuCauDichVuKyThuat.DichVuKyThuatBenhVienId : item.YeuCauKhamBenh.DichVuKhamBenhBenhVienId,

                    YeuCauDichVuChiDinhId = item.YeuCauDichVuKyThuat.Id != null ? item.YeuCauDichVuKyThuatId : item.YeuCauKhamBenhId,
                    NhomChiDinhId = (int)(item.YeuCauDichVuKyThuat != null ? Enums.EnumNhomGoiDichVu.DichVuKyThuat : Enums.EnumNhomGoiDichVu.DichVuKhamBenh),

                    MaDichVuYeuCau = item.Ma,
                    TenDichVuYeuCau = item.Ten,

                    KhoId = item.KhoLinhId ?? 0,
                    TenKho = item.KhoLinh == null ? "" : item.KhoLinh.Ten,
                    LoaiKho = item.KhoLinh.LoaiKho,

                    DonViTinh = item.DonViTinh,
                    TenDuongDung = "", //item.DuongDung,

                    DonGia = item.DonGiaBan, //item.DonGiaNhap,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.KhongTinhPhi != true ? item.DonGiaBan * Convert.ToDecimal(item.SoLuong) : 0, //item.DonGiaNhap

                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    LaBHYT = item.LaVatTuBHYT,

                    PhieuLinh = item.YeuCauLinhVatTu != null ?
                        item.YeuCauLinhVatTu.SoPhieu : (item.YeuCauLinhVatTuChiTiets.Any(a => a.YeuCauLinhVatTu.DuocDuyet != false) ? item.YeuCauLinhVatTuChiTiets.Where(a => a.YeuCauLinhVatTu.DuocDuyet != false).Select(a => a.YeuCauLinhVatTu.SoPhieu).Join(", ") : ""),
                    PhieuXuat = item.XuatKhoVatTuChiTiet.XuatKhoVatTu != null ? item.XuatKhoVatTuChiTiet.XuatKhoVatTu.SoPhieu : "",

                    TinhPhi = item.KhongTinhPhi == null ? true : !item.KhongTinhPhi.Value,
                    CreatedOn = item.CreatedOn,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    TenNhanVienChiDinh = item.NhanVienChiDinh.User.HoTen,

                    // cập nhật 24/05/2021
                    Ma = item.Ma,
                    Ten = item.Ten,
                    DVT = item.DonViTinh,
                    NhaSX = item.NhaSanXuat,
                    NuocSX = item.NuocSanXuat,

                    VatTuThuocBenhVienId = item.VatTuBenhVienId,
                    TinhTrang = item.XuatKhoVatTuChiTiet.XuatKhoVatTuId != null,
                    IsKhoLe = item.KhoLinh != null && item.KhoLinh.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoLe && item.LoaiPhieuLinh == Enums.EnumLoaiPhieuLinh.LinhBu,
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.YeuCauTraVatTuTuBenhNhanChiTiets.Any()

                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToListAsync();
            }

            var result = lstGhiNhanVTHHThuoc
                .GroupBy(x => new { x.NhomYeuCauId, x.MaDichVuYeuCau, x.TenDichVuYeuCau, x.TenKho, x.YeuCauDichVuChiDinhId, x.NhomChiDinhId, x.TinhPhi, x.CreatedOn })
                .Select(item => new GhiNhanVatTuTieuHaoThuocGroupParentGridVo()
                {
                    TenDichVu = item.First().TenDichVu,
                    DichVuId = item.First().DichVuId,

                    YeuCauDichVuChiDinhId = item.First().YeuCauDichVuChiDinhId,
                    NhomChiDinhId = item.First().NhomChiDinhId,

                    MaDichVuYeuCau = item.First().MaDichVuYeuCau,
                    TenDichVuYeuCau = item.First().TenDichVuYeuCau,

                    KhoId = item.First().KhoId,
                    TenKho = item.First().TenKho,
                    LoaiKho = item.First().LoaiKho,

                    DonViTinh = item.First().DonViTinh,
                    TenDuongDung = item.First().TenDuongDung,
                    SoLuong = item.Sum(a => a.SoLuong),

                    ThanhTien = item.Sum(a => a.ThanhTien ?? 0),

                    //DonGiaBaoHiem = item.Select(a => a.DonGiaBaoHiem).Distinct().Count() > 1 ? null : item.Select(a => a.DonGiaBaoHiem).First(),
                    DuocHuongBaoHiem = item.First().DuocHuongBaoHiem,
                    LaBHYT = item.First().LaBHYT,

                    PhieuLinh = item.First().PhieuLinh,
                    PhieuXuat = item.First().PhieuXuat,

                    TinhPhi = item.First().TinhPhi,
                    CreatedOn = item.First().CreatedOn,
                    ThoiGianChiDinh = item.First().ThoiGianChiDinh,
                    TenNhanVienChiDinh = item.First().TenNhanVienChiDinh,

                    // cập nhật 24/05/2021
                    Ma = item.First().Ma,
                    Ten = item.First().Ten,
                    DVT = item.First().DonViTinh,
                    NhaSX = item.First().NhaSX,
                    NuocSX = item.First().NuocSX,

                    VatTuThuocBenhVienId = item.First().VatTuThuocBenhVienId,
                    TinhTrang = item.First().TinhTrang,
                    IsKhoLe = item.First().IsKhoLe,
                    IsKhoTong = item.First().IsKhoTong,
                    IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet = item.First().IsCoYeuCauTraVatTuThuocTuBenhNhanChiTiet,

                    YeuCauGhiNhanVTTHThuocs = item.ToList(),
                    ThongTinGias = item.GroupBy(a => new { a.DonGia, a.DonGiaBaoHiem }).Select(a => new GhiNhanVatTuTieuHaoThuocGroupGiaGridVo
                    {
                        IsTinhPhi = a.First().TinhPhi,
                        DonGia = a.First().DonGia,
                        SoLuong = a.Sum(b => b.SoLuong),
                        DonGiaBaoHiem = a.First().DonGiaBaoHiem
                    }).ToList()
                })
                .OrderBy(x => x.CreatedOn).ThenBy(x => x.TenDichVu).ToList();
            return result;
        }

        #region //BVHD-3889
        public async Task<GridDataSource> GetDataForGridNhomDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            long tiepNhanNgoaiTruId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                tiepNhanNgoaiTruId = long.Parse(queryInfo.AdditionalSearchString);
            }

            var thoiDiemNhapVien = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == tiepNhanNgoaiTruId)
                .Select(x => x.NoiTruBenhAn.ThoiDiemNhapVien).FirstOrDefault();

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
            .Where(x => x.YeuCauTiepNhanId == tiepNhanNgoaiTruId
                        && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            .Select(x => new ThongTinDichVuChiDinhNgoaiTruVo()
            {
                MaDichVu = x.MaDichVu,
                TenDichVu = x.TenDichVu,
                DichVuKhamId = x.YeuCauKhamBenhId,
                TenDichVuKham = x.YeuCauKhamBenh.TenDichVu,
                ThoiDiemChiDinh = x.ThoiDiemChiDinh,
                ThoiDiemNhapVien = thoiDiemNhapVien
            }).GroupBy(x => new { x.DichVuKhamId, x.TenDichVuKham, x.ChiDinhSauNhapVien })
            .Select(item => new NhomDichVuKyThuatChiDinhNgoaiTruGridVo
            {
                Id = item.Key.DichVuKhamId ?? 0, // gán tạm để order by
                TenDichVu = item.Key.TenDichVuKham,
                ChiDinhSauNhapVien = item.Key.ChiDinhSauNhapVien
            })
            .OrderByDescending(x => !x.ChiDinhSauNhapVien).ThenBy(x => x.Id)
            .ToList();

            var result = query.ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridNhomDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            long tiepNhanNgoaiTruId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                tiepNhanNgoaiTruId = long.Parse(queryInfo.AdditionalSearchString);
            }

            var thoiDiemNhapVien = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == tiepNhanNgoaiTruId)
                .Select(x => x.NoiTruBenhAn.ThoiDiemNhapVien).FirstOrDefault();

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == tiepNhanNgoaiTruId
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                .Select(x => new ThongTinDichVuChiDinhNgoaiTruVo()
                {
                    MaDichVu = x.MaDichVu,
                    TenDichVu = x.TenDichVu,
                    DichVuKhamId = x.YeuCauKhamBenhId,
                    TenDichVuKham = x.YeuCauKhamBenh.TenDichVu,
                    ThoiDiemChiDinh = x.ThoiDiemChiDinh,
                    ThoiDiemNhapVien = thoiDiemNhapVien
                }).GroupBy(x => new { x.DichVuKhamId, x.TenDichVuKham, x.ChiDinhSauNhapVien })
                .Select(item => new NhomDichVuKyThuatChiDinhNgoaiTruGridVo
                {
                    TenDichVu = item.Key.TenDichVuKham,
                    ChiDinhSauNhapVien = item.Key.ChiDinhSauNhapVien
                })
                .ToList();

            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDataForGridDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new DichVuNgoaiTruTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuNgoaiTruTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var tiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .First(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == timKiemNangCaoObj.TiepNhanNgoaiTruId);

            var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
               // .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                //.Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                .Where(x => ((x.BenhNhanId == tiepNhanNoiTru.BenhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == tiepNhanNoiTru.BenhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                .ToList();

            var lstMienGiamChiPhi = _mienGiamChiPhiRepository.TableNoTracking
                .Include(x => x.YeuCauGoiDichVu).ThenInclude(x => x.ChuongTrinhGoiDichVu)
                .Where(x => x.YeuCauDichVuKyThuatId != null
                            && x.YeuCauTiepNhanId == timKiemNangCaoObj.TiepNhanNgoaiTruId
                            && x.YeuCauDichVuKyThuat.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (timKiemNangCaoObj.YeuCauKhamBenhId == 0 || x.YeuCauDichVuKyThuat.YeuCauKhamBenhId == timKiemNangCaoObj.YeuCauKhamBenhId)
                            && (timKiemNangCaoObj.ChiDinhSauNhapVien 
                                ? x.YeuCauDichVuKyThuat.ThoiDiemChiDinh > tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien 
                                : x.YeuCauDichVuKyThuat.ThoiDiemChiDinh <= tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien))
                .ToList();

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.TiepNhanNgoaiTruId
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (timKiemNangCaoObj.YeuCauKhamBenhId == 0 || x.YeuCauKhamBenhId == timKiemNangCaoObj.YeuCauKhamBenhId)
                            && (timKiemNangCaoObj.ChiDinhSauNhapVien ? x.ThoiDiemChiDinh > tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien : x.ThoiDiemChiDinh <= tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien))
                .Select(item => new KhamBenhGoiDichVuGridVo()
                {
                    Id = item.Id,
                    Nhom = (string.IsNullOrEmpty(item.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten) ? "" : item.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ten + " - ") + item.NhomDichVuBenhVien.Ten,
                    NhomId = (int)Enums.EnumNhomGoiDichVu.DichVuKyThuat,
                    LoaiYeuCauDichVuId = item.DichVuKyThuatBenhVien.Id,
                    NhomGiaDichVuBenhVienId = item.NhomGiaDichVuKyThuatBenhVienId,
                    Ma = item.DichVuKyThuatBenhVien.Ma,
                    MaGiaDichVu = item.DichVuKyThuatBenhVien.DichVuKyThuat.MaGia,
                    MaTT37 = item.DichVuKyThuatBenhVien.DichVuKyThuat.Ma4350,
                    TenDichVu = item.DichVuKyThuatBenhVien.Ten,
                    TenTT43 = item.TenGiaDichVu,
                    TenLoaiGia = item.NhomGiaDichVuKyThuatBenhVien.Ten,
                    LoaiGia = item.NhomGiaDichVuKyThuatBenhVienId,
                    DonGia = item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia,
                    BHYTThanhToan = 0,
                    NoiThucHien = item.NoiThucHien.Ten,
                    NoiThucHienId = item.NoiThucHienId ?? 0,
                    TenNguoiThucHien = item.NhanVienThucHien.User.HoTen,
                    NguoiThucHienId = item.NhanVienThucHienId,
                    SoLuong = Convert.ToDouble(item.SoLan),
                    TrangThaiDichVu = item.TrangThai.GetDescription(),
                    TrangThaiDichVuId = (int)item.TrangThai,
                    NhomChiPhiDichVuKyThuatId = item.DichVuKyThuatBenhVien.DichVuKyThuat.NhomChiPhi,
                    KiemTraBHYTXacNhan = item.BaoHiemChiTra != null,
                    isCheckRowItem = false,
                    DaThanhToan = item.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan,
                    DonGiaBaoHiem = item.DonGiaBaoHiem,
                    DuocHuongBaoHiem = item.DuocHuongBaoHiem,
                    KhongThucHien = item.YeuCauDichVuKyThuatTuongTrinhPTTT != null && item.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true,
                    LyDoKhongThucHien = item.YeuCauDichVuKyThuatTuongTrinhPTTT.LyDoKhongThucHien,
                    YeuCauGoiDichVuId = item.YeuCauGoiDichVuId,
                    TenGoiDichVu = item.YeuCauGoiDichVu != null ?
                                    "Dịch vụ chọn từ gói: " + (item.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + item.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper()
                                    : (lstMienGiamChiPhi.Where(x => x.YeuCauDichVuKyThuatId == item.Id).Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null) ?
                                        lstMienGiamChiPhi.Where(x => x.YeuCauDichVuKyThuatId == item.Id)
                                            .Where(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null)
                                            .Select(a => "Dịch vụ khuyến mãi chọn từ gói: " + (a.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.Ten + " - " + a.YeuCauGoiDichVu.ChuongTrinhGoiDichVu.TenGoiDichVu).ToUpper())
                                            .First() : null),
                    IsDichVuXetNghiem = item.NhomDichVuBenhVien.Ma == "XN" || item.NhomDichVuBenhVien.NhomDichVuBenhVienCha.Ma == "XN",
                    BenhPhamXetNghiem = item.BenhPhamXetNghiem,
                    ThoiGianChiDinh = item.ThoiDiemChiDinh,
                    KhongTinhPhi = item.KhongTinhPhi,
                    ThanhTien = item.KhongTinhPhi == true ? 0 : ((item.YeuCauGoiDichVuId != null ? item.DonGiaSauChietKhau : item.Gia) * (decimal)item.SoLan),
                    IsDichVuHuyThanhToan = item.TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan && item.TaiKhoanBenhNhanChis.Any(),
                    LyDoHuyDichVu = item.LyDoHuyDichVu,
                    NguoiChiDinhDisplay = item.NhanVienChiDinh.User.HoTen,

                    // gói marketing
                    CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId)),
                    CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats.Any(b => b.DichVuKyThuatBenhVienId == item.DichVuKyThuatBenhVienId)),
                    CoThongTinMienGiam = lstMienGiamChiPhi.Where(x => x.YeuCauDichVuKyThuatId == item.Id).Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                    // cập nhật kiểm tra dịch vụ khác 4 nhóm: PTTT, CDHA, TDCN, XN thì cho phép hoàn thành, hủy hoàn thành
                    LoaiDichVuKyThuat = item.LoaiDichVuKyThuat,
                    LyDoHuyTrangThaiDaThucHien = item.LyDoHuyTrangThaiDaThucHien,
                    ThoiDiemThucHien = item.ThoiDiemThucHien,

                    //BVHD-3889
                    YeuCauKhamBenhId = item.YeuCauKhamBenhId
                })
                .Skip(queryInfo.Skip).Take(queryInfo.Take)
                .ToList();

            var result = query.ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridDichVuTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var timKiemNangCaoObj = new DichVuNgoaiTruTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuNgoaiTruTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var tiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .First(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == timKiemNangCaoObj.TiepNhanNgoaiTruId);

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.TiepNhanNgoaiTruId
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && (timKiemNangCaoObj.YeuCauKhamBenhId == 0 || x.YeuCauKhamBenhId == timKiemNangCaoObj.YeuCauKhamBenhId)
                            && (timKiemNangCaoObj.ChiDinhSauNhapVien ? x.ThoiDiemChiDinh > tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien : x.ThoiDiemChiDinh <= tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien))
                .Select(item => new KhamBenhGoiDichVuGridVo()
                {
                    Id = item.Id
                });

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetDataForGridNhomDichVuCLSTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            long tiepNhanNgoaiTruId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                tiepNhanNgoaiTruId = long.Parse(queryInfo.AdditionalSearchString);
            }

            var thoiDiemNhapVien = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == tiepNhanNgoaiTruId)
                .Select(x => x.NoiTruBenhAn.ThoiDiemNhapVien).FirstOrDefault();

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
            .Where(x => x.YeuCauTiepNhanId == tiepNhanNgoaiTruId
                        && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                        && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem 
                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh 
                            || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
            .Select(x => new ThongTinDichVuChiDinhNgoaiTruVo()
            {
                MaDichVu = x.MaDichVu,
                TenDichVu = x.TenDichVu,
                DichVuKhamId = x.YeuCauKhamBenhId,
                TenDichVuKham = x.YeuCauKhamBenh.TenDichVu,
                ThoiDiemChiDinh = x.ThoiDiemChiDinh,
                ThoiDiemNhapVien = thoiDiemNhapVien
            }).GroupBy(x => new { x.DichVuKhamId, x.TenDichVuKham, x.ChiDinhSauNhapVien })
            .Select(item => new NhomDichVuKyThuatChiDinhNgoaiTruGridVo
            {
                Id = item.Key.DichVuKhamId ?? 0, // gán tạm để order by
                TenDichVu = item.Key.TenDichVuKham,
                ChiDinhSauNhapVien = item.Key.ChiDinhSauNhapVien
            })
            .OrderByDescending(x => !x.ChiDinhSauNhapVien).ThenBy(x => x.Id)
            .ToList();

            var result = query.ToArray();

            return new GridDataSource
            {
                Data = result,
                TotalRowCount = result.Length
            };
        }
        public async Task<GridDataSource> GetTotalPageForGridNhomDichVuCLSTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            long tiepNhanNgoaiTruId = 0;
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                tiepNhanNgoaiTruId = long.Parse(queryInfo.AdditionalSearchString);
            }

            var thoiDiemNhapVien = _yeuCauTiepNhanRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == tiepNhanNgoaiTruId)
                .Select(x => x.NoiTruBenhAn.ThoiDiemNhapVien).FirstOrDefault();

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == tiepNhanNgoaiTruId
                            && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                            && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                                || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh
                                || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang))
                .Select(x => new ThongTinDichVuChiDinhNgoaiTruVo()
                {
                    MaDichVu = x.MaDichVu,
                    TenDichVu = x.TenDichVu,
                    DichVuKhamId = x.YeuCauKhamBenhId,
                    TenDichVuKham = x.YeuCauKhamBenh.TenDichVu,
                    ThoiDiemChiDinh = x.ThoiDiemChiDinh,
                    ThoiDiemNhapVien = thoiDiemNhapVien
                }).GroupBy(x => new { x.DichVuKhamId, x.TenDichVuKham, x.ChiDinhSauNhapVien })
                .Select(item => new NhomDichVuKyThuatChiDinhNgoaiTruGridVo
                {
                    TenDichVu = item.Key.TenDichVuKham,
                    ChiDinhSauNhapVien = item.Key.ChiDinhSauNhapVien
                })
                .ToList();

            var countTask = query.Count();
            return new GridDataSource { TotalRowCount = countTask };
        }

        public async Task<GridDataSource> GetDataForGridKetQuaCDHATheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new DichVuNgoaiTruTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuNgoaiTruTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var tiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .First(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == timKiemNangCaoObj.TiepNhanNgoaiTruId);

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.TiepNhanNgoaiTruId
                            && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                            && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                            && (timKiemNangCaoObj.YeuCauKhamBenhId == 0 || x.YeuCauKhamBenhId == timKiemNangCaoObj.YeuCauKhamBenhId) 
                            && (timKiemNangCaoObj.ChiDinhSauNhapVien
                                ? x.ThoiDiemChiDinh > tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien
                                : x.ThoiDiemChiDinh <= tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien))
               .Select(s => new KetQuaCLSGridVo()
               {
                   Id = s.Id,
                   NoiDung = s.TenDichVu,
                   NguoiThucHien = s.NhanVienThucHien.User.HoTen,
                   NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                   BacSiKetLuan = s.NhanVienKetLuan.User.HoTen,
                   NgayKetLuan = s.ThoiDiemKetLuan != null ? s.ThoiDiemKetLuan.Value.ApplyFormatDateTimeSACH() : null,
                   LoaiKetQuaId = s.NhomDichVuBenhVien.NhomDichVuBenhVienChaId,
                   LoaiKetQuaCLS = s.LoaiDichVuKyThuat.GetDescription(),
                   YeuCauTiepNhanId = s.YeuCauTiepNhanId,
                   IsDisable = true,
               });

            query = query.ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);
            var dataCanLamSangs = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray();

            return new GridDataSource { Data = dataCanLamSangs };
        }
        public async Task<GridDataSource> GetTotalPageForGridKetQuaCDHATheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new DichVuNgoaiTruTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuNgoaiTruTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var tiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .First(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == timKiemNangCaoObj.TiepNhanNgoaiTruId);

            var query = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.TiepNhanNgoaiTruId
                            && x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                            && (x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang || x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                            && (timKiemNangCaoObj.YeuCauKhamBenhId == 0 || x.YeuCauKhamBenhId == timKiemNangCaoObj.YeuCauKhamBenhId)
                            && (timKiemNangCaoObj.ChiDinhSauNhapVien
                                ? x.ThoiDiemChiDinh > tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien
                                : x.ThoiDiemChiDinh <= tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien))
                .Select(s => new KetQuaCLSGridVo()
                {
                    Id = s.Id,
                    NoiDung = s.TenDichVu,
                    NgayThucHien = s.ThoiDiemThucHien != null ? s.ThoiDiemThucHien.Value.ApplyFormatDateTimeSACH() : null,
                    BacSiKetLuan = s.NhanVienKetLuan.User.HoTen
                })
                .ApplyLike(queryInfo.SearchTerms.ToLower(), g => g.NoiDung, g => g.NgayThucHien, g => g.BacSiKetLuan, g => g.BacSiKetLuanRemoveDictrict, g => g.NoiDungRemoveDictrict);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        //clone từ khám bệnh
        public async Task<GridDataSource> GetDataForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new DichVuNgoaiTruTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuNgoaiTruTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var tiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .First(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == timKiemNangCaoObj.TiepNhanNgoaiTruId);

            var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.TiepNhanNgoaiTruId
                            && (x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                            && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            && (timKiemNangCaoObj.YeuCauKhamBenhId == 0 || x.YeuCauKhamBenhId == timKiemNangCaoObj.YeuCauKhamBenhId)
                            && (timKiemNangCaoObj.ChiDinhSauNhapVien
                                ? x.ThoiDiemChiDinh > tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien
                                : x.ThoiDiemChiDinh <= tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien))
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User);
            var lstYeuCauDichVuKyThuatId = yeuCauDichVuKyThuats.Select(c => c.Id);
            var phienXetNghiemCTs = yeuCauDichVuKyThuats.SelectMany(c => c.PhienXetNghiemChiTiets).Where(c => c.ThoiDiemKetLuan != null)
                                            .Include(x => x.KetQuaXetNghiemChiTiets)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
                                            .Include(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User);

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();
            var chiTietKetQuaXetNghiems = new List<KQXetNghiemChiTiet>();

            if (phienXetNghiemCTs.Any())
            {
                foreach (var yeuCauDichVuKyThuatId in lstYeuCauDichVuKyThuatId)
                {
                    if (!phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                    var res = phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.ToList();

                    listChiTiet.AddRange(res);
                }
            }


            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            chiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<KQXetNghiemChiTiet>(), true);

            return new GridDataSource { Data = chiTietKetQuaXetNghiems.ToArray() };
        }
        public async Task<GridDataSource> GetTotalPageForGridKetQuaXetNghiemTheoThoiGianNhapVienAsync(QueryInfo queryInfo)
        {
            var timKiemNangCaoObj = new DichVuNgoaiTruTimKiemVo();
            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString) && queryInfo.AdditionalSearchString.Contains("{"))
            {
                timKiemNangCaoObj = JsonConvert.DeserializeObject<DichVuNgoaiTruTimKiemVo>(queryInfo.AdditionalSearchString);
            }

            var tiepNhanNoiTru = _yeuCauTiepNhanRepository.TableNoTracking
                .Include(x => x.NoiTruBenhAn)
                .First(x => x.YeuCauTiepNhanNgoaiTruCanQuyetToanId == timKiemNangCaoObj.TiepNhanNgoaiTruId);

            var yeuCauDichVuKyThuats = _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauTiepNhanId == timKiemNangCaoObj.TiepNhanNgoaiTruId
                            && (x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien)
                            && x.LoaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem
                            && (timKiemNangCaoObj.YeuCauKhamBenhId == 0 || x.YeuCauKhamBenhId == timKiemNangCaoObj.YeuCauKhamBenhId)
                            && (timKiemNangCaoObj.ChiDinhSauNhapVien
                                ? x.ThoiDiemChiDinh > tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien
                                : x.ThoiDiemChiDinh <= tiepNhanNoiTru.NoiTruBenhAn.ThoiDiemNhapVien))
                .Include(x => x.NhanVienThucHien).ThenInclude(x => x.User)
                .Include(x => x.NhanVienKetLuan).ThenInclude(x => x.User);
            var lstYeuCauDichVuKyThuatId = yeuCauDichVuKyThuats.Select(c => c.Id);
            var phienXetNghiemCTs = yeuCauDichVuKyThuats.SelectMany(c => c.PhienXetNghiemChiTiets).Where(c => c.ThoiDiemKetLuan != null)
                                            .Include(x => x.KetQuaXetNghiemChiTiets)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.YeuCauDichVuKyThuat)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuKyThuatBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.MayXetNghiem)
                                            .Include(x => x.NhomDichVuBenhVien)
                                            .Include(x => x.KetQuaXetNghiemChiTiets).ThenInclude(x => x.DichVuXetNghiem)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienYeuCau).ThenInclude(x => x.User)
                                            .Include(x => x.YeuCauChayLaiXetNghiem).ThenInclude(x => x.NhanVienDuyet).ThenInclude(x => x.User);

            var listChiTiet = new List<KetQuaXetNghiemChiTiet>();
            var chiTietKetQuaXetNghiems = new List<KQXetNghiemChiTiet>();

            if (phienXetNghiemCTs.Any())
            {
                foreach (var yeuCauDichVuKyThuatId in lstYeuCauDichVuKyThuatId)
                {
                    if (!phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.Any()) continue;
                    var res = phienXetNghiemCTs.Where(p => p.YeuCauDichVuKyThuatId == yeuCauDichVuKyThuatId).Last().KetQuaXetNghiemChiTiets.ToList();

                    listChiTiet.AddRange(res);
                }
            }


            listChiTiet = listChiTiet.OrderBy(p => p.SoThuTu ?? 0).ThenBy(p => p.DichVuXetNghiemId).ToList();
            chiTietKetQuaXetNghiems = AddDetailDataChild(listChiTiet, listChiTiet, new List<KQXetNghiemChiTiet>(), true);
            return new GridDataSource { TotalRowCount = chiTietKetQuaXetNghiems.Count() };
        }
        #endregion
        #endregion
    }
}
