using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        public async Task XuLyThemYeuCauDichVuKyThuatMultiselectAsync(ChiDinhDichVuKyThuatMultiselectVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet, long? phieuDieuTriId)
        {
            var coBHYT = yeuCauTiepNhanChiTiet.CoBHYT ?? false;
            //var yeuCauKhamBenh = yeuCauTiepNhanChiTiet.YeuCauKhamBenhs.First(x => x.Id == yeuCauVo.YeuCauKhamBenhId);
            //var yeuCauKyThuat = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Add;
            var currentUserId = _userAgentHelper.GetCurrentUserId();
            var currentPhongLamViecId = _userAgentHelper.GetCurrentNoiLLamViecId();

            var ngayDieuTri = yeuCauTiepNhanChiTiet.NoiTruBenhAn?.NoiTruPhieuDieuTris.FirstOrDefault(p => p.Id == yeuCauVo.PhieuDieuTriId)?.NgayDieuTri ?? DateTime.Now;

            var yeuCauDichVuKyThuatCuoiCung = yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Where(p => p.TrangThai != EnumTrangThaiYeuCauDichVuKyThuat.DaHuy)
                                                                            .OrderByDescending(p => p.Id)
                                                                            .FirstOrDefault();
            //var yeuCauDichVuKyThuatCuoiCung = yeuCauKhamBenh.YeuCauDichVuKyThuats.OrderByDescending(x => x.Id).FirstOrDefault();
            yeuCauVo.YeuCauDichVuKyThuatCuoiCungId = yeuCauDichVuKyThuatCuoiCung == null ? 0 : yeuCauDichVuKyThuatCuoiCung.Id;


            var lstNhomDichVuBenhVien = _nhomDichVuBenhVienRepository.TableNoTracking.ToList();

            // xử lý get thông tin dịch vụ kỹ thuật và thêm mới
            foreach (var item in yeuCauVo.DichVuKyThuatBenhVienChiDinhs)
            {
                var itemObj = JsonConvert.DeserializeObject<ItemChiDinhDichVuKyThuatVo>(item);

                var newYeuCauDichVuKyThuat = new YeuCauDichVuKyThuat()
                {
                    DichVuKyThuatBenhVienId = itemObj.DichVuId,
                    NhomDichVuBenhVienId = itemObj.NhomId,
                    NoiThucHienId = itemObj.NoiThucHienId
                };

                var dvkt = _dichVuKyThuatBenhVienRepository.GetById(newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId, x => x.Include(o => o.DichVuKyThuatBenhVienGiaBaoHiems)
                    .Include(o => o.DichVuKyThuatVuBenhVienGiaBenhViens)
                    .Include(o => o.DichVuKyThuat));

                var dvktGiaBH = dvkt.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));
                //var dvktGiaBV = dvkt.DichVuKyThuatVuBenhVienGiaBenhViens.First(o => o.TuNgay <= DateTime.Now && (o.DenNgay == null || DateTime.Now <= o.DenNgay.Value));

                var cauHinhNhomGiaThuongBenhVien = _cauHinhService.GetSetting("CauHinhDichVuKyThuat.NhomGiaThuong");
                long.TryParse(cauHinhNhomGiaThuongBenhVien?.Value, out long nhomGiaThuongId);
                var dvktGiaBV = dvkt.DichVuKyThuatVuBenhVienGiaBenhViens
                    .Where(o => o.TuNgay.Date <= DateTime.Now.Date && (o.DenNgay == null || DateTime.Now.Date <= o.DenNgay.Value.Date))
                    .OrderByDescending(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomGiaThuongId)
                    .ThenBy(x => x.CreatedOn)
                    .First();

                var dtudDVKTBV = yeuCauTiepNhanChiTiet.DoiTuongUuDai?.DoiTuongUuDaiDichVuKyThuatBenhViens?.FirstOrDefault(o =>
                                            o.DichVuKyThuatBenhVienId == newYeuCauDichVuKyThuat.DichVuKyThuatBenhVienId && o.DichVuKyThuatBenhVien.CoUuDai == true);

                var duocHuongBaoHiem = coBHYT && dvktGiaBH != null && dvktGiaBH.Gia != 0;

                newYeuCauDichVuKyThuat.DuocHuongBaoHiem = duocHuongBaoHiem;
                newYeuCauDichVuKyThuat.BaoHiemChiTra = null;

                newYeuCauDichVuKyThuat.YeuCauTiepNhanId = yeuCauVo.YeuCauTiepNhanId;
                newYeuCauDichVuKyThuat.MaDichVu = dvkt.Ma;
                newYeuCauDichVuKyThuat.TenDichVu = dvkt.Ten;
                //newYeuCauDichVuKyThuat.DuocHuongBaoHiem = duocHuongBaoHiem;
                newYeuCauDichVuKyThuat.Gia = dvktGiaBV.Gia;
                newYeuCauDichVuKyThuat.NhomGiaDichVuKyThuatBenhVienId = dvktGiaBV.NhomGiaDichVuKyThuatBenhVienId;
                newYeuCauDichVuKyThuat.NhomChiPhi = dvkt.DichVuKyThuat != null ? dvkt.DichVuKyThuat.NhomChiPhi : EnumDanhMucNhomTheoChiPhi.DVKTThanhToanTheoTyLe;
                newYeuCauDichVuKyThuat.SoLan = 1;
                newYeuCauDichVuKyThuat.TiLeUuDai = dtudDVKTBV?.TiLeUuDai;
                newYeuCauDichVuKyThuat.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
                newYeuCauDichVuKyThuat.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                newYeuCauDichVuKyThuat.NhanVienChiDinhId = currentUserId;
                newYeuCauDichVuKyThuat.NoiChiDinhId = currentPhongLamViecId;
                newYeuCauDichVuKyThuat.ThoiDiemChiDinh = DateTime.Now;
                newYeuCauDichVuKyThuat.ThoiDiemDangKy = ngayDieuTri.Date == DateTime.Now.Date ? DateTime.Now : ngayDieuTri;
                newYeuCauDichVuKyThuat.NhomDichVuBenhVienId = dvkt.NhomDichVuBenhVienId;
                newYeuCauDichVuKyThuat.LoaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(newYeuCauDichVuKyThuat.NhomDichVuBenhVienId, lstNhomDichVuBenhVien);
                newYeuCauDichVuKyThuat.MaGiaDichVu = dvkt.DichVuKyThuat?.MaGia;
                newYeuCauDichVuKyThuat.TenGiaDichVu = dvkt.DichVuKyThuat?.TenGia;
                newYeuCauDichVuKyThuat.KhongTinhPhi = !yeuCauVo.KhongTinhPhi;
                newYeuCauDichVuKyThuat.NoiTruPhieuDieuTriId = phieuDieuTriId;

                // get người thực hiện mặc định

                var nguoiThucHien = await GetBacSiThucHienMacDinh(newYeuCauDichVuKyThuat.NoiThucHienId ?? 0);
                if (nguoiThucHien != null)
                {
                    newYeuCauDichVuKyThuat.NhanVienThucHienId = nguoiThucHien.KeyId;
                }

                if (dvktGiaBH != null)
                {
                    newYeuCauDichVuKyThuat.DonGiaBaoHiem = dvktGiaBH.Gia;
                    newYeuCauDichVuKyThuat.TiLeBaoHiemThanhToan = dvktGiaBH.TiLeBaoHiemThanhToan;
                }

                yeuCauTiepNhanChiTiet.YeuCauDichVuKyThuats.Add(newYeuCauDichVuKyThuat);
                //BaseRepository.Add(newYeuCauDichVuKyThuat);
                //await BaseRepository.AddAsync(newYeuCauDichVuKyThuat);
            }
        }

        private async Task<LookupItemVo> GetBacSiThucHienMacDinh(long noiThucHienId)
        {
            //todo: có cập nhật bỏ await
            var nhanVien = _hoatDongNhanVienRepository.TableNoTracking
                .Include(hd => hd.NhanVien).ThenInclude(nv => nv.User)
                .Include(hd => hd.NhanVien).ThenInclude(nv => nv.ChucDanh)
                .Include(hd => hd.PhongBenhVien)
                .Where(hd => hd.PhongBenhVienId == noiThucHienId && hd.NhanVien.ChucDanh.NhomChucDanhId == (long)EnumNhomChucDanh.BacSi)
                .Select(hd => hd.NhanVien)
                .Select(s => new LookupItemVo
                {
                    DisplayName = s.User.HoTen,
                    KeyId = s.Id
                })
                .FirstOrDefault();

            return nhanVien;
        }


        #region BVHD-3575

        public async Task XuLyTaoYeuCauNgoaiTruTheoNoiTru(YeuCauTiepNhan tiepNhanNoiTru)
        {
            var yeuCauTiepNhanNgoaiTru = new YeuCauTiepNhan
            {
                BenhNhanId = tiepNhanNoiTru.BenhNhanId,
                HoTen = tiepNhanNoiTru.HoTen,
                NgaySinh = tiepNhanNoiTru.NgaySinh,
                ThangSinh = tiepNhanNoiTru.ThangSinh,
                NamSinh = tiepNhanNoiTru.NamSinh,
                SoChungMinhThu = tiepNhanNoiTru.SoChungMinhThu,
                GioiTinh = tiepNhanNoiTru.GioiTinh,
                NhomMau = tiepNhanNoiTru.NhomMau,
                YeuToRh = tiepNhanNoiTru.YeuToRh,
                NgheNghiepId = tiepNhanNoiTru.NgheNghiepId,
                NoiLamViec = tiepNhanNoiTru.NoiLamViec,
                QuocTichId = tiepNhanNoiTru.QuocTichId,
                DanTocId = tiepNhanNoiTru.DanTocId,
                DiaChi = tiepNhanNoiTru.DiaChi,
                PhuongXaId = tiepNhanNoiTru.PhuongXaId,
                QuanHuyenId = tiepNhanNoiTru.QuanHuyenId,
                TinhThanhId = tiepNhanNoiTru.TinhThanhId,
                SoDienThoai = tiepNhanNoiTru.SoDienThoai,
                Email = tiepNhanNoiTru.Email,
                NoiGioiThieuId = tiepNhanNoiTru.NoiGioiThieuId,
                HinhThucDenId = tiepNhanNoiTru.HinhThucDenId,
                DuocUuDai = tiepNhanNoiTru.DuocUuDai,
                DoiTuongUuDaiId = tiepNhanNoiTru.DoiTuongUuDaiId,
                CongTyUuDaiId = tiepNhanNoiTru.CongTyUuDaiId,
                NguoiLienHeHoTen = tiepNhanNoiTru.NguoiLienHeHoTen,
                NguoiLienHeQuanHeNhanThanId = tiepNhanNoiTru.NguoiLienHeQuanHeNhanThanId,
                NguoiLienHeSoDienThoai = tiepNhanNoiTru.NguoiLienHeSoDienThoai,
                NguoiLienHeEmail = tiepNhanNoiTru.NguoiLienHeEmail,
                NguoiLienHeDiaChi = tiepNhanNoiTru.NguoiLienHeDiaChi,
                NguoiLienHePhuongXaId = tiepNhanNoiTru.NguoiLienHePhuongXaId,
                NguoiLienHeQuanHuyenId = tiepNhanNoiTru.NguoiLienHeQuanHuyenId,
                NguoiLienHeTinhThanhId = tiepNhanNoiTru.NguoiLienHeTinhThanhId,
                CoBHYT = tiepNhanNoiTru.CoBHYT,
                BHYTMaSoThe = tiepNhanNoiTru.BHYTMaSoThe,
                BHYTMucHuong = tiepNhanNoiTru.BHYTMucHuong,
                BHYTMaDKBD = tiepNhanNoiTru.BHYTMaDKBD,
                BHYTNgayHieuLuc = tiepNhanNoiTru.BHYTNgayHieuLuc,
                BHYTNgayHetHan = tiepNhanNoiTru.BHYTNgayHetHan,
                BHYTDiaChi = tiepNhanNoiTru.BHYTDiaChi,
                BHYTCoQuanBHXH = tiepNhanNoiTru.BHYTCoQuanBHXH,
                BHYTNgayDu5Nam = tiepNhanNoiTru.BHYTNgayDu5Nam,
                BHYTNgayDuocMienCungChiTra = tiepNhanNoiTru.BHYTNgayDuocMienCungChiTra,
                BHYTMaKhuVuc = tiepNhanNoiTru.BHYTMaKhuVuc,
                BHYTDuocMienCungChiTra = tiepNhanNoiTru.BHYTDuocMienCungChiTra,
                BHYTGiayMienCungChiTraId = tiepNhanNoiTru.BHYTGiayMienCungChiTraId,
                CoBHTN = tiepNhanNoiTru.CoBHTN,
                LoaiYeuCauTiepNhan = Enums.EnumLoaiYeuCauTiepNhan.KhamChuaBenhNgoaiTru,
                MaYeuCauTiepNhan = tiepNhanNoiTru.MaYeuCauTiepNhan,
                ThoiDiemTiepNhan = tiepNhanNoiTru.ThoiDiemTiepNhan, // DateTime.Now,
                LyDoVaoVien = tiepNhanNoiTru.LyDoVaoVien ?? Enums.EnumLyDoVaoVien.DungTuyen,
                TrieuChungTiepNhan = tiepNhanNoiTru.TrieuChungTiepNhan,
                LoaiTaiNan = tiepNhanNoiTru.LoaiTaiNan,
                DuocChuyenVien = tiepNhanNoiTru.DuocChuyenVien,
                GiayChuyenVienId = tiepNhanNoiTru.GiayChuyenVienId,
                ThoiGianChuyenVien = tiepNhanNoiTru.ThoiGianChuyenVien,
                NoiChuyenId = tiepNhanNoiTru.NoiChuyenId,
                SoChuyenTuyen = tiepNhanNoiTru.SoChuyenTuyen,
                TuyenChuyen = tiepNhanNoiTru.TuyenChuyen,
                LyDoChuyen = tiepNhanNoiTru.LyDoChuyen,
                DoiTuongUuTienKhamChuaBenhId = tiepNhanNoiTru.DoiTuongUuTienKhamChuaBenhId,
                TrangThaiYeuCauTiepNhan = Enums.EnumTrangThaiYeuCauTiepNhan.DangThucHien,
                ThoiDiemCapNhatTrangThai = tiepNhanNoiTru.ThoiDiemCapNhatTrangThai, // DateTime.Now,
                TinhTrangThe = tiepNhanNoiTru.TinhTrangThe,
                IsCheckedBHYT = tiepNhanNoiTru.IsCheckedBHYT,
                TuNhap = tiepNhanNoiTru.TuNhap,
                NguoiGioiThieuId = tiepNhanNoiTru.NguoiGioiThieuId,
                QuyetToanTheoNoiTru = true,
                NhanVienTiepNhanId = tiepNhanNoiTru.NhanVienTiepNhanId
            };

            var cauHinhLyDoTiepNhanLaKham = _cauHinhService.GetSetting("CauHinhKhamSucKhoe.LyDoTiepNhanLaKhamBenh");
            long.TryParse(cauHinhLyDoTiepNhanLaKham?.Value, out long lyDoTiepNhanKhamBenhId);
            yeuCauTiepNhanNgoaiTru.LyDoTiepNhanId = lyDoTiepNhanKhamBenhId == 0 ? (long?)null : lyDoTiepNhanKhamBenhId;

            foreach (var yeuCauTiepNhanCongTyBaoHiemTuNhan in tiepNhanNoiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans)
            {
                yeuCauTiepNhanNgoaiTru.YeuCauTiepNhanCongTyBaoHiemTuNhans.Add(new YeuCauTiepNhanCongTyBaoHiemTuNhan
                {
                    CongTyBaoHiemTuNhanId = yeuCauTiepNhanCongTyBaoHiemTuNhan.CongTyBaoHiemTuNhanId,
                    MaSoThe = yeuCauTiepNhanCongTyBaoHiemTuNhan.MaSoThe,
                    DiaChi = yeuCauTiepNhanCongTyBaoHiemTuNhan.DiaChi,
                    SoDienThoai = yeuCauTiepNhanCongTyBaoHiemTuNhan.SoDienThoai,
                    NgayHieuLuc = yeuCauTiepNhanCongTyBaoHiemTuNhan.NgayHieuLuc,
                    NgayHetHan = yeuCauTiepNhanCongTyBaoHiemTuNhan.NgayHetHan
                });
            }

            tiepNhanNoiTru.YeuCauTiepNhanNgoaiTruCanQuyetToan = yeuCauTiepNhanNgoaiTru;
            BaseRepository.Context.SaveChanges();
        }

        public async Task<NoiTruPhieuDieuTri> GetNoiTruPhieuDieuTriAsync(long phieuDieuTriId)
        {
            var phieuDieuTri =
                await _noiTruPhieuDieuTriRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Id == phieuDieuTriId);
            return phieuDieuTri;
        }

        public async Task<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> GetNoiTruBenhAnAsync(string maTiepNhan)
        {
            var benhAn = _noiTruBenhAnRepository.TableNoTracking
                .FirstOrDefault(x => x.YeuCauTiepNhan.MaYeuCauTiepNhan.Equals(maTiepNhan ?? ""));
            return benhAn;
        }
        #endregion

        #region BVHD-3916
        public async Task<string> GetGhiChuCanLamSangTheoPhieuDieuTri(long noiTruPhieuDieuTriId)
        {
            var ghiChu = _noiTruPhieuDieuTriRepository.TableNoTracking
                .Where(x => x.Id == noiTruPhieuDieuTriId)
                .Select(x => x.GhiChuCanLamSang)
                .FirstOrDefault();
            return ghiChu;
        }

        public async Task CapNhatGhiChuCanLamSangAsync(GhiChuCanLamSangVo updateVo)
        {
            var phieuDieuTri = _noiTruPhieuDieuTriRepository.Table
                .FirstOrDefault(x => x.Id == updateVo.NoiTruPhieuDieuTriId);
            if (phieuDieuTri != null)
            {
                phieuDieuTri.GhiChuCanLamSang = updateVo.GhiChuCanLamSang;
            }

            _noiTruPhieuDieuTriRepository.Context.SaveChanges();
        }
        #endregion
    }
}
