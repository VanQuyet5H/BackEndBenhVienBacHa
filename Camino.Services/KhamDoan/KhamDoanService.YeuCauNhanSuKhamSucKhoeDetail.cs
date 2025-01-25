using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Services.KhamDoan
{
    public partial class KhamDoanService
    {
        public async Task<KhamDoanHopDongVaDiaDiemResultVo> GetHdKhamVaDiaDiemAsync(long id)
        {
            var query = _hopDongKhamSucKhoeRepository.TableNoTracking
                .Where(q => q.Id == id)
                .Select(q => new KhamDoanHopDongVaDiaDiemResultVo
                {
                    SoNguoiKham = q.SoNguoiKham,
                    NgayHieuLuc = q.NgayHieuLuc,
                    NgayKetThuc = q.NgayKetThuc
                });

            var queryList = _hopDongKhamSucKhoeDiaDiemRepository.TableNoTracking
                .Where(q => q.HopDongKhamSucKhoeId == id)
                .Select(q => new KhamDoanHopDongDiaDiemResultVo
                {
                    Id = q.Id,
                    GhiChu = q.GhiChu,
                    Ngay = q.Ngay,
                    CongViec = q.CongViec,
                    DiaDiem = q.DiaDiem
                });

            var queryTask = query.FirstOrDefaultAsync();
            var queryListTask = queryList.ToListAsync();

            await Task.WhenAll(queryTask, queryListTask);

            queryTask.Result.KhamDoanHopDongDiaDiems = queryListTask.Result.ToList();

            return queryTask.Result;
        }

        public async Task<KhamDoanThongTinNhanVienLienQuanResultVo> GetNhanVienRelatedInfoAsync(long id)
        {
            var query = _khoaPhongNhanVienRepository.TableNoTracking
                .Where(q => q.NhanVienId == id)
                .Select(q => new KhamDoanThongTinNhanVienLienQuanResultVo
                {
                    DonVi = q.KhoaPhong.Ten
                }).Distinct();

            var queryDetail = _nhanVienRepository.TableNoTracking.Where(q => q.Id == id)
                .Select(q => q.User.SoDienThoai);

            var khoaPhongList = query.ToListAsync();
            var sdt = queryDetail.FirstOrDefaultAsync();

            await Task.WhenAll(khoaPhongList, sdt);
            var donViResult = string.Empty;

            if (khoaPhongList.Result.Any())
            {
                if (khoaPhongList.Result.Count > 1)
                {
                    khoaPhongList.Result.ForEach(item => donViResult += item.DonVi + "; ");
                }
                if (khoaPhongList.Result.Count == 1)
                {
                    khoaPhongList.Result.ForEach(item => donViResult += item.DonVi);
                }
            }

            return new KhamDoanThongTinNhanVienLienQuanResultVo
            {
                SoDienThoai = sdt.Result,
                DonVi = donViResult
            };
        }

        public async Task<string> GetNhanVienAsync(long nhanVienId)
        {
            return await _userRepository.TableNoTracking.Where(q => q.Id == nhanVienId)
                .Select(q => q.HoTen).FirstOrDefaultAsync();
        }

        public async Task<long> GetLoaiNhanVienAsync(long nhanVienId)
        {
            return await _nhanVienRepository.TableNoTracking.Where(q => q.Id == nhanVienId)
                .Select(q => q.ChucDanhId != null ? q.ChucDanh.NhomChucDanhId : 0).FirstOrDefaultAsync();
        }

        public async Task ThemYeuCauNhanSuAsync(YeuCauNhanSuKhamSucKhoe yeuCauNhanSuKhamSucKhoeEntity)
        {
            yeuCauNhanSuKhamSucKhoeEntity.NhanVienGuiYeuCauId = _userAgentHelper.GetCurrentUserId();
            yeuCauNhanSuKhamSucKhoeEntity.NgayGuiYeuCau = DateTime.Now;

            await _yeuCauNhanSuKhamSucKhoeRepository.AddAsync(yeuCauNhanSuKhamSucKhoeEntity);
        }

        public async Task UpdateYeuCauNhanSuAsync(YeuCauNhanSuKhamSucKhoe yeuCauNhanSuKhamSucKhoeEntity)
        {
            yeuCauNhanSuKhamSucKhoeEntity.NhanVienGuiYeuCauId = _userAgentHelper.GetCurrentUserId();
            yeuCauNhanSuKhamSucKhoeEntity.NgayGuiYeuCau = DateTime.Now;

            await _yeuCauNhanSuKhamSucKhoeRepository.UpdateAsync(yeuCauNhanSuKhamSucKhoeEntity);
        }

        public async Task<YeuCauNhanSuKhamSucKhoe> GetByYeuCauNhanSuKhamSucKhoeIdAsync
        (long id, Func<IQueryable<YeuCauNhanSuKhamSucKhoe>, IIncludableQueryable<YeuCauNhanSuKhamSucKhoe, object>> includes = null)
        {
            return await _yeuCauNhanSuKhamSucKhoeRepository.GetByIdAsync(id, includes);
        }

        public TrangThaiKhamDoanAndSoLuongResultVo GetTrangThaiAndSoLuong(
            YeuCauNhanSuKhamSucKhoe ycNhanSuKhamSucKhoeEntity)
        {
            var soLuongBs =
                ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w =>
                    w.NhanVien != null && w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 1);

            var soLuongDd =
                ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w =>
                    w.NhanVien != null && w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 5);

            var soLuongNvKhac =
                ycNhanSuKhamSucKhoeEntity.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w =>
                    w.NhanVien != null && w.NhanVien.ChucDanh != null &&
                    w.NhanVien.ChucDanh.NhomChucDanhId != 1 && w.NhanVien.ChucDanh.NhomChucDanhId != 5 || w.NhanVien == null || w.NhanVien.ChucDanh == null);

            if (ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet == null && ycNhanSuKhamSucKhoeEntity.DuocNhanSuDuyet == null &&
                ycNhanSuKhamSucKhoeEntity.DuocGiamDocDuyet == null ||
                ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet != false && ycNhanSuKhamSucKhoeEntity.DuocNhanSuDuyet == null &&
                ycNhanSuKhamSucKhoeEntity.DuocGiamDocDuyet == null ||
                ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet != false && ycNhanSuKhamSucKhoeEntity.DuocNhanSuDuyet != false &&
                ycNhanSuKhamSucKhoeEntity.DuocGiamDocDuyet == null)
            {
                return new TrangThaiKhamDoanAndSoLuongResultVo
                {
                    TongNvKhac = soLuongNvKhac,
                    TongSoBs = soLuongBs,
                    TongSoDd = soLuongDd,
                    TrangThai = EnumTrangThaiKhamDoan.ChoDuyet
                };
            }

            if (ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet == true && ycNhanSuKhamSucKhoeEntity.DuocNhanSuDuyet == true &&
                ycNhanSuKhamSucKhoeEntity.DuocGiamDocDuyet == true)
            {
                return new TrangThaiKhamDoanAndSoLuongResultVo
                {
                    TongNvKhac = soLuongNvKhac,
                    TongSoBs = soLuongBs,
                    TongSoDd = soLuongDd,
                    TrangThai = EnumTrangThaiKhamDoan.DaDuyet
                };
            }

            if (ycNhanSuKhamSucKhoeEntity.DuocKHTHDuyet == false ||
                ycNhanSuKhamSucKhoeEntity.DuocNhanSuDuyet == false ||
                ycNhanSuKhamSucKhoeEntity.DuocGiamDocDuyet == false)
            {
                return new TrangThaiKhamDoanAndSoLuongResultVo
                {
                    TongNvKhac = soLuongNvKhac,
                    TongSoBs = soLuongBs,
                    TongSoDd = soLuongDd,
                    TrangThai = EnumTrangThaiKhamDoan.TuChoi
                };
            }
            return new TrangThaiKhamDoanAndSoLuongResultVo
            {
                TongNvKhac = soLuongNvKhac,
                TongSoBs = soLuongBs,
                TongSoDd = soLuongDd,
                TrangThai = null
            };
        }

        public async Task DeleteByYcNhanSuKhamSucKhoeIdAsync(long id)
        {
            await _yeuCauNhanSuKhamSucKhoeRepository.DeleteByIdAsync(id);
        }
    }
}