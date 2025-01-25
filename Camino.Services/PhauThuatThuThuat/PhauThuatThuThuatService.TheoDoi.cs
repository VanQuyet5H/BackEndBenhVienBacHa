using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial class PhauThuatThuThuatService
    {
        public async Task<string> GetTemplateKhamTheoDoi()
        {
            return await _templateKhamTheoDoiRepository.TableNoTracking.Select(p => p.ComponentDynamics).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckPreviousKhamTheoDoi(long theoDoiSauPhauThuatThuThuatId)
        {
            var khamTheoDoi = await _khamTheoDoiRepository.TableNoTracking.Where(p => p.TheoDoiSauPhauThuatThuThuatId == theoDoiSauPhauThuatThuThuatId)
                                                                          .OrderByDescending(p => p.Id)
                                                                          .FirstOrDefaultAsync();

            if(khamTheoDoi == null)
            {
                return true;
            }
            else
            {
                return khamTheoDoi.ThoiDiemHoanThanhKham != null ? true : false;
            }
        }

        public async Task BenhNhanTuVongKhiTheoDoi(long? yeuCauTiepNhanId, long? theoDoiSauPhauThuatThuThuatId, long? nhanVienKetLuanId, long? phongBenhVienId, EnumThoiGianTuVongPTTTTheoNgay? thoiGianTuVong, EnumTuVongPTTTTheoNgay? tuVong)
        {
            DateTime thoiDiemTuVong = DateTime.Now;

            //Update TheoDoiSauPhauThuatThuThuat
            var theoDoiSauPhauThuatThuThuat = await _theoDoiSauPhauThuatThuThuatRepository.TableNoTracking.Where(p => p.Id == theoDoiSauPhauThuatThuThuatId).FirstOrDefaultAsync();

            theoDoiSauPhauThuatThuThuat.ThoiDiemKetThucTheoDoi = thoiDiemTuVong;
            theoDoiSauPhauThuatThuThuat.TrangThai = EnumTrangThaiTheoDoiSauPhauThuatThuThuat.KetThucTheoDoi;
            theoDoiSauPhauThuatThuThuat.TuVongTrongPTTT = tuVong;
            theoDoiSauPhauThuatThuThuat.KhoangThoiGianTuVong = thoiGianTuVong;
            theoDoiSauPhauThuatThuThuat.ThoiDiemTuVong = thoiDiemTuVong;

            await _theoDoiSauPhauThuatThuThuatRepository.UpdateAsync(theoDoiSauPhauThuatThuThuat);

            //Update YeuCauDichVuKyThuat cho những lần mới
            var lstYeuCauDichVuKyThuatMoi = await BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                            p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat &&
                                                                                            p.TheoDoiSauPhauThuatThuThuatId == theoDoiSauPhauThuatThuThuatId &&
                                                                                            p.NoiThucHienId == phongBenhVienId)
                                                                                .ToListAsync();

            //int lanThucHienHienTai = 0;
            //int? lanThucHienTruoc = await BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            //                                                                        p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
            //                                                            .OrderByDescending(p => p.LanThucHien)
            //                                                            .Select(p => p.LanThucHien)
            //                                                            .FirstOrDefaultAsync();

            //lanThucHienHienTai = lanThucHienTruoc == null ? 0 : lanThucHienTruoc.GetValueOrDefault() + 1;

            foreach (var item in lstYeuCauDichVuKyThuatMoi)
            {
                //item.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
                item.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.DaThucHien;
                item.ThoiDiemHoanThanh = thoiDiemTuVong;
                item.NhanVienKetLuanId = nhanVienKetLuanId;
                //item.LanThucHien = lanThucHienHienTai;

                await BaseRepository.UpdateAsync(item);
            }

            ////Update tất cả YeuCauDichVuKyThuat PTTT
            //var lstYeuCauDichVuKyThuat = await BaseRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
            //                                                                             p.LoaiDichVuKyThuat == LoaiDichVuKyThuat.ThuThuatPhauThuat)
            //                                                                 .ToListAsync();

            //foreach (var item in lstYeuCauDichVuKyThuat)
            //{
            //    item.TrangThai = EnumTrangThaiYeuCauDichVuKyThuat.ChuaThucHien;
            //    await BaseRepository.UpdateAsync(item);
            //}

            //Update YeuCauTiepNhan
            var yeuCauTiepNhan = await _yeuCauTiepNhanRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId).FirstOrDefaultAsync();

            yeuCauTiepNhan.TuVongTrongPTTT = tuVong;
            yeuCauTiepNhan.KhoangThoiGianTuVong = thoiGianTuVong;
            yeuCauTiepNhan.ThoiDiemTuVong = thoiDiemTuVong;

            await _yeuCauTiepNhanRepository.UpdateAsync(yeuCauTiepNhan);

            //Xoá hàng đợi
            var lstPhongBenhVienHangDoi = await _phongBenhVienHangDoiRepository.TableNoTracking
                .Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                            p.PhongBenhVienId == phongBenhVienId &&
                            p.YeuCauDichVuKyThuat != null &&
                            p.YeuCauDichVuKyThuat.TheoDoiSauPhauThuatThuThuatId == theoDoiSauPhauThuatThuThuatId)
                .ToListAsync();

            await _phongBenhVienHangDoiRepository.DeleteAsync(lstPhongBenhVienHangDoi);
        }
    }
}