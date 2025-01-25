using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.TiemChungs;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.TiemChung
{
    public partial class TiemChungService
    {
        #region Kiểm tra data
        public async Task<bool> KiemTraTatCaVacxinDaThucHienAsync(long yeuCauKhamSangLocId, bool isKhacPhongHienTai = true)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var kiemTra = await _yeuCauDichVuKyThuatRepository.TableNoTracking
                .Where(x => x.YeuCauDichVuKyThuatKhamSangLocTiemChungId == yeuCauKhamSangLocId
                            && x.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                            && x.TiemChung != null
                            && (isKhacPhongHienTai != true || x.NoiThucHienId != phongHienTaiId))
                .AnyAsync(x => x.TiemChung.TrangThaiTiemChung == Enums.TrangThaiTiemChung.ChuaTiemChung 
                               || x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien 
                               || x.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien);
            return !kiemTra;
        }
        #endregion

        #region get data
        public async Task<YeuCauDichVuKyThuatKhamSangLocTiemChung> GetThongTinTiemChungTheoPhongThucHienAsync(long yeuCauKhamSangLocId, bool isTheoPhongHienTai)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var thongTinTiemChung = await _yeuCauDichVuKyThuatKhamSangLocTiemChungRepository.Table
                .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.YeuCauTiepNhan)
                .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPham)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.TiemChung)
                    .ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPhamChiTietViTris)
                    .ThenInclude(x => x.NhapKhoDuocPhamChiTiet).ThenInclude(x => x.NhapKhoDuocPhams)
                .Include(x => x.YeuCauDichVuKyThuats).ThenInclude(x => x.PhongBenhVienHangDois)
                .Include(x => x.YeuCauDichVuKyThuat).ThenInclude(x => x.MienGiamChiPhis)
                .Where(x => x.Id == yeuCauKhamSangLocId
                            && (isTheoPhongHienTai != true || x.YeuCauDichVuKyThuats.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy && a.NoiThucHienId == phongHienTaiId)))
                .FirstOrDefaultAsync();
            if (thongTinTiemChung == null)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            return thongTinTiemChung;
        }


        #endregion

        #region Xử lý lưu data
        public async Task XuLyLuuThongTinThucHienTiemAsync(YeuCauDichVuKyThuatKhamSangLocTiemChung thongTinTiemChung, bool? IsHoanThanhTiem = false)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentUserId = _userAgentHelper.GetCurrentUserId();

            var phieuXuatTemp = new Core.Domain.Entities.XuatKhos.XuatKhoDuocPham()
            {
                LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
                LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
                NguoiXuatId = currentUserId,
                LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
                NgayXuat = DateTime.Now
            };

            var phieuXuatNew = phieuXuatTemp.Clone();
            foreach (var yeuCauDichVuKyThuat in thongTinTiemChung.YeuCauDichVuKyThuats.Where(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy))
            {
                if (yeuCauDichVuKyThuat.NoiThucHienId == phongHienTaiId)
                {
                    // trường hợp lưu bình thường theo phòng
                    if (yeuCauDichVuKyThuat.TiemChung.TrangThaiTiemChung == Enums.TrangThaiTiemChung.DaTiemChung
                        && yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien)
                    {
                        yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                        //yeuCauDichVuKyThuat.NhanVienThucHienId = yeuCauDichVuKyThuat.NhanVienKetLuanId = yeuCauDichVuKyThuat.TiemChung.NhanVienTiemId;
                        //yeuCauDichVuKyThuat.ThoiDiemThucHien = yeuCauDichVuKyThuat.ThoiDiemKetLuan = yeuCauDichVuKyThuat.ThoiDiemHoanThanh = yeuCauDichVuKyThuat.TiemChung.ThoiDiemTiem;

                        // xử lý thông tin xuất
                        // ở màn hình khám sàng lọc, đã thực hiện gán thông tin xuất
                        var tenBenhNhan = thongTinTiemChung.YeuCauDichVuKyThuat.YeuCauTiepNhan.HoTen;
                        phieuXuatNew.KhoXuatId = phieuXuatTemp.KhoXuatId = yeuCauDichVuKyThuat.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamChiTietViTris.Select(x => x.NhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.KhoId).First();
                        var phieuXuatDaXuat = thongTinTiemChung.YeuCauDichVuKyThuats.Where(o=>o.TrangThai!=Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                            .Select(x => x.TiemChung.XuatKhoDuocPhamChiTiet)
                            .Where(x =>x!=null && x.XuatKhoDuocPham != null)
                            .Select(x => x.XuatKhoDuocPham)
                            .FirstOrDefault(x =>
                            x.LoaiXuatKho == Enums.XuatKhoDuocPham.XuatChoBenhNhan
                            && x.TenNguoiNhan != null
                            && x.TenNguoiNhan.Trim().ToLower() == tenBenhNhan.Trim().ToLower()
                            && x.NguoiXuatId == currentUserId
                            && x.KhoXuatId == phieuXuatTemp.KhoXuatId);
                        if (phieuXuatDaXuat != null)
                        {
                            phieuXuatNew = phieuXuatDaXuat;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(phieuXuatNew.TenNguoiNhan))
                            {
                                phieuXuatNew.TenNguoiNhan = tenBenhNhan;
                            }
                            else
                            {
                                if (phieuXuatNew.TenNguoiNhan.Trim().ToLower() != tenBenhNhan.Trim().ToLower())
                                {
                                    phieuXuatNew = phieuXuatTemp.Clone();
                                    phieuXuatNew.KhoXuatId = phieuXuatTemp.KhoXuatId;
                                }
                            }
                        }

                        yeuCauDichVuKyThuat.TiemChung.XuatKhoDuocPhamChiTiet.NgayXuat = DateTime.Now;
                        yeuCauDichVuKyThuat.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham = phieuXuatNew;
                    }

                    // trường hợp tường trình lại thực hiện tiêm
                    else if (yeuCauDichVuKyThuat.TiemChung.TrangThaiTiemChung == Enums.TrangThaiTiemChung.DaTiemChung
                             && yeuCauDichVuKyThuat.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien
                             && IsHoanThanhTiem == true)
                    {
                        yeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                    }

                    yeuCauDichVuKyThuat.NhanVienThucHienId = yeuCauDichVuKyThuat.NhanVienKetLuanId = yeuCauDichVuKyThuat.TiemChung.NhanVienTiemId;
                    yeuCauDichVuKyThuat.ThoiDiemThucHien = yeuCauDichVuKyThuat.ThoiDiemKetLuan = yeuCauDichVuKyThuat.ThoiDiemHoanThanh = yeuCauDichVuKyThuat.TiemChung.ThoiDiemTiem;
                }
            }


            // trường hợp hoàn thành tiêm:
            // 1. cập nhật thông tin hoàn thành
            // 2. xóa hết hàng đợi của khám sàng lọc và vacxin
            if (IsHoanThanhTiem == true)
            {
                var kiemTra = await KiemTraTatCaVacxinDaThucHienAsync(thongTinTiemChung.Id);
                if (!kiemTra)
                {
                    throw new Exception(_localizationService.GetResource("ThucHienTiem.HoanThanhTiem.CoVacXinChuaTiem"));
                }

                thongTinTiemChung.YeuCauDichVuKyThuat.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                //thongTinTiemChung.YeuCauDichVuKyThuat.NhanVienKetLuanId = currentUserId;
                thongTinTiemChung.YeuCauDichVuKyThuat.ThoiDiemHoanThanh = DateTime.Now;
                if (thongTinTiemChung.ThoiDiemHoanThanhKhamSangLoc == null)
                {
                    thongTinTiemChung.ThoiDiemHoanThanhKhamSangLoc = DateTime.Now;
                }

                foreach (var hangDoi in thongTinTiemChung.YeuCauDichVuKyThuat.PhongBenhVienHangDois)
                {
                    hangDoi.WillDelete = true;
                }

                foreach (var vacxin in thongTinTiemChung.YeuCauDichVuKyThuats)
                {
                    foreach (var hangDoiVacxin in vacxin.PhongBenhVienHangDois)
                    {
                        hangDoiVacxin.WillDelete = true;
                    }
                }
            }

            await _yeuCauDichVuKyThuatKhamSangLocTiemChungRepository.Context.SaveChangesAsync();
        }

        public async Task CapNhatKhamLaiKhamSangLocTiemChungAsync(KhamTiemChungMoLaiVo thongTinKham)
        {
            var yeuCauKhamSangLoc = _yeuCauDichVuKyThuatRepository.GetById(thongTinKham.YeuCauKhamTiemChungId,
                x => x.Include(y => y.YeuCauTiepNhan)
                    .Include(y => y.KhamSangLocTiemChung)
                    .Include(y => y.PhongBenhVienHangDois));
            if (yeuCauKhamSangLoc.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung || yeuCauKhamSangLoc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            if (yeuCauKhamSangLoc.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
            {
                throw new Exception(_localizationService.GetResource("KhamLaiKhamSangLoc.YeuCauTiepNhan.KhongConHieuLuc"));
            }

            if (yeuCauKhamSangLoc.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc == null) //yeuCauKhamSangLoc.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien || 
            {
                throw new Exception(_localizationService.GetResource("KhamLaiKhamSangLoc.YeuCauKhamSangLoc.DangThucHien"));
            }

            yeuCauKhamSangLoc.KhamSangLocTiemChung.ThoiDiemHoanThanhKhamSangLoc = null;
            yeuCauKhamSangLoc.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien;

            // mục đích dùng biến này để làm cờ hiển thị ds đã hoàn thành tiêm
            //yeuCauKhamSangLoc.ThoiDiemHoanThanh = null;

            // xóa hết hàng đợi cũ tránh lỗi
            foreach (var hangDoiCuChuaXoa in yeuCauKhamSangLoc.PhongBenhVienHangDois)
            {
                if (hangDoiCuChuaXoa.PhongBenhVienId != thongTinKham.PhongBenhVienId)
                {
                    hangDoiCuChuaXoa.WillDelete = true;
                }
            }

            if (!yeuCauKhamSangLoc.PhongBenhVienHangDois.Any(x => !x.WillDelete && x.PhongBenhVienId == thongTinKham.PhongBenhVienId))
            {
                // tạo hàng đợi mới
                var maxSoThuTuHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking.Select(x => x.SoThuTu).Max();
                var phongBenhVienHangDoiNew = new PhongBenhVienHangDoi();
                phongBenhVienHangDoiNew.PhongBenhVienId = thongTinKham.PhongBenhVienId;
                phongBenhVienHangDoiNew.YeuCauTiepNhanId = yeuCauKhamSangLoc.YeuCauTiepNhanId;
                phongBenhVienHangDoiNew.YeuCauDichVuKyThuatId = yeuCauKhamSangLoc.Id;
                phongBenhVienHangDoiNew.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                phongBenhVienHangDoiNew.SoThuTu = maxSoThuTuHangDoi + 1;
                yeuCauKhamSangLoc.PhongBenhVienHangDois.Add(phongBenhVienHangDoiNew);
            }
            

            _dichVuKyThuatBenhVienRepository.Context.SaveChanges();
        }

        public async Task CapNhatKhamLaiThucHienTiemTheoPhongAsync(KhamTiemChungMoLaiVo thongTinKham)
        {
            var yeuCauKhamSangLoc = _yeuCauDichVuKyThuatRepository.GetById(thongTinKham.YeuCauKhamTiemChungId,
                x => x.Include(y => y.YeuCauTiepNhan)
                    .Include(y => y.KhamSangLocTiemChung).ThenInclude(y => y.YeuCauDichVuKyThuats).ThenInclude(y => y.PhongBenhVienHangDois));
            if (yeuCauKhamSangLoc.LoaiDichVuKyThuat != Enums.LoaiDichVuKyThuat.SangLocTiemChung 
                || yeuCauKhamSangLoc.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                || !yeuCauKhamSangLoc.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien && a.NoiThucHienId == thongTinKham.PhongBenhVienId))
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            if (yeuCauKhamSangLoc.YeuCauTiepNhan.TrangThaiYeuCauTiepNhan != Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien)
            {
                throw new Exception(_localizationService.GetResource("KhamLaiKhamSangLoc.YeuCauTiepNhan.KhongConHieuLuc"));
            }

            if (yeuCauKhamSangLoc.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Any(a => a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien 
                                                                                     && a.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy
                                                                                     && a.NoiThucHienId == thongTinKham.PhongBenhVienId
                                                                                     && (a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan 
                                                                                         ||a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.BaoLanhThanhToan 
                                                                                         || a.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
            {
                throw new Exception(_localizationService.GetResource("KhamLaiKhamSangLoc.VacXin.DangThucHien"));
            }


            var maxSoThuTuHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking.Select(x => x.SoThuTu).Max();
            foreach (var vacxin in yeuCauKhamSangLoc.KhamSangLocTiemChung.YeuCauDichVuKyThuats.Where(a => a.TrangThai == Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien
                                                                                                       && a.NoiThucHienId == thongTinKham.PhongBenhVienId))
            {
                vacxin.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.DangThucHien;

                // xóa hết hàng đợi cũ tránh lỗi
                foreach (var hangDoiCuChuaXoa in vacxin.PhongBenhVienHangDois)
                {
                    hangDoiCuChuaXoa.WillDelete = true;
                }

                // tạo hàng đợi mới
                var phongBenhVienHangDoiNew = new PhongBenhVienHangDoi();
                phongBenhVienHangDoiNew.PhongBenhVienId = thongTinKham.PhongBenhVienId;
                phongBenhVienHangDoiNew.YeuCauTiepNhanId = yeuCauKhamSangLoc.YeuCauTiepNhanId;
                phongBenhVienHangDoiNew.YeuCauDichVuKyThuatId = vacxin.Id;
                phongBenhVienHangDoiNew.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                phongBenhVienHangDoiNew.SoThuTu = maxSoThuTuHangDoi + 1;
                vacxin.PhongBenhVienHangDois.Add(phongBenhVienHangDoiNew);
            }

            _dichVuKyThuatBenhVienRepository.Context.SaveChanges();
        }

        public async Task XuLyHuyTiemVacxinAsync(long yeuCauVacxinId)
        {
            var phongHienTaiId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var yeuCauTiemVacxin = _yeuCauDichVuKyThuatRepository.Table
                .Where(x => x.Id == yeuCauVacxinId)
                .Include(x => x.PhongBenhVienHangDois)
                .Include(x => x.TiemChung).ThenInclude(x => x.XuatKhoDuocPhamChiTiet).ThenInclude(x => x.XuatKhoDuocPham)
                .FirstOrDefault();
            if (yeuCauTiemVacxin == null || yeuCauTiemVacxin.TiemChung.TrangThaiTiemChung != Enums.TrangThaiTiemChung.DaTiemChung || yeuCauTiemVacxin.NoiThucHienId != phongHienTaiId)
            {
                throw new Exception(_localizationService.GetResource("ApiError.EntityNull"));
            }

            var xuatKhoId = yeuCauTiemVacxin.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId;
            var xuatKhoChiTietId = yeuCauTiemVacxin.TiemChung.XuatKhoDuocPhamChiTietId;

            // kiểm tra phiếu xuất hiện tại có xuất chi tiết nào khác nữa không, nếu có thì ko xóa phiếu xuất, nếu ko thì xóa luôn phiếu xuất
            if (xuatKhoId != null && xuatKhoChiTietId != null)
            {
                var xuatKhoChiTietTheoXuatKhos = _xuatKhoDuocPhamChiTietRepository.TableNoTracking
                    .Where(x => x.Id != xuatKhoChiTietId && x.XuatKhoDuocPhamId == xuatKhoId)
                    .ToList();
                if (!xuatKhoChiTietTheoXuatKhos.Any())
                {
                    yeuCauTiemVacxin.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;
                }
            }

            yeuCauTiemVacxin.TiemChung.XuatKhoDuocPhamChiTiet.NgayXuat = null;
            yeuCauTiemVacxin.TiemChung.XuatKhoDuocPhamChiTiet.XuatKhoDuocPhamId = null;
            yeuCauTiemVacxin.TiemChung.TrangThaiTiemChung = Enums.TrangThaiTiemChung.ChuaTiemChung;
            yeuCauTiemVacxin.TiemChung.NhanVienTiemId = null;
            yeuCauTiemVacxin.TiemChung.ThoiDiemTiem = null;
            yeuCauTiemVacxin.TrangThai = Enums.EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            yeuCauTiemVacxin.NhanVienThucHienId = null;
            yeuCauTiemVacxin.ThoiDiemThucHien = null;
            yeuCauTiemVacxin.NhanVienKetLuanId = null;
            yeuCauTiemVacxin.ThoiDiemKetLuan = null;
            yeuCauTiemVacxin.ThoiDiemHoanThanh = null;

            if (!yeuCauTiemVacxin.PhongBenhVienHangDois.Any(x => !x.WillDelete && x.PhongBenhVienId == phongHienTaiId))
            {
                // tạo hàng đợi mới
                var maxSoThuTuHangDoi = _phongBenhVienHangDoiRepository.TableNoTracking.Select(x => x.SoThuTu).Max();
                var phongBenhVienHangDoiNew = new PhongBenhVienHangDoi();
                phongBenhVienHangDoiNew.PhongBenhVienId = phongHienTaiId;
                phongBenhVienHangDoiNew.YeuCauTiepNhanId = yeuCauTiemVacxin.YeuCauTiepNhanId;
                phongBenhVienHangDoiNew.YeuCauDichVuKyThuatId = yeuCauTiemVacxin.Id;
                phongBenhVienHangDoiNew.TrangThai = Enums.EnumTrangThaiHangDoi.ChoKham;
                phongBenhVienHangDoiNew.SoThuTu = maxSoThuTuHangDoi + 1;
                yeuCauTiemVacxin.PhongBenhVienHangDois.Add(phongBenhVienHangDoiNew);
            }
            _dichVuKyThuatBenhVienRepository.Context.SaveChanges();
        }
        #endregion
    }
}
