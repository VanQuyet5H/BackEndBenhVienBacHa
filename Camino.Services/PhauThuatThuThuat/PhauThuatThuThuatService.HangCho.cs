using Camino.Core.Data;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        #region Thông Tin Danh Sách Hàng chờ 
        public async Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachChoPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var yeuCauTiepNhanDangThucHienIds = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuatId != null &&                            
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                .Select(p => p.YeuCauTiepNhanId).Distinct().ToList();

            var lstChoPhauThuat = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuatId != null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            !yeuCauTiepNhanDangThucHienIds.Contains(p.YeuCauTiepNhanId) &&
                            p.YeuCauTiepNhanId != yeuCauTiepNhanHienTaiId &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId ?? 0,
                    MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ApplyLike(searchString, x => x.MaBN.ToString(), x => x.HoTen, x => x.MaYeuCauTiepNhan)
                .ToList();

            return lstChoPhauThuat.GroupBy(p => p.YeuCauTiepNhanId).Select(grp => grp.FirstOrDefault()).ToList();
        }
        public async Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachChoPhauThuatThuThuatHienTaiAsyncOld(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var lstExistedYeuCauTiepNhanId = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            //p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh == null &&
                            p.YeuCauTiepNhan.YeuCauDichVuKyThuats.Any(p2 => p2.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien && p2.PhongBenhVienHangDois.Any(p3 => p3.PhongBenhVienId == phongKhamHienTaiId)))
                .GroupBy(p => p.YeuCauTiepNhanId)
                .ToListAsync();

            var lstChoPhauThuat = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.ChoKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            !lstExistedYeuCauTiepNhanId.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            //!(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true) &&
                            //!lstKhongThucHien.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            //p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            p.YeuCauTiepNhanId != yeuCauTiepNhanHienTaiId &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId ?? 0,
                    MaBN = p.YeuCauDichVuKyThuat.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ApplyLike(searchString, x => x.MaBN.ToString(), x => x.HoTen, x => x.MaYeuCauTiepNhan)
                .ToListAsync();

            return lstChoPhauThuat.GroupBy(p => p.YeuCauTiepNhanId).Select(grp => grp.FirstOrDefault()).ToList();
        }
        public async Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachDangPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var yeuCauTiepNhanDangThucHienIds = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuatId != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&                            
                            p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien)
                .Select(p => p.YeuCauTiepNhanId).Distinct().ToList();

            var yeuCauTiepNhanTheoDoiSauPhauThuatThuThuatIds = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuatId != null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                            p.YeuCauTiepNhan.TheoDoiSauPhauThuatThuThuats.Any(p2 => p2.TrangThai == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.DangTheoDoi))
                .Select(p => p.YeuCauTiepNhanId).Distinct().ToList();

            var lstDangPhauThuat = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuatId != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            yeuCauTiepNhanDangThucHienIds.Contains(p.YeuCauTiepNhanId) &&
                            !yeuCauTiepNhanTheoDoiSauPhauThuatThuThuatIds.Contains(p.YeuCauTiepNhanId) &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            p.YeuCauTiepNhanId != yeuCauTiepNhanHienTaiId &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId ?? 0,
                    MaBN = p.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ApplyLike(searchString, x => x.MaBN.ToString(), x => x.HoTen, x => x.MaYeuCauTiepNhan)
                .ToList();

            return lstDangPhauThuat.GroupBy(p => p.YeuCauTiepNhanId).Select(grp => grp.FirstOrDefault()).ToList();
        }
        public async Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachDangPhauThuatThuThuatHienTaiAsyncOld(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var lstExistedYeuCauTiepNhanId = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            //p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauTiepNhan.YeuCauDichVuKyThuats.Any(p2 => p2.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien && p2.PhongBenhVienHangDois.Any(p3 => p3.PhongBenhVienId == phongKhamHienTaiId)))
                .GroupBy(p => p.YeuCauTiepNhanId)
                .ToListAsync();

            var lstExistedTheoDoiSauPhauThuatThuThuatId = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            //p.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && //do LanThucHien != null -> lấy sai Kết luận & Theo dõi
                            p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                            p.YeuCauTiepNhan.TheoDoiSauPhauThuatThuThuats.Any(p2 => p2.TrangThai == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.DangTheoDoi))
                .GroupBy(p => p.YeuCauTiepNhanId)
                .ToListAsync();

            var lstDangPhauThuat = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            lstExistedYeuCauTiepNhanId.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            !lstExistedTheoDoiSauPhauThuatThuThuatId.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            //!lstKhongThucHien.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            p.YeuCauTiepNhanId != yeuCauTiepNhanHienTaiId &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId ?? 0,
                    MaBN = p.YeuCauDichVuKyThuat.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ApplyLike(searchString, x => x.MaBN.ToString(), x => x.HoTen, x => x.MaYeuCauTiepNhan)
                .ToListAsync();

            return lstDangPhauThuat.GroupBy(p => p.YeuCauTiepNhanId).Select(grp => grp.FirstOrDefault()).ToList();
        }

        public async Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachTheoDoiPhauThuatThuThuatHienTaiAsync(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var lstTheoDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuatId != null &&                            
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauTiepNhan.TheoDoiSauPhauThuatThuThuats.Any(p2 => p2.TrangThai == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.DangTheoDoi) &&
                            p.YeuCauTiepNhanId != yeuCauTiepNhanHienTaiId &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId ?? 0,
                    MaBN = p.YeuCauDichVuKyThuat.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ApplyLike(searchString, x => x.MaBN.ToString(), x => x.HoTen, x => x.MaYeuCauTiepNhan)
                .ToList();

            return lstTheoDoi.GroupBy(p => p.YeuCauTiepNhanId).Select(grp => grp.FirstOrDefault()).ToList();
        }
        public async Task<ICollection<PhauThuatThuThuatGridVo>> GetDanhSachTheoDoiPhauThuatThuThuatHienTaiAsyncOld(long phongKhamHienTaiId, string searchString, long? yeuCauTiepNhanHienTaiId)
        {
            var lstExistedTheoDoiSauPhauThuatThuThuatId = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            //p.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && //do LanThucHien != null -> lấy sai Kết luận & Theo dõi
                            p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                            p.YeuCauTiepNhan.TheoDoiSauPhauThuatThuThuats.Any(p2 => p2.TrangThai == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.DangTheoDoi))
                .GroupBy(p => p.YeuCauTiepNhanId)
                .ToListAsync();

            var lstTheoDoi = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            lstExistedTheoDoiSauPhauThuatThuThuatId.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            //!lstKhongThucHien.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            //!(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true) &&
                            p.YeuCauTiepNhanId != yeuCauTiepNhanHienTaiId &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    BenhNhanId = p.YeuCauTiepNhan.BenhNhanId ?? 0,
                    MaBN = p.YeuCauDichVuKyThuat.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ApplyLike(searchString, x => x.MaBN.ToString(), x => x.HoTen, x => x.MaYeuCauTiepNhan)
                .ToListAsync();

            return lstTheoDoi.GroupBy(p => p.YeuCauTiepNhanId).Select(grp => grp.FirstOrDefault()).ToList();
        }

        #endregion

        public async Task<YeuCauTiepNhan> GetThongTinBenhNhanDangTuongTrinh(long phongKhamHienTaiId)
        {
            var benhNhanDangTuongTrinh = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                                                                                                          p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                                                                                                          p.SoThuTu == 1 &&
                                                                                                          p.YeuCauDichVuKyThuat != null &&
                                                                                                          //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                                                                                                          p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                          p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                                                                                                          //!lstExistedTheoDoiSauPhauThuatThuThuatId.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                                                                                                          //!lstKhongThucHien.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                                                                                                          p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                                                                                                          !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                                                                                              .Select(p => p.YeuCauTiepNhanId)
                                                                                              .FirstOrDefaultAsync();

            if (benhNhanDangTuongTrinh == 0)
            {
                return null;
            }
            else
            {
                await BatDauKhamBenhNhanPTTT(0, benhNhanDangTuongTrinh, phongKhamHienTaiId);
                return await GetThongTinBenhNhanTheoYeuCauTiepNhan(benhNhanDangTuongTrinh);
            }
        }

        public async Task<YeuCauTiepNhan> GetThongTinBenhNhanTheoYeuCauTiepNhan(long yeuCauTiepNhanId)
        {
            return _yeuCauTiepNhanRepository.TableNoTracking
                                                                  .Include(p => p.BenhNhan)//.ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                                                                  //.Include(p => p.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                                                                  //.Include(p => p.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVus).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                                                                  //.Include(p => p.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                                                                  //.Include(p => p.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                                                                  //.Include(p => p.BenhNhan).ThenInclude(z => z.YeuCauGoiDichVuSoSinhs).ThenInclude(z => z.ChuongTrinhGoiDichVu).ThenInclude(z => z.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                                                                  .Include(p => p.DanToc)
                                                                  .Include(p => p.NgheNghiep)
                                                                  .Include(p => p.LyDoTiepNhan)
                                                                  .Include(p => p.NoiTruBenhAn)
                                                                  .Where(p => p.Id == yeuCauTiepNhanId)
                                                                  .FirstOrDefault();
        }
        public async Task<EnumTrangThaiPhauThuatThuThuat> GetTrangThaiPhauThuatThuThuat(long yeuCauTiepNhanId, long phongKhamHienTaiId)
        {
            var thongTinHangDois = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(o => o.YeuCauTiepNhanId == yeuCauTiepNhanId && o.PhongBenhVienId == phongKhamHienTaiId && o.YeuCauDichVuKyThuatId != null && o.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
                .Select(o => new
                {
                    o.YeuCauTiepNhan.TuVongTrongPTTT,
                    o.YeuCauTiepNhan.KhoangThoiGianTuVong,
                    o.YeuCauTiepNhan.ThoiDiemTuVong,
                    o.YeuCauDichVuKyThuatId,
                    o.YeuCauDichVuKyThuat.TrangThai,
                    o.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId,
                    o.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT,
                    TrangThaiTheoDoiSauPhauThuatThuThuats = o.YeuCauTiepNhan.TheoDoiSauPhauThuatThuThuats.Select(t=>t.TrangThai).ToList()
                }).ToList();

            if(thongTinHangDois.Any(p => p.TuVongTrongPTTT != null || p.KhoangThoiGianTuVong != null || p.ThoiDiemTuVong != null))
            {
                return EnumTrangThaiPhauThuatThuThuat.TuVong;
            }
            var lstDangThucHien = thongTinHangDois
                .Where(o => o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien && !(o.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien == true))
                .ToList();

            if (lstDangThucHien.Any())
            {
                var lstDangTheoDoi = thongTinHangDois
                .Where(o => o.TheoDoiSauPhauThuatThuThuatId != null
                            && o.TrangThaiTheoDoiSauPhauThuatThuThuats.Any(p => p == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.DangTheoDoi)
                            && o.YeuCauDichVuKyThuatTuongTrinhPTTT?.ThoiDiemKetThucTuongTrinh != null
                            && !(o.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && o.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien == true))
                .ToList();
                if (lstDangTheoDoi.Any())
                {
                    return EnumTrangThaiPhauThuatThuThuat.TheoDoi;
                }
                else
                {
                    return EnumTrangThaiPhauThuatThuThuat.DangPhauThuat;
                }
            }
            else
            {
                var lstChoPhauThuat = thongTinHangDois.Where(o => o.YeuCauDichVuKyThuatTuongTrinhPTTT?.KhongThucHien != true)
                    .ToList();
                if (lstChoPhauThuat.Any())
                {
                    return EnumTrangThaiPhauThuatThuThuat.ChoPhauThuat;
                }
                else
                {
                    return EnumTrangThaiPhauThuatThuThuat.KhongCo;
                }
            }

            return EnumTrangThaiPhauThuatThuThuat.ChuyenGiao;
        }
        public async Task<EnumTrangThaiPhauThuatThuThuat> GetTrangThaiPhauThuatThuThuatOld(long yeuCauTiepNhanId, long phongKhamHienTaiId)
        {
            var isTuVong = await _yeuCauTiepNhanRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId && (p.TuVongTrongPTTT != null || p.KhoangThoiGianTuVong != null || p.ThoiDiemTuVong != null)).FirstOrDefaultAsync();
            if (isTuVong != null)
            {
                return EnumTrangThaiPhauThuatThuThuat.TuVong;
            }

            var lstExistedYeuCauTiepNhanId = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            //p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh == null &&
                            p.YeuCauTiepNhan.YeuCauDichVuKyThuats.Any(p2 => p2.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien && p2.PhongBenhVienHangDois.Any(p3 => p3.PhongBenhVienId == phongKhamHienTaiId)) &&
                            !(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true))
                .GroupBy(p => p.YeuCauTiepNhanId)
                .ToListAsync();

            var lstExistedTheoDoiSauPhauThuatThuThuatId = await _phongBenhVienHangDoiRepository.TableNoTracking
                   .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                               //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                               p.SoThuTu != 0 &&
                               p.YeuCauDichVuKyThuat != null &&
                               //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                               p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                               //p.YeuCauDichVuKyThuat.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && //do LanThucHien != null -> lấy sai Kết luận & Theo dõi
                               p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId != null &&
                               p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                               p.YeuCauTiepNhan.TheoDoiSauPhauThuatThuThuats.Any(p2 => p2.TrangThai == EnumTrangThaiTheoDoiSauPhauThuatThuThuat.DangTheoDoi) &&
                               p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.ThoiDiemKetThucTuongTrinh != null &&
                               !(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true))
                   .GroupBy(p => p.YeuCauTiepNhanId)
                   .ToListAsync();

            if (!lstExistedYeuCauTiepNhanId.Any(p => p.Key == yeuCauTiepNhanId))
            {
                var lstChoPhauThuat = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.ChoKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            !lstExistedYeuCauTiepNhanId.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            //!lstKhongThucHien.Any(p2 => p2.Key == yeuCauTiepNhanId))
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true)
                //!(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    MaBN = p.YeuCauDichVuKyThuat.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ToListAsync();

                if (lstChoPhauThuat.Any())
                {
                    return EnumTrangThaiPhauThuatThuThuat.ChoPhauThuat;
                }
                else
                {
                    return EnumTrangThaiPhauThuatThuThuat.KhongCo;
                }
            }
            else
            {
                if (lstExistedYeuCauTiepNhanId.Any(p => p.Key == yeuCauTiepNhanId) && !lstExistedTheoDoiSauPhauThuatThuThuatId.Any(p => p.Key == yeuCauTiepNhanId))
                {
                    return EnumTrangThaiPhauThuatThuThuat.DangPhauThuat;
                }
                else
                {
                    if (lstExistedTheoDoiSauPhauThuatThuThuatId.Any(p => p.Key == yeuCauTiepNhanId))
                    {
                        return EnumTrangThaiPhauThuatThuThuat.TheoDoi;
                    }
                }
            }

            return EnumTrangThaiPhauThuatThuThuat.ChuyenGiao;
        }

        public async Task<List<PhongBenhVienHangDoi>> GetPhongBenhVienHangDoiTuongTrinhLai(long yeuCauTiepNhanId, long phongKhamHienTaiId)
        {
            var phongBenhVienHangDois = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                                                                                                         p.SoThuTu != 0 &&
                                                                                                         p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                         p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                                                                                                         p.YeuCauDichVuKyThuat != null &&
                                                                                                         p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien &&
                                                                                                         p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                                                                             .Include(p => p.YeuCauDichVuKyThuat).ThenInclude(p => p.TheoDoiSauPhauThuatThuThuat)
                                                                                             .ToListAsync();

            return phongBenhVienHangDois;
        }
        public async Task<YeuCauTiepNhan> GetThongTinBenhNhanTiepTheo(long phongKhamHienTaiId, long yeuCauTiepNhanHienTaiId)
        {
            var yeuCauTiepNhanDangThucHienIds = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            //p.YeuCauTiepNhan.Id != yeuCauTiepNhanHienTaiId &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien )
                .Select(p => p.YeuCauTiepNhanId).Distinct().ToList();

            var phongBenhVienHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            !yeuCauTiepNhanDangThucHienIds.Contains(p.YeuCauTiepNhanId) &&
                            p.YeuCauTiepNhanId != yeuCauTiepNhanHienTaiId &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                .OrderBy(p => p.SoThuTu)
                .FirstOrDefault();

            if (phongBenhVienHangDoi != null)
            {
                return await GetThongTinBenhNhanTheoYeuCauTiepNhan(phongBenhVienHangDoi.YeuCauTiepNhanId);
            }

            return null;
        }
        public async Task<YeuCauTiepNhan> GetThongTinBenhNhanTiepTheoOld(long phongKhamHienTaiId, long yeuCauTiepNhanHienTaiId)
        {
            var lstExistedYeuCauTiepNhanId = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauTiepNhan.Id != yeuCauTiepNhanHienTaiId &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauTiepNhan.YeuCauDichVuKyThuats.Any(p2 => p2.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien && p2.PhongBenhVienHangDois.Any(p3 => p3.PhongBenhVienId == phongKhamHienTaiId)))
                .GroupBy(p => p.YeuCauTiepNhanId)
                .ToListAsync();

            var lstChoPhauThuat = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.PhongBenhVienId == phongKhamHienTaiId &&
                            //p.TrangThai == EnumTrangThaiHangDoi.ChoKham &&
                            p.SoThuTu != 0 &&
                            p.YeuCauDichVuKyThuat != null &&
                            //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                            p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                            p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            !lstExistedYeuCauTiepNhanId.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            //!lstKhongThucHien.Any(p2 => p2.Key == p.YeuCauTiepNhanId) &&
                            p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien != true &&
                            !(p.YeuCauTiepNhan.TuVongTrongPTTT != null || p.YeuCauTiepNhan.KhoangThoiGianTuVong != null || p.YeuCauTiepNhan.ThoiDiemTuVong != null))
                //!(p.YeuCauDichVuKyThuat.TrangThai == EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien && p.YeuCauDichVuKyThuat.YeuCauDichVuKyThuatTuongTrinhPTTT.KhongThucHien == true))
                .OrderBy(p => p.SoThuTu)
                .Select(p => new PhauThuatThuThuatGridVo
                {
                    Id = p.Id,
                    SoThuTu = p.SoThuTu,
                    MaBN = p.YeuCauDichVuKyThuat.YeuCauTiepNhan.BenhNhan.MaBN,
                    HoTen = p.YeuCauTiepNhan.HoTen,
                    TenGioiTinh = p.YeuCauTiepNhan.GioiTinh.GetDescription(),
                    Tuoi = p.YeuCauTiepNhan.NamSinh == null ? 0 : (DateTime.Now.Year - p.YeuCauTiepNhan.NamSinh.Value),
                    MaYeuCauTiepNhan = p.YeuCauTiepNhan.MaYeuCauTiepNhan,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId,
                    CoBaoHiem = p.YeuCauTiepNhan.CoBHYT ?? false
                })
                .ToListAsync();

            var benhNhanTiepTheo = lstChoPhauThuat.GroupBy(p => p.YeuCauTiepNhanId).Select(grp => grp.FirstOrDefault()).FirstOrDefault();

            if (benhNhanTiepTheo != null)
            {
                return await GetThongTinBenhNhanTheoYeuCauTiepNhan(benhNhanTiepTheo.YeuCauTiepNhanId);
            }

            return null;
        }

        public async Task<bool> KiemTraConYeuCauDichVuKyThuatTaiPhong(long phongBenhVienId, long yeuCauTiepNhanId)
        {
            return await _phongBenhVienHangDoiRepository.TableNoTracking.AnyAsync(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                       p.YeuCauDichVuKyThuat != null &&
                                                                                       //p.YeuCauDichVuKyThuat.LanThucHien == null &&
                                                                                       p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                       p.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != EnumTrangThaiYeuCauTiepNhan.DaHuy &&
                                                                                       p.YeuCauTiepNhanId == yeuCauTiepNhanId);
        }

        public async Task<bool> KiemTraCoBenhNhanKhacDangKhamTrongPhong(long currentUserId, long phongBenhVienId)
        {
            var benhNhanDangKham = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                    p.YeuCauDichVuKyThuat != null &&
                                                                                                    p.SoThuTu == 0 &&
                                                                                                    p.CreatedById == currentUserId) //fix lại
                                                                                        .FirstOrDefaultAsync();

            if (benhNhanDangKham == null)
            {
                return await _phongBenhVienHangDoiRepository.TableNoTracking.AnyAsync(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                           p.YeuCauDichVuKyThuat != null &&
                                                                                           p.SoThuTu == 0);
            }
            else
            {
                return false;
            }
        }

        public async Task BatDauKhamBenhNhanPTTT(long yeuCauTiepNhanDangKhamId, long yeuCauTiepNhanBatDauKhamId, long phongBenhVienId)
        {
            if (yeuCauTiepNhanDangKhamId == 0 || yeuCauTiepNhanDangKhamId == yeuCauTiepNhanBatDauKhamId)
            {
                //TrangThai = 2 & SoThuTu = 1
                var phongBenhVienHangDoiBatDauKham = _phongBenhVienHangDoiRepository.Table.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                                                                                                                      p.YeuCauDichVuKyThuat != null &&
                                                                                                                      p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                                                      p.YeuCauTiepNhanId == yeuCauTiepNhanBatDauKhamId)
                                                                                                          .ToList();                

                foreach (var item in phongBenhVienHangDoiBatDauKham)
                {
                    item.SoThuTu = 1;
                    item.TrangThai = EnumTrangThaiHangDoi.DangKham;
                }

                _phongBenhVienHangDoiRepository.Context.SaveChanges();

                ////Cộng STT dịch vụ còn lại
                //int stt = 2;
                //var phongBenhVienHangDoiConLai = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                //                                                                                                  p.YeuCauTiepNhanId != yeuCauTiepNhanBatDauKhamId)
                //                                                                                      .OrderBy(p => p.SoThuTu)
                //                                                                                      .ToListAsync();

                //foreach (var item in phongBenhVienHangDoiConLai)
                //{
                //    item.SoThuTu = stt;
                //    stt++;
                //}

                //await _phongBenhVienHangDoiRepository.UpdateAsync(phongBenhVienHangDoiConLai);
            }
            else
            {
                #region cập nhật 12/12/2022 code logic hiện tại
                ////Người bệnh đang khám
                //var phongBenhVienHangDoiDangKham = _phongBenhVienHangDoiRepository.Table.Where(p => p.PhongBenhVienId == phongBenhVienId && p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                //                                                                                                    p.YeuCauDichVuKyThuat != null &&
                //                                                                                                    p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                //                                                                                                    p.YeuCauTiepNhanId != yeuCauTiepNhanBatDauKhamId)
                //                                                                                        .ToList();

                //foreach (var item in phongBenhVienHangDoiDangKham)
                //{
                //    item.TrangThai = EnumTrangThaiHangDoi.ChoKham;
                //}

                ////Người bệnh bắt đầu khám
                //var phongBenhVienHangDoiBatDauKham = _phongBenhVienHangDoiRepository.Table.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                //                                                                                                      p.YeuCauDichVuKyThuat != null &&
                //                                                                                                      p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                //                                                                                                      p.YeuCauTiepNhanId == yeuCauTiepNhanBatDauKhamId)
                //                                                                                          .ToList();                


                //foreach (var item in phongBenhVienHangDoiBatDauKham)
                //{
                //    item.SoThuTu = 1;
                //    item.TrangThai = EnumTrangThaiHangDoi.DangKham;
                //}
                #endregion

                #region Cập nhật 12/12/2022: gộp get người bệnh đang khám và bắt đầu khám theo logic hiện tại phía trên
                var hangDois = _phongBenhVienHangDoiRepository.Table
                            .Where(p => p.PhongBenhVienId == phongBenhVienId
                                        && (p.YeuCauTiepNhanId == yeuCauTiepNhanBatDauKhamId || p.TrangThai == EnumTrangThaiHangDoi.DangKham)
                                        && p.YeuCauDichVuKyThuat != null
                                        && p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat).ToList();
                foreach (var hangDoi in hangDois)
                {
                    if (hangDoi.YeuCauTiepNhanId == yeuCauTiepNhanBatDauKhamId)
                    {
                        hangDoi.SoThuTu = 1;
                        hangDoi.TrangThai = EnumTrangThaiHangDoi.DangKham;
                    }
                    else
                    {
                        hangDoi.TrangThai = EnumTrangThaiHangDoi.ChoKham;
                    }
                }

                #endregion

                _phongBenhVienHangDoiRepository.Context.SaveChanges();

                ////Cộng STT dịch vụ còn lại
                //int stt = 2;
                //var phongBenhVienHangDoiConLai = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                //                                                                                                  p.YeuCauTiepNhanId != yeuCauTiepNhanBatDauKhamId && p.YeuCauTiepNhanId != yeuCauTiepNhanDangKhamId)
                //                                                                                      .OrderBy(p => p.SoThuTu)
                //                                                                                      .ToListAsync();

                //foreach (var item in phongBenhVienHangDoiConLai)
                //{
                //    item.SoThuTu = stt;
                //    stt++;
                //}

                //await _phongBenhVienHangDoiRepository.UpdateAsync(phongBenhVienHangDoiConLai);

                ////Người bệnh đang khám
                //var phongBenhVienHangDoiDangKham = await _phongBenhVienHangDoiRepository.TableNoTracking.Where(p => p.PhongBenhVienId == phongBenhVienId &&
                //                                                                                                    p.YeuCauDichVuKyThuat != null &&
                //                                                                                                    p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                //                                                                                                    p.YeuCauTiepNhanId == yeuCauTiepNhanDangKhamId)
                //                                                                                        .ToListAsync();

                //if (phongBenhVienHangDoiDangKham == null)
                //{
                //    throw new Exception();
                //}

                //foreach (var item in phongBenhVienHangDoiDangKham)
                //{
                //    item.SoThuTu = stt;
                //    item.TrangThai = EnumTrangThaiHangDoi.ChoKham;
                //    stt++;
                //}

                //await _phongBenhVienHangDoiRepository.UpdateAsync(phongBenhVienHangDoiDangKham);
            }
        }

        public async Task HuyKhamBenhNhanPTTT(long phongBenhVienId)
        {
            var phongBenhVienHangDois = _phongBenhVienHangDoiRepository.Table.Where(p => p.PhongBenhVienId == phongBenhVienId && p.TrangThai == EnumTrangThaiHangDoi.DangKham &&
                                                                                                         p.YeuCauDichVuKyThuat != null &&
                                                                                                         p.YeuCauDichVuKyThuat.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
                                                                                             .ToList();
            
            foreach (var item in phongBenhVienHangDois)
            {
                item.TrangThai = EnumTrangThaiHangDoi.ChoKham;
            }

            _phongBenhVienHangDoiRepository.Context.SaveChanges();
        }

        public async Task<bool> CoDuocHuongBHYT(long yeuCauDichVuKyThuatId)
        {
            #region Cập nhật 27/12/2022
            ////cập nhật 27/05/2021: bỏ await
            //var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking.Where(p => p.Id == yeuCauDichVuKyThuatId)
            //                                                                              .Include(p => p.YeuCauKhamBenh)
            //                                                                              .FirstOrDefault();

            var yeuCauDichVuKyThuat = _yeuCauDichVuKyThuatRepository.TableNoTracking
                                .Where(p => p.Id == yeuCauDichVuKyThuatId)
                                .Select(p => new { p.DuocHuongBaoHiem }).FirstOrDefault();
            #endregion

            return yeuCauDichVuKyThuat.DuocHuongBaoHiem;
            //return ((yeuCauDichVuKyThuat.YeuCauKhamBenhId != null && yeuCauDichVuKyThuat.YeuCauKhamBenh.DuocHuongBaoHiem) || (yeuCauDichVuKyThuat.NoiTruPhieuDieuTriId != null && yeuCauDichVuKyThuat.DuocHuongBaoHiem));
            //return yeuCauDichVuKyThuat?.YeuCauKhamBenh?.DuocHuongBaoHiem ?? false;
        }
    }
}
