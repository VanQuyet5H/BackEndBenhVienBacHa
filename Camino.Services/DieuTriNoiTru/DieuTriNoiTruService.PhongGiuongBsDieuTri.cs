using Camino.Core.Data;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain;

namespace Camino.Services.DieuTriNoiTru
{
    public partial class DieuTriNoiTruService
    {
        //public bool IsDaChiDinhBacSiVaGiuong(long yeuCauTiepNhanId)
        //{
        //    //Update cấp giường cho trẻ em
        //    var khoaSan = _cauHinhService.GetSetting("CauHinhNoiTru.KhoaPhuSan");
        //    long.TryParse(khoaSan?.Value, out long khoaSanId);

        //    var yeuCauTiepNhan = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId &&
        //                                                                   p.NoiTruBenhAn.NoiTruEkipDieuTris.Any())
        //                                                       .Include(p => p.NoiTruBenhAn).ThenInclude(p => p.NoiTruKhoaPhongDieuTris)
        //                                                       .Include(p => p.YeuCauDichVuGiuongBenhViens)
        //                                                       .FirstOrDefault();

        //    if(yeuCauTiepNhan == null)
        //    {
        //        return false;
        //    }

        //    return yeuCauTiepNhan.NoiTruBenhAn.LoaiBenhAn != LoaiBenhAn.TreSoSinh ?
        //        yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan) : 
        //        (
        //            yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Any(p => p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan) ||
        //            (
        //                yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.LastOrDefault() != null &&
        //                yeuCauTiepNhan.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.LastOrDefault().KhoaPhongChuyenDenId == khoaSanId
        //            )
        //        );

        //    //Lỗi Rewriting child expression from type 'System.Nullable`1[System.Int64]' to type 'System.Collections.Generic.IEnumerable`1[System.Nullable`1[System.Int64]]' is not allowed
        //    //Chưa biết cách fix
        //    //return BaseRepository.TableNoTracking.Any(p => p.Id == yeuCauTiepNhanId &&
        //    //                                               p.NoiTruBenhAn.NoiTruEkipDieuTris.Any() &&
        //    //                                               (
        //    //                                                    //nếu BA != sơ sinh thì bắt buộc phải có giường
        //    //                                                    (p.NoiTruBenhAn.LoaiBenhAn != LoaiBenhAn.TreSoSinh && p.YeuCauDichVuGiuongBenhViens.Any(p2 => p2.DoiTuongSuDung == DoiTuongSuDung.BenhNhan)) ||
        //    //                                                    //nếu BA = sơ sinh thì nếu đang nằm tại khoa sản thì ko cần chỉ định giường (nằm chung với mẹ), còn nếu nằm khác khoa sản thì bắt buộc phải có giường
        //    //                                                    (p.NoiTruBenhAn.LoaiBenhAn == LoaiBenhAn.TreSoSinh && (
        //    //                                                        (p.NoiTruBenhAn.NoiTruKhoaPhongDieuTris.LastOrDefault().KhoaPhongChuyenDenId == khoaSanId) ||
        //    //                                                        (p.YeuCauDichVuGiuongBenhViens.Any(p2 => p2.DoiTuongSuDung == DoiTuongSuDung.BenhNhan))
        //    //                                                    ))
        //    //                                               ));
        //}

        public bool IsDaChiDinhBacSi(long yeuCauTiepNhanId)
        {
            return BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId &&
                                                             p.NoiTruBenhAn.NoiTruEkipDieuTris.Any())
                                                 .Any();
        }

