using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Services.KhamDoan
{
    // partial interface for Yeu Cau Nhan Su Kham Suc Khoe List
    public partial class KhamDoanService
    {
        public async Task<GridDataSource> GetDataForYeuCauNhanSuKhamSucKhoeGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var queryObject = new KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo>(queryInfo.AdditionalSearchString);
            }

            if (queryObject?.TrangThai != null && queryObject.TrangThai.DaDuyet == false && queryObject.TrangThai.ChoDuyet == false && queryObject.TrangThai.TuChoi == false)
            {
                queryObject.TrangThai.DaDuyet = true;
                queryObject.TrangThai.ChoDuyet = true;
                queryObject.TrangThai.TuChoi = true;
            }

            var queryDangChoDuyet = GetYeuCauNhanSuChoDuyet(queryInfo, queryObject);
            var queryDaDuyet = GetYeuCauNhanSuDaDuyet(queryInfo, queryObject);
            var queryTuChoiDuyet = GetYeuCauNhanSuTuChoiDuyet(queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo()).AsQueryable();

            var isHaveQuery = false;
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.ChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.TuChoi)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayGui asc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var queryTask = exportExcel == true ? query.ToArrayAsync() : query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        private IQueryable<KhamDoanYeuCauNhanSuKhamSucKhoeGridVo> GetYeuCauNhanSuChoDuyet(QueryInfo queryInfo, KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo queryObject)
        {
            var result = _yeuCauNhanSuKhamSucKhoeRepository.TableNoTracking
                .Where(p => p.DuocKHTHDuyet == null && p.DuocNhanSuDuyet == null && p.DuocGiamDocDuyet == null ||
                            p.DuocKHTHDuyet != false && p.DuocNhanSuDuyet == null && p.DuocGiamDocDuyet == null ||
                            p.DuocKHTHDuyet != false && p.DuocNhanSuDuyet != false && p.DuocGiamDocDuyet == null)
                .Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo
                {
                    Id = s.Id,
                    TrangThai = EnumTrangThaiKhamDoan.ChoDuyet,
                    NgayKhthDuyet = s.NgayKHTHDuyet,
                    TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    HopDong = s.HopDongKhamSucKhoe.SoHopDong,
                    SoLuongBn = s.HopDongKhamSucKhoe.SoNguoiKham,
                    NgayBatDauKham = s.HopDongKhamSucKhoe.NgayHieuLuc,
                    NgayKetThucKham = s.HopDongKhamSucKhoe.NgayKetThuc,
                    NgayGui = s.NgayGuiYeuCau,
                    NguoiGui = s.NhanVienGuiYeuCau.User.HoTen,
                    KhthDuyet = s.NhanVienKHTHDuyet.User.HoTen,
                    NsDuyet = s.NhanVienNhanSuDuyet.User.HoTen,
                    NgayNsDuyet = s.NgayNhanSuDuyet,
                    GdDuyet = s.GiamDoc.User.HoTen,
                    NgayGdDuyet = s.NgayGiamDocDuyet,
                    SoLuongBs = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 1),
                    SoLuongDd = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 5),
                    NhanVienKhac = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId != 1 && w.NhanVien.ChucDanh.NhomChucDanhId != 5 || w.NhanVien.ChucDanh == null)
                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.TenCongTy,
                    g => g.HopDong,
                    g => g.NguoiGui,
                    g => g.KhthDuyet,
                    g => g.NsDuyet,
                    g => g.GdDuyet
                );

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau asc,Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.NgayGui != null && queryObject.NgayGui.startDate != null)
                {
                    var tuNgay = queryObject.NgayGui.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => tuNgay <= p.NgayGui.Date);
                }
                if (queryObject.NgayGui != null && queryObject.NgayGui.endDate != null)
                {
                    var denNgay = queryObject.NgayGui.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayGui.Date);
                }

                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayKHTHDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayKhthDuyet != null && tuNgay <= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayKHTHDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayKhthDuyet != null && denNgay >= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayNhanSuDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayNsDuyet != null && tuNgay <= p.NgayNsDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayNhanSuDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayNsDuyet != null && denNgay >= p.NgayNsDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayGiamDocDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayGdDuyet != null && tuNgay <= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayGiamDocDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayGdDuyet != null && denNgay >= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
            }

            return result;
        }

        private IQueryable<KhamDoanYeuCauNhanSuKhamSucKhoeGridVo> GetYeuCauNhanSuDaDuyet(QueryInfo queryInfo, KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo queryObject)
        {
            var result = _yeuCauNhanSuKhamSucKhoeRepository.TableNoTracking
                .Where(p => p.DuocKHTHDuyet == true && p.DuocNhanSuDuyet == true && p.DuocGiamDocDuyet == true)
                .Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo
                {
                    Id = s.Id,
                    TrangThai = EnumTrangThaiKhamDoan.DaDuyet,
                    NgayKhthDuyet = s.NgayKHTHDuyet,
                    TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    HopDong = s.HopDongKhamSucKhoe.SoHopDong,
                    SoLuongBn = s.HopDongKhamSucKhoe.SoNguoiKham,
                    NgayBatDauKham = s.HopDongKhamSucKhoe.NgayHieuLuc,
                    NgayKetThucKham = s.HopDongKhamSucKhoe.NgayKetThuc,
                    NgayGui = s.NgayGuiYeuCau,
                    NguoiGui = s.NhanVienGuiYeuCau.User.HoTen,
                    KhthDuyet = s.NhanVienKHTHDuyet.User.HoTen,
                    NsDuyet = s.NhanVienNhanSuDuyet.User.HoTen,
                    NgayNsDuyet = s.NgayNhanSuDuyet,
                    GdDuyet = s.GiamDoc.User.HoTen,
                    NgayGdDuyet = s.NgayGiamDocDuyet,
                    SoLuongBs = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 1),
                    SoLuongDd = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 5),
                    NhanVienKhac = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId != 1 && w.NhanVien.ChucDanh.NhomChucDanhId != 5 || w.NhanVien.ChucDanh == null)

                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.TenCongTy,
                    g => g.HopDong,
                    g => g.NguoiGui,
                    g => g.KhthDuyet,
                    g => g.NsDuyet,
                    g => g.GdDuyet
                );

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau asc,Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.NgayGui != null && queryObject.NgayGui.startDate != null)
                {
                    var tuNgay = queryObject.NgayGui.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => tuNgay <= p.NgayGui.Date);
                }
                if (queryObject.NgayGui != null && queryObject.NgayGui.endDate != null)
                {
                    var denNgay = queryObject.NgayGui.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayGui.Date);
                }

                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayKHTHDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayKhthDuyet != null && tuNgay <= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayKHTHDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayKhthDuyet != null && denNgay >= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayNhanSuDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayNsDuyet != null && tuNgay <= p.NgayNsDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayNhanSuDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayNsDuyet != null && denNgay >= p.NgayNsDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayGiamDocDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayGdDuyet != null && tuNgay <= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayGiamDocDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayGdDuyet != null && denNgay >= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
            }

            return result;
        }

        private IQueryable<KhamDoanYeuCauNhanSuKhamSucKhoeGridVo> GetYeuCauNhanSuTuChoiDuyet(QueryInfo queryInfo, KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo queryObject)
        {
            var result = _yeuCauNhanSuKhamSucKhoeRepository.TableNoTracking
                .Where(p => p.DuocKHTHDuyet == false || p.DuocNhanSuDuyet == false || p.DuocGiamDocDuyet == false)
                .Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo
                {
                    Id = s.Id,
                    TrangThai = EnumTrangThaiKhamDoan.TuChoi,
                    NgayKhthDuyet = s.NgayKHTHDuyet,
                    TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    HopDong = s.HopDongKhamSucKhoe.SoHopDong,
                    SoLuongBn = s.HopDongKhamSucKhoe.SoNguoiKham,
                    NgayBatDauKham = s.HopDongKhamSucKhoe.NgayHieuLuc,
                    NgayKetThucKham = s.HopDongKhamSucKhoe.NgayKetThuc,
                    NgayGui = s.NgayGuiYeuCau,
                    NguoiGui = s.NhanVienGuiYeuCau.User.HoTen,
                    KhthDuyet = s.NhanVienKHTHDuyet.User.HoTen,
                    NsDuyet = s.NhanVienNhanSuDuyet.User.HoTen,
                    NgayNsDuyet = s.NgayNhanSuDuyet,
                    GdDuyet = s.GiamDoc.User.HoTen,
                    NgayGdDuyet = s.NgayGiamDocDuyet,
                    SoLuongBs = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 1),
                    SoLuongDd = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 5),
                    NhanVienKhac = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId != 1 && w.NhanVien.ChucDanh.NhomChucDanhId != 5 || w.NhanVien.ChucDanh == null)
                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.TenCongTy,
                    g => g.HopDong,
                    g => g.NguoiGui,
                    g => g.KhthDuyet,
                    g => g.NsDuyet,
                    g => g.GdDuyet
                );

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau asc,Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.NgayGui != null && queryObject.NgayGui.startDate != null)
                {
                    var tuNgay = queryObject.NgayGui.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => tuNgay <= p.NgayGui.Date);
                }
                if (queryObject.NgayGui != null && queryObject.NgayGui.endDate != null)
                {
                    var denNgay = queryObject.NgayGui.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayGui.Date);
                }

                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayKHTHDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayKhthDuyet != null && tuNgay <= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayKHTHDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayKhthDuyet != null && denNgay >= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayNhanSuDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayNsDuyet != null && tuNgay <= p.NgayNsDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayNhanSuDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayNsDuyet != null && denNgay >= p.NgayNsDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayGiamDocDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayGdDuyet != null && tuNgay <= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayGiamDocDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayGdDuyet != null && denNgay >= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
            }

            return result;
        }

        public async Task<GridDataSource> GetTotalPageForYeuCauNhanSuKhamSucKhoeGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo>(queryInfo.AdditionalSearchString);
            }

            if (queryObject?.TrangThai != null && queryObject.TrangThai.DaDuyet == false && queryObject.TrangThai.ChoDuyet == false && queryObject.TrangThai.TuChoi == false)
            {
                queryObject.TrangThai.DaDuyet = true;
                queryObject.TrangThai.ChoDuyet = true;
                queryObject.TrangThai.TuChoi = true;
            }

            var queryDangChoDuyet = GetYeuCauNhanSuChoDuyet(queryInfo, queryObject);
            var queryDaDuyet = GetYeuCauNhanSuDaDuyet(queryInfo, queryObject);
            var queryTuChoiDuyet = GetYeuCauNhanSuTuChoiDuyet(queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo()).AsQueryable();

            var isHaveQuery = false;
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.ChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.TuChoi)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                }
            }
            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<GridDataSource> GetDataForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var queryObject = new KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo>(queryInfo.AdditionalSearchString);
            }

            if (queryObject?.TrangThai != null && queryObject.TrangThai.DaDuyet == false && queryObject.TrangThai.ChoDuyet == false && queryObject.TrangThai.TuChoi == false)
            {
                queryObject.TrangThai.DaDuyet = true;
                queryObject.TrangThai.ChoDuyet = true;
                queryObject.TrangThai.TuChoi = true;
            }

            var queryDangChoDuyet = GetYeuCauNhanSuChoDuyetPhongNhanSu(queryInfo, queryObject);
            var queryDaDuyet = GetYeuCauNhanSuDaDuyet(queryInfo, queryObject);
            var queryTuChoiDuyet = GetYeuCauNhanSuTuChoiDuyetPhongNhanSu(queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo()).AsQueryable();

            var isHaveQuery = false;
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.ChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.TuChoi)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayGui asc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var queryTask = query.Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForYeuCauNhanSuKhamSucKhoePhongNhanSuGridAsync(QueryInfo queryInfo)
        {
            var queryObject = new KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo();

            if (!string.IsNullOrEmpty(queryInfo.AdditionalSearchString))
            {
                queryObject = JsonConvert.DeserializeObject<KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo>(queryInfo.AdditionalSearchString);
            }

            if (queryObject?.TrangThai != null && queryObject.TrangThai.DaDuyet == false && queryObject.TrangThai.ChoDuyet == false && queryObject.TrangThai.TuChoi == false)
            {
                queryObject.TrangThai.DaDuyet = true;
                queryObject.TrangThai.ChoDuyet = true;
                queryObject.TrangThai.TuChoi = true;
            }

            var queryDangChoDuyet = GetYeuCauNhanSuChoDuyetPhongNhanSu(queryInfo, queryObject);
            var queryDaDuyet = GetYeuCauNhanSuDaDuyet(queryInfo, queryObject);
            var queryTuChoiDuyet = GetYeuCauNhanSuTuChoiDuyetPhongNhanSu(queryInfo, queryObject);

            var query = BaseRepository.TableNoTracking.Where(p => p.Id == 0).Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo()).AsQueryable();

            var isHaveQuery = false;
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.ChoDuyet)
            {
                query = queryDangChoDuyet;
                isHaveQuery = true;
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.DaDuyet)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryDaDuyet);
                }
                else
                {
                    query = queryDaDuyet;
                    isHaveQuery = true;
                }
            }
            if (queryObject != null && queryObject.TrangThai != null && queryObject.TrangThai.TuChoi)
            {
                if (isHaveQuery)
                {
                    query = query.Concat(queryTuChoiDuyet);
                }
                else
                {
                    query = queryTuChoiDuyet;
                }
            }

            if (queryInfo.SortString != null && !queryInfo.SortString.Equals("NgayGui asc,Id asc") && (queryInfo.SortString.Contains("desc") || queryInfo.SortString.Contains("asc")))
            {
                query = query.OrderBy(queryInfo.SortString);
            }

            var countTask = query.CountAsync();

            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        private IQueryable<KhamDoanYeuCauNhanSuKhamSucKhoeGridVo> GetYeuCauNhanSuChoDuyetPhongNhanSu(QueryInfo queryInfo, KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo queryObject)
        {
            var result = _yeuCauNhanSuKhamSucKhoeRepository.TableNoTracking
                .Where(p => (p.DuocKHTHDuyet == null && p.DuocNhanSuDuyet == null && p.DuocGiamDocDuyet == null ||
                            p.DuocKHTHDuyet != false && p.DuocNhanSuDuyet == null && p.DuocGiamDocDuyet == null ||
                            p.DuocKHTHDuyet != false && p.DuocNhanSuDuyet != false && p.DuocGiamDocDuyet == null) && p.DuocKHTHDuyet == true)
                .Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo
                {
                    Id = s.Id,
                    TrangThai = EnumTrangThaiKhamDoan.ChoDuyet,
                    NgayKhthDuyet = s.NgayKHTHDuyet,
                    TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    HopDong = s.HopDongKhamSucKhoe.SoHopDong,
                    SoLuongBn = s.HopDongKhamSucKhoe.SoNguoiKham,
                    NgayBatDauKham = s.HopDongKhamSucKhoe.NgayHieuLuc,
                    NgayKetThucKham = s.HopDongKhamSucKhoe.NgayKetThuc,
                    NgayGui = s.NgayGuiYeuCau,
                    NguoiGui = s.NhanVienGuiYeuCau.User.HoTen,
                    KhthDuyet = s.NhanVienKHTHDuyet.User.HoTen,
                    NsDuyet = s.NhanVienNhanSuDuyet.User.HoTen,
                    NgayNsDuyet = s.NgayNhanSuDuyet,
                    GdDuyet = s.GiamDoc.User.HoTen,
                    NgayGdDuyet = s.NgayGiamDocDuyet,
                    SoLuongBs = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 1),
                    SoLuongDd = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 5),
                    NhanVienKhac = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId != 1 && w.NhanVien.ChucDanh.NhomChucDanhId != 5 || w.NhanVien.ChucDanh == null)
                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.TenCongTy,
                    g => g.HopDong,
                    g => g.NguoiGui,
                    g => g.KhthDuyet,
                    g => g.NsDuyet,
                    g => g.GdDuyet
                );

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau asc,Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.NgayGui != null && queryObject.NgayGui.startDate != null)
                {
                    var tuNgay = queryObject.NgayGui.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => tuNgay <= p.NgayGui.Date);
                }
                if (queryObject.NgayGui != null && queryObject.NgayGui.endDate != null)
                {
                    var denNgay = queryObject.NgayGui.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayGui.Date);
                }

                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayKHTHDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayKhthDuyet != null && tuNgay <= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayKHTHDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayKhthDuyet != null && denNgay >= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayNhanSuDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayNsDuyet != null && tuNgay <= p.NgayNsDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayNhanSuDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayNsDuyet != null && denNgay >= p.NgayNsDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayGiamDocDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayGdDuyet != null && tuNgay <= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayGiamDocDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayGdDuyet != null && denNgay >= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
            }

            return result;
        }

        private IQueryable<KhamDoanYeuCauNhanSuKhamSucKhoeGridVo> GetYeuCauNhanSuTuChoiDuyetPhongNhanSu(QueryInfo queryInfo, KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo queryObject)
        {
            var result = _yeuCauNhanSuKhamSucKhoeRepository.TableNoTracking
                .Where(p => (p.DuocKHTHDuyet == false || p.DuocNhanSuDuyet == false || p.DuocGiamDocDuyet == false) && p.DuocKHTHDuyet == true)
                .Select(s => new KhamDoanYeuCauNhanSuKhamSucKhoeGridVo
                {
                    Id = s.Id,
                    TrangThai = EnumTrangThaiKhamDoan.TuChoi,
                    NgayKhthDuyet = s.NgayKHTHDuyet,
                    TenCongTy = s.HopDongKhamSucKhoe.CongTyKhamSucKhoe.Ten,
                    HopDong = s.HopDongKhamSucKhoe.SoHopDong,
                    SoLuongBn = s.HopDongKhamSucKhoe.SoNguoiKham,
                    NgayBatDauKham = s.HopDongKhamSucKhoe.NgayHieuLuc,
                    NgayKetThucKham = s.HopDongKhamSucKhoe.NgayKetThuc,
                    NgayGui = s.NgayGuiYeuCau,
                    NguoiGui = s.NhanVienGuiYeuCau.User.HoTen,
                    KhthDuyet = s.NhanVienKHTHDuyet.User.HoTen,
                    NsDuyet = s.NhanVienNhanSuDuyet.User.HoTen,
                    NgayNsDuyet = s.NgayNhanSuDuyet,
                    GdDuyet = s.GiamDoc.User.HoTen,
                    NgayGdDuyet = s.NgayGiamDocDuyet,
                    SoLuongBs = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 1),
                    SoLuongDd = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId == 5),
                    NhanVienKhac = s.YeuCauNhanSuKhamSucKhoeChiTiets.Count(w => w.NhanVien.ChucDanh != null && w.NhanVien.ChucDanh.NhomChucDanhId != 1 && w.NhanVien.ChucDanh.NhomChucDanhId != 5 || w.NhanVien.ChucDanh == null)
                })
                .ApplyLike(queryInfo.SearchTerms,
                    g => g.TenCongTy,
                    g => g.HopDong,
                    g => g.NguoiGui,
                    g => g.KhthDuyet,
                    g => g.NsDuyet,
                    g => g.GdDuyet
                );

            if (queryInfo.SortString != null && queryInfo.SortString.Equals("NgayYeuCau asc,Id asc"))
            {
                result = result.OrderBy(queryInfo.SortString);
            }

            if (queryObject != null)
            {
                if (queryObject.NgayGui != null && queryObject.NgayGui.startDate != null)
                {
                    var tuNgay = queryObject.NgayGui.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => tuNgay <= p.NgayGui.Date);
                }
                if (queryObject.NgayGui != null && queryObject.NgayGui.endDate != null)
                {
                    var denNgay = queryObject.NgayGui.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => denNgay >= p.NgayGui.Date);
                }

                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayKHTHDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayKhthDuyet != null && tuNgay <= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayKHTHDuyet != null && queryObject.NgayKHTHDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayKHTHDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayKhthDuyet != null && denNgay >= p.NgayKhthDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayNhanSuDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayNsDuyet != null && tuNgay <= p.NgayNsDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayNhanSuDuyet != null && queryObject.NgayNhanSuDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayNhanSuDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayNsDuyet != null && denNgay >= p.NgayNsDuyet.GetValueOrDefault().Date);
                }

                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.startDate != null)
                {
                    var tuNgay = queryObject.NgayGiamDocDuyet.startDate.GetValueOrDefault().Date;

                    result = result.Where(p => p.NgayGdDuyet != null && tuNgay <= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
                if (queryObject.NgayGiamDocDuyet != null && queryObject.NgayGiamDocDuyet.endDate != null)
                {
                    var denNgay = queryObject.NgayGiamDocDuyet.endDate.GetValueOrDefault().Date;
                    result = result.Where(p => p.NgayGdDuyet != null && denNgay >= p.NgayGdDuyet.GetValueOrDefault().Date);
                }
            }

            return result;
        }

    }
}