        public bool IsDaChiDinhGiuong(long yeuCauTiepNhanId)
        {
            return BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId &&
                                                             p.YeuCauDichVuGiuongBenhViens.Any(p2 => p2.DoiTuongSuDung == DoiTuongSuDung.BenhNhan))
                                                 .Any();
        }

        public bool IsDichVuGiuongAvailable(long dichVuGiuongId, DateTime thoiGianNhan)
        {
            return _dichVuGiuongBenhVienRepository.TableNoTracking.Any(p => p.Id == dichVuGiuongId &&
                                                                            p.HieuLuc &&
                                                                            p.DichVuGiuongBenhVienGiaBenhViens.Any(o => o.TuNgay.Date <= thoiGianNhan.Date && (o.DenNgay == null || thoiGianNhan.Date <= o.DenNgay.Value.Date)));
        }

        public bool IsGiuongAvailable(long giuongId, long? yeuCauDichVuGiuongBenhVienId, DateTime? thoiGianNhan, DateTime? thoiGianTra)
        {
            var maxBenhNhan = int.Parse(_cauHinhService.GetSetting("CauHinhNoiTru.SoLuongBenhNhanToiDaTrenGiuong").Value);

            var hoatDongGiuongBenhs = _hoatDongGiuongBenhRepository.TableNoTracking.Where(p => p.GiuongBenhId == giuongId &&
                                                                                               (yeuCauDichVuGiuongBenhVienId == null || p.YeuCauDichVuGiuongBenhVienId != yeuCauDichVuGiuongBenhVienId) && (
                                                                                                    (
                                                                                                        thoiGianNhan == null ||
                                                                                                        ((p.ThoiDiemBatDau <= thoiGianNhan.Value && (p.ThoiDiemKetThuc == null || thoiGianNhan.Value <= p.ThoiDiemKetThuc)) ||
                                                                                                        p.ThoiDiemBatDau >= thoiGianNhan.Value)
                                                                                                    ) &&
                                                                                                    (
                                                                                                        thoiGianTra == null ||
                                                                                                        (p.ThoiDiemBatDau <= thoiGianTra.Value)
                                                                                                    )
                                                                                               ))
                                                                                  .ToList();

            return hoatDongGiuongBenhs.Count < maxBenhNhan;
        }

        public DateTime GetThoiDiemNhapVien(long yeuCauTiepNhanId)
        {
            var thoiDiemNhapVien = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                                                                 .Select(p => p.NoiTruBenhAn.ThoiDiemNhapVien)
                                                                 .FirstOrDefault();

            if (thoiDiemNhapVien.Second > 0 || thoiDiemNhapVien.Millisecond > 0)
            {
                return new DateTime(thoiDiemNhapVien.Year, thoiDiemNhapVien.Month, thoiDiemNhapVien.Day, thoiDiemNhapVien.Hour, thoiDiemNhapVien.Minute, 0, 0).AddMinutes(1);
            }

            return thoiDiemNhapVien;
        }

        public async Task<List<ChanDoanICD>> GetChanDoanICD(DropDownListRequestModel model)
        {
            var lstICD = await _icdRepository.TableNoTracking.Select(item => new ChanDoanICD
            {
                KeyId = item.Id,
                Ma = item.Ma,
                TenTiengViet = item.TenTiengViet,
                TenTiengAnh = item.TenTiengAnh
            })
                                                             .ApplyLike(model.Query, p => p.Ma, p => p.TenTiengViet, p => p.TenTiengAnh)
                                                             .Take(model.Take)
                                                             .ToListAsync();

            return lstICD;
        }

        #region Bác sĩ điều trị
        public async Task<long> GetFirstEkipDieuTriId(long noiTruBenhAnId)
        {
            var noiTruEkipDieuTri = await _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruBenhAnId)
                                                                                      .FirstOrDefaultAsync();

            return noiTruEkipDieuTri?.Id ?? (long)0;
        }

        public async Task<GridDataSource> GetDanhSachEkipDieuTriForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == yeuCauTiepNhanId)
                                                                    .Select(p => new EkipDieuTriGridVo
                                                                    {
                                                                        Id = p.Id,
                                                                        NoiTruBenhAnId = p.NoiTruBenhAnId,
                                                                        BacSiId = p.BacSiId,
                                                                        BacSiDisplay = p.BacSi.User.HoTen,
                                                                        DieuDuongId = p.DieuDuongId,
                                                                        DieuDuongDisplay = p.DieuDuong.User.HoTen,
                                                                        KhoaPhongDieuTriId = p.KhoaPhongDieuTriId,
                                                                        KhoaPhongDieuTriDisplay = p.KhoaPhongDieuTri.Ten,
                                                                        NhanVienLapId = p.NhanVienLapId,
                                                                        NhanVienLapDisplay = p.NhanVienLap.User.HoTen,
                                                                        TuNgay = p.TuNgay,
                                                                        DenNgay = p.DenNgay,
                                                                        IsFirstData = false
                                                                    });

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            var firstEkipDieuTriId = await GetFirstEkipDieuTriId(yeuCauTiepNhanId);

            var thoiDiemRaVien = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                                                               .Select(p => p.NoiTruBenhAn.ThoiDiemRaVien)
                                                               .FirstOrDefault();

            foreach (var item in queryTask.Result)
            {
                if (item.Id == firstEkipDieuTriId)
                {
                    item.IsFirstData = true;
                }

                if (thoiDiemRaVien != null && item.DenNgay == null)
                {
                    item.DenNgay = thoiDiemRaVien;
                }
            }

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachEkipDieuTriForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = await _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == yeuCauTiepNhanId)
                                                                          .CountAsync();

            return new GridDataSource
            {
                TotalRowCount = query
            };
        }

        public bool KiemTraTonTaiLichBacSiDieuTriTuNgay(DateTime? tuNgay, long noiTruBenhAnId, long noiTruEkipDieuTriId)
        {
            if (tuNgay == null)
            {
                return true;
            }

            return !_noiTruEkipDieuTriRepository.TableNoTracking.Any(p => p.NoiTruBenhAnId == noiTruBenhAnId &&
                                                                          p.Id != noiTruEkipDieuTriId &&
                                                                          (p.TuNgay == tuNgay.Value ||
                                                                          (p.DenNgay != null && tuNgay.Value >= p.TuNgay && tuNgay.Value <= p.DenNgay)));
        }

        public bool KiemTraTuNgayVoiNgayNhapVien(DateTime? tuNgay, long noiTruBenhAnId)
        {
            if (tuNgay == null)
            {
                return true;
            }

            return !(tuNgay.Value < _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == noiTruBenhAnId)
                                                                           .Select(p => p.ThoiDiemNhapVien)
                                                                           .FirstOrDefault());
        }

        public List<NoiTruEkipDieuTri> XuLyThemBacSiDieuTri(NoiTruEkipDieuTri noiTruEkipDieuTri)
        {
            var lstNoiTruEkipDieuTri = new List<NoiTruEkipDieuTri>();

            var lichDieuTriTruocTuNgayGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                        p.DenNgay == null &&
                                                                                                        p.TuNgay < noiTruEkipDieuTri.TuNgay)
                                                                                            .FirstOrDefault();

            if (lichDieuTriTruocTuNgayGanNhat == null)
            {
                //x...[] hoặc x...[
                if (noiTruEkipDieuTri.DenNgay == null)
                {
                    var lichDieuTriSauTuNgayGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                              p.TuNgay > noiTruEkipDieuTri.TuNgay)
                                                                                                  .OrderBy(p => p.TuNgay)
                                                                                                  .FirstOrDefault();

                    if (lichDieuTriSauTuNgayGanNhat != null)
                    {
                        noiTruEkipDieuTri.DenNgay = lichDieuTriSauTuNgayGanNhat.TuNgay.AddMinutes(-1);
                    }
                }
            }
            else
            {
                //[...x
                lichDieuTriTruocTuNgayGanNhat.DenNgay = noiTruEkipDieuTri.TuNgay.AddMinutes(-1);
                lstNoiTruEkipDieuTri.Add(lichDieuTriTruocTuNgayGanNhat);
            }

            lstNoiTruEkipDieuTri.Add(noiTruEkipDieuTri);

            return lstNoiTruEkipDieuTri;
        }

        public List<NoiTruEkipDieuTri> XuLySuaBacSiDieuTri(NoiTruEkipDieuTri noiTruEkipDieuTri)
        {
            var lstNoiTruEkipDieuTri = new List<NoiTruEkipDieuTri>();

            var oldNoiTruEkipDieuTri = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.Id == noiTruEkipDieuTri.Id)
                                                                                   .FirstOrDefault();

            var oldLichDieuTriTruocGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == oldNoiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                     p.Id != oldNoiTruEkipDieuTri.Id &&
                                                                                                     p.TuNgay < oldNoiTruEkipDieuTri.TuNgay)
                                                                                         .OrderByDescending(p => p.TuNgay)
                                                                                         .FirstOrDefault();

            var newLichDieuTriTruocGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                     p.Id != noiTruEkipDieuTri.Id &&
                                                                                                     p.TuNgay < noiTruEkipDieuTri.TuNgay)
                                                                                         .OrderByDescending(p => p.TuNgay)
                                                                                         .FirstOrDefault();

            if (oldLichDieuTriTruocGanNhat != null && (newLichDieuTriTruocGanNhat == null || oldLichDieuTriTruocGanNhat.Id != newLichDieuTriTruocGanNhat.Id))
            {
                var oldLichDieuTriSauGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == oldNoiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                       p.Id != oldNoiTruEkipDieuTri.Id &&
                                                                                                       p.TuNgay > oldNoiTruEkipDieuTri.TuNgay)
                                                                                           .OrderBy(p => p.TuNgay)
                                                                                           .FirstOrDefault();

                if (oldLichDieuTriSauGanNhat != null)
                {
                    oldLichDieuTriTruocGanNhat.DenNgay = oldLichDieuTriSauGanNhat.TuNgay.AddMinutes(-1);
                }
                else
                {
                    oldLichDieuTriTruocGanNhat.DenNgay = null;
                }

                lstNoiTruEkipDieuTri.Add(oldLichDieuTriTruocGanNhat);
            }

            if (newLichDieuTriTruocGanNhat != null)
            {
                newLichDieuTriTruocGanNhat.DenNgay = noiTruEkipDieuTri.TuNgay.AddMinutes(-1);
                lstNoiTruEkipDieuTri.Add(newLichDieuTriTruocGanNhat);
            }

            var newLichDieuTriSauGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                   p.Id != noiTruEkipDieuTri.Id &&
                                                                                                   p.TuNgay > noiTruEkipDieuTri.TuNgay)
                                                                                       .OrderBy(p => p.TuNgay)
                                                                                       .FirstOrDefault();

            if (newLichDieuTriSauGanNhat != null)
            {
                noiTruEkipDieuTri.DenNgay = newLichDieuTriSauGanNhat.TuNgay.AddMinutes(-1);
            }

            lstNoiTruEkipDieuTri.Add(noiTruEkipDieuTri);

            return lstNoiTruEkipDieuTri;
        }

        public List<NoiTruEkipDieuTri> XuLyXoaBacSiDieuTri(NoiTruEkipDieuTri noiTruEkipDieuTri)
        {
            var lstNoiTruEkipDieuTri = new List<NoiTruEkipDieuTri>();

            var lichDieuTriTruocTuNgayGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                        p.Id != noiTruEkipDieuTri.Id &&
                                                                                                        p.TuNgay < noiTruEkipDieuTri.TuNgay)
                                                                                            .OrderByDescending(p => p.TuNgay)
                                                                                            .FirstOrDefault();

            if (lichDieuTriTruocTuNgayGanNhat != null)
            {
                var lichDieuTriSauTuNgayGanNhat = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruEkipDieuTri.NoiTruBenhAnId &&
                                                                                                          p.Id != noiTruEkipDieuTri.Id &&
                                                                                                          p.TuNgay > noiTruEkipDieuTri.TuNgay)
                                                                                              .OrderBy(p => p.TuNgay)
                                                                                              .FirstOrDefault();

                if (lichDieuTriSauTuNgayGanNhat != null)
                {
                    lichDieuTriTruocTuNgayGanNhat.DenNgay = lichDieuTriSauTuNgayGanNhat.TuNgay.AddMinutes(-1);
                }
                else
                {
                    lichDieuTriTruocTuNgayGanNhat.DenNgay = null;
                }

                lstNoiTruEkipDieuTri.Add(lichDieuTriTruocTuNgayGanNhat);

            }

            return lstNoiTruEkipDieuTri;
        }
        #endregion

        #region Cấp giường
        public async Task<List<ChiPhiKhamChuaBenhNoiTruVo>> GetChiPhiGiuongNoiTrus(long yeuCauTiepNhanId)
        {
            //YeuCauTiepNhanBaseService: GetDanhSachChiPhiNoiTruChuaThu
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId, o => o.Include(x => x.NoiTruBenhAn)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.KhoaPhong)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.PhongBenhVien)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.GiuongBenh)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.KhoaPhong)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.PhongBenhVien)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.GiuongBenh)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.NoiChiDinh).ThenInclude(gb => gb.KhoaPhong)
                                                                                .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong));

            var queryDichVuGiuongChiPhiBenhVien = await BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).Include(cc => cc.CongTyBaoHiemTuNhanCongNos).Include(cc => cc.MienGiamChiPhis)
                .Where(yc => yc.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan || yc.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan ||
                              yc.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan)
                .Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.NgayPhatSinh,
                    DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.DichVuGiuong,
                    Ma = s.Ma,
                    Nhom = NhomChiPhiNoiTru.DichVuGiuong.GetDescription(),
                    TenDichVu = s.Ten,
                    LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.Gia,
                    DanhSachCongNoChoThus = s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }),
                    DanhSachMienGiamVos =
                        s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null).GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                            (k, v) => new DanhSachMienGiamVo
                            {
                                LoaiMienGiam = k.LoaiMienGiam,
                                LoaiChietKhau = k.LoaiChietKhau,
                                TheVoucherId = k.TheVoucherId,
                                MaTheVoucher = k.MaTheVoucher,
                                DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                                TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                                SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                            }).Where(o => o.SoTien != 0),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    CapNhatThanhToan = s.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.KhoaPhong.Ten,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    CheckedDefault = true
                }).ToListAsync();

            var queryDichVuGiuongChiPhiBHYT = await BaseRepository.TableNoTracking.Where(o => o.Id == yeuCauTiepNhanId)
                .SelectMany(o => o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ToListAsync();

            List<YeuCauDichVuGiuongBenhVienChiPhiBHYT> dichVuGiuongBenhVienChiPhiBhyts;
            List<ChiPhiKhamChuaBenhNoiTruVo> dichVuGiuongs;

            if (yeuCauTiepNhan.NoiTruBenhAn != null && yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)
            {
                var chiPhiGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
                dichVuGiuongBenhVienChiPhiBhyts = chiPhiGiuong.Item2;
                dichVuGiuongs = chiPhiGiuong.Item1.Select(s => new ChiPhiKhamChuaBenhNoiTruVo
                {
                    Id = s.Id,
                    NgayPhatSinh = s.NgayPhatSinh,
                    DichVuBenhVienId = s.DichVuGiuongBenhVienId,
                    LoaiNhom = NhomChiPhiNoiTru.DichVuGiuong,
                    Ma = s.Ma,
                    Nhom = NhomChiPhiNoiTru.DichVuGiuong.GetDescription(),
                    TenDichVu = s.Ten,
                    LoaiGia = string.Empty,
                    Soluong = s.SoLuong,
                    DonGia = s.Gia,
                    DuocHuongBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any(),
                    KiemTraBHYTXacNhan = false,
                    DonGiaBHYT = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.DonGiaBaoHiem.GetValueOrDefault() ?? 0,
                    TiLeBaoHiemThanhToan = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.TiLeBaoHiemThanhToan.GetValueOrDefault() ?? 0,
                    MucHuongBaoHiem = s.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.FirstOrDefault()?.MucHuongBaoHiem.GetValueOrDefault() ?? 0,
                    DanhSachCongNoChoThus = (s.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan || s.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan) ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Count == 0 ? new List<DanhSachCongNoVo>() : s.CongTyBaoHiemTuNhanCongNos.Where(o => o.TaiKhoanBenhNhanThuId == null).Select(o => new DanhSachCongNoVo { CongTyBaoHiemTuNhanId = o.CongTyBaoHiemTuNhanId, SoTienCongNoChoThu = o.SoTien }),
                    DanhSachMienGiamVos = (s.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan || s.TrangThaiThanhToan == TrangThaiThanhToan.BaoLanhThanhToan) ? new List<DanhSachMienGiamVo>() :
                        s.MienGiamChiPhis.Count == 0 ? new List<DanhSachMienGiamVo>() : s.MienGiamChiPhis.Where(o => o.TaiKhoanBenhNhanThuId == null).GroupBy(o => new { o.LoaiMienGiam, o.TheVoucherId, o.MaTheVoucher, o.DoiTuongUuDaiId, o.LoaiChietKhau }, o => o,
                            (k, v) => new DanhSachMienGiamVo
                            {
                                LoaiMienGiam = k.LoaiMienGiam,
                                LoaiChietKhau = k.LoaiChietKhau,
                                TheVoucherId = k.TheVoucherId,
                                MaTheVoucher = k.MaTheVoucher,
                                DoiTuongUuDaiId = k.DoiTuongUuDaiId,
                                TiLe = v.Select(o => o.TiLe).DefaultIfEmpty().Sum(),
                                SoTien = v.Select(o => o.SoTien).DefaultIfEmpty().Sum()
                            }).Where(o => o.SoTien != 0),
                    GhiChuMienGiamThem = s.GhiChuMienGiamThem,
                    CapNhatThanhToan = s.TrangThaiThanhToan == TrangThaiThanhToan.CapNhatThanhToan,
                    DaThanhToan = s.SoTienBenhNhanDaChi.GetValueOrDefault(),
                    Khoa = s.KhoaPhong.Ten,
                    DoiTuongSuDung = s.DoiTuongSuDung,
                    CheckedDefault = true
                }).ToList();
            }
            else
            {
                dichVuGiuongBenhVienChiPhiBhyts = queryDichVuGiuongChiPhiBHYT;
                dichVuGiuongs = queryDichVuGiuongChiPhiBenhVien;
                foreach (var chiPhiKhamChuaBenhNoiTruVo in dichVuGiuongs.Where(o => o.DoiTuongSuDung == DoiTuongSuDung.BenhNhan))
                {
                    var chiPhiGiuongBHYT = dichVuGiuongBenhVienChiPhiBhyts.FirstOrDefault(o => o.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId == chiPhiKhamChuaBenhNoiTruVo.Id);
                    if (chiPhiGiuongBHYT != null)
                    {
                        chiPhiKhamChuaBenhNoiTruVo.DuocHuongBHYT = true;
                        chiPhiKhamChuaBenhNoiTruVo.KiemTraBHYTXacNhan = chiPhiGiuongBHYT.BaoHiemChiTra != null;
                        chiPhiKhamChuaBenhNoiTruVo.DonGiaBHYT = chiPhiGiuongBHYT.DonGiaBaoHiem.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.TiLeBaoHiemThanhToan = chiPhiGiuongBHYT.TiLeBaoHiemThanhToan.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.MucHuongBaoHiem = chiPhiGiuongBHYT.MucHuongBaoHiem.GetValueOrDefault();
                        chiPhiKhamChuaBenhNoiTruVo.YeuCauDichVuGiuongBenhVienChiPhiBHYTIds = new List<long> { chiPhiGiuongBHYT.Id };
                    }
                }
            }

            return dichVuGiuongs;
        }

        public async Task<long> GetFirstYeuCauDichVuGiuongBenhVienId(long yeuCauTiepNhanId)
        {
            var yeuCauDichVuGiuongBenhVien = await _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                                    p.DoiTuongSuDung == DoiTuongSuDung.BenhNhan)
                                                                                                        .FirstOrDefaultAsync();
            return yeuCauDichVuGiuongBenhVien?.Id ?? (long)0;
        }

        public async Task<GridDataSource> GetDanhSachCapGiuongForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId)
                                                                             .Select(p => new CapGiuongGridVo
                                                                             {
                                                                                 Id = p.Id,
                                                                                 PhongChiDinhId = p.NoiChiDinhId,
                                                                                 PhongChiDinhDisplay = p.NoiChiDinh.Ten,
                                                                                 //KhoaChiDinhId = p.NoiChiDinhId, //data là phòng, không phải khoa
                                                                                 //KhoaChiDinhDisplay = p.NoiChiDinh.Ten,
                                                                                 GiuongBenhId = p.GiuongBenhId,
                                                                                 TenGiuong = p.GiuongBenh != null ? $"{p.GiuongBenh.Ten} - {p.GiuongBenh.PhongBenhVien.Ten}" : "",
                                                                                 LoaiGiuong = p.LoaiGiuong,
                                                                                 DichVuGiuongBenhVienId = p.DichVuGiuongBenhVienId,
                                                                                 DichVuGiuongBenhVienDisplay = p.DichVuGiuongBenhVien.Ten,
                                                                                 DoiTuongSuDung = p.DoiTuongSuDung,
                                                                                 ThoiGianNhan = p.ThoiDiemBatDauSuDung.GetValueOrDefault(),
                                                                                 ThoiGianTra = p.ThoiDiemKetThucSuDung,
                                                                                 BaoPhong = p.BaoPhong.GetValueOrDefault(),
                                                                                 GhiChu = p.GhiChu,
                                                                                 DaQuyetToan = p.YeuCauTiepNhan.QuyetToanTheoNoiTru,
                                                                                 MaDichVuGiuongBenhVien = p.Ma,
                                                                                 IsFirstData = false,
                                                                                 IsReadOnly = false
                                                                             });

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            var firstYeuCauDichVuGiuongId = await GetFirstYeuCauDichVuGiuongBenhVienId(yeuCauTiepNhanId);
            var danhSachChiPhiGiuong = await GetChiPhiGiuongNoiTrus(yeuCauTiepNhanId);
            var theBHYT = await BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                                                              .SelectMany(p => p.YeuCauTiepNhanTheBHYTs)
                                                              .LastOrDefaultAsync();

            var currentPhongId = _userAgentHelper.GetCurrentNoiLLamViecId();
            var currentKhoaId = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == currentPhongId)
                                                                        .Select(p => p.KhoaPhongId)
                                                                        .FirstOrDefault();

            var thoiDiemRaVien = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                                                               .Select(p => p.NoiTruBenhAn.ThoiDiemRaVien)
                                                               .FirstOrDefault();

            foreach (var item in queryTask.Result)
            {
                var dichVuGiuongBenhVien = await _dichVuGiuongBenhVienRepository.TableNoTracking.Where(x => x.Id == item.DichVuGiuongBenhVienId)
                                                                                                .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuGiuongBenhVien)
                                                                                                .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                                                                                                .FirstOrDefaultAsync();

                var giaBaoHiem = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= item.ThoiGianNhan && (o.DenNgay == null || item.ThoiGianNhan <= o.DenNgay.Value));
                //var giaBenhVien = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.OrderBy(x => x.NhomGiaDichVuGiuongBenhVien.Ten == "Thường").Select(x => x).FirstOrDefault();
                var giaBenhViens = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.Where(p => p.TuNgay <= item.ThoiGianNhan && (p.DenNgay == null || item.ThoiGianNhan <= p.DenNgay.Value))
                                                                                        .ToList();
                var giaBenhVien = new DichVuGiuongBenhVienGiaBenhVien();

                if (item.BaoPhong)
                {
                    giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng").FirstOrDefault();

                    if (giaBenhVien == null)
                    {
                        throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaBaoPhong.KhongCoHieuLuc"));
                    }
                }
                else
                {
                    giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() != "bao phòng").FirstOrDefault();

                    if (giaBenhVien == null)
                    {
                        throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaThuong.KhongCoHieuLuc"));
                    }
                }

                item.DonGia = giaBenhVien?.Gia ?? 0;

                //item.DonGiaBHYT = giaBaoHiem?.Gia ?? 0;
                item.DonGiaBHYT = theBHYT != null && giaBaoHiem != null ? giaBaoHiem?.Gia ?? 0 : 0;
                item.ThanhTienTamTinh = danhSachChiPhiGiuong.Where(p => p.Ma == item.MaDichVuGiuongBenhVien &&
                                                                        p.DoiTuongSuDung == item.DoiTuongSuDung &&
                                                                        item.ThoiGianNhan.Date <= p.NgayPhatSinh.GetValueOrDefault().Date &&
                                                                        p.NgayPhatSinh.GetValueOrDefault().Date <= item.ThoiGianTra.GetValueOrDefault(DateTime.Now.Date))
                                                            .GroupBy(p => new { p.NgayPhatSinh })
                                                            .Sum(p => p.First().ThanhTien);

                item.LoaiGiaDichVuCoHieuLuc = dichVuGiuongBenhVien.DichVuGiuongBenhVienGiaBenhViens.Count(o => o.TuNgay.Date <= item.ThoiGianNhan.Date &&
                                                                                                               (o.DenNgay == null || item.ThoiGianNhan.Date <= o.DenNgay.Value.Date));

                var khoaChiDinh = _phongBenhVienRepository.TableNoTracking.Where(p => p.Id == item.PhongChiDinhId)
                                                                            .Select(p => p.KhoaPhong)
                                                                            .FirstOrDefault();

                item.KhoaChiDinhId = khoaChiDinh.Id;
                item.KhoaChiDinhDisplay = khoaChiDinh.Ten;

                if (currentKhoaId != khoaChiDinh.Id)
                {
                    item.IsReadOnly = true;
                }

                if (item.Id == firstYeuCauDichVuGiuongId)
                {
                    item.IsFirstData = true;
                }

                if (thoiDiemRaVien != null && item.ThoiGianTra == null)
                {
                    item.ThoiGianTra = thoiDiemRaVien;
                }
            }

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachCapGiuongForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = await _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId)
                                                                                   .CountAsync();

            return new GridDataSource
            {
                TotalRowCount = query
            };
        }

        public ChiTietSuDungGiuongTheoNgayVo GetDanhSachChiTietSuDungGiuongTheoNgayForGrid(long yeuCauTiepNhanId)
        {
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId,
                o => o.Include(x => x.NoiTruBenhAn)
                    .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.KhoaPhong)
                    .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.PhongBenhVien)
                    .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBenhViens).ThenInclude(gb => gb.GiuongBenh)
                    .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.KhoaPhong)
                    .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.PhongBenhVien)
                    .Include(x => x.YeuCauDichVuGiuongBenhVienChiPhiBHYTs).ThenInclude(gb => gb.GiuongBenh)
                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.NoiChiDinh).ThenInclude(gb => gb.KhoaPhong)
                    .Include(x => x.YeuCauDichVuGiuongBenhViens).ThenInclude(dvg => dvg.GiuongBenh).ThenInclude(gb => gb.PhongBenhVien).ThenInclude(gb => gb.KhoaPhong));
            var chiTietSuDungGiuongTheoNgay = new ChiTietSuDungGiuongTheoNgayVo();
            if (yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemRaVien == null)
            {
                chiTietSuDungGiuongTheoNgay.IsReadOnly = true;
                var chiPhiGiuong = TinhChiPhiDichVuGiuong(yeuCauTiepNhan);
                MapChiTietSuDungGiuongTheoNgay(yeuCauTiepNhan.BenhNhanId, chiTietSuDungGiuongTheoNgay, chiPhiGiuong.Item1, chiPhiGiuong.Item2);
            }
            else
            {
                chiTietSuDungGiuongTheoNgay.IsReadOnly = yeuCauTiepNhan.NoiTruBenhAn.DaQuyetToan == true;
                MapChiTietSuDungGiuongTheoNgay(yeuCauTiepNhan.BenhNhanId, chiTietSuDungGiuongTheoNgay, yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.ToList(), yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.ToList());
            }

            //ChiTietSuDungGiuongTheoNgayVo chiTietSuDungGiuongTheoNgay = new ChiTietSuDungGiuongTheoNgayVo()
            //{
            //    IsReadOnly = true,
            //    SuDungGiuongTheoNgays =
            //    {
            //        new SuDungGiuongTheoNgayVo()
            //        {
            //            NgayPhatSinh = DateTime.ParseExact("01/10/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            //            GiuongBenhVienChiPhis =
            //            {
            //                new GiuongBenhVienChiPhiVo()
            //                {
            //                    Loai = "Dịch vụ giường BV",
            //                    ChiTietGiuongBenhVienChiPhis =
            //                    {
            //                        new ChiTietGiuongBenhVienChiPhiVo() { Id = 1, KhoaChiDinhId = 27, KhoaChiDinhDisplay = "Khoa Phụ sản", GiuongChuyenDenId = 1, GiuongChuyenDenDisplay = "G801 - P101", LoaiGiuong = EnumLoaiGiuong.Noi, BaoPhong = false,
            //                            DichVuGiuongId = 1, DichVuGiuongDisplay = "Giường Ngoại khoa loại 3 Hạng III Khoa Ngoại tổng hợp", DoiTuong = DoiTuongSuDung.BenhNhan, SoLuong = 1, SoLuongGhep = 1, DonGia = 1100000, SoTienBaoHiem = 0, ThanhTien = 1100000 }
            //                    }
            //                },
            //                new GiuongBenhVienChiPhiVo()
            //                {
            //                    Loai = "Dịch vụ giường BHYT",
            //                    ChiTietGiuongBenhVienChiPhis =
            //                    {
            //                        new ChiTietGiuongBenhVienChiPhiVo() { Id = 2, KhoaChiDinhId = 27, KhoaChiDinhDisplay = "Khoa Phụ sản", GiuongChuyenDenId = 1, GiuongChuyenDenDisplay = "G801 - P101", LoaiGiuong = EnumLoaiGiuong.Noi, BaoPhong = false, DichVuGiuongId = 1, DichVuGiuongDisplay = "Giường Ngoại khoa loại 3 Hạng III Khoa Ngoại tổng hợp", DoiTuong = DoiTuongSuDung.BenhNhan, SoLuong = 1, SoLuongGhep = 1, DonGia = 1100000, SoTienBaoHiem = 0, ThanhTien = 1100000 },
            //                        new ChiTietGiuongBenhVienChiPhiVo() { Id = 3, KhoaChiDinhId = 27, KhoaChiDinhDisplay = "Khoa Phụ sản", GiuongChuyenDenId = 1, GiuongChuyenDenDisplay = "G801 - P101", LoaiGiuong = EnumLoaiGiuong.Noi, BaoPhong = false, DichVuGiuongId = 1, DichVuGiuongDisplay = "Giường Ngoại khoa loại 3 Hạng III Khoa Ngoại tổng hợp", DoiTuong = DoiTuongSuDung.BenhNhan, SoLuong = 1, SoLuongGhep = 1, DonGia = 1100000, SoTienBaoHiem = 0, ThanhTien = 1100000 }
            //                    }
            //                },
            //            }
            //        },

            //        new SuDungGiuongTheoNgayVo()
            //        {
            //            NgayPhatSinh = DateTime.ParseExact("02/10/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            //            GiuongBenhVienChiPhis =
            //            {
            //                new GiuongBenhVienChiPhiVo()
            //                {
            //                    Loai = "Dịch vụ giường BV",
            //                    ChiTietGiuongBenhVienChiPhis =
            //                    {
            //                        new ChiTietGiuongBenhVienChiPhiVo() { Id = 4, KhoaChiDinhId = 27, KhoaChiDinhDisplay = "Khoa Phụ sản", GiuongChuyenDenId = 1, GiuongChuyenDenDisplay = "G801 - P101", LoaiGiuong = EnumLoaiGiuong.Noi, BaoPhong = false, DichVuGiuongId = 1, DichVuGiuongDisplay = "Giường Ngoại khoa loại 3 Hạng III Khoa Ngoại tổng hợp", DoiTuong = DoiTuongSuDung.BenhNhan, SoLuong = 1, SoLuongGhep = 1, DonGia = 1100000, SoTienBaoHiem = 0, ThanhTien = 1100000 },
            //                        new ChiTietGiuongBenhVienChiPhiVo() { Id = 5, KhoaChiDinhId = 27, KhoaChiDinhDisplay = "Khoa Phụ sản", GiuongChuyenDenId = 1, GiuongChuyenDenDisplay = "G801 - P101", LoaiGiuong = EnumLoaiGiuong.Noi, BaoPhong = false, DichVuGiuongId = 1, DichVuGiuongDisplay = "Giường Ngoại khoa loại 3 Hạng III Khoa Ngoại tổng hợp", DoiTuong = DoiTuongSuDung.BenhNhan, SoLuong = 1, SoLuongGhep = 1, DonGia = 1100000, SoTienBaoHiem = 0, ThanhTien = 1100000 }
            //                    }
            //                },
            //                new GiuongBenhVienChiPhiVo()
            //                {
            //                    Loai = "Dịch vụ giường BHYT",
            //                    ChiTietGiuongBenhVienChiPhis =
            //                    {
            //                        new ChiTietGiuongBenhVienChiPhiVo() { Id = 6, KhoaChiDinhId = 27, KhoaChiDinhDisplay = "Khoa Phụ sản", GiuongChuyenDenId = 1, GiuongChuyenDenDisplay = "G801 - P101", LoaiGiuong = EnumLoaiGiuong.Noi, BaoPhong = false, DichVuGiuongId = 1, DichVuGiuongDisplay = "Giường Ngoại khoa loại 3 Hạng III Khoa Ngoại tổng hợp", DoiTuong = DoiTuongSuDung.BenhNhan, SoLuong = 1, SoLuongGhep = 1, DonGia = 1100000, SoTienBaoHiem = 0, ThanhTien = 1100000 },
            //                        new ChiTietGiuongBenhVienChiPhiVo() { Id = 7, KhoaChiDinhId = 27, KhoaChiDinhDisplay = "Khoa Phụ sản", GiuongChuyenDenId = 1, GiuongChuyenDenDisplay = "G801 - P101", LoaiGiuong = EnumLoaiGiuong.Noi, BaoPhong = false, DichVuGiuongId = 1, DichVuGiuongDisplay = "Giường Ngoại khoa loại 3 Hạng III Khoa Ngoại tổng hợp", DoiTuong = DoiTuongSuDung.BenhNhan, SoLuong = 1, SoLuongGhep = 1, DonGia = 1100000, SoTienBaoHiem = 0, ThanhTien = 1100000 }
            //                    }
            //                },
            //            }
            //        }
            //    }
            //};

            return chiTietSuDungGiuongTheoNgay;
        }

        private void MapChiTietSuDungGiuongTheoNgay(long? benhNhanId, ChiTietSuDungGiuongTheoNgayVo chiTietSuDungGiuongTheoNgay, List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> chiPhiGiuongBenhViens, List<YeuCauDichVuGiuongBenhVienChiPhiBHYT> chiPhiGiuongBHYTs)
        {
            var chiPhiGiuongBenhVienDateGroup = chiPhiGiuongBenhViens.GroupBy(o => o.NgayPhatSinh.Date).OrderBy(o => o.Key);
            var chiPhiGiuongBHYTDateGroup = chiPhiGiuongBHYTs.GroupBy(o => o.NgayPhatSinh.Date).OrderBy(o => o.Key);

            var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats)
                .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
                .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                            && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                            && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy)
                .ToList();
            foreach (var chiPhiGiuongBenhVienByDate in chiPhiGiuongBenhVienDateGroup)
            {
                var suDungGiuongTheoNgayVo = new SuDungGiuongTheoNgayVo { NgayPhatSinh = chiPhiGiuongBenhVienByDate.Key };
                suDungGiuongTheoNgayVo.ChiTietGiuongBenhVienChiPhis = chiPhiGiuongBenhVienByDate.Select(o => new ChiTietGiuongBenhVienChiPhiVo
                {
                    Id = o.Id,
                    LoaiChiPhiGiuongBenh = LoaiChiPhiGiuongBenh.ChiPhiGiuongBenhVien,
                    KhoaChiDinhId = o.KhoaPhongId,
                    KhoaChiDinhDisplay = o.KhoaPhong.Ten,
                    GiuongChuyenDenId = o.GiuongBenhId,
                    GiuongChuyenDenDisplay = o.GiuongBenh != null ? $"{o.GiuongBenh.Ten} - {o.PhongBenhVien.Ma}" : string.Empty,
                    LoaiGiuong = o.LoaiGiuong,
                    BaoPhong = o.BaoPhong,
                    DichVuGiuongId = o.DichVuGiuongBenhVienId,
                    DichVuGiuongDisplay = o.Ten,
                    DoiTuong = o.DoiTuongSuDung,
                    SoLuong = o.SoLuong,
                    SoLuongGhep = o.SoLuongGhep,
                    DonGia = o.Gia,
                    SoTienBaoHiem = 0,
                    ThanhTien = o.Gia * (decimal)o.SoLuong / o.SoLuongGhep,

                    CoDichVuNayTrongGoi = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(b => b.DichVuGiuongBenhVienId == o.DichVuGiuongBenhVienId)),
                    CoDichVuNayTrongGoiKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any(b => b.DichVuGiuongBenhVienId == o.DichVuGiuongBenhVienId)),
                    CoThongTinMienGiam = o.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),
                    YeuCauGoiDichVuId = o.YeuCauGoiDichVuId,
                    LaDichVuTrongGoi = o.YeuCauGoiDichVuId != null,
                    LaDichVuKhuyenMai = goiDichVus.Any() && goiDichVus.Any(a => a.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs.Any(b => b.DichVuGiuongBenhVienId == o.DichVuGiuongBenhVienId))
                    && o.MienGiamChiPhis.Any(a => a.DaHuy != true && a.YeuCauGoiDichVuId != null),

                    ChiTietGiuongBenhVienChiPhiBHYTs = o.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Select(p => new ChiTietGiuongBenhVienChiPhiVo
                    {
                        Id = p.Id,
                        LoaiChiPhiGiuongBenh = LoaiChiPhiGiuongBenh.ChiPhiGiuongBHYT,
                        KhoaChiDinhId = p.KhoaPhongId,
                        KhoaChiDinhDisplay = p.KhoaPhong.Ten,
                        GiuongChuyenDenId = p.GiuongBenhId,
                        GiuongChuyenDenDisplay = p.GiuongBenh != null ? $"{p.GiuongBenh.Ten} - {p.PhongBenhVien.Ma}" : string.Empty,
                        LoaiGiuong = p.LoaiGiuong,
                        BaoPhong = false,
                        DichVuGiuongId = p.DichVuGiuongBenhVienId,
                        DichVuGiuongDisplay = p.Ten,
                        DoiTuong = DoiTuongSuDung.BenhNhan,
                        SoLuong = p.SoLuong,
                        SoLuongGhep = p.SoLuongGhep,
                        DonGia = p.DonGiaBaoHiem.GetValueOrDefault(),
                        SoTienBaoHiem = 0,
                        ThanhTien = p.DonGiaBaoHiem.GetValueOrDefault() * (decimal)p.SoLuong / p.SoLuongGhep,
                        ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId = p.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId
                    }).ToList()
                }).ToList();
                chiTietSuDungGiuongTheoNgay.SuDungGiuongTheoNgays.Add(suDungGiuongTheoNgayVo);
            }

            //foreach (var chiPhiGiuongBenhVienByDate in chiPhiGiuongBenhVienDateGroup)
            //{
            //    var suDungGiuongTheoNgayVo = new SuDungGiuongTheoNgayVo { NgayPhatSinh = chiPhiGiuongBenhVienByDate.Key };
            //    suDungGiuongTheoNgayVo.GiuongBenhVienChiPhis.Add(new GiuongBenhVienChiPhiVo
            //    {
            //        LoaiChiPhiGiuongBenh = LoaiChiPhiGiuongBenh.ChiPhiGiuongBenhVien,
            //        ChiTietGiuongBenhVienChiPhis = chiPhiGiuongBenhVienByDate.Select(o =>
            //            new ChiTietGiuongBenhVienChiPhiVo
            //            {
            //                Id = o.Id,
            //                LoaiChiPhiGiuongBenh = LoaiChiPhiGiuongBenh.ChiPhiGiuongBenhVien,
            //                KhoaChiDinhId = o.KhoaPhongId,
            //                KhoaChiDinhDisplay = o.KhoaPhong.Ten,
            //                GiuongChuyenDenId = o.GiuongBenhId,
            //                GiuongChuyenDenDisplay = $"{o.GiuongBenh.Ten} - {o.PhongBenhVien.Ma}",
            //                LoaiGiuong = o.LoaiGiuong,
            //                BaoPhong = o.BaoPhong,
            //                DichVuGiuongId = o.DichVuGiuongBenhVienId,
            //                DichVuGiuongDisplay = o.Ten,
            //                DoiTuong = o.DoiTuongSuDung,
            //                SoLuong = o.SoLuong,
            //                SoLuongGhep = o.SoLuongGhep,
            //                DonGia = o.Gia,
            //                SoTienBaoHiem = 0,
            //                ThanhTien = o.Gia * (decimal)o.SoLuong / o.SoLuongGhep
            //            }).ToList()
            //    });
            //    chiTietSuDungGiuongTheoNgay.SuDungGiuongTheoNgays.Add(suDungGiuongTheoNgayVo);
            //}
            //foreach (var chiPhiGiuongBHYTByDate in chiPhiGiuongBHYTDateGroup)
            //{
            //    var suDungGiuongTheoNgayVo = chiTietSuDungGiuongTheoNgay.SuDungGiuongTheoNgays.FirstOrDefault(o => o.NgayPhatSinh == chiPhiGiuongBHYTByDate.Key);
            //    suDungGiuongTheoNgayVo?.GiuongBenhVienChiPhis.Add(new GiuongBenhVienChiPhiVo
            //    {
            //        LoaiChiPhiGiuongBenh = LoaiChiPhiGiuongBenh.ChiPhiGiuongBHYT,
            //        ChiTietGiuongBenhVienChiPhis = chiPhiGiuongBHYTByDate.Select(o =>
            //            new ChiTietGiuongBenhVienChiPhiVo
            //            {
            //                Id = o.Id,
            //                LoaiChiPhiGiuongBenh = LoaiChiPhiGiuongBenh.ChiPhiGiuongBenhVien,
            //                KhoaChiDinhId = o.KhoaPhongId,
            //                KhoaChiDinhDisplay = o.KhoaPhong.Ten,
            //                GiuongChuyenDenId = o.GiuongBenhId,
            //                GiuongChuyenDenDisplay = $"{o.GiuongBenh.Ten} - {o.PhongBenhVien.Ma}",
            //                LoaiGiuong = o.LoaiGiuong,
            //                BaoPhong = false,
            //                DichVuGiuongId = o.DichVuGiuongBenhVienId,
            //                DichVuGiuongDisplay = o.Ten,
            //                DoiTuong = DoiTuongSuDung.BenhNhan,
            //                SoLuong = o.SoLuong,
            //                SoLuongGhep = o.SoLuongGhep,
            //                DonGia = o.DonGiaBaoHiem.GetValueOrDefault(),
            //                SoTienBaoHiem = 0,
            //                ThanhTien = o.DonGiaBaoHiem.GetValueOrDefault() * (decimal)o.SoLuong / o.SoLuongGhep,
            //                ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId = o.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId
            //            }).ToList()
            //    });
            //}
        }

        public async Task SuaDanhSachChiTietSuDungGiuongTheoNgay(YeuCauTiepNhan yeuCauTiepNhan, ChiTietSuDungGiuongTheoNgayVo chiTietSuDungGiuongTheoNgayVo)
        {
            List<long> yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds = new List<long>();
            List<long> yeuCauDichVuGiuongBenhVienChiPhiBHYTIds = new List<long>();
            var noiChiDinh = _phongBenhVienRepository.GetById(_userAgentHelper.GetCurrentNoiLLamViecId());
            var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
                                 .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
                                 .Where(x => ((x.BenhNhanId == yeuCauTiepNhan.BenhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == yeuCauTiepNhan.BenhNhanId && x.GoiSoSinh == true))
                                             && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                                  && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy);
            var tongSoLanSuDungDichVuGiuong = chiTietSuDungGiuongTheoNgayVo.SuDungGiuongTheoNgays
                    .SelectMany(x => x.ChiTietGiuongBenhVienChiPhis.Where(c => goiDichVus.Any(v => v.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(s => s.DichVuGiuongBenhVienId == c.DichVuGiuongId)) && c.LaDichVuTrongGoi)).Sum(v => v.SoLuong);

            var tongSoLanToiDaTheoDichVuGiuong = goiDichVus.SelectMany(v => v.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Where(c => chiTietSuDungGiuongTheoNgayVo.SuDungGiuongTheoNgays.Any(x => x.ChiTietGiuongBenhVienChiPhis.Any(z => z.DichVuGiuongId == c.DichVuGiuongBenhVienId)))).Sum(s => s.SoLan);
            if (tongSoLanSuDungDichVuGiuong > tongSoLanToiDaTheoDichVuGiuong)
            {
                throw new Exception("SL nhập vượt quá SL trong gói");
            }
            //Thêm & sửa
            foreach (var item in chiTietSuDungGiuongTheoNgayVo.SuDungGiuongTheoNgays)
            {
                foreach (var giuongBenhVienChiPhiVo in item.ChiTietGiuongBenhVienChiPhis)
                {
                    var dichVuGiuong = _dichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.Id == giuongBenhVienChiPhiVo.DichVuGiuongId)
                                                                                      .Include(x => x.DichVuGiuong)
                                                                                      .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                                                                                      .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuGiuongBenhVien)
                                                                                      .FirstOrDefault();

                    var giuongBenh = _giuongBenhRepository.TableNoTracking.Where(p => p.Id == giuongBenhVienChiPhiVo.GiuongChuyenDenId)
                                                                          .Include(p => p.PhongBenhVien)
                                                                          .FirstOrDefault();
                   
                    if (dichVuGiuong == null)
                    {
                        throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuong.NotExists"));
                    }

                    if (!dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Any())
                    {
                        throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuongNhomGia.NotExists"));
                    }

                    var giaBaoHiem = dichVuGiuong.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= item.NgayPhatSinh && (o.DenNgay == null || item.NgayPhatSinh <= o.DenNgay.Value));
                    //var giaBenhVien = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.OrderBy(x => x.NhomGiaDichVuGiuongBenhVien.Ten == "Thường").Select(x => x).First();
                    var giaBenhViens = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Where(p => p.TuNgay <= item.NgayPhatSinh && (p.DenNgay == null || item.NgayPhatSinh <= p.DenNgay.Value))
                                                                                    .ToList();
                    var giaBenhVien = new DichVuGiuongBenhVienGiaBenhVien();

                    if (giuongBenhVienChiPhiVo.BaoPhong.GetValueOrDefault())
                    {
                        giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng").FirstOrDefault();

                        if (giaBenhVien == null)
                        {
                            throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaBaoPhong.KhongCoHieuLuc"));
                        }
                    }
                    else
                    {
                        giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() != "bao phòng").FirstOrDefault();

                        if (giaBenhVien == null)
                        {
                            throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaThuong.KhongCoHieuLuc"));
                        }
                    }

                    if (giuongBenhVienChiPhiVo.isCreated)
                    {
                        var dichVuGiuongBenhVienChiPhiBenhVien = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
                        {
                            NgayPhatSinh = item.NgayPhatSinh,
                            YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                            DichVuGiuongBenhVienId = giuongBenhVienChiPhiVo.DichVuGiuongId,
                            NhomGiaDichVuGiuongBenhVienId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId,
                            GiuongBenhId = giuongBenhVienChiPhiVo.GiuongChuyenDenId,
                            //GiuongBenh = dvGiuongBenhVien.GiuongBenh,
                            PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
                            //PhongBenhVien = dvGiuongBenhVien.GiuongBenh.PhongBenhVien,
                            KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
                            //KhoaPhong = dvGiuongBenhVien.GiuongBenh.PhongBenhVien.KhoaPhong,
                            Ten = dichVuGiuong.Ten,
                            Ma = dichVuGiuong.Ma,
                            MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
                            LoaiGiuong = (EnumLoaiGiuong)giuongBenhVienChiPhiVo.LoaiGiuong,
                            MoTa = dichVuGiuong.MoTa,
                            Gia = giaBenhVien.Gia,
                            BaoPhong = giuongBenhVienChiPhiVo.BaoPhong,
                            SoLuong = giuongBenhVienChiPhiVo.SoLuong,
                            SoLuongGhep = giuongBenhVienChiPhiVo.SoLuongGhep,
                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                            GhiChu = string.Empty,
                            DoiTuongSuDung = giuongBenhVienChiPhiVo.DoiTuong ?? DoiTuongSuDung.BenhNhan,
                            HeThongTuPhatSinh = true
                        };
                        if (giuongBenhVienChiPhiVo.LaDichVuTrongGoi)
                        {
                            if (dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId == null)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = (long)yeuCauTiepNhan.BenhNhanId,
                                    DichVuId = giuongBenhVienChiPhiVo.DichVuGiuongId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                                    SoLuong = (int)giuongBenhVienChiPhiVo.SoLuong
                                };
                                await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                                dichVuGiuongBenhVienChiPhiBenhVien.Gia = thongTin.DonGia;
                                dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                                dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                                dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;
                            }
                        }
                        else
                        {
                            dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = null;
                            dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = null;
                            dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = null;
                        }

                        yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVien);

                        var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

                        if (theBHYT != null && giaBaoHiem != null)
                        {
                            var dichVuGiuongBenhVienChiPhiBHYT = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
                            {
                                NgayPhatSinh = item.NgayPhatSinh,
                                YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                                DichVuGiuongBenhVienId = giuongBenhVienChiPhiVo.DichVuGiuongId,
                                GiuongBenhId = giuongBenhVienChiPhiVo.GiuongChuyenDenId,
                                //GiuongBenh = dvGiuongBHYTs[i].GiuongBenh,
                                PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
                                //PhongBenhVien = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien,
                                KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
                                //KhoaPhong = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien.KhoaPhong,
                                Ten = dichVuGiuong.Ten,
                                Ma = dichVuGiuong.Ma,
                                MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
                                LoaiGiuong = (EnumLoaiGiuong)giuongBenhVienChiPhiVo.LoaiGiuong,
                                MoTa = dichVuGiuong.MoTa,
                                SoLuong = giuongBenhVienChiPhiVo.SoLuong,
                                SoLuongGhep = giuongBenhVienChiPhiVo.SoLuongGhep,
                                DuocHuongBaoHiem = true,
                                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                                GhiChu = string.Empty,
                                DonGiaBaoHiem = giaBaoHiem.Gia,
                                MucHuongBaoHiem = theBHYT.MucHuong,
                                TiLeBaoHiemThanhToan = 100,
                                HeThongTuPhatSinh = true,
                                ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien = dichVuGiuongBenhVienChiPhiBenhVien
                            };
                            if (giuongBenhVienChiPhiVo.LaDichVuTrongGoi)
                            {
                                if (dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId == null)
                                {
                                    var thongTin = new ThongTinDichVuTrongGoi()
                                    {
                                        BenhNhanId = (long)yeuCauTiepNhan.BenhNhanId,
                                        DichVuId = giuongBenhVienChiPhiVo.DichVuGiuongId,
                                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                                        SoLuong = (int)giuongBenhVienChiPhiVo.SoLuong
                                    };
                                    await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                                    dichVuGiuongBenhVienChiPhiBenhVien.Gia = thongTin.DonGia;
                                    dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau =
                                        thongTin.DonGiaTruocChietKhau;
                                    dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                                    dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;
                                }
                            }
                            else
                            {
                                dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = null;
                                dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = null;
                                dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = null;
                            }

                            yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYT);
                        }
                    }
                    else
                    {
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Add(giuongBenhVienChiPhiVo.Id);
                        var yeuCauDichVuGiuongBenhVienChiPhiBenhVien = yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(p => p.Id == giuongBenhVienChiPhiVo.Id)
                                                                                                                               .FirstOrDefault();

                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId = giuongBenhVienChiPhiVo.DichVuGiuongId;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ten = dichVuGiuong.Ten;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ma = dichVuGiuong.Ma;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.LoaiGiuong = (EnumLoaiGiuong)giuongBenhVienChiPhiVo.LoaiGiuong;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.MoTa = dichVuGiuong.MoTa;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia = giaBenhVien.Gia;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.BaoPhong = giuongBenhVienChiPhiVo.BaoPhong;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DoiTuongSuDung = giuongBenhVienChiPhiVo.DoiTuong ?? DoiTuongSuDung.BenhNhan;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.SoLuong = giuongBenhVienChiPhiVo.SoLuong;
                        yeuCauDichVuGiuongBenhVienChiPhiBenhVien.SoLuongGhep = giuongBenhVienChiPhiVo.SoLuongGhep;
                        if (giuongBenhVienChiPhiVo.LaDichVuTrongGoi)
                        {
                            if (yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId == null)
                            {
                                var thongTin = new ThongTinDichVuTrongGoi()
                                {
                                    BenhNhanId = (long)yeuCauTiepNhan.BenhNhanId,
                                    DichVuId = giuongBenhVienChiPhiVo.DichVuGiuongId,
                                    NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
                                    SoLuong = (int)giuongBenhVienChiPhiVo.SoLuong
                                };
                                await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
                                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia = thongTin.DonGia;
                                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
                                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
                                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;
                            }
                        }
                        else
                        {
                            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = null;
                            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = null;
                            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = null;
                        }

                        var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

                        if (theBHYT != null && giaBaoHiem != null)
                        {
                            if (!yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any())
                            {
                                var dichVuGiuongBenhVienChiPhiBHYT = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
                                {
                                    NgayPhatSinh = item.NgayPhatSinh,
                                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                                    DichVuGiuongBenhVienId = giuongBenhVienChiPhiVo.DichVuGiuongId,
                                    GiuongBenhId = giuongBenhVienChiPhiVo.GiuongChuyenDenId,
                                    //GiuongBenh = dvGiuongBHYTs[i].GiuongBenh,
                                    PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
                                    //PhongBenhVien = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien,
                                    KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
                                    //KhoaPhong = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien.KhoaPhong,
                                    Ten = dichVuGiuong.Ten,
                                    Ma = dichVuGiuong.Ma,
                                    MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
                                    LoaiGiuong = (EnumLoaiGiuong)giuongBenhVienChiPhiVo.LoaiGiuong,
                                    MoTa = dichVuGiuong.MoTa,
                                    SoLuong = giuongBenhVienChiPhiVo.SoLuong,
                                    SoLuongGhep = giuongBenhVienChiPhiVo.SoLuongGhep,
                                    DuocHuongBaoHiem = true,
                                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                                    GhiChu = string.Empty,
                                    DonGiaBaoHiem = giaBaoHiem.Gia,
                                    MucHuongBaoHiem = theBHYT.MucHuong,
                                    TiLeBaoHiemThanhToan = 100,
                                    HeThongTuPhatSinh = true,
                                    ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Id
                                };

                                yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYT);
                            }
                            else
                            {
                                if (yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.LastOrDefault().DichVuGiuongBenhVienId != giuongBenhVienChiPhiVo.DichVuGiuongId)
                                {
                                    var ycBHYT = yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Where(p => p.Id == yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.LastOrDefault().Id)
                                                                                                     .FirstOrDefault();

                                    ycBHYT.WillDelete = true;

                                    var dichVuGiuongBenhVienChiPhiBHYT = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
                                    {
                                        NgayPhatSinh = item.NgayPhatSinh,
                                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                                        DichVuGiuongBenhVienId = giuongBenhVienChiPhiVo.DichVuGiuongId,
                                        GiuongBenhId = giuongBenhVienChiPhiVo.GiuongChuyenDenId,
                                        //GiuongBenh = dvGiuongBHYTs[i].GiuongBenh,
                                        PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
                                        //PhongBenhVien = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien,
                                        KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
                                        //KhoaPhong = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien.KhoaPhong,
                                        Ten = dichVuGiuong.Ten,
                                        Ma = dichVuGiuong.Ma,
                                        MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
                                        LoaiGiuong = (EnumLoaiGiuong)giuongBenhVienChiPhiVo.LoaiGiuong,
                                        MoTa = dichVuGiuong.MoTa,
                                        SoLuong = giuongBenhVienChiPhiVo.SoLuong,
                                        SoLuongGhep = giuongBenhVienChiPhiVo.SoLuongGhep,
                                        DuocHuongBaoHiem = true,
                                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                                        GhiChu = string.Empty,
                                        DonGiaBaoHiem = giaBaoHiem.Gia,
                                        MucHuongBaoHiem = theBHYT.MucHuong,
                                        TiLeBaoHiemThanhToan = 100,
                                        HeThongTuPhatSinh = true,
                                        ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Id
                                    };

                                    yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYT);
                                }
                            }
                        }
                    }
                }
            }

            //Xoá
            foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens)
            {
                if (!yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Any(p => p == yeuCauDichVuGiuong.Id) && yeuCauDichVuGiuong.Id != 0)
                {
                    yeuCauDichVuGiuong.WillDelete = true;
                }
            }

            foreach (var yeuCauDichVuGiuong in yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs)
            {
                if (!yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Any(p => p == yeuCauDichVuGiuong.ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId) && yeuCauDichVuGiuong.Id != 0)
                {
                    yeuCauDichVuGiuong.WillDelete = true;
                }
            }
        }

        public async Task<List<LookupItemVo>> GetListDichVuGiuongBenhVien(DropDownListRequestModel queryInfo)
        {
            var lstDichVuGiuong = await _dichVuGiuongBenhVienRepository.TableNoTracking.Select(p => new LookupItemVo
            {
                KeyId = p.Id,
                DisplayName = p.Ten
            })
            .ApplyLike(queryInfo.Query, p => p.DisplayName)
            .Take(queryInfo.Take)
            .ToListAsync();

            return lstDichVuGiuong;
        }

        public LookupItemVo GetLoaiGiuong(EnumLoaiGiuong loaiGiuong)
        {
            var item = EnumHelper.GetListEnum<EnumLoaiGiuong>()
                                 .Where(p => p == loaiGiuong)
                                 .Select(p => new LookupItemVo
                                 {
                                     KeyId = Convert.ToInt32(p),
                                     DisplayName = p.GetDescription()
                                 })
                                 .FirstOrDefault();

            return item;
        }

        public List<LookupItemVo> GetListGiuongChoChiTietSuDungTheoNgay(DropDownListRequestModel queryInfo)
        {
            var queryObject = new GiuongSuDungTheoNgaySearch();

            if (!string.IsNullOrEmpty(queryInfo.ParameterDependencies))
            {
                queryObject = JsonConvert.DeserializeObject<GiuongSuDungTheoNgaySearch>(queryInfo.ParameterDependencies);
            }

            var lstGiuong = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == queryObject.YeuCauTiepNhanId && p.GiuongBenhId != null &&
                                                                                             queryObject.NgayPhatSinh.Date >= p.ThoiDiemBatDauSuDung.Value.Date &&
                                                                                             (p.ThoiDiemKetThucSuDung == null || queryObject.NgayPhatSinh.Date <= p.ThoiDiemKetThucSuDung.Value.Date))
                                                                                 .Select(p => new LookupItemVo()
                                                                                 {
                                                                                     KeyId = p.GiuongBenhId.GetValueOrDefault(),
                                                                                     DisplayName = $"{p.GiuongBenh.Ten} - {p.GiuongBenh.PhongBenhVien.Ma}",
                                                                                 })
                                                                                 .ApplyLike(queryInfo.Query, o => o.DisplayName)
                                                                                 .GroupBy(p => new { p.KeyId })
                                                                                 .Select(p => new LookupItemVo()
                                                                                 {
                                                                                     KeyId = p.First().KeyId,
                                                                                     DisplayName = p.First().DisplayName,
                                                                                 })
                                                                                 .Take(queryInfo.Take);

            return lstGiuong.ToList();
        }

        public List<LookupItemVo> GetListDoiTuongSuDung(DropDownListRequestModel queryInfo)
        {
            var lstEnumDoiTuongSuDung = EnumHelper.GetListEnum<DoiTuongSuDung>()
                                                  .Select(item => new LookupItemVo
                                                  {
                                                      KeyId = Convert.ToInt32(item),
                                                      DisplayName = item.GetDescription()
                                                  }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                //lstEnumDoiTuongSuDung = lstEnumDoiTuongSuDung.Where(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
                lstEnumDoiTuongSuDung = lstEnumDoiTuongSuDung.Where(p => p.DisplayName.ToLower().RemoveVietnameseDiacritics().Contains(queryInfo.Query.RemoveVietnameseDiacritics().ToLower())).ToList();
            }

            return lstEnumDoiTuongSuDung;
        }

        public async Task XuLyThemCapGiuong(YeuCauTiepNhan yeuCauTiepNhan, CapGiuongVo capGiuongVo)
        {
            //Thêm dịch vụ giường
            var dichVuGiuong = await _dichVuGiuongBenhVienRepository.TableNoTracking.Include(x => x.DichVuGiuong)
                                                                                    .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                                                                                    .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuGiuongBenhVien)
                                                                                    .Where(x => x.Id == capGiuongVo.DichVuGiuongBenhVienId)
                                                                                    .FirstOrDefaultAsync();

            var giuongBenh = await _giuongBenhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == capGiuongVo.GiuongBenhId.GetValueOrDefault());

            if (dichVuGiuong == null)
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuong.NotExists"));
            }

            if (!dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Any())
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuongNhomGia.NotExists"));
            }

            var giaBaoHiem = dichVuGiuong.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= capGiuongVo.ThoiDiemBatDauSuDung && (o.DenNgay == null || capGiuongVo.ThoiDiemBatDauSuDung <= o.DenNgay.Value));
            //var giaBenhVien = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.OrderBy(x => x.NhomGiaDichVuGiuongBenhVien.Ten == "Thường").Select(x => x).First();
            var giaBenhViens = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Where(p => p.TuNgay <= capGiuongVo.ThoiDiemBatDauSuDung && (p.DenNgay == null || capGiuongVo.ThoiDiemBatDauSuDung <= p.DenNgay.Value))
                                                                            .ToList();
            var giaBenhVien = new DichVuGiuongBenhVienGiaBenhVien();

            if (capGiuongVo.BaoPhong)
            {
                giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng").FirstOrDefault();

                if (giaBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaBaoPhong.KhongCoHieuLuc"));
                }
            }
            else
            {
                giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() != "bao phòng").FirstOrDefault();

                if (giaBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaThuong.KhongCoHieuLuc"));
                }
            }

            var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

            var newYeuCauDichVuGiuong = new YeuCauDichVuGiuongBenhVien()
            {
                DichVuGiuongBenhVienId = capGiuongVo.DichVuGiuongBenhVienId,
                GiuongBenhId = capGiuongVo.GiuongBenhId,
                NhomGiaDichVuGiuongBenhVienId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId,
                Ten = dichVuGiuong.Ten,
                Ma = dichVuGiuong.Ma,
                MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong.MaTT37 : null,
                TenGiuong = giuongBenh?.Ten ?? "",
                MaGiuong = giuongBenh?.Ma ?? "",
                LoaiGiuong = capGiuongVo.LoaiGiuong != null ? (EnumLoaiGiuong)capGiuongVo.LoaiGiuong : dichVuGiuong.LoaiGiuong, //đã xử lý UI, chỉ đề phòng
                MoTa = dichVuGiuong.MoTa,
                KhongTinhPhi = false,
                DuocHuongBaoHiem = theBHYT != null && giaBaoHiem != null,
                BaoHiemChiTra = null,
                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                TrangThai = EnumTrangThaiGiuongBenh.ChuaThucHien,
                NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId(),
                NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                ThoiDiemChiDinh = DateTime.Now,
                DonGiaBaoHiem = giaBaoHiem?.Gia,
                TiLeBaoHiemThanhToan = giaBaoHiem?.TiLeBaoHiemThanhToan,
                MucHuongBaoHiem = theBHYT?.MucHuong,
                Gia = giaBenhVien.Gia,

                //ThoiDiemBatDauSuDung = yeuCauTiepNhan.NoiTruBenhAn.ThoiDiemNhapVien,
                ThoiDiemBatDauSuDung = capGiuongVo.ThoiDiemBatDauSuDung,
                ThoiDiemKetThucSuDung = capGiuongVo.ThoiDiemKetThucSuDung,
                BaoPhong = capGiuongVo.BaoPhong,
                GhiChu = capGiuongVo.GhiChu,
                DoiTuongSuDung = capGiuongVo.DoiTuongSuDung
            };

            // kiểm tra còn tồn dịch vụ giường trong gói ko
            var dichVuInfo = new DichVuGiuongTrongGoiVo()
            {
                DichVuBenhVienId = capGiuongVo.DichVuGiuongBenhVienId,
                BenhNhanId = yeuCauTiepNhan.BenhNhanId ?? 0,
                NhomGiaDichVuId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId,
                YeuCauGoiDichVuId = capGiuongVo.YeuCauGoiDichVuId
            };
            GetDichVuGiuongTrongGoiTheoBenhNhan(yeuCauTiepNhan.BenhNhan, dichVuInfo, newYeuCauDichVuGiuong);

            if (capGiuongVo.GiuongBenhId != null)
            {
                newYeuCauDichVuGiuong.HoatDongGiuongBenhs.Add(new HoatDongGiuongBenh()
                {
                    GiuongBenhId = capGiuongVo.GiuongBenhId.Value,
                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                    ThoiDiemBatDau = capGiuongVo.ThoiDiemBatDauSuDung,
                    ThoiDiemKetThuc = capGiuongVo.ThoiDiemKetThucSuDung,
                    TinhTrangBenhNhan = TinhTrangBenhNhan.DangDieuTri
                });

            }

            yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Add(newYeuCauDichVuGiuong);
        }

        public async Task XuLySuaHoatDongGiuong(YeuCauTiepNhan yeuCauTiepNhan, CapGiuongVo capGiuongVo)
        {
            //Sửa hoạt động giường bệnh
            var yeuCauDichVuGiuong = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.Single(c => c.Id == capGiuongVo.Id);

            if (capGiuongVo.GiuongBenhId != null)
            {
                if (yeuCauDichVuGiuong.GiuongBenhId != null) //nếu giường trước đó là của BN (có giường)
                {
                    var hoatDongGiuongBenh = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.SelectMany(p => p.HoatDongGiuongBenhs)
                                                                                       .Where(p => p.GiuongBenhId == yeuCauDichVuGiuong.GiuongBenhId &&
                                                                                                   p.YeuCauDichVuGiuongBenhVienId == yeuCauDichVuGiuong.Id)
                                                                                       .Single();

                    hoatDongGiuongBenh.GiuongBenhId = capGiuongVo.GiuongBenhId.Value;
                    hoatDongGiuongBenh.ThoiDiemBatDau = capGiuongVo.ThoiDiemBatDauSuDung;
                    hoatDongGiuongBenh.ThoiDiemKetThuc = capGiuongVo.ThoiDiemKetThucSuDung;
                }
                else //giường trước đó là của người nhà (không giường) -> k có hoạt động
                {
                    yeuCauDichVuGiuong.HoatDongGiuongBenhs.Add(new HoatDongGiuongBenh()
                    {
                        GiuongBenhId = capGiuongVo.GiuongBenhId.Value,
                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
                        ThoiDiemBatDau = capGiuongVo.ThoiDiemBatDauSuDung,
                        ThoiDiemKetThuc = capGiuongVo.ThoiDiemKetThucSuDung,
                        TinhTrangBenhNhan = TinhTrangBenhNhan.DangDieuTri
                    });
                }
            }
            else //Chuyển thành giường người nhà
            {
                if (yeuCauDichVuGiuong.GiuongBenhId != null) //giường BN đổi thành của người nhà -> bỏ hoạt động giường
                {
                    var hoatDongGiuongBenh = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.SelectMany(p => p.HoatDongGiuongBenhs)
                                                                                       .Where(p => p.GiuongBenhId == yeuCauDichVuGiuong.GiuongBenhId &&
                                                                                                   p.YeuCauDichVuGiuongBenhVienId == yeuCauDichVuGiuong.Id)
                                                                                       .Single();

                    hoatDongGiuongBenh.WillDelete = true;
                }
            }

            //Sửa yêu cầu dịch vụ giường bệnh viện
            var dichVuGiuong = await _dichVuGiuongBenhVienRepository.TableNoTracking.Include(x => x.DichVuGiuong)
                                                                                    .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                                                                                    .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuGiuongBenhVien)
                                                                                    .Where(x => x.Id == capGiuongVo.DichVuGiuongBenhVienId)
                                                                                    .FirstOrDefaultAsync();

            var giuongBenh = await _giuongBenhRepository.TableNoTracking.FirstOrDefaultAsync(p => p.Id == capGiuongVo.GiuongBenhId.GetValueOrDefault());

            if (dichVuGiuong == null)
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuong.NotExists"));
            }

            if (!dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Any())
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuongNhomGia.NotExists"));
            }

            var giaBaoHiem = dichVuGiuong.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= capGiuongVo.ThoiDiemBatDauSuDung && (o.DenNgay == null || capGiuongVo.ThoiDiemBatDauSuDung <= o.DenNgay.Value));
            //var giaBenhVien = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.OrderBy(x => x.NhomGiaDichVuGiuongBenhVien.Ten == "Thường").Select(x => x).First();
            var giaBenhViens = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Where(p => p.TuNgay <= capGiuongVo.ThoiDiemBatDauSuDung && (p.DenNgay == null || capGiuongVo.ThoiDiemBatDauSuDung <= p.DenNgay.Value))
                                                                            .ToList();
            var giaBenhVien = new DichVuGiuongBenhVienGiaBenhVien();

            if (capGiuongVo.BaoPhong)
            {
                giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng").FirstOrDefault();

                if (giaBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaBaoPhong.KhongCoHieuLuc"));
                }
            }
            else
            {
                giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() != "bao phòng").FirstOrDefault();

                if (giaBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaThuong.KhongCoHieuLuc"));
                }
            }
            var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

            yeuCauDichVuGiuong.DichVuGiuongBenhVienId = capGiuongVo.DichVuGiuongBenhVienId;
            yeuCauDichVuGiuong.GiuongBenhId = capGiuongVo.GiuongBenhId;
            yeuCauDichVuGiuong.NhomGiaDichVuGiuongBenhVienId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId;
            yeuCauDichVuGiuong.Ten = dichVuGiuong.Ten;
            yeuCauDichVuGiuong.Ma = dichVuGiuong.Ma;
            yeuCauDichVuGiuong.MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong.MaTT37 : null;
            yeuCauDichVuGiuong.TenGiuong = giuongBenh?.Ten ?? "";
            yeuCauDichVuGiuong.MaGiuong = giuongBenh?.Ma ?? "";
            yeuCauDichVuGiuong.LoaiGiuong = capGiuongVo.LoaiGiuong != null ? (EnumLoaiGiuong)capGiuongVo.LoaiGiuong : dichVuGiuong.LoaiGiuong; //đã xử lý UI, chỉ đề phòng
            yeuCauDichVuGiuong.MoTa = dichVuGiuong.MoTa;
            yeuCauDichVuGiuong.KhongTinhPhi = false;
            yeuCauDichVuGiuong.DuocHuongBaoHiem = theBHYT != null && giaBaoHiem != null;
            yeuCauDichVuGiuong.BaoHiemChiTra = null;
            yeuCauDichVuGiuong.TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan;
            yeuCauDichVuGiuong.TrangThai = EnumTrangThaiGiuongBenh.ChuaThucHien;
            yeuCauDichVuGiuong.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
            yeuCauDichVuGiuong.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
            yeuCauDichVuGiuong.ThoiDiemChiDinh = DateTime.Now;
            yeuCauDichVuGiuong.DonGiaBaoHiem = giaBaoHiem?.Gia;
            yeuCauDichVuGiuong.TiLeBaoHiemThanhToan = giaBaoHiem?.TiLeBaoHiemThanhToan;
            yeuCauDichVuGiuong.MucHuongBaoHiem = theBHYT?.MucHuong;
            yeuCauDichVuGiuong.Gia = giaBenhVien.Gia;
            yeuCauDichVuGiuong.ThoiDiemBatDauSuDung = capGiuongVo.ThoiDiemBatDauSuDung;
            yeuCauDichVuGiuong.ThoiDiemKetThucSuDung = capGiuongVo.ThoiDiemKetThucSuDung;
            yeuCauDichVuGiuong.BaoPhong = capGiuongVo.BaoPhong;
            yeuCauDichVuGiuong.GhiChu = capGiuongVo.GhiChu;
            yeuCauDichVuGiuong.DoiTuongSuDung = capGiuongVo.DoiTuongSuDung;

            // kiểm tra còn tồn dịch vụ giường trong gói ko
            var dichVuInfo = new DichVuGiuongTrongGoiVo()
            {
                DichVuBenhVienId = capGiuongVo.DichVuGiuongBenhVienId,
                BenhNhanId = yeuCauTiepNhan.BenhNhanId ?? 0,
                NhomGiaDichVuId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId,
                YeuCauGoiDichVuId = capGiuongVo.YeuCauGoiDichVuId
            };
            GetDichVuGiuongTrongGoiTheoBenhNhan(yeuCauTiepNhan.BenhNhan, dichVuInfo, yeuCauDichVuGiuong);
        }

        public void XuLyThoiGianTraGiuong(YeuCauTiepNhan yeuCauTiepNhan, DateTime thoiDiemBatDauSuDung, DateTime? thoiDiemKetThucSuDung, DoiTuongSuDung? doiTuongSuDung, long currentYeuCauDichVuGiuongBenhVienId)
        {
            if (doiTuongSuDung != DoiTuongSuDung.BenhNhan)
            {
                return;
            }

            var yeuCauDichVuGiuongBenhVienTruoc = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.LastOrDefault(p => p.ThoiDiemBatDauSuDung < thoiDiemBatDauSuDung &&
                                                                                                                p.ThoiDiemKetThucSuDung == null &&
                                                                                                                p.DoiTuongSuDung == doiTuongSuDung);

            if (yeuCauDichVuGiuongBenhVienTruoc != null)
            {
                yeuCauDichVuGiuongBenhVienTruoc.ThoiDiemKetThucSuDung = thoiDiemBatDauSuDung.AddMinutes(-1);

                var hoatDongGiuongBenh = yeuCauDichVuGiuongBenhVienTruoc.HoatDongGiuongBenhs.Where(p => p.GiuongBenhId == yeuCauDichVuGiuongBenhVienTruoc.GiuongBenhId).FirstOrDefault();
                hoatDongGiuongBenh.ThoiDiemKetThuc = thoiDiemBatDauSuDung.AddMinutes(-1);
            }

            if (thoiDiemKetThucSuDung == null)
            {
                var yeuCauDichVuGiuongBenhVienSau = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.ThoiDiemBatDauSuDung > thoiDiemBatDauSuDung &&
                                                                                                                   p.DoiTuongSuDung == doiTuongSuDung);

                if (yeuCauDichVuGiuongBenhVienSau != null)
                {
                    var newYeuCauDichVuGiuongBenhVien = yeuCauTiepNhan.YeuCauDichVuGiuongBenhViens.FirstOrDefault(p => p.Id == currentYeuCauDichVuGiuongBenhVienId);
                    newYeuCauDichVuGiuongBenhVien.ThoiDiemKetThucSuDung = yeuCauDichVuGiuongBenhVienSau.ThoiDiemBatDauSuDung.Value.AddMinutes(-1);

                    var hoatDongGiuongBenh = newYeuCauDichVuGiuongBenhVien.HoatDongGiuongBenhs.Where(p => p.GiuongBenhId == newYeuCauDichVuGiuongBenhVien.GiuongBenhId).FirstOrDefault();
                    hoatDongGiuongBenh.ThoiDiemKetThuc = yeuCauDichVuGiuongBenhVienSau.ThoiDiemBatDauSuDung.Value.AddMinutes(-1);
                }
            }
        }

        public List<YeuCauDichVuGiuongBenhVien> XuLySuaCapGiuong(CapGiuongVo capGiuongVo, DoiTuongSuDung? oldDoiTuongSuDung)
        {
            //new NguoiNha && old NguoiNha
            if (capGiuongVo.DoiTuongSuDung != DoiTuongSuDung.BenhNhan && oldDoiTuongSuDung != DoiTuongSuDung.BenhNhan)
            {
                return new List<YeuCauDichVuGiuongBenhVien>();
            }

            //new NguoiNha && old BenhNhan
            if (capGiuongVo.DoiTuongSuDung != DoiTuongSuDung.BenhNhan && oldDoiTuongSuDung == DoiTuongSuDung.BenhNhan)
            {
                var lstYeuCauDichVuGiuong = new List<YeuCauDichVuGiuongBenhVien>();

                var oldYeuCauDichVuGiuongBenhVien = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.Id == capGiuongVo.Id)
                                                                                                         .FirstOrDefault();

                var yeuCauDichVuGiuongTruoc = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == oldYeuCauDichVuGiuongBenhVien.YeuCauTiepNhanId &&
                                                                                                               p.Id != oldYeuCauDichVuGiuongBenhVien.Id &&
                                                                                                               p.ThoiDiemBatDauSuDung < oldYeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung &&
                                                                                                               p.DoiTuongSuDung == oldYeuCauDichVuGiuongBenhVien.DoiTuongSuDung)
                                                                                                   .Include(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                                                                                                   .OrderByDescending(p => p.ThoiDiemBatDauSuDung)
                                                                                                   .FirstOrDefault();

                if (yeuCauDichVuGiuongTruoc != null)
                {
                    var yeuCauDichVuGiuongSau = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == oldYeuCauDichVuGiuongBenhVien.YeuCauTiepNhanId &&
                                                                                                                 p.Id != oldYeuCauDichVuGiuongBenhVien.Id &&
                                                                                                                 p.ThoiDiemBatDauSuDung > oldYeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung &&
                                                                                                                 p.DoiTuongSuDung == oldYeuCauDichVuGiuongBenhVien.DoiTuongSuDung)
                                                                                                     .OrderBy(p => p.ThoiDiemBatDauSuDung)
                                                                                                     .FirstOrDefault();

                    if (yeuCauDichVuGiuongSau != null)
                    {
                        yeuCauDichVuGiuongTruoc.ThoiDiemKetThucSuDung = yeuCauDichVuGiuongSau.ThoiDiemBatDauSuDung.GetValueOrDefault().AddMinutes(-1);
                    }
                    else
                    {
                        yeuCauDichVuGiuongTruoc.ThoiDiemKetThucSuDung = null;
                    }

                    lstYeuCauDichVuGiuong.Add(yeuCauDichVuGiuongTruoc);
                }

                return lstYeuCauDichVuGiuong;
            }

            //new BenhNhan
            var lstYeuCauDichVuGiuongBenhVien = new List<YeuCauDichVuGiuongBenhVien>();

            var oldYeuCauDichVuGiuong = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.Id == capGiuongVo.Id)
                                                                                             .FirstOrDefault();

            var oldYeuCauDichVuGiuongTruoc = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == oldYeuCauDichVuGiuong.YeuCauTiepNhanId &&
                                                                                                              p.Id != oldYeuCauDichVuGiuong.Id &&
                                                                                                              p.ThoiDiemBatDauSuDung < oldYeuCauDichVuGiuong.ThoiDiemBatDauSuDung &&
                                                                                                              p.DoiTuongSuDung == oldYeuCauDichVuGiuong.DoiTuongSuDung)
                                                                                                  .Include(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                                                                                                  .OrderByDescending(p => p.ThoiDiemBatDauSuDung)
                                                                                                  .FirstOrDefault();

            var newYeuCauDichVuGiuongTruoc = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == capGiuongVo.YeuCauTiepNhanId &&
                                                                                                              p.Id != capGiuongVo.Id &&
                                                                                                              p.ThoiDiemBatDauSuDung < capGiuongVo.ThoiDiemBatDauSuDung &&
                                                                                                              p.DoiTuongSuDung == capGiuongVo.DoiTuongSuDung)
                                                                                                  .Include(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                                                                                                  .OrderByDescending(p => p.ThoiDiemBatDauSuDung)
                                                                                                  .FirstOrDefault();

            //if (oldYeuCauDichVuGiuongTruoc != null && (newYeuCauDichVuGiuongTruoc == null || oldYeuCauDichVuGiuongTruoc.Id != newYeuCauDichVuGiuongTruoc.Id))
            if (oldYeuCauDichVuGiuongTruoc != null && oldYeuCauDichVuGiuongTruoc.DoiTuongSuDung == DoiTuongSuDung.BenhNhan && (newYeuCauDichVuGiuongTruoc == null || oldYeuCauDichVuGiuongTruoc.Id != newYeuCauDichVuGiuongTruoc.Id))
            {
                var oldYeuCauDichVuGiuongSau = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == oldYeuCauDichVuGiuong.YeuCauTiepNhanId &&
                                                                                                                p.Id != oldYeuCauDichVuGiuong.Id &&
                                                                                                                p.ThoiDiemBatDauSuDung > oldYeuCauDichVuGiuong.ThoiDiemBatDauSuDung &&
                                                                                                                p.DoiTuongSuDung == oldYeuCauDichVuGiuong.DoiTuongSuDung)
                                                                                                    .OrderBy(p => p.ThoiDiemBatDauSuDung)
                                                                                                    .FirstOrDefault();

                if (oldYeuCauDichVuGiuongSau != null)
                {
                    oldYeuCauDichVuGiuongTruoc.ThoiDiemKetThucSuDung = oldYeuCauDichVuGiuongSau.ThoiDiemBatDauSuDung.GetValueOrDefault().AddMinutes(-1);
                }
                else
                {
                    oldYeuCauDichVuGiuongTruoc.ThoiDiemKetThucSuDung = null;
                }

                lstYeuCauDichVuGiuongBenhVien.Add(oldYeuCauDichVuGiuongTruoc);
            }

            if (newYeuCauDichVuGiuongTruoc != null)
            {
                newYeuCauDichVuGiuongTruoc.ThoiDiemKetThucSuDung = capGiuongVo.ThoiDiemBatDauSuDung.AddMinutes(-1);
                lstYeuCauDichVuGiuongBenhVien.Add(newYeuCauDichVuGiuongTruoc);
            }

            var newYeuCauDichVuGiuongSau = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == capGiuongVo.YeuCauTiepNhanId &&
                                                                                                            p.Id != capGiuongVo.Id &&
                                                                                                            p.ThoiDiemBatDauSuDung > capGiuongVo.ThoiDiemBatDauSuDung &&
                                                                                                            p.DoiTuongSuDung == capGiuongVo.DoiTuongSuDung)
                                                                                                .OrderBy(p => p.ThoiDiemBatDauSuDung)
                                                                                                .FirstOrDefault();

            if (newYeuCauDichVuGiuongSau != null)
            {
                capGiuongVo.ThoiDiemKetThucSuDung = newYeuCauDichVuGiuongSau.ThoiDiemBatDauSuDung.GetValueOrDefault().AddMinutes(-1);
            }

            //lstYeuCauDichVuGiuongBenhVien.Add(capGiuongVo);

            return lstYeuCauDichVuGiuongBenhVien;
        }

        public List<YeuCauDichVuGiuongBenhVien> XuLyXoaCapGiuong(YeuCauDichVuGiuongBenhVien yeuCauDichVuGiuongBenhVien)
        {
            if (yeuCauDichVuGiuongBenhVien.DoiTuongSuDung != DoiTuongSuDung.BenhNhan)
            {
                return new List<YeuCauDichVuGiuongBenhVien>();
            }

            var lstYeuCauDichVuGiuong = new List<YeuCauDichVuGiuongBenhVien>();

            var yeuCauDichVuGiuongTruoc = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauDichVuGiuongBenhVien.YeuCauTiepNhanId &&
                                                                                                           p.Id != yeuCauDichVuGiuongBenhVien.Id &&
                                                                                                           p.ThoiDiemBatDauSuDung < yeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung &&
                                                                                                           p.DoiTuongSuDung == yeuCauDichVuGiuongBenhVien.DoiTuongSuDung)
                                                                                               .Include(p => p.GiuongBenh).ThenInclude(p => p.PhongBenhVien)
                                                                                               .OrderByDescending(p => p.ThoiDiemBatDauSuDung)
                                                                                               .FirstOrDefault();

            if (yeuCauDichVuGiuongTruoc != null)
            {
                var yeuCauDichVuGiuongSau = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauDichVuGiuongBenhVien.YeuCauTiepNhanId &&
                                                                                                             p.Id != yeuCauDichVuGiuongBenhVien.Id &&
                                                                                                             p.ThoiDiemBatDauSuDung > yeuCauDichVuGiuongBenhVien.ThoiDiemBatDauSuDung &&
                                                                                                             p.DoiTuongSuDung == yeuCauDichVuGiuongBenhVien.DoiTuongSuDung)
                                                                                                 .OrderBy(p => p.ThoiDiemBatDauSuDung)
                                                                                                 .FirstOrDefault();

                if (yeuCauDichVuGiuongSau != null)
                {
                    yeuCauDichVuGiuongTruoc.ThoiDiemKetThucSuDung = yeuCauDichVuGiuongSau.ThoiDiemBatDauSuDung.GetValueOrDefault().AddMinutes(-1);
                }
                else
                {
                    yeuCauDichVuGiuongTruoc.ThoiDiemKetThucSuDung = null;
                }

                lstYeuCauDichVuGiuong.Add(yeuCauDichVuGiuongTruoc);
            }

            return lstYeuCauDichVuGiuong;
        }

        public bool KiemTraThoiGianNhanTonTaiBenhNhanTrongLichChuyenPhong(DateTime? thoiGianNhan, DoiTuongSuDung? doiTuongSuDung, long yeuCauTiepNhanId, long yeuCauDichVuGiuongBenhVienId)
        {
            if (thoiGianNhan == null || doiTuongSuDung == DoiTuongSuDung.NguoiNha)
            {
                return true;
            }

            return !_yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Any(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                   p.Id != yeuCauDichVuGiuongBenhVienId &&
                                                                                   p.DoiTuongSuDung == doiTuongSuDung &&
                                                                                   (
                                                                                        p.ThoiDiemBatDauSuDung.GetValueOrDefault() == thoiGianNhan.GetValueOrDefault() ||
                                                                                        (p.ThoiDiemKetThucSuDung != null && p.ThoiDiemKetThucSuDung.GetValueOrDefault() == thoiGianNhan.GetValueOrDefault()) ||
                                                                                        (p.ThoiDiemBatDauSuDung.GetValueOrDefault() <= thoiGianNhan.GetValueOrDefault() && p.ThoiDiemKetThucSuDung != null && thoiGianNhan.GetValueOrDefault() <= p.ThoiDiemKetThucSuDung.GetValueOrDefault())
                                                                                   ));
        }

        public bool KiemTraThoiGianNhanGiuongVoiNgayNhapVien(DateTime? thoiGianNhan, long noiTruBenhAnId)
        {
            if (thoiGianNhan == null)
            {
                return true;
            }

            return !(thoiGianNhan.Value < _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == noiTruBenhAnId)
                                                                                 .Select(p => p.ThoiDiemNhapVien)
                                                                                 .FirstOrDefault());
        }

        public ThoiDiemNhanGiuongVo GetThoiDiemChiDinhGiuong(long yeuCauTiepNhanId)
        {
            var latestNoiTruKhoaPhongDieuTri = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == yeuCauTiepNhanId)
                                                                                                .LastOrDefault();

            var latestGiuong = _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.YeuCauTiepNhanId == yeuCauTiepNhanId &&
                                                                                                ((p.ThoiDiemBatDauSuDung != null && p.ThoiDiemBatDauSuDung >= latestNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa) ||
                                                                                                (p.ThoiDiemKetThucSuDung != null && p.ThoiDiemKetThucSuDung >= latestNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)))
                                                                                    .FirstOrDefault();

            if (latestGiuong == null)
            {
                return new ThoiDiemNhanGiuongVo
                {
                    ThoiDiemNhanGiuong = latestNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa,
                    MinThoiDiemNhanGiuong = latestNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa
                };
            }

            return new ThoiDiemNhanGiuongVo
            {
                ThoiDiemNhanGiuong = DateTime.Now,
                MinThoiDiemNhanGiuong = latestNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa
            };
        }

        public ThongTinGiaDichVuGiuongVo GetDonGiaDichVuGiuong(long yeuCauTiepNhanId, long dichVuGiuongId, DateTime ngayPhatSinh, bool? baoPhong)
        {
            var yeuCauTiepNhan = BaseRepository.GetById(yeuCauTiepNhanId, o => o.Include(p => p.YeuCauTiepNhanTheBHYTs));

            var dichVuGiuong = _dichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.Id == dichVuGiuongId)
                                                                             .Include(x => x.DichVuGiuong)
                                                                             .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
                                                                             .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuGiuongBenhVien)
                                                                             .FirstOrDefault();

            if (dichVuGiuong == null)
            {
                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuong.NotExists"));
            }

            var giaBaoHiem = dichVuGiuong.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= ngayPhatSinh && (o.DenNgay == null || ngayPhatSinh <= o.DenNgay.Value));
            var giaBenhViens = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Where(p => p.TuNgay <= ngayPhatSinh && (p.DenNgay == null || ngayPhatSinh <= p.DenNgay.Value))
                                                                            .ToList();
            var giaBenhVien = new DichVuGiuongBenhVienGiaBenhVien();

            if (baoPhong.GetValueOrDefault())
            {
                giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng").FirstOrDefault();

                if (giaBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaBaoPhong.KhongCoHieuLuc"));
                }
            }
            else
            {
                giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() != "bao phòng").FirstOrDefault();

                if (giaBenhVien == null)
                {
                    throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaThuong.KhongCoHieuLuc"));
                }
            }

            var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

            return new ThongTinGiaDichVuGiuongVo
            {
                DonGia = giaBenhVien.Gia,
                DonGiaBHYT = theBHYT != null && giaBaoHiem != null ? giaBaoHiem.Gia : 0
            };

            //List<long> yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds = new List<long>();
            //List<long> yeuCauDichVuGiuongBenhVienChiPhiBHYTIds = new List<long>();
            //var noiChiDinh = _phongBenhVienRepository.GetById(_userAgentHelper.GetCurrentNoiLLamViecId());

            ////Thêm & sửa
            //foreach (var item in chiTietSuDungGiuongTheoNgayVo.SuDungGiuongTheoNgays)
            //{
            //    foreach (var item2 in item.ChiTietGiuongBenhVienChiPhis)
            //    {
            //        var dichVuGiuong = _dichVuGiuongBenhVienRepository.TableNoTracking.Where(p => p.Id == item2.DichVuGiuongId)
            //                                                                          .Include(x => x.DichVuGiuong)
            //                                                                          .Include(x => x.DichVuGiuongBenhVienGiaBaoHiems)
            //                                                                          .Include(x => x.DichVuGiuongBenhVienGiaBenhViens).ThenInclude(y => y.NhomGiaDichVuGiuongBenhVien)
            //                                                                          .FirstOrDefault();

            //        var giuongBenh = _giuongBenhRepository.TableNoTracking.Where(p => p.Id == item2.GiuongChuyenDenId)
            //                                                              .Include(p => p.PhongBenhVien)
            //                                                              .FirstOrDefault();

            //        if (dichVuGiuong == null)
            //        {
            //            throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuong.NotExists"));
            //        }

            //        if (!dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Any())
            //        {
            //            throw new Exception(_localizationService.GetResource("NoiTruBenhAn.DichVuGiuongNhomGia.NotExists"));
            //        }

            //        var giaBaoHiem = dichVuGiuong.DichVuGiuongBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay <= item.NgayPhatSinh && (o.DenNgay == null || item.NgayPhatSinh <= o.DenNgay.Value));
            //        //var giaBenhVien = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.OrderBy(x => x.NhomGiaDichVuGiuongBenhVien.Ten == "Thường").Select(x => x).First();
            //        var giaBenhViens = dichVuGiuong.DichVuGiuongBenhVienGiaBenhViens.Where(p => p.TuNgay <= item.NgayPhatSinh && (p.DenNgay == null || item.NgayPhatSinh <= p.DenNgay.Value))
            //                                                                        .ToList();
            //        var giaBenhVien = new DichVuGiuongBenhVienGiaBenhVien();

            //        if (item2.BaoPhong.GetValueOrDefault())
            //        {
            //            giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() == "bao phòng").FirstOrDefault();

            //            if (giaBenhVien == null)
            //            {
            //                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaBaoPhong.KhongCoHieuLuc"));
            //            }
            //        }
            //        else
            //        {
            //            giaBenhVien = giaBenhViens.Where(p => p.NhomGiaDichVuGiuongBenhVien.Ten.ToLower().Trim() != "bao phòng").FirstOrDefault();

            //            if (giaBenhVien == null)
            //            {
            //                throw new Exception(_localizationService.GetResource("NoiTruBenhAn.NhomGiaThuong.KhongCoHieuLuc"));
            //            }
            //        }

            //        if (item2.isCreated)
            //        {
            //            var dichVuGiuongBenhVienChiPhiBenhVien = new YeuCauDichVuGiuongBenhVienChiPhiBenhVien
            //            {
            //                NgayPhatSinh = item.NgayPhatSinh,
            //                YeuCauTiepNhanId = yeuCauTiepNhan.Id,
            //                DichVuGiuongBenhVienId = item2.DichVuGiuongId,
            //                NhomGiaDichVuGiuongBenhVienId = giaBenhVien.NhomGiaDichVuGiuongBenhVienId,
            //                GiuongBenhId = item2.GiuongChuyenDenId,
            //                //GiuongBenh = dvGiuongBenhVien.GiuongBenh,
            //                PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
            //                //PhongBenhVien = dvGiuongBenhVien.GiuongBenh.PhongBenhVien,
            //                KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
            //                //KhoaPhong = dvGiuongBenhVien.GiuongBenh.PhongBenhVien.KhoaPhong,
            //                Ten = dichVuGiuong.Ten,
            //                Ma = dichVuGiuong.Ma,
            //                MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
            //                LoaiGiuong = (EnumLoaiGiuong)item2.LoaiGiuong,
            //                MoTa = dichVuGiuong.MoTa,
            //                Gia = giaBenhVien.Gia,
            //                BaoPhong = item2.BaoPhong,
            //                SoLuong = item2.SoLuong,
            //                SoLuongGhep = item2.SoLuongGhep,
            //                TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
            //                GhiChu = string.Empty,
            //                DoiTuongSuDung = item2.DoiTuong ?? DoiTuongSuDung.BenhNhan,
            //                HeThongTuPhatSinh = true
            //            };
            //            if (item2.LaDichVuTrongGoi)
            //            {
            //                if (dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId == null)
            //                {
            //                    var thongTin = new ThongTinDichVuTrongGoi()
            //                    {
            //                        BenhNhanId = (long)yeuCauTiepNhan.BenhNhanId,
            //                        DichVuId = item2.DichVuGiuongId,
            //                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
            //                        SoLuong = (int)item2.SoLuong
            //                    };
            //                    await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
            //                    dichVuGiuongBenhVienChiPhiBenhVien.Gia = thongTin.DonGia;
            //                    dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
            //                    dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
            //                    dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;
            //                }
            //            }
            //            else
            //            {
            //                dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = null;
            //                dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = null;
            //                dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = null;
            //            }

            //            yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Add(dichVuGiuongBenhVienChiPhiBenhVien);

            //            var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

            //            if (theBHYT != null && giaBaoHiem != null)
            //            {
            //                var dichVuGiuongBenhVienChiPhiBHYT = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
            //                {
            //                    NgayPhatSinh = item.NgayPhatSinh,
            //                    YeuCauTiepNhanId = yeuCauTiepNhan.Id,
            //                    DichVuGiuongBenhVienId = item2.DichVuGiuongId,
            //                    GiuongBenhId = item2.GiuongChuyenDenId,
            //                    //GiuongBenh = dvGiuongBHYTs[i].GiuongBenh,
            //                    PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
            //                    //PhongBenhVien = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien,
            //                    KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
            //                    //KhoaPhong = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien.KhoaPhong,
            //                    Ten = dichVuGiuong.Ten,
            //                    Ma = dichVuGiuong.Ma,
            //                    MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
            //                    LoaiGiuong = (EnumLoaiGiuong)item2.LoaiGiuong,
            //                    MoTa = dichVuGiuong.MoTa,
            //                    SoLuong = item2.SoLuong,
            //                    SoLuongGhep = item2.SoLuongGhep,
            //                    DuocHuongBaoHiem = true,
            //                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
            //                    GhiChu = string.Empty,
            //                    DonGiaBaoHiem = giaBaoHiem.Gia,
            //                    MucHuongBaoHiem = theBHYT.MucHuong,
            //                    TiLeBaoHiemThanhToan = 100,
            //                    HeThongTuPhatSinh = true,
            //                    ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVien = dichVuGiuongBenhVienChiPhiBenhVien
            //                };
            //                if (item2.LaDichVuTrongGoi)
            //                {
            //                    if (dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId == null)
            //                    {
            //                        var thongTin = new ThongTinDichVuTrongGoi()
            //                        {
            //                            BenhNhanId = (long)yeuCauTiepNhan.BenhNhanId,
            //                            DichVuId = item2.DichVuGiuongId,
            //                            NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
            //                            SoLuong = (int)item2.SoLuong
            //                        };
            //                        await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
            //                        dichVuGiuongBenhVienChiPhiBenhVien.Gia = thongTin.DonGia;
            //                        dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau =
            //                            thongTin.DonGiaTruocChietKhau;
            //                        dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
            //                        dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;
            //                    }
            //                }
            //                else
            //                {
            //                    dichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = null;
            //                    dichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = null;
            //                    dichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = null;
            //                }

            //                yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYT);
            //            }
            //        }
            //        else
            //        {
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVienIds.Add(item2.Id);
            //            var yeuCauDichVuGiuongBenhVienChiPhiBenhVien = yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBenhViens.Where(p => p.Id == item2.Id)
            //                                                                                                                   .FirstOrDefault();

            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DichVuGiuongBenhVienId = item2.DichVuGiuongId;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ten = dichVuGiuong.Ten;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Ma = dichVuGiuong.Ma;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.LoaiGiuong = (EnumLoaiGiuong)item2.LoaiGiuong;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.MoTa = dichVuGiuong.MoTa;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia = giaBenhVien.Gia;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.BaoPhong = item2.BaoPhong;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DoiTuongSuDung = item2.DoiTuong ?? DoiTuongSuDung.BenhNhan;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.SoLuong = item2.SoLuong;
            //            yeuCauDichVuGiuongBenhVienChiPhiBenhVien.SoLuongGhep = item2.SoLuongGhep;
            //            if (item2.LaDichVuTrongGoi)
            //            {
            //                if (yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId == null)
            //                {
            //                    var thongTin = new ThongTinDichVuTrongGoi()
            //                    {
            //                        BenhNhanId = (long)yeuCauTiepNhan.BenhNhanId,
            //                        DichVuId = item2.DichVuGiuongId,
            //                        NhomGoiDichVu = EnumNhomGoiDichVu.DichVuGiuongBenh,
            //                        SoLuong = (int)item2.SoLuong
            //                    };
            //                    await _khamBenhService.GetYeuCauGoiDichVuTheoDichVuChiDinhAsync(thongTin);
            //                    yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Gia = thongTin.DonGia;
            //                    yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = thongTin.DonGiaTruocChietKhau;
            //                    yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = thongTin.DonGiaSauChietKhau;
            //                    yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = thongTin.YeuCauGoiDichVuId;
            //                }
            //            }
            //            else
            //            {
            //                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaTruocChietKhau = null;
            //                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.DonGiaSauChietKhau = null;
            //                yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauGoiDichVuId = null;
            //            }

            //            var theBHYT = yeuCauTiepNhan.YeuCauTiepNhanTheBHYTs.LastOrDefault();

            //            if (theBHYT != null && giaBaoHiem != null)
            //            {
            //                if (!yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Any())
            //                {
            //                    var dichVuGiuongBenhVienChiPhiBHYT = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
            //                    {
            //                        NgayPhatSinh = item.NgayPhatSinh,
            //                        YeuCauTiepNhanId = yeuCauTiepNhan.Id,
            //                        DichVuGiuongBenhVienId = item2.DichVuGiuongId,
            //                        GiuongBenhId = item2.GiuongChuyenDenId,
            //                        //GiuongBenh = dvGiuongBHYTs[i].GiuongBenh,
            //                        PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
            //                        //PhongBenhVien = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien,
            //                        KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
            //                        //KhoaPhong = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien.KhoaPhong,
            //                        Ten = dichVuGiuong.Ten,
            //                        Ma = dichVuGiuong.Ma,
            //                        MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
            //                        LoaiGiuong = (EnumLoaiGiuong)item2.LoaiGiuong,
            //                        MoTa = dichVuGiuong.MoTa,
            //                        SoLuong = item2.SoLuong,
            //                        SoLuongGhep = item2.SoLuongGhep,
            //                        DuocHuongBaoHiem = true,
            //                        TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
            //                        GhiChu = string.Empty,
            //                        DonGiaBaoHiem = giaBaoHiem.Gia,
            //                        MucHuongBaoHiem = theBHYT.MucHuong,
            //                        TiLeBaoHiemThanhToan = 100,
            //                        HeThongTuPhatSinh = true,
            //                        ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Id
            //                    };

            //                    yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYT);
            //                }
            //                else
            //                {
            //                    if (yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.LastOrDefault().DichVuGiuongBenhVienId != item2.DichVuGiuongId)
            //                    {
            //                        var ycBHYT = yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Where(p => p.Id == yeuCauDichVuGiuongBenhVienChiPhiBenhVien.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.LastOrDefault().Id)
            //                                                                                         .FirstOrDefault();

            //                        ycBHYT.WillDelete = true;

            //                        var dichVuGiuongBenhVienChiPhiBHYT = new YeuCauDichVuGiuongBenhVienChiPhiBHYT()
            //                        {
            //                            NgayPhatSinh = item.NgayPhatSinh,
            //                            YeuCauTiepNhanId = yeuCauTiepNhan.Id,
            //                            DichVuGiuongBenhVienId = item2.DichVuGiuongId,
            //                            GiuongBenhId = item2.GiuongChuyenDenId,
            //                            //GiuongBenh = dvGiuongBHYTs[i].GiuongBenh,
            //                            PhongBenhVienId = giuongBenh?.PhongBenhVienId ?? noiChiDinh.Id,
            //                            //PhongBenhVien = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien,
            //                            KhoaPhongId = giuongBenh?.PhongBenhVien.KhoaPhongId ?? noiChiDinh.KhoaPhongId,
            //                            //KhoaPhong = dvGiuongBHYTs[i].GiuongBenh.PhongBenhVien.KhoaPhong,
            //                            Ten = dichVuGiuong.Ten,
            //                            Ma = dichVuGiuong.Ma,
            //                            MaTT37 = dichVuGiuong.DichVuGiuongId != null ? dichVuGiuong.DichVuGiuong?.MaTT37 : null,
            //                            LoaiGiuong = (EnumLoaiGiuong)item2.LoaiGiuong,
            //                            MoTa = dichVuGiuong.MoTa,
            //                            SoLuong = item2.SoLuong,
            //                            SoLuongGhep = item2.SoLuongGhep,
            //                            DuocHuongBaoHiem = true,
            //                            TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
            //                            GhiChu = string.Empty,
            //                            DonGiaBaoHiem = giaBaoHiem.Gia,
            //                            MucHuongBaoHiem = theBHYT.MucHuong,
            //                            TiLeBaoHiemThanhToan = 100,
            //                            HeThongTuPhatSinh = true,
            //                            ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId = yeuCauDichVuGiuongBenhVienChiPhiBenhVien.Id
            //                        };

            //                        yeuCauTiepNhan.YeuCauDichVuGiuongBenhVienChiPhiBHYTs.Add(dichVuGiuongBenhVienChiPhiBHYT);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public bool KiemTraThoiGianNhanGiuongVoiThoiDiemChiDinhGiuong(DateTime? thoiGianNhan, long noiTruBenhAnId)
        {
            var listGiuong =
                _yeuCauDichVuGiuongBenhVienRepository.TableNoTracking.Where(p =>
                    p.YeuCauTiepNhanId == noiTruBenhAnId);
            if (thoiGianNhan == null || !listGiuong.Any())
            {
                return true;
            }
            var thoiDiemNhanGiuong = GetThoiDiemChiDinhGiuong(noiTruBenhAnId);

            return !(thoiGianNhan.Value < thoiDiemNhanGiuong.MinThoiDiemNhanGiuong);

        }
        #endregion

        #region Chuyển khoa
        public async Task<long> GetFirstKhoaPhongDieuTriId(long noiTruBenhAnId)
        {
            var khoaPhongDieuTri = await _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruBenhAnId)
                                                                                          .FirstOrDefaultAsync();

            return khoaPhongDieuTri?.Id ?? (long)0;
        }

        public async Task<NoiTruKhoaPhongDieuTri> GetLastKhoaPhongDieuTri(long noiTruBenhAnId)
        {
            return await _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruBenhAnId)
                                                                          .Include(p => p.KhoaPhongChuyenDi)
                                                                          .Include(p => p.KhoaPhongChuyenDen)
                                                                          .Include(p => p.ChanDoanVaoKhoaICD)
                                                                          .LastOrDefaultAsync();
        }

        public async Task<GridDataSource> GetDanhSachChuyenKhoaForGrid(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == yeuCauTiepNhanId)
                                                                         .Select(p => new ChuyenKhoaGridVo
                                                                         {
                                                                             Id = p.Id,
                                                                             NoiTruBenhAnId = p.NoiTruBenhAnId,
                                                                             KhoaPhongChuyenDiId = p.KhoaPhongChuyenDiId,
                                                                             KhoaPhongChuyenDiDisplay = p.KhoaPhongChuyenDi.Ten,
                                                                             KhoaPhongChuyenDenId = p.KhoaPhongChuyenDenId,
                                                                             KhoaPhongChuyenDenDisplay = p.KhoaPhongChuyenDen.Ten,
                                                                             ThoiDiemVaoKhoa = p.ThoiDiemVaoKhoa,
                                                                             ThoiDiemRaKhoa = p.ThoiDiemRaKhoa,
                                                                             ChanDoanVaoKhoaICDId = p.ChanDoanVaoKhoaICDId,
                                                                             ChanDoanVaoKhoaICDDisplay = p.ChanDoanVaoKhoaICDId != null ? $"{p.ChanDoanVaoKhoaICD.Ma} - {p.ChanDoanVaoKhoaICD.TenTiengViet}" : "",
                                                                             ChanDoanVaoKhoaGhiChu = p.ChanDoanVaoKhoaGhiChu,
                                                                             LyDoChuyenKhoa = p.LyDoChuyenKhoa,
                                                                             NhanVienChiDinhId = p.NhanVienChiDinhId,
                                                                             NhanVienChiDinhDisplay = p.NhanVienChiDinh.User.HoTen,
                                                                             IsFirstData = false
                                                                         });

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            var firstKhoaPhongDieuTriId = await GetFirstKhoaPhongDieuTriId(yeuCauTiepNhanId);

            var thoiDiemRaVien = BaseRepository.TableNoTracking.Where(p => p.Id == yeuCauTiepNhanId)
                                                               .Select(p => p.NoiTruBenhAn.ThoiDiemRaVien)
                                                               .FirstOrDefault();

            foreach (var item in queryTask.Result)
            {
                //thoiDiemVaoKhoa <= p.TuNgay < p.DenNgay <= thoiDiemRaKhoa
                var thoiDiemVaoKhoa = item.ThoiDiemVaoKhoa;
                var thoiDiemRaKhoa = item.ThoiDiemRaKhoa;

                var lstBacSi = _noiTruEkipDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == yeuCauTiepNhanId &&
                                                                                       (
                                                                                            (thoiDiemRaKhoa == null && (p.TuNgay >= thoiDiemVaoKhoa || p.DenNgay.GetValueOrDefault() >= thoiDiemVaoKhoa)) ||
                                                                                            (thoiDiemRaKhoa == null && p.DenNgay == null) ||
                                                                                            (thoiDiemRaKhoa != null &&
                                                                                            (
                                                                                                (p.TuNgay >= thoiDiemVaoKhoa && p.TuNgay <= thoiDiemRaKhoa) ||
                                                                                                (p.DenNgay >= thoiDiemVaoKhoa && p.DenNgay <= thoiDiemRaKhoa) ||
                                                                                                (p.TuNgay <= thoiDiemVaoKhoa && p.DenNgay >= thoiDiemVaoKhoa && p.TuNgay <= thoiDiemRaKhoa && p.DenNgay >= thoiDiemRaKhoa)
                                                                                            )) ||
                                                                                            (thoiDiemRaKhoa != null && p.DenNgay == null && p.TuNgay <= thoiDiemRaKhoa)
                                                                                       )
                                                                                 )
                                                                           .Include(p => p.BacSi)
                                                                           .ThenInclude(p => p.User)
                                                                           .ToList();

                item.BacSiDieuTriId = lstBacSi.Select(p => p.BacSiId).ToList();
                item.BacSiDieuTriDisplay = lstBacSi.Select(p => p.BacSi.User.HoTen).Join(", ");

                if (item.Id == firstKhoaPhongDieuTriId)
                {
                    item.IsFirstData = true;
                }

                if (thoiDiemRaVien != null && item.ThoiDiemRaKhoa == null)
                {
                    item.ThoiDiemRaKhoa = thoiDiemRaVien;
                }
            }

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPagesDanhSachChuyenKhoaForGrid(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            long.TryParse(queryInfo.AdditionalSearchString, out long yeuCauTiepNhanId);

            var query = await _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == yeuCauTiepNhanId)
                                                                               .CountAsync();

            return new GridDataSource
            {
                TotalRowCount = query
            };
        }

        public bool KiemTraTonTaiLichChuyenPhong(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId, long noiTruKhoaPhongDieuTriId)
        {
            if (thoiDiemVaoKhoa == null)
            {
                return true;
            }

            return !_noiTruKhoaPhongDieuTriRepository.TableNoTracking.Any(p => p.NoiTruBenhAnId == noiTruBenhAnId &&
                                                                               p.Id != noiTruKhoaPhongDieuTriId &&
                                                                               (p.ThoiDiemVaoKhoa == thoiDiemVaoKhoa.Value ||
                                                                               (p.ThoiDiemRaKhoa != null && thoiDiemVaoKhoa.Value >= p.ThoiDiemVaoKhoa && thoiDiemVaoKhoa.Value <= p.ThoiDiemRaKhoa)));
        }

        public bool KiemTraThoiDiemVaoKhoaVoiNgayNhapVien(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId)
        {
            if (thoiDiemVaoKhoa == null)
            {
                return true;
            }

            return !(thoiDiemVaoKhoa.Value < _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == noiTruBenhAnId)
                                                                                    .Select(p => p.ThoiDiemNhapVien)
                                                                                    .FirstOrDefault());
        }

        public bool KiemTraKhoaChuyenDenKhacKhoaChuyenDi(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId, long? khoaChuyenDenId, long id)
        {
            if (thoiDiemVaoKhoa == null || khoaChuyenDenId == null)
            {
                return true;
            }

            var khoaPhongDieuTri = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.Id != id &&
                                                                                                p.NoiTruBenhAnId == noiTruBenhAnId &&
                                                                                                p.ThoiDiemVaoKhoa < thoiDiemVaoKhoa)
                                                                                    .OrderByDescending(p => p.ThoiDiemVaoKhoa)
                                                                                    .FirstOrDefault();

            if (khoaPhongDieuTri == null)
            {
                return true;
            }


            return !(khoaChuyenDenId == khoaPhongDieuTri.KhoaPhongChuyenDenId);
        }

        public bool KiemTraTonTaiLichDieuTri(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId)
        {
            if (thoiDiemVaoKhoa == null)
            {
                return true;
            }
            var phieuDieuTriSauNgayChuyenKhoas = _noiTruPhieuDieuTriRepository.TableNoTracking
                .Include(o => o.YeuCauDichVuKyThuats)
                .Include(o => o.YeuCauTruyenMaus)
                .Include(o => o.YeuCauDuocPhamBenhViens).ThenInclude(o => o.YeuCauTraDuocPhamTuBenhNhanChiTiets)
                .Include(o => o.YeuCauVatTuBenhViens).ThenInclude(o => o.YeuCauTraVatTuTuBenhNhanChiTiets)
                .Where(p => p.NoiTruBenhAnId == noiTruBenhAnId && p.NgayDieuTri > thoiDiemVaoKhoa.Value);
            if (phieuDieuTriSauNgayChuyenKhoas.Any())
            {
                //Kiểm tra ko phát sinh dịch vụ đã hoàn thành trong tờ điều trị tương lai
                var kiemTraDVKTTuongLai = phieuDieuTriSauNgayChuyenKhoas.SelectMany(c => c.YeuCauDichVuKyThuats.Where(o => o.NoiTruPhieuDieuTri != null && o.TrangThai != Enums.EnumTrangThaiYeuCauDichVuKyThuat.DaHuy));
                //Kiểm tra ko phát sinh truyền máu đã hoàn thành trong tờ điều trị tương lai
                var kiemTraMauTuongLai = phieuDieuTriSauNgayChuyenKhoas.SelectMany(c => c.YeuCauTruyenMaus.Where(o => o.NoiTruPhieuDieuTri != null && o.TrangThai != Enums.EnumTrangThaiYeuCauTruyenMau.DaHuy));
                //Kiểm tra ko phát sinh thuốc đã hoàn thành trong tờ điều trị tương lai
                var kiemTraThuocTuongLai = phieuDieuTriSauNgayChuyenKhoas.SelectMany(c => c.YeuCauDuocPhamBenhViens
                    .Where(x => x.NoiTruPhieuDieuTri != null && x.TrangThai != Enums.EnumYeuCauDuocPhamBenhVien.DaHuy && (x.SoLuong - (x.YeuCauTraDuocPhamTuBenhNhanChiTiets.Any() ? x.YeuCauTraDuocPhamTuBenhNhanChiTiets.Sum(o => o.SoLuongTra) : 0)) > 0));
                //Kiểm tra ko phát sinh vật tư đã hoàn thành trong tờ điều trị tương lai
                var kiemTraVatTuTuongLai = phieuDieuTriSauNgayChuyenKhoas.SelectMany(c => c.YeuCauVatTuBenhViens
                                        .Where(x => x.NoiTruPhieuDieuTri != null && x.TrangThai != Enums.EnumYeuCauVatTuBenhVien.DaHuy && (x.SoLuong - (x.YeuCauTraVatTuTuBenhNhanChiTiets.Any() ? x.YeuCauTraVatTuTuBenhNhanChiTiets.Sum(o => o.SoLuongTra) : 0)) > 0));
                if (kiemTraDVKTTuongLai.Any())
                {
                    return false;
                }
                if (kiemTraThuocTuongLai.Any())
                {
                    return false;
                }
                if (kiemTraVatTuTuongLai.Any())
                {
                    return false;
                }
                if (kiemTraMauTuongLai.Any())
                {
                    return false;
                }
            }
            return true;
        }

        public bool KiemTraSuaTrongPhamViBanDau(NoiTruKhoaPhongDieuTri noiTruKhoaPhongDieuTri)
        {
            var oldNoiTruKhoaPhongDieuTri = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.Id == noiTruKhoaPhongDieuTri.Id)
                                                                                             .FirstOrDefault();

            return noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa >= oldNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa && noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa <= oldNoiTruKhoaPhongDieuTri.ThoiDiemRaKhoa;
        }

        public List<NoiTruKhoaPhongDieuTri> XuLyThemKhoaPhongDieuTri(NoiTruKhoaPhongDieuTri noiTruKhoaPhongDieuTri)
        {
            var lstNoiTruKhoaPhongDieuTri = new List<NoiTruKhoaPhongDieuTri>();

            var noiTruKhoaPhongDieuTriTruoc = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                           p.NoiTruBenhAnId == noiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                           p.ThoiDiemVaoKhoa < noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                               .OrderByDescending(p => p.ThoiDiemVaoKhoa)
                                                                                               .FirstOrDefault();

            if (noiTruKhoaPhongDieuTriTruoc != null)
            {
                noiTruKhoaPhongDieuTriTruoc.ThoiDiemRaKhoa = noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa.AddMinutes(-1);
                lstNoiTruKhoaPhongDieuTri.Add(noiTruKhoaPhongDieuTriTruoc);

                noiTruKhoaPhongDieuTri.KhoaPhongChuyenDiId = noiTruKhoaPhongDieuTriTruoc.KhoaPhongChuyenDenId;
            }
            else
            {
                noiTruKhoaPhongDieuTri.KhoaPhongChuyenDiId = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == noiTruKhoaPhongDieuTri.NoiTruBenhAnId)
                                                                                                    .Select(p => p.KhoaPhongNhapVienId)
                                                                                                    .FirstOrDefault();
            }

            var noiTruKhoaPhongDieuTriSau = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                         p.NoiTruBenhAnId == noiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                         p.ThoiDiemVaoKhoa > noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                             .OrderBy(p => p.ThoiDiemVaoKhoa)
                                                                                             .FirstOrDefault();

            if (noiTruKhoaPhongDieuTriSau != null)
            {
                noiTruKhoaPhongDieuTriSau.KhoaPhongChuyenDiId = noiTruKhoaPhongDieuTri.KhoaPhongChuyenDenId;
                lstNoiTruKhoaPhongDieuTri.Add(noiTruKhoaPhongDieuTriSau);

                noiTruKhoaPhongDieuTri.ThoiDiemRaKhoa = noiTruKhoaPhongDieuTriSau.ThoiDiemVaoKhoa.AddMinutes(-1);
            }
            else
            {
                noiTruKhoaPhongDieuTri.ThoiDiemRaKhoa = null;
            }

            lstNoiTruKhoaPhongDieuTri.Add(noiTruKhoaPhongDieuTri);

            return lstNoiTruKhoaPhongDieuTri;
        }

        public List<NoiTruKhoaPhongDieuTri> XuLySuaKhoaPhongDieuTri(NoiTruKhoaPhongDieuTri noiTruKhoaPhongDieuTri)
        {
            var lstNoiTruKhoaPhongDieuTri = new List<NoiTruKhoaPhongDieuTri>();

            var oldNoiTruKhoaPhongDieuTri = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.Id == noiTruKhoaPhongDieuTri.Id)
                                                                                             .FirstOrDefault();

            var oldNoiTruKhoaPhongDieuTriTruoc = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == oldNoiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                              p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                              p.ThoiDiemVaoKhoa < oldNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                                  .OrderByDescending(p => p.ThoiDiemVaoKhoa)
                                                                                                  .FirstOrDefault();

            var newNoiTruKhoaPhongDieuTriTruoc = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                              p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                              p.ThoiDiemVaoKhoa < noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                                  .OrderByDescending(p => p.ThoiDiemVaoKhoa)
                                                                                                  .FirstOrDefault();

            if (oldNoiTruKhoaPhongDieuTriTruoc != null && (newNoiTruKhoaPhongDieuTriTruoc == null || oldNoiTruKhoaPhongDieuTriTruoc.Id != newNoiTruKhoaPhongDieuTriTruoc.Id))
            {
                var oldNoiTruKhoaPhongDieuTriSau = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == oldNoiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                                p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                                p.ThoiDiemVaoKhoa > oldNoiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                                    .OrderBy(p => p.ThoiDiemVaoKhoa)
                                                                                                    .FirstOrDefault();

                if (oldNoiTruKhoaPhongDieuTriSau != null)
                {
                    oldNoiTruKhoaPhongDieuTriTruoc.ThoiDiemRaKhoa = oldNoiTruKhoaPhongDieuTriSau.ThoiDiemVaoKhoa.AddMinutes(-1);
                    oldNoiTruKhoaPhongDieuTriSau.KhoaPhongChuyenDiId = oldNoiTruKhoaPhongDieuTriTruoc.KhoaPhongChuyenDenId;

                    lstNoiTruKhoaPhongDieuTri.Add(oldNoiTruKhoaPhongDieuTriTruoc);
                    lstNoiTruKhoaPhongDieuTri.Add(oldNoiTruKhoaPhongDieuTriSau);
                }
                else
                {
                    oldNoiTruKhoaPhongDieuTriTruoc.ThoiDiemRaKhoa = null;
                    lstNoiTruKhoaPhongDieuTri.Add(oldNoiTruKhoaPhongDieuTriTruoc);
                }
            }

            if (newNoiTruKhoaPhongDieuTriTruoc != null)
            {
                newNoiTruKhoaPhongDieuTriTruoc.ThoiDiemRaKhoa = noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa.AddMinutes(-1);
                lstNoiTruKhoaPhongDieuTri.Add(newNoiTruKhoaPhongDieuTriTruoc);

                noiTruKhoaPhongDieuTri.KhoaPhongChuyenDiId = newNoiTruKhoaPhongDieuTriTruoc.KhoaPhongChuyenDenId;
            }
            else
            {
                noiTruKhoaPhongDieuTri.KhoaPhongChuyenDiId = _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == noiTruKhoaPhongDieuTri.NoiTruBenhAnId)
                                                                                                    .Select(p => p.KhoaPhongNhapVienId)
                                                                                                    .FirstOrDefault();
            }

            var newNoiTruKhoaPhongDieuTriSau = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                            p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                            p.ThoiDiemVaoKhoa > noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                                .OrderBy(p => p.ThoiDiemVaoKhoa)
                                                                                                .FirstOrDefault();

            if (newNoiTruKhoaPhongDieuTriSau != null)
            {
                noiTruKhoaPhongDieuTri.ThoiDiemRaKhoa = newNoiTruKhoaPhongDieuTriSau.ThoiDiemVaoKhoa.AddMinutes(-1);
                newNoiTruKhoaPhongDieuTriSau.KhoaPhongChuyenDiId = noiTruKhoaPhongDieuTri.KhoaPhongChuyenDenId;

                lstNoiTruKhoaPhongDieuTri.Add(newNoiTruKhoaPhongDieuTriSau);
            }

            lstNoiTruKhoaPhongDieuTri.Add(noiTruKhoaPhongDieuTri);

            return lstNoiTruKhoaPhongDieuTri;
        }

        public List<NoiTruKhoaPhongDieuTri> XuLyXoaKhoaPhongDieuTri(NoiTruKhoaPhongDieuTri noiTruKhoaPhongDieuTri)
        {
            var lstNoiTruKhoaPhongDieuTri = new List<NoiTruKhoaPhongDieuTri>();

            var noiTruKhoaPhongDieuTriTruoc = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                           p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                           p.ThoiDiemVaoKhoa < noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                               .OrderByDescending(p => p.ThoiDiemVaoKhoa)
                                                                                               .FirstOrDefault();

            if (noiTruKhoaPhongDieuTriTruoc != null)
            {
                var noiTruKhoaPhongDieuTriSau = _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruKhoaPhongDieuTri.NoiTruBenhAnId &&
                                                                                                             p.Id != noiTruKhoaPhongDieuTri.Id &&
                                                                                                             p.ThoiDiemVaoKhoa > noiTruKhoaPhongDieuTri.ThoiDiemVaoKhoa)
                                                                                                 .OrderBy(p => p.ThoiDiemVaoKhoa)
                                                                                                 .FirstOrDefault();

                if (noiTruKhoaPhongDieuTriSau != null)
                {
                    noiTruKhoaPhongDieuTriTruoc.ThoiDiemRaKhoa = noiTruKhoaPhongDieuTriSau.ThoiDiemVaoKhoa.AddMinutes(-1);
                    noiTruKhoaPhongDieuTriSau.KhoaPhongChuyenDiId = noiTruKhoaPhongDieuTriTruoc.KhoaPhongChuyenDenId;

                    lstNoiTruKhoaPhongDieuTri.Add(noiTruKhoaPhongDieuTriTruoc);
                    lstNoiTruKhoaPhongDieuTri.Add(noiTruKhoaPhongDieuTriSau);
                }
                else
                {
                    noiTruKhoaPhongDieuTriTruoc.ThoiDiemRaKhoa = null;
                    lstNoiTruKhoaPhongDieuTri.Add(noiTruKhoaPhongDieuTriTruoc);
                }
            }

            return lstNoiTruKhoaPhongDieuTri;
        }

        public async Task<NoiTruKhoaPhongDieuTri> GetCurrentNoiTruKhoaPhongDieuTri(long noiTruBenhAnId)
        {
            return await _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Include(p => p.KhoaPhongChuyenDi)
                                                                          .Include(p => p.KhoaPhongChuyenDen)
                                                                          .Include(p => p.ChanDoanVaoKhoaICD)
                                                                          .Where(p => p.NoiTruBenhAnId == noiTruBenhAnId &&
                                                                                      p.ThoiDiemVaoKhoa <= DateTime.Now &&
                                                                                      (p.ThoiDiemRaKhoa == null || p.ThoiDiemRaKhoa.Value >= DateTime.Now))
                                                                          .FirstOrDefaultAsync();
        }

        public async Task<KhoaPhongChuyenDen> GetCurrentKhoaHienTaiBenhNhanChuyenDen(long noiTruBenhAnId)
        {
            var khoaPhongDieuTri = await _noiTruKhoaPhongDieuTriRepository.TableNoTracking.Where(p => p.NoiTruBenhAnId == noiTruBenhAnId &&
                                                                                                      p.ThoiDiemVaoKhoa <= DateTime.Now &&
                                                                                                      (p.ThoiDiemRaKhoa == null || p.ThoiDiemRaKhoa.Value >= DateTime.Now))
                                                                                          .Include(p => p.KhoaPhongChuyenDen)
                                                                                          .FirstOrDefaultAsync();

            if (khoaPhongDieuTri != null)
            {
                return new KhoaPhongChuyenDen()
                {
                    Id = khoaPhongDieuTri.KhoaPhongChuyenDen.Id,
                    DisplayName = khoaPhongDieuTri.KhoaPhongChuyenDen.Ten
                };
            }
            else
            {
                return await _noiTruBenhAnRepository.TableNoTracking.Where(p => p.Id == noiTruBenhAnId)
                                                                    .Select(p => new KhoaPhongChuyenDen()
                                                                    {
                                                                        Id = p.KhoaPhongNhapVienId,
                                                                        DisplayName = p.KhoaPhongNhapVien.Ten
                                                                    })
                                                                    .FirstOrDefaultAsync();
            }
        }

        public async Task<List<LookupItemVo>> GetDanhSachKhoaChuyenDen(DropDownListRequestModel model)
        {
            var lst = await _khoaPhongRepository.TableNoTracking.Where(p => p.IsDisabled != true 

                                                                            //BVHD-3883
                                                                            && p.CoKhamNoiTru != null && p.CoKhamNoiTru == true)
                                                                .Select(item => new LookupItemVo()
                                                                {
                                                                    KeyId = item.Id,
                                                                    DisplayName = item.Ten
                                                                })
                                                                .ApplyLike(model.Query, x => x.DisplayName)
                                                                .OrderByDescending(p => p.DisplayName)
                                                                .Take(model.Take)
                                                                .ToListAsync();

            return lst;
        }
        #endregion

        #region Chung
        public bool CompareTuNgayDenNgay(DateTime? tuNgay, DateTime? denNgay)
        {
            if (denNgay.HasValue && denNgay.HasValue)
            {
                return denNgay.Value <= tuNgay.Value ? false : true;
            }
            else
            {
                return true;
            }
        }
        #endregion

        public bool KiemTraDichVuCoTrongGoi(long benhNhanId, long? dichVuGiuongBenhVienId)
        {
            var goiDichVus = _yeuCauGoiDichVuRepository.TableNoTracking
               .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuDichVuGiuongs)
               .Include(x => x.ChuongTrinhGoiDichVu).ThenInclude(y => y.ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs)
               .Where(x => ((x.BenhNhanId == benhNhanId && x.GoiSoSinh != true) || (x.BenhNhanSoSinhId == benhNhanId && x.GoiSoSinh == true))
                           && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.ChuaThucHien
                           && x.TrangThai != EnumTrangThaiYeuCauGoiDichVu.DaHuy);
            if (dichVuGiuongBenhVienId != null && goiDichVus.Any(z => z.ChuongTrinhGoiDichVu != null
                                 && z.ChuongTrinhGoiDichVu.ChuongTrinhGoiDichVuDichVuGiuongs.Any(zx => zx.DichVuGiuongBenhVienId == dichVuGiuongBenhVienId)))
            {
                return true;
            }
            return false;
        }
    }
}